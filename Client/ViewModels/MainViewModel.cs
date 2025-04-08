using System.Windows;
using System.Windows.Input;
using Client.Models;
using Client.Services;
using Client.Views;

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the main application window, managing navigation and user actions
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        /// <summary>
        /// Gets or sets the currently active ViewModel
        /// </summary>
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Command to open the home view
        /// </summary>
        public ICommand OpenHomeCommand { get; }

        /// <summary>
        /// Command to open the admin panel
        /// </summary>
        public ICommand OpenAdminPanelCommand { get; }

        /// <summary>
        /// Command to open the subjects view
        /// </summary>
        public ICommand OpenSubjectsCommand { get; }

        /// <summary>
        /// Command to log out the current user
        /// </summary>
        public ICommand LogoutCommand { get; }

        private readonly IApiService _apiService;

        /// <summary>
        /// Gets a value indicating whether the current user is an admin
        /// </summary>
        public bool IsAdmin => GlobalState.CurrentUser?.Role == "admin";

        /// <summary>
        /// Gets a value indicating whether the current user is a teacher
        /// </summary>
        public bool IsTeacher => GlobalState.CurrentUser?.Role == "teacher";

        /// <summary>
        /// Initializes a new instance of the MainViewModel class with a default API service
        /// </summary>
        public MainViewModel() : this(new Client.Services.ApiService())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class with the specified API service
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;

            if (!string.IsNullOrEmpty(GlobalState.Token))
            {
                _apiService.SetToken(GlobalState.Token);
            }

            OpenAdminPanelCommand = new RelayCommand(_ => OpenAdminPanel());
            OpenSubjectsCommand = new RelayCommand(_ => OpenSubjects());
            LogoutCommand = new RelayCommand(_ => Logout());

            OpenSubjectsCommand = new RelayCommand(_ => OpenSubjects());
            OpenHomeCommand = new RelayCommand(_ => OpenHomeView());

            // By default we can open some starting page
            CurrentViewModel = new HomeViewModel(_apiService);
        }

        /// <summary>
        /// Opens the home view
        /// </summary>
        private void OpenHomeView()
        {
            CurrentViewModel = new HomeViewModel(_apiService);
        }

        /// <summary>
        /// Opens the admin panel if the current user is an admin
        /// </summary>
        private void OpenAdminPanel()
        {
            if (IsAdmin)
            {
                System.Diagnostics.Debug.WriteLine("Opening admin panel");
                CurrentViewModel = new AdminPanelViewModel(_apiService);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User is not an admin");
            }
        }

        /// <summary>
        /// Opens the subjects view
        /// </summary>
        private void OpenSubjects()
        {
            CurrentViewModel = new SubjectsViewModel(_apiService);
        }

        /// <summary>
        /// Logs out the current user and returns to the login screen
        /// </summary>
        private void Logout()
        {
            // Clear current user data
            GlobalState.Clear();

            // Open the login window
            var navService = new NavigationService();
            navService.ShowLoginWindow();

            // Close the main application window
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
