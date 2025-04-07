using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Views
{
    public partial class RenameSubjectDialogView : Window
    {
        public RenameSubjectDialogView()
        {
            InitializeComponent();

            // Когда окно загрузится, подпишемся на RequestClose
            this.Loaded += RenameSubjectDialog_Loaded;
        }

        private void RenameSubjectDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is RenameSubjectDialogViewModel vm)
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