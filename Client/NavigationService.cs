namespace Client
{
    public class NavigationService
    {
        public void ShowMainWindow()
        {
            // Закрыть окно авторизации, открыть MainWindow
            var mainWindow = new Views.MainWindow();
            mainWindow.Show();

            // Можно найти и закрыть LoginView
            // ...
        }
    }
}
