using System.Windows.Input;

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the dialog that allows adding a new user.
    /// Handles user input validation and dialog result.
    /// </summary>
    public class AddUserDialogViewModel : BaseViewModel
    {
        private string _userName;
        /// <summary>
        /// Gets or sets the name of the user being added.
        /// </summary>
        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        private string _login;
        /// <summary>
        /// Gets or sets the login (username) for the user being added.
        /// </summary>
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        private string _passwordHash;
        /// <summary>
        /// Gets or sets the password hash for the user being added.
        /// </summary>
        public string PasswordHash
        {
            get => _passwordHash;
            set { _passwordHash = value; OnPropertyChanged(); }
        }

        private string _selectedRole = "student";
        /// <summary>
        /// Gets or sets the selected role for the user being added.
        /// Default value is "student".
        /// </summary>
        public string SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        /// <summary>
        /// Gets or sets the error message to display when validation fails.
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

        /// <summary>
        /// Gets a value indicating whether the dialog was confirmed (Save button was clicked).
        /// </summary>
        public bool DialogResultOk { get; private set; } = false;

        /// <summary>
        /// Event that signals the dialog should be closed.
        /// </summary>
        public event EventHandler RequestClose;

        /// <summary>
        /// Command that executes when the Save button is clicked.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Command that executes when the Cancel button is clicked.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddUserDialogViewModel"/> class.
        /// </summary>
        public AddUserDialogViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        /// <summary>
        /// Handles the Save button click event.
        /// Validates that all required fields are filled and sets dialog result to true.
        /// </summary>
        void Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserName) ||
                    string.IsNullOrWhiteSpace(Login) ||
                    string.IsNullOrWhiteSpace(PasswordHash))
                {
                    ErrorMessage = "All fields (Name, Login, Password) must be filled.";
                    return;
                }

                DialogResultOk = true;
                OnRequestClose();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Handles the Cancel button click event.
        /// Sets dialog result to false and requests dialog closure.
        /// </summary>
        private void Cancel()
        {
            DialogResultOk = false;
            OnRequestClose();
        }

        /// <summary>
        /// Raises the RequestClose event to signal that the dialog should be closed.
        /// </summary>
        private void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
