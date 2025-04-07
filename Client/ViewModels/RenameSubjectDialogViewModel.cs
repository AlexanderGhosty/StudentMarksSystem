using Client.ViewModels;
using Client;
using System.Windows.Input;
using System.Windows;

public class RenameSubjectDialogViewModel : BaseViewModel
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

    public bool DialogResultOk { get; private set; }

    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }

    public event EventHandler RequestClose;

    // Передаём старое название, чтобы пользователь видел, что именно редактирует
    public RenameSubjectDialogViewModel(string currentTitle)
    {
        _subjectTitle = currentTitle;

        OkCommand = new RelayCommand(_ => OnOk());
        CancelCommand = new RelayCommand(_ => OnCancel());
    }

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

    private void OnCancel()
    {
        DialogResultOk = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
