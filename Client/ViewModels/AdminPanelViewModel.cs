using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Models;
using Client.Services;
using Client.Views;
using System.Windows;


namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the admin panel that provides functionality to manage users
    /// </summary>
    public class AdminPanelViewModel : BaseViewModel
    {
        /// <summary>
        /// Collection of all users in the system
        /// </summary>
        public ObservableCollection<User> Users { get; set; }
            = new ObservableCollection<User>();

        private User _selectedUser;
        /// <summary>
        /// Gets or sets the currently selected user in the admin panel
        /// </summary>
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }


        // Commands
        /// <summary>
        /// Command to load all users from the server
        /// </summary>
        public ICommand LoadUsersCommand { get; }

        /// <summary>
        /// Command to refresh the users list from the server
        /// </summary>
        public ICommand RefreshUsersCommand { get; }

        /// <summary>
        /// Command to add a new user to the system
        /// </summary>
        public ICommand AddUserCommand { get; }

        /// <summary>
        /// Command to delete the selected user from the system
        /// </summary>
        public ICommand DeleteUserCommand { get; }

        private readonly IApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the AdminPanelViewModel class
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        public AdminPanelViewModel(IApiService apiService)
        {
            _apiService = apiService;

            LoadUsersCommand = new RelayCommand(async _ => await LoadUsersAsync());
            // "Refresh" will call the same method again
            RefreshUsersCommand = new RelayCommand(async _ => await LoadUsersAsync());

            AddUserCommand = new RelayCommand(async _ => await AddUserAsync());
            DeleteUserCommand = new RelayCommand(async _ => await DeleteUserAsync());

            // Load users on initialization
            LoadUsersCommand.Execute(null);
        }

        /// <summary>
        /// Loads all users from the server
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
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

        /// <summary>
        /// Opens a dialog to add a new user and sends the data to the server
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        private async Task AddUserAsync()
        {
            try
            {
                // Open add user dialog
                var dialog = new AddUserDialog();
                var dialogVm = new AddUserDialogViewModel();
                dialog.DataContext = dialogVm;
                // dialog.Owner = Application.Current.MainWindow;

                bool? result = dialog.ShowDialog();

                if (result == true && dialogVm.DialogResultOk)
                {
                    var newUser = new User
                    {
                        Name = dialogVm.UserName,
                        Login = dialogVm.Login,
                        PasswordHash = dialogVm.PasswordHash,
                        Role = dialogVm.SelectedRole
                    };

                    // Send POST /users
                    var createResult = await _apiService.CreateUserAsync(newUser);
                    if (createResult.IsSuccess && createResult.CreatedUser != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Users.Add(createResult.CreatedUser);
                        });
                    }
                    else
                    {
                        MessageBox.Show(
                            createResult.ErrorMessage ?? "Неизвестная ошибка при создании пользователя.",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding user: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes the currently selected user from the system
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
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
