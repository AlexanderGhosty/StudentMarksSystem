using System.Windows;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class AddUserDialogViewModel : BaseViewModel
    {
        private string _userName;
        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        private string _login;
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _selectedRole = "student";
        public string SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
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

        public bool DialogResultOk { get; private set; } = false;

        // Событие, сигнализирующее о необходимости закрыть диалог
        public event EventHandler RequestClose;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddUserDialogViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        

void Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserName) ||
                    string.IsNullOrWhiteSpace(Login) ||
                    string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Все поля (Имя, Логин, Пароль) должны быть заполнены.";
                    return;
                }

                DialogResultOk = true;
                OnRequestClose();
            }
            catch (Exception ex)
            {
                // Можно добавить логирование
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
            }
        }

        private void Cancel()
        {
            // Отмена
            DialogResultOk = false;
            OnRequestClose();
        }

        private void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
