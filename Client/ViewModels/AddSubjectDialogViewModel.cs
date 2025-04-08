using System.Windows.Input;

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the dialog that allows adding a new subject.
    /// Handles input validation and dialog result.
    /// </summary>
    public class AddSubjectDialogViewModel : BaseViewModel
    {
        private string _subjectTitle;
        /// <summary>
        /// Gets or sets the title of the new subject being added.
        /// </summary>
        public string SubjectTitle
        {
            get => _subjectTitle;
            set
            {
                _subjectTitle = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dialog was confirmed (OK button was clicked).
        /// </summary>
        public bool DialogResultOk { get; private set; } = false;

        // Commands
        /// <summary>
        /// Command that executes when the OK button is clicked.
        /// </summary>
        public ICommand OkCommand { get; }

        /// <summary>
        /// Command that executes when the Cancel button is clicked.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Event that is raised when the dialog should be closed.
        /// </summary>
        public event EventHandler RequestClose;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSubjectDialogViewModel"/> class.
        /// </summary>
        public AddSubjectDialogViewModel()
        {
            OkCommand = new RelayCommand(_ => OnOk());
            CancelCommand = new RelayCommand(_ => OnCancel());
        }

        /// <summary>
        /// Handles the OK button click event.
        /// Validates that subject title is not empty and sets dialog result to true.
        /// </summary>
        private void OnOk()
        {
            if (string.IsNullOrWhiteSpace(SubjectTitle))
            {
                return;
            }

            DialogResultOk = true;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Cancel button click event.
        /// Sets dialog result to false and requests dialog closure.
        /// </summary>
        private void OnCancel()
        {
            DialogResultOk = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
