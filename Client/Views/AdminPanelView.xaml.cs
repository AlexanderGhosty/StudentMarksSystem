using System.Windows;
using System.Windows.Controls;
using Client.ViewModels;

namespace Client.Views
{
    public partial class AdminPanelView : UserControl
    {
        public AdminPanelView()
        {
            InitializeComponent();
            // DataContext может устанавливаться автоматически через MainViewModel
        }
    }
}
