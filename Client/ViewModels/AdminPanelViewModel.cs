using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Models;
using Client.Services;

namespace Client.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        public ObservableCollection<User> Users { get; set; }
            = new ObservableCollection<User>();

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadUsersCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        private readonly IApiService _apiService;

        public AdminPanelViewModel(IApiService apiService)
        {
            _apiService = apiService;
            LoadUsersCommand = new RelayCommand(async _ => await LoadUsersAsync());
            AddUserCommand = new RelayCommand(async _ => await AddUserAsync());
            DeleteUserCommand = new RelayCommand(async _ => await DeleteUserAsync());

            // Загрузим при инициализации
            LoadUsersCommand.Execute(null);
        }

        private async System.Threading.Tasks.Task LoadUsersAsync()
        {
            Users.Clear();
            var result = await _apiService.GetAllUsersAsync();
            if (result.IsSuccess)
            {
                foreach (var u in result.Users)
                    Users.Add(u);
            }
        }

        private async System.Threading.Tasks.Task AddUserAsync()
        {
            // В реальном приложении открываем диалог, получаем данные
            var newUser = new User
            {
                Name = "Новый пользователь",
                Login = "new_login",
                Password = "1234",
                Role = "teacher"
            };
            var result = await _apiService.CreateUserAsync(newUser);
            if (result.IsSuccess && result.CreatedUser != null)
            {
                Users.Add(result.CreatedUser);
            }
        }

        private async System.Threading.Tasks.Task DeleteUserAsync()
        {
            if (SelectedUser == null) return;
            var result = await _apiService.DeleteUserAsync(SelectedUser.Id);
            if (result.IsSuccess)
            {
                Users.Remove(SelectedUser);
            }
        }
    }
}
