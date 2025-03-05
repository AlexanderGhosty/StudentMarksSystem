using System.Windows;
using System.Windows.Controls;
using Client.ViewModels;

namespace Client.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            // Подписка на событие из ViewModel
            this.Loaded += (s, e) =>
            {
                if (DataContext is LoginViewModel vm)
                {
                    vm.RequestClose += (sender, args) => this.Close();
                }
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
