using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Models;
using Client.Services;
using Client.Views;
using System.Windows;

namespace Client.ViewModels
{
    public class SubjectsViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        #region Свойства и коллекции

        // Список предметов
        public ObservableCollection<Subject> Subjects { get; set; }
            = new ObservableCollection<Subject>();

        private Subject _selectedSubject;
        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged();

                // Если выбран предмет и мы являемся учителем или админом, сразу загрузим оценки.
                if (IsTeacherOrAdmin && _selectedSubject != null)
                {
                    LoadGradesCommand.Execute(null);
                }
            }
        }

        // Список оценок для выбранного предмета (teacher/admin) или всех оценок студента (если student).
        public ObservableCollection<Grade> Grades { get; set; }
            = new ObservableCollection<Grade>();

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

        #endregion

        #region Роли
        // Если роль "admin" -> администратор
        public bool IsAdmin => GlobalState.CurrentUser?.Role == "admin";

        // Если роль "teacher" -> преподаватель
        public bool IsTeacher => GlobalState.CurrentUser?.Role == "teacher";

        // Admin или Teacher
        public bool IsTeacherOrAdmin => IsTeacher || IsAdmin;

        // Студент
        public bool IsStudent => GlobalState.CurrentUser?.Role == "student";
        #endregion

        #region Команды

        // Предметы
        public ICommand LoadSubjectsCommand { get; }
        public ICommand AddSubjectCommand { get; }
        public ICommand DeleteSubjectCommand { get; }

        // Оценки
        public ICommand LoadGradesCommand { get; }
        public ICommand AddOrUpdateGradeCommand { get; }
        public ICommand DeleteGradeCommand { get; }

        #endregion

        public SubjectsViewModel(IApiService apiService)
        {
            _apiService = apiService;

            // Команда загрузки списка предметов
            LoadSubjectsCommand = new RelayCommand(async _ => await LoadSubjectsAsync());

            // Команды добавления/удаления предмета (только для admin)
            AddSubjectCommand = new RelayCommand(async _ => await AddSubjectAsync(), _ => IsAdmin);
            DeleteSubjectCommand = new RelayCommand(async _ => await DeleteSubjectAsync(),
                                                   _ => IsAdmin && SelectedSubject != null);

            // Команды для оценок
            LoadGradesCommand = new RelayCommand(async _ => await LoadGradesAsync(),
                                                 _ => IsTeacherOrAdmin && SelectedSubject != null);

            AddOrUpdateGradeCommand = new RelayCommand(async _ => await AddOrUpdateGradeAsync(),
                                                       _ => IsTeacherOrAdmin && SelectedSubject != null);

            DeleteGradeCommand = new RelayCommand(async _ => await DeleteGradeAsync(),
                                                  _ => IsTeacherOrAdmin && SelectedGrade != null);

            // Сразу загружаем список предметов
            LoadSubjectsCommand.Execute(null);
        }

        #region Методы для работы с предметами

        private async System.Threading.Tasks.Task LoadSubjectsAsync()
        {
            Subjects.Clear();

            // Если студент, вы можете (по желанию) ему и вовсе не грузить список предметов.
            // Но в данном примере мы просто грузим все предметы – студент будет видеть
            // список, но не сможет ничего менять.
            var result = await _apiService.GetAllSubjectsAsync();
            if (result.IsSuccess && result.Subjects != null)
            {
                foreach (var s in result.Subjects)
                    Subjects.Add(s);
            }

            // Если это студент, сразу грузим его собственные оценки,
            // чтобы он видел их без привязки к выбранному предмету.
            if (IsStudent)
            {
                await LoadStudentGradesAsync();
            }
        }

        private async System.Threading.Tasks.Task AddSubjectAsync()
        {
            // Открываем диалог (у вас уже есть логика AddSubjectDialogView + AddSubjectDialogViewModel)
            var dialogVm = new AddSubjectDialogViewModel();
            var dialog = new AddSubjectDialogView
            {
                DataContext = dialogVm
            };

            bool? result = dialog.ShowDialog();
            if (result == true && dialogVm.DialogResultOk)
            {
                var newSubject = new Subject
                {
                    Title = dialogVm.SubjectTitle
                };

                var createResult = await _apiService.CreateSubjectAsync(newSubject);
                if (createResult.IsSuccess && createResult.CreatedSubject != null)
                {
                    Subjects.Add(createResult.CreatedSubject);
                }
                else
                {
                    MessageBox.Show(createResult.ErrorMessage ?? "Ошибка при создании предмета");
                }
            }
        }

        private async System.Threading.Tasks.Task DeleteSubjectAsync()
        {
            if (SelectedSubject == null) return;

            var confirm = MessageBox.Show(
                $"Удалить предмет '{SelectedSubject.Title}'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                var result = await _apiService.DeleteSubjectAsync(SelectedSubject.Id);
                if (result.IsSuccess)
                {
                    Subjects.Remove(SelectedSubject);
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage ?? "Ошибка при удалении предмета");
                }
            }
        }

        #endregion

        #region Методы для работы с оценками

        /// <summary>
        /// Загрузить оценки по выбранному предмету (если teacher/admin),
        /// либо (если студент) – этот метод не вызываем. Для студента отдельный метод.
        /// </summary>
        private async System.Threading.Tasks.Task LoadGradesAsync()
        {
            if (SelectedSubject == null)
                return;

            Grades.Clear();
            var gradesRes = await _apiService.GetGradesBySubjectAsync(SelectedSubject.Id);
            if (gradesRes.IsSuccess)
            {
                if (gradesRes.Grades != null && gradesRes.Grades.Count > 0)
                {
                    foreach (var g in gradesRes.Grades)
                        Grades.Add(g);
                }
                else
                {
                    MessageBox.Show($"Нет оценок для предмета '{SelectedSubject.Title}'.");
                }
            }
            else
            {
                MessageBox.Show(gradesRes.ErrorMessage ?? $"Не удалось получить оценки для предмета ID={SelectedSubject.Id}");
            }
        }


        /// <summary>
        /// Загрузить все оценки текущего студента (без привязки к предмету).
        /// </summary>
        private async System.Threading.Tasks.Task LoadStudentGradesAsync()
        {
            Grades.Clear();
            var resp = await _apiService.GetGradesByStudentAsync(GlobalState.CurrentUser.Id);
            if (resp.IsSuccess && resp.Grades != null)
            {
                foreach (var g in resp.Grades)
                    Grades.Add(g);
            }
            else
            {
                MessageBox.Show(resp.ErrorMessage ?? "Ошибка при загрузке ваших (студенческих) оценок");
            }
        }

        /// <summary>
        /// Добавление или изменение оценки (teacher/admin).
        /// В примере – показываем упрощённый диалог ввода ID студента и значение оценки.
        /// Можно оформить отдельным View + VM, как с предметами.
        /// </summary>
        private async System.Threading.Tasks.Task AddOrUpdateGradeAsync()
        {
            if (SelectedSubject == null) return;

            // Пример: выводим 2 InputBox. В реальном проекте лучше отдельное диалоговое окно.
            // 1) ID студента
            var studentIdString = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите ID студента:", "Новая/Изменить оценка", "");
            if (!int.TryParse(studentIdString, out int studentId)) return;

            // 2) Оценка
            var gradeValueString = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите значение оценки (число):", "Новая/Изменить оценка", "");
            if (!int.TryParse(gradeValueString, out int gradeValue)) return;

            var grade = new Grade
            {
                StudentId = studentId,
                SubjectId = SelectedSubject.Id,
                GradeValue = gradeValue
            };

            var res = await _apiService.AddOrUpdateGradeAsync(grade);
            if (!res.IsSuccess)
            {
                MessageBox.Show(res.ErrorMessage ?? "Ошибка при добавлении/изменении оценки");
                return;
            }

            // Если сервер вернул обновлённый объект, можно сразу обновить локальную коллекцию,
            // либо проще – заново подгрузить список.
            // Для наглядности перезагрузим.
            await LoadGradesAsync();
        }

        private async System.Threading.Tasks.Task DeleteGradeAsync()
        {
            if (SelectedGrade == null) return;

            var confirm = MessageBox.Show(
                $"Удалить оценку {SelectedGrade.GradeValue} для студента ID={SelectedGrade.StudentId}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                var res = await _apiService.DeleteGradeAsync(SelectedGrade.Id);
                if (res.IsSuccess)
                {
                    Grades.Remove(SelectedGrade);
                }
                else
                {
                    MessageBox.Show(res.ErrorMessage ?? "Ошибка при удалении оценки");
                }
            }
        }

        #endregion
    }
}
