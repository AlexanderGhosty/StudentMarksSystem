using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Client.Models;

namespace Client.ViewModels
{
    public class AddSubjectDialogViewModel : BaseViewModel
    {
        private string _subjectTitle;
        public string SubjectTitle
        {
            get => _subjectTitle;
            set
            {
                _subjectTitle = value;
                OnPropertyChanged();
            }
        }


        public bool DialogResultOk { get; private set; } = false;

        // Команды
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        // Событие для закрытия окна
        public event EventHandler RequestClose;

        public AddSubjectDialogViewModel()
        {
            OkCommand = new RelayCommand(_ => OnOk());
            CancelCommand = new RelayCommand(_ => OnCancel());
        }

        private void OnOk()
        {
            if (string.IsNullOrWhiteSpace(SubjectTitle))
            {
                return;
            }

            DialogResultOk = true;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void OnCancel()
        {
            DialogResultOk = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
