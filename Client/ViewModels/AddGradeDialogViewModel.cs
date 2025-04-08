using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Client.Models;
using Client.Services;

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the dialog that allows adding grades to students.
    /// Handles search functionality and grade selection.
    /// </summary>
    public class AddGradeDialogViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddGradeDialogViewModel"/> class.
        /// </summary>
        /// <param name="apiService">Service for making API requests to the server.</param>
        public AddGradeDialogViewModel(IApiService apiService)
        {
            _apiService = apiService;

            Users = new ObservableCollection<User>();

            SearchCommand = new RelayCommand(async _ => await SearchUsersAsync());

            // По умолчанию «5»
            SelectedGrade = "5";
        }

        private string _searchText;
        /// <summary>
        /// Gets or sets the search text entered by the user to filter students by name.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets the collection of users (students) that match the search criteria.
        /// </summary>
        public ObservableCollection<User> Users { get; set; }

        private User _selectedUser;
        /// <summary>
        /// Gets or sets the currently selected student from the search results.
        /// </summary>
        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        // ComboBox = "1", "2", "3", "4", "5"
        private object _selectedGrade;
        /// <summary>
        /// Gets or sets the currently selected grade.
        /// ComboBox options are "1", "2", "3", "4", "5".
        /// Default value is "5".
        /// </summary>
        public string SelectedGrade
        {
            get => _selectedGrade?.ToString();
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length >= 39)
                {
                    _selectedGrade = value.Substring(38, 1);
                }
                else
                {
                    _selectedGrade = value;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Command that is executed when the search button is clicked.
        /// Triggers the student search based on the entered search text.
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// Asynchronously searches for users with the "student" role whose names
        /// match the search criteria entered by the user.
        /// Populates the Users collection with matching results.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SearchUsersAsync()
        {
            var result = await _apiService.GetAllUsersAsync();
            if (result.IsSuccess && result.Users != null)
            {
                var filtered = result.Users
                    .Where(u => u.Role == "student")
                    .Where(u => u.Name.ToLower().Contains(SearchText?.ToLower() ?? ""));

                Users.Clear();
                foreach (var u in filtered)
                {
                    Users.Add(u);
                }

                if (!Users.Any())
                {
                    MessageBox.Show("Не найдено ни одного студента по данному запросу.");
                }
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке списка пользователей.");
            }
        }
    }
}
