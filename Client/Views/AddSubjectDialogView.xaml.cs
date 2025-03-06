using Client.ViewModels;
using System;
using System.Windows;

namespace Client.Views
{
    public partial class AddSubjectDialogView : Window
    {
        public AddSubjectDialogView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Подпишемся на RequestClose
            if (this.DataContext is AddSubjectDialogViewModel vm)
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
