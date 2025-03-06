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
            }
        }

        // Если роль текущего пользователя "admin", возвращаем true
        public bool IsAdmin => GlobalState.CurrentUser?.Role == "admin";

        public ICommand LoadSubjectsCommand { get; }
        public ICommand AddSubjectCommand { get; }
        public ICommand DeleteSubjectCommand { get; }

        public SubjectsViewModel(IApiService apiService)
        {
            _apiService = apiService;
            // Команда загрузки
            LoadSubjectsCommand = new RelayCommand(async _ => await LoadSubjectsAsync());

            // Команды добавления/удаления
            AddSubjectCommand = new RelayCommand(async _ => await AddSubjectAsync(), _ => IsAdmin);
            DeleteSubjectCommand = new RelayCommand(async _ => await DeleteSubjectAsync(), _ => IsAdmin);

            // Загрузим предметы при инициализации:
            LoadSubjectsCommand.Execute(null);
        }

        private async System.Threading.Tasks.Task LoadSubjectsAsync()
        {
            Subjects.Clear();
            var result = await _apiService.GetAllSubjectsAsync();
            if (result.IsSuccess)
            {
                foreach (var s in result.Subjects)
                    Subjects.Add(s);
            }
        }

        private async System.Threading.Tasks.Task AddSubjectAsync()
        {
            
            // 2) Создаём VM диалога, наполняем коллекцию Teachers
            var dialogVm = new AddSubjectDialogViewModel();

            // 3) Создаем само окно и связываем VM
            var dialog = new AddSubjectDialogView
            {
                DataContext = dialogVm,
                //Owner = Application.Current.MainWindow
            };

            // 4) Показываем диалог
            bool? result = dialog.ShowDialog();
            if (result == true && dialogVm.DialogResultOk)
            {

                var newSubject = new Subject
                {
                    Title = dialogVm.SubjectTitle
                };

                // 5) POST /subjects
                var createResult = await _apiService.CreateSubjectAsync(newSubject);
                if (createResult.IsSuccess && createResult.CreatedSubject != null)
                {
                    // Добавим в локальную коллекцию
                    Subjects.Add(createResult.CreatedSubject);
                }
                else
                {
                    // Вывести сообщение об ошибке
                }
            }
        }


        private async System.Threading.Tasks.Task DeleteSubjectAsync()
        {
            if (SelectedSubject == null) return;
            var result = await _apiService.DeleteSubjectAsync(SelectedSubject.Id);
            if (result.IsSuccess)
            {
                Subjects.Remove(SelectedSubject);
            }
        }


    }
}
