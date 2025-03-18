using Client.Models;
using Client.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class GradesViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        public GradesViewModel(IApiService apiService)
        {
            _apiService = apiService;

            Grades = new ObservableCollection<Grade>();

            LoadGradesCommand = new RelayCommand(async _ => await LoadGradesAsync());
            AddOrUpdateGradeCommand = new RelayCommand(async _ => await AddOrUpdateGradeAsync());

            // Если хотите сразу загружать что-то при инициализации, раскомментируйте:
            // LoadGradesCommand.Execute(null);
        }

        // Список всех оценок, загружаемых из API
        public ObservableCollection<Grade> Grades { get; set; }

        // Выбранный предмет
        private Subject _selectedSubject;
        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged();
                // При смене предмета сразу подгрузим оценки
                if (_selectedSubject != null)
                {
                    LoadGradesCommand.Execute(null);
                }
            }
        }

        // Выбранная оценка (когда пользователь кликает по строке в DataGrid)
        private Grade _selectedGrade;
        public Grade SelectedGrade
        {
            get => _selectedGrade;
            set
            {
                _selectedGrade = value;
                OnPropertyChanged();
            }
        }

        // Поле для ввода/редактирования нового значения оценки
        private int _newGradeValue;
        public int NewGradeValue
        {
            get => _newGradeValue;
            set
            {
                _newGradeValue = value;
                OnPropertyChanged();
            }
        }

        // Команды
        public ICommand LoadGradesCommand { get; }
        public ICommand AddOrUpdateGradeCommand { get; }

        // Загрузка оценок по выбранному предмету
        private async System.Threading.Tasks.Task LoadGradesAsync()
        {
            Grades.Clear();
            var currentUser = GlobalState.CurrentUser;
            if (currentUser == null) return;

            if (currentUser.Role == "student")
            {
                var response = await _apiService.GetGradesByStudentAsync(currentUser.Id);
                if (response.IsSuccess)
                {
                    foreach (var g in response.Grades)
                    {
                        Grades.Add(g);
                    }
                }
            }
            else if (currentUser.Role == "teacher")
            {
                if (SelectedSubject == null) return;
                var response = await _apiService.GetGradesBySubjectAsync(SelectedSubject.Id);
                // ...
            }
            else if (currentUser.Role == "admin")
            {
                // возможно, админ видит всё или выбирает конкретного студента/предмет
            }
        }


        // Добавить/обновить оценку:
        // Логика: берём SelectedGrade (или новую Grade), ставим GradeValue = NewGradeValue.
        private async System.Threading.Tasks.Task AddOrUpdateGradeAsync()
        {
            if (SelectedSubject == null)
                return;

            if (SelectedGrade == null)
            {
                // Вариант, когда мы хотим поставить оценку студенту,
                // которого нет в списке. Придётся спросить у пользователя studentId,
                // либо иметь выпадающий список студентов. Для примера - заглушка:
                return;
            }

            // Допишем новое значение:
            SelectedGrade.GradeValue = NewGradeValue;

            // Отправляем на сервер
            var result = await _apiService.AddGradeAsync(SelectedGrade);
            if (result.IsSuccess && result.CreatedGrade != null)
            {
                // Обновим в локальной коллекции
                // 1) ищем элемент по (studentId, subjectId)
                var updated = result.CreatedGrade;
                var existing = Grades
                    .FirstOrDefault(x => x.StudentId == updated.StudentId && x.SubjectId == updated.SubjectId);

                if (existing != null)
                {
                    // обновляем поля
                    existing.GradeValue = updated.GradeValue;
                    existing.StudentName = updated.StudentName;
                    existing.SubjectTitle = updated.SubjectTitle;
                    // Для уведомления UI можно вызвать OnPropertyChanged на каждом поле
                    // или удалить/добавить заново. Например:
                    var idx = Grades.IndexOf(existing);
                    Grades.RemoveAt(idx);
                    Grades.Insert(idx, existing);
                }
                else
                {
                    // Если новой записи не было, добавим
                    Grades.Add(updated);
                }
            }
            else
            {
                // Вывести ошибку
            }
        }
    }
}
