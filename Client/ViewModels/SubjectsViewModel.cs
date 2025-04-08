using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Models;
using Client.Services;
using Client.Views;
using System.Windows;

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel для работы с предметами и оценками.
    /// Обеспечивает управление списком предметов, их созданием/удалением,
    /// а также просмотр и управление оценками по предметам.
    /// </summary>
    public class SubjectsViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        #region Свойства и коллекции

        /// <summary>
        /// Коллекция всех доступных предметов. 
        /// Для учителей и администраторов показывает все предметы.
        /// Для студентов — только предметы, по которым у них есть оценки.
        /// </summary>
        public ObservableCollection<Subject> Subjects { get; set; }
            = new ObservableCollection<Subject>();

        private Subject _selectedSubject;
        /// <summary>
        /// Выбранный в данный момент предмет.
        /// При изменении автоматически загружает оценки по этому предмету.
        /// </summary>
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

        /// <summary>
        /// Коллекция оценок для выбранного предмета.
        /// Для преподавателей и администраторов отображает все оценки по предмету.
        /// Для студентов показывает только их собственные оценки по выбранному предмету.
        /// </summary>
        public ObservableCollection<Grade> Grades { get; set; }
            = new ObservableCollection<Grade>();

        private Grade _selectedGrade;
        /// <summary>
        /// Выбранная оценка в списке. Используется для операций с оценками.
        /// </summary>
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
        /// <summary>
        /// Указывает, является ли текущий пользователь администратором.
        /// </summary>
        public bool IsAdmin => GlobalState.CurrentUser?.Role == "admin";

        /// <summary>
        /// Указывает, является ли текущий пользователь преподавателем.
        /// </summary>
        public bool IsTeacher => GlobalState.CurrentUser?.Role == "teacher";

        /// <summary>
        /// Указывает, является ли текущий пользователь преподавателем или администратором.
        /// </summary>
        public bool IsTeacherOrAdmin => IsTeacher || IsAdmin;

        /// <summary>
        /// Указывает, является ли текущий пользователь студентом.
        /// </summary>
        public bool IsStudent => GlobalState.CurrentUser?.Role == "student";
        #endregion

        #region Команды

        /// <summary>
        /// Команда для загрузки списка предметов.
        /// </summary>
        public ICommand LoadSubjectsCommand { get; }

        /// <summary>
        /// Команда для добавления нового предмета (доступна только администратору).
        /// </summary>
        public ICommand AddSubjectCommand { get; }

        /// <summary>
        /// Команда для удаления выбранного предмета (доступна только администратору).
        /// </summary>
        public ICommand DeleteSubjectCommand { get; }

        /// <summary>
        /// Команда для переименования выбранного предмета (доступна только администратору).
        /// </summary>
        public ICommand RenameSubjectCommand { get; }

        /// <summary>
        /// Команда для загрузки оценок по выбранному предмету.
        /// </summary>
        public ICommand LoadGradesCommand { get; }

        /// <summary>
        /// Команда для добавления или изменения оценки (доступна преподавателям и администраторам).
        /// </summary>
        public ICommand AddOrUpdateGradeCommand { get; }

        /// <summary>
        /// Команда для удаления выбранной оценки (доступна преподавателям и администраторам).
        /// </summary>
        public ICommand DeleteGradeCommand { get; }

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SubjectsViewModel"/>.
        /// </summary>
        /// <param name="apiService">Сервис для взаимодействия с API.</param>
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

        /// <summary>
        /// Загружает список предметов в зависимости от роли пользователя.
        /// Для студентов загружаются только предметы, по которым у них есть оценки.
        /// Для преподавателей и администраторов загружаются все предметы.
        /// </summary>
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

        /// <summary>
        /// Открывает диалоговое окно для создания нового предмета 
        /// и отправляет запрос на его добавление в систему.
        /// </summary>
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

        /// <summary>
        /// Удаляет выбранный предмет после подтверждения пользователем.
        /// </summary>
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

        /// <summary>
        /// Открывает диалоговое окно для переименования выбранного предмета 
        /// и отправляет запрос на обновление его названия.
        /// </summary>
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
        /// Загружает оценки по выбранному предмету. 
        /// Для преподавателей и администраторов загружает все оценки по предмету.
        /// Для студентов загружает только их собственные оценки по выбранному предмету.
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
        /// Загружает все оценки текущего студента (без привязки к конкретному предмету).
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
        /// Открывает диалоговое окно для добавления или изменения оценки.
        /// Доступно только для преподавателей и администраторов.
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

        /// <summary>
        /// Удаляет выбранную оценку после подтверждения пользователем.
        /// Доступно только для преподавателей и администраторов.
        /// </summary>
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
