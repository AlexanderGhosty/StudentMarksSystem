using Client.Services;

namespace Client.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _login;
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private string _role;
        public string Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }

        // Конструктор
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
        public HomeViewModel() : this(new ApiService())
        {
        }
    }
}
