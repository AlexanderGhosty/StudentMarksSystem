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

                // Дергаем CanExecuteChanged, чтобы кнопки, завязанные на SelectedSubject, обновились
                if (DeleteSubjectCommand is RelayCommand delSubCmd)
                    delSubCmd.RaiseCanExecuteChanged();
                
                if (RenameSubjectCommand is RelayCommand renSubCmd)
                    renSubCmd.RaiseCanExecuteChanged();

                if (LoadGradesCommand is RelayCommand loadGradesCmd)
                    loadGradesCmd.RaiseCanExecuteChanged();

                if (AddOrUpdateGradeCommand is RelayCommand addUpdCmd)
                    addUpdCmd.RaiseCanExecuteChanged();

                // Если выбран предмет и мы являемся учителем или админом, сразу загрузим оценки.
                if (_selectedSubject != null)
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

                // Чтобы кнопка удаления оценки включалась/выключалась
                if (DeleteGradeCommand is RelayCommand delGradeCmd)
                    delGradeCmd.RaiseCanExecuteChanged();
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
        public ICommand RenameSubjectCommand { get; }

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
            RenameSubjectCommand = new RelayCommand(async _ => await RenameSubjectAsync(),
                                            _ => IsAdmin && SelectedSubject != null);

            LoadGradesCommand = new RelayCommand(async _ => await LoadGradesAsync(), _ => SelectedSubject != null);

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

            // Если студент — выбираем только те предметы, по которым у него есть оценки
            if (IsStudent)
            {
                // 1) Получаем оценки студента
                var gradesResp = await _apiService.GetGradesByStudentAsync(GlobalState.CurrentUser.Id);
                if (!gradesResp.IsSuccess || gradesResp.Grades == null)
                {
                    MessageBox.Show(gradesResp.ErrorMessage ?? "Ошибка при загрузке ваших оценок");
                    return;
                }

                // Список уникальных Id предметов, по которым есть оценки у студента
                var subjectIds = gradesResp.Grades
                                           .Select(g => g.SubjectId)
                                           .Distinct()
                                           .ToList();

                // 2) Загружаем все предметы и фильтруем
                var subjectsResp = await _apiService.GetAllSubjectsAsync();
                if (subjectsResp.IsSuccess && subjectsResp.Subjects != null)
                {
                    var filteredSubjects = subjectsResp.Subjects
                                                       .Where(s => subjectIds.Contains(s.Id));

                    foreach (var s in filteredSubjects)
                        Subjects.Add(s);
                }
                else
                {
                    MessageBox.Show(subjectsResp.ErrorMessage ?? "Ошибка при загрузке предметов");
                }
            }
            else
            {
                // Если роль teacher или admin – загружаем все предметы
                var result = await _apiService.GetAllSubjectsAsync();
                if (result.IsSuccess && result.Subjects != null)
                {
                    foreach (var s in result.Subjects)
                        Subjects.Add(s);
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage ?? "Ошибка при загрузке предметов");
                }
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

        private async Task RenameSubjectAsync()
        {
            if (SelectedSubject == null) return;

            // 1) Открываем диалог или любой способ получить новое название.
            // Ниже для примера вызываем некий "диалог переименования".
            var dialogVm = new RenameSubjectDialogViewModel(SelectedSubject.Title);
            var dialog = new RenameSubjectDialogView
            {
                DataContext = dialogVm
            };

            bool? result = dialog.ShowDialog();
            if (result == true && dialogVm.DialogResultOk)
            {
                // Получаем новое название, которое пользователь ввёл
                string newTitle = dialogVm.SubjectTitle;

                // 2) Шлём запрос на сервер
                var updateResult = await _apiService.UpdateSubjectAsync(SelectedSubject.Id, newTitle);
                if (updateResult.IsSuccess)
                {
                    // 3) Обновляем локально выбранный предмет, чтобы UI сразу отобразил изменения
                    SelectedSubject.Title = newTitle;
                    // Если нужно обновить сразу в ObservableCollection, 
                    // то можно вызвать OnPropertyChanged("Subjects") или вручную обновить саму коллекцию
                    OnPropertyChanged(nameof(Subjects));
                }
                else
                {
                    MessageBox.Show(updateResult.ErrorMessage ?? "Не удалось переименовать предмет");
                }
            }
        }

        #endregion

        #region Методы для работы с оценками

        /// <summary>
        /// Загрузить оценки по выбранному предмету 
        /// </summary>
        private async System.Threading.Tasks.Task LoadGradesAsync()
        {
            if (SelectedSubject == null)
                return;

            Grades.Clear();

            if (IsTeacherOrAdmin)
            {
                // Как и раньше: грузим все оценки по предмету
                var gradesRes = await _apiService.GetGradesBySubjectAsync(SelectedSubject.Id);
                if (gradesRes.IsSuccess && gradesRes.Grades != null)
                {
                    foreach (var g in gradesRes.Grades)
                        Grades.Add(g);
                }
                else if (gradesRes.Grades == null)
                {
                    MessageBox.Show(gradesRes.ErrorMessage ?? "Оценки отсутствуют");
                }
                else
                {
                    MessageBox.Show(gradesRes.ErrorMessage ?? "Ошибка при получении оценок");
                }
            }
            else if (IsStudent)
            {
                // Для студента – грузим все его оценки и фильтруем по предмету
                var resp = await _apiService.GetGradesByStudentAsync(GlobalState.CurrentUser.Id);
                if (resp.IsSuccess && resp.Grades != null)
                {
                    var subjectGrades = resp.Grades
                                            .Where(g => g.SubjectId == SelectedSubject.Id);
                    foreach (var g in subjectGrades)
                        Grades.Add(g);
                }
                else
                {
                    MessageBox.Show(resp.ErrorMessage ?? "Ошибка при загрузке ваших (студенческих) оценок");
                }
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
            if (SelectedSubject == null)
                return;
            
            // Открываем диалоговое окно
            var gradeDialog = new GradeDialog(_apiService);

            bool? dialogResult = gradeDialog.ShowDialog();
            if (dialogResult == true)
            {
                // Получаем выбранного пользователя и оценку из ViewModel диалога
                var vm = gradeDialog.ViewModel;
                if (vm.SelectedUser == null)
                {
                    MessageBox.Show("Не выбран студент.");
                    return;
                }
                if (!int.TryParse(vm.SelectedGrade, out int gradeValue))
                {
                    MessageBox.Show("Некорректное значение оценки.");
                    return;
                }

                // Формируем объект Grade
                var grade = new Grade
                {
                    StudentId = vm.SelectedUser.Id,
                    SubjectId = SelectedSubject.Id,
                    GradeValue = gradeValue
                };

                // Отправляем на сервер
                var res = await _apiService.AddGradeAsync(grade);
                if (!res.IsSuccess)
                {
                    MessageBox.Show(res.ErrorMessage ?? "Ошибка при добавлении/изменении оценки");
                    return;
                }

                // Перезагрузим список оценок (для обновления UI)
                await LoadGradesAsync();
            }
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
