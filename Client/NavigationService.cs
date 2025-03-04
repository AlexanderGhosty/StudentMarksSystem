// NavigationService.cs
using Client.Views;

namespace Client
{
    public class NavigationService
    {
        public void ShowMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        public void ShowLoginWindow()
        {
            var loginWindow = new LoginView();
            loginWindow.Show();
        }
    }
}
