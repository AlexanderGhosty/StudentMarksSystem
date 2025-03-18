using System.Windows;
using Client.Services;
using Client.ViewModels;

namespace Client.Views
{
    public partial class GradeDialog : Window
    {
        public GradeDialogViewModel ViewModel { get; private set; }

        public GradeDialog(IApiService _apiService)
        {
            InitializeComponent();

            // Создаём ViewModel и присваиваем DataContext
            ViewModel = new GradeDialogViewModel(_apiService);
            DataContext = ViewModel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что выбраны студент и оценка
            if (ViewModel.SelectedUser == null)
            {
                MessageBox.Show("Пожалуйста, выберите студента из списка.");
                return;
            }
            if (string.IsNullOrEmpty(ViewModel.SelectedGrade))
            {
                MessageBox.Show("Пожалуйста, выберите оценку (от 1 до 5).");
                return;
            }
            // При успешном заполнении закрываем диалог с DialogResult = true
            DialogResult = true;
            Close();
        }
    }
}
