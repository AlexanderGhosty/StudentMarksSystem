using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Models;
using Client.Services;

namespace Client.ViewModels
{
    public class SubjectsViewModel : BaseViewModel
    {
        public ObservableCollection<Subject> Subjects { get; set; }
            = new ObservableCollection<Subject>();

        private Subject _selectedSubject;
        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged();
                // Можно подгрузить оценки по выбранному предмету и т.д.
            }
        }

        public ICommand LoadSubjectsCommand { get; }
        public ICommand AddSubjectCommand { get; }
        public ICommand DeleteSubjectCommand { get; }

        private readonly IApiService _apiService;

        public SubjectsViewModel(IApiService apiService)
        {
            _apiService = apiService;
            LoadSubjectsCommand = new RelayCommand(async _ => await LoadSubjectsAsync());
            AddSubjectCommand = new RelayCommand(async _ => await AddSubjectAsync());
            DeleteSubjectCommand = new RelayCommand(async _ => await DeleteSubjectAsync());

            LoadSubjectsCommand.Execute(null);
        }

        private async System.Threading.Tasks.Task LoadSubjectsAsync()
        {
            Subjects.Clear();
            var result = await _apiService.GetAllSubjectsAsync();
            if (result.IsSuccess)
            {
                foreach (var s in result.Subjects)
                    Subjects.Add(s);
            }
        }

        private async System.Threading.Tasks.Task AddSubjectAsync()
        {
            var newSubject = new Subject
            {
                Title = "Новый предмет",
                TeacherId = null
            };
            var result = await _apiService.CreateSubjectAsync(newSubject);
            if (result.IsSuccess && result.CreatedSubject != null)
            {
                Subjects.Add(result.CreatedSubject);
            }
        }

        private async System.Threading.Tasks.Task DeleteSubjectAsync()
        {
            if (SelectedSubject == null) return;
            var result = await _apiService.DeleteSubjectAsync(SelectedSubject.Id);
            if (result.IsSuccess)
            {
                Subjects.Remove(SelectedSubject);
            }
        }
    }
}
