﻿using System.Windows.Input;
using System.Windows.Navigation;
using Client.Models;
using Client.Services;

namespace Client.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _login;
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
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        // Команда входа
        public ICommand LoginCommand { get; }

        private readonly IApiService _apiService;
        private readonly NavigationService _navigation; // Условный сервис навигации

        public LoginViewModel(IApiService apiService, NavigationService nav)
        {
            _apiService = apiService;
            _navigation = nav;
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
        }

        private async System.Threading.Tasks.Task LoginAsync()
        {
            ErrorMessage = string.Empty;
            var response = await _apiService.LoginAsync(Login, Password);
            if (response.IsSuccess)
            {
                // Сохраняем текущего пользователя
                GlobalState.CurrentUser = response.User;
                // Переходим к основному окну
                _navigation.ShowMainWindow();
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }
        }
    }
}
