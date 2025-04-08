using System.Windows.Input;
using System.Windows.Navigation;
using Client.Models;
using Client.Services;

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the login screen that handles user authentication
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private string _login;
        /// <summary>
        /// Gets or sets the user login name
        /// </summary>
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        /// <summary>
        /// Gets or sets the user password
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage;
        /// <summary>
        /// Gets or sets the error message displayed to the user
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        // Login command
        /// <summary>
        /// Command to execute the login process
        /// </summary>
        public ICommand LoginCommand { get; }

        private readonly IApiService _apiService;
        private readonly NavigationService _navigation; // Conditional navigation service

        /// <summary>
        /// Initializes a new instance of the LoginViewModel class with specified services
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        /// <param name="nav">Navigation service for screen transitions</param>
        public LoginViewModel(IApiService apiService, NavigationService nav)
        {
            _apiService = apiService;
            _navigation = nav;
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
        }

        /// <summary>
        /// Initializes a new instance of the LoginViewModel class with default services
        /// </summary>
        public LoginViewModel() : this(new Client.Services.ApiService(), new Client.NavigationService())
        {
        }

        // Event that triggers window closing
        /// <summary>
        /// Event that is raised when the login window should be closed
        /// </summary>
        public event EventHandler RequestClose;

        /// <summary>
        /// Performs the login operation by authenticating with the API service
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        private async System.Threading.Tasks.Task LoginAsync()
        {
            ErrorMessage = string.Empty;
            var response = await _apiService.LoginAsync(Login, Password);
            if (response.IsSuccess)
            {
                // Store current user in GlobalState
                GlobalState.CurrentUser = response.User;
                GlobalState.Token = response.Token;

                // Set token in ApiService
                _apiService.SetToken(response.Token);

                // Open the main window
                _navigation.ShowMainWindow();

                // Raise event to close the LoginView
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }
        }
    }
}
