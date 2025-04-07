using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Client;
using Client.Models;
using Client.Services;

namespace Client.ViewModels
{
    public class AddGradeDialogViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        public AddGradeDialogViewModel(IApiService apiService)
        {
            // Экземпляр ApiService для запросов на сервер
            _apiService = apiService;

            // Коллекция результатов поиска
            Users = new ObservableCollection<User>();

            // Команда поиска
            SearchCommand = new RelayCommand(async _ => await SearchUsersAsync());

            // По умолчанию «5»
            SelectedGrade = "5";
        }

        // Текст, который вводит пользователь для поиска по имени
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        // Результат поиска — список пользователей (студентов)
        public ObservableCollection<User> Users { get; set; }

        // Текущий выбранный студент
        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        // Список оценок в ComboBox = "1", "2", "3", "4", "5"
        private object _selectedGrade;
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

        // Команда, вызываемая при нажатии кнопки "Найти"
        public ICommand SearchCommand { get; }

        // Логика поиска пользователей (только студенты)
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
