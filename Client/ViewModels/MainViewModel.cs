using System.Windows.Input;
using Client.Models;
using Client.Services;

namespace Client.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenAdminPanelCommand { get; }
        public ICommand OpenSubjectsCommand { get; }
        public ICommand LogoutCommand { get; }

        private readonly IApiService _apiService;

        public bool IsAdmin => GlobalState.CurrentUser?.Role == "admin";
        public bool IsTeacher => GlobalState.CurrentUser?.Role == "teacher";

        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;

            OpenAdminPanelCommand = new RelayCommand(_ => OpenAdminPanel());
            OpenSubjectsCommand = new RelayCommand(_ => OpenSubjects());
            LogoutCommand = new RelayCommand(_ => Logout());

            // По умолчанию можем открыть какую-то стартовую страницу
            CurrentViewModel = new HomeViewModel();
        }

        private void OpenAdminPanel()
        {
            if (IsAdmin)
                CurrentViewModel = new AdminPanelViewModel(_apiService);
        }

        private void OpenSubjects()
        {
            CurrentViewModel = new SubjectsViewModel(_apiService);
        }

        private void Logout()
        {
            GlobalState.CurrentUser = null;
            // Закрыть текущее окно, открыть заново окно авторизации
            // Или вызвать другой метод навигации
        }
    }
}
