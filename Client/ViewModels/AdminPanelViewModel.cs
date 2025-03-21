﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Models;
using Client.Services;
using Client.Views;
using System.Windows;


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


        // Команды
        public ICommand LoadUsersCommand { get; }
        public ICommand RefreshUsersCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        private readonly IApiService _apiService;

        public AdminPanelViewModel(IApiService apiService)
        {
            _apiService = apiService;

            LoadUsersCommand = new RelayCommand(async _ => await LoadUsersAsync());
            // "Обновить" будет повторно вызывать тот же метод
            RefreshUsersCommand = new RelayCommand(async _ => await LoadUsersAsync());

            AddUserCommand = new RelayCommand(async _ => await AddUserAsync());
            DeleteUserCommand = new RelayCommand(async _ => await DeleteUserAsync());

            // Загрузим пользователей при инициализации
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

        private async Task AddUserAsync()
        {
            try
            {
                // Открываем диалоговое окно добавления
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

                    // Отправляем POST /users
                    var createResult = await _apiService.CreateUserAsync(newUser);
                    if (createResult.IsSuccess && createResult.CreatedUser != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Users.Add(createResult.CreatedUser);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении пользователя: " + ex.Message);
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
