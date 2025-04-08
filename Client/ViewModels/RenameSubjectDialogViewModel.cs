using Client.ViewModels;
using Client;
using System.Windows.Input;
using System.Windows;

/// <summary>
/// ViewModel for the dialog that allows users to rename a subject.
/// Implements the MVVM pattern for managing subject renaming operations.
/// </summary>
public class RenameSubjectDialogViewModel : BaseViewModel
{
    private string _subjectTitle;

    /// <summary>
    /// Gets or sets the title of the subject being renamed.
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
    /// Gets a value indicating whether the dialog was closed with OK result.
    /// </summary>
    public bool DialogResultOk { get; private set; }

    /// <summary>
    /// Gets the command that confirms renaming and closes the dialog.
    /// </summary>
    public ICommand OkCommand { get; }

    /// <summary>
    /// Gets the command that cancels renaming and closes the dialog.
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <summary>
    /// Event that is raised when the dialog needs to be closed.
    /// </summary>
    public event EventHandler RequestClose;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenameSubjectDialogViewModel"/> class.
    /// </summary>
    /// <param name="currentTitle">The current title of the subject being renamed.</param>
    public RenameSubjectDialogViewModel(string currentTitle)
    {
        _subjectTitle = currentTitle;

        OkCommand = new RelayCommand(_ => OnOk());
        CancelCommand = new RelayCommand(_ => OnCancel());
    }

    /// <summary>
    /// Handles the OK button click by validating input and closing the dialog with success.
    /// </summary>
    private void OnOk()
    {
        if (string.IsNullOrWhiteSpace(SubjectTitle))
        {
            MessageBox.Show("Введите новое название предмета.");
            return;
        }

        DialogResultOk = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the Cancel button click by closing the dialog without saving changes.
    /// </summary>
    private void OnCancel()
    {
        DialogResultOk = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
