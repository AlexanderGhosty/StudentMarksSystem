using System.Windows;
using Client.ViewModels;

namespace Client.Views
{
    public partial class AddUserDialog : Window
    {
        public AddUserDialog()
        {
            InitializeComponent();

            // Когда окно загрузится, подпишемся на RequestClose
            this.Loaded += AddUserDialog_Loaded;
        }

        private void AddUserDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Если DataContext - наша VM, подпишемся на событие RequestClose
            if (DataContext is AddUserDialogViewModel vm)
            {
                vm.RequestClose += (s, args) =>
                {
                    this.DialogResult = vm.DialogResultOk;
                    this.Close();
                };
            }
        }
    }
}
