using Client.Services;

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the home page that displays current user information
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        private string _name;
        /// <summary>
        /// Gets or sets the name of the current user
        /// </summary>
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _login;
        /// <summary>
        /// Gets or sets the login (username) of the current user
        /// </summary>
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        private int _id;
        /// <summary>
        /// Gets or sets the ID of the current user
        /// </summary>
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private string _role;
        /// <summary>
        /// Gets or sets the role of the current user
        /// </summary>
        public string Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }

        // Constructor
        /// <summary>
        /// Initializes a new instance of the HomeViewModel class with the specified API service
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        public HomeViewModel(IApiService apiService)
        {
            _apiService = apiService;

            if (GlobalState.CurrentUser != null)
            {
                Name = GlobalState.CurrentUser.Name;
                Login = GlobalState.CurrentUser.Login;
                Id = GlobalState.CurrentUser.Id;
                Role = GlobalState.CurrentUser.Role;
            }
        }

        /// <summary>
        /// Initializes a new instance of the HomeViewModel class with a default API service
        /// </summary>
        public HomeViewModel() : this(new ApiService())
        {
        }
    }
}
