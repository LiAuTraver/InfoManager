using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InfoManager.Models;
using InfoManager.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PropertyChangedEventArgs = ABI.System.ComponentModel.PropertyChangedEventArgs;

namespace InfoManager.ViewModels;

public partial class DataViewModel : ObservableRecipient, INotifyPropertyChanged
{
    private readonly StudentService _studentService = new(null, true);
    private readonly StudentService _previousStudentService = new(null, true);

    public ObservableCollection<Student> Source
    {
        get;
    } = [];

    public IRelayCommand SortByIdCommand
    {
        get;
    }

    public IRelayCommand SortByAverageCommand
    {
        get;
    }

    public IRelayCommand SortByNameCommand
    {
        get;
    }

    public DataViewModel()
    {
        SortByIdCommand = new RelayCommand<bool>(SortById);
        SortByAverageCommand = new RelayCommand<bool>(SortByAverage);
        SortByNameCommand = new RelayCommand<bool>(SortByName);
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is not string newFilePath)
        {
            return;
        }

        var data = await _studentService.GetGridDataAsync(newFilePath);
        if (data is null)
        {
            return;
        }

        // save a copy of the original data
        // todo: how to handle null data?
        // _originalData = _students.Clone() as Students ?? new Students(null, null);
        Source.Clear();
        foreach (var student in data)
        {
            Source.Add(student);
        }

        SortById(true); // Default sort by ID
    }

    private void UpdateSource(List<Student> sortedData, object parameter)
    {
        if (parameter is not bool isAscending)
        {
            return;
        }

        if (isAscending is not true)
        {
            sortedData.Reverse();
        }

        Source.Clear();
        foreach (var student in sortedData)
        {
            Source.Add(student);
        }
    }

    private void SortById(bool isAscending)
    {
        var sortedData = Source
            .OrderBy(student => int.TryParse(student.Id, out var numericId) ? numericId : int.MaxValue)
            .ThenBy(student => student.Id).ToList();
        UpdateSource(sortedData, isAscending);
    }

    private void SortByAverage(bool isAscending)
    {
        var sortedData = Source.OrderBy(student => student.Average).ToList();
        UpdateSource(sortedData, isAscending);
    }

    private void SortByName(bool isAscending)
    {
        var sortedData = Source.OrderBy(student => student.Name).ToList();
        UpdateSource(sortedData, isAscending);
    }

    // ReSharper disable MemberCanBePrivate.Global
    public async Task<bool> AddDataAsync(Student pendingAppendStudent)
    // ReSharper restore MemberCanBePrivate.Global
    {
        _studentService.AddStudent(pendingAppendStudent);
        Source.Add(pendingAppendStudent); // source must be called to update the UI
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> AddDataAsync(string id, string name, string grades)
    {
        if (_studentService.FindStudent(id) is not null)
        {
            return false;
        }

        return await AddDataAsync(new Student(id, name, grades));
    }

    public bool? IsDataChanged(Student student)
        => _studentService.IsStudentInfoChanged(student)
            switch
        {
            true =>
                // todo:update data in the UI
                true,
            false =>
                // todo:also need to update the UI
                false,
            _ => null
        };

    public async Task<bool> DeleteDataAsync(string id)
    {
        if (_studentService.FindStudent(id) is not { } pendingDroppedStudent)
        {
            return false;
        }

        _studentService.DeleteStudent(pendingDroppedStudent);
        Source.Remove(pendingDroppedStudent);
        await Task.CompletedTask;
        return true;
    }

    public async Task SaveDataToFileAsync(bool isOverwrite)
    {
        if (isOverwrite is not true)
        {
            await _previousStudentService.SaveStudentsAsync(null);
        }
        else
        {
            await _studentService.SaveStudentsAsync(null);
        }
    }
}
// inherited from ObservableRecipient, no need to implement
// public event PropertyChangedEventHandler? PropertyChanged;
// protected virtual void OnPropertyChanged(string propertyName)
// {
//     PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
// }


// not used or not working

//     public async Task ConfirmBeforeClosing(NavigatingCancelEventArgs e)
// {
//     var warningDialog = new ContentDialog
//     {
//         Title = "Warning",
//         Content = "You have unsaved changes. Save or discard before leaving.",
//         PrimaryButtonText = "Stay",
//         SecondaryButtonText = "Leave",
//         DefaultButton = ContentDialogButton.Primary,
//         XamlRoot = App.MainWindow.Content.XamlRoot
//     };
//     e.Cancel = await warningDialog.ShowAsync() == ContentDialogResult.Primary;
// }

//private async void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
//{
//    if (sender is not Student student)
//    {
//        return;
//    }

//    var originalStudent = Source.FirstOrDefault(s => s.MyIndex == student.MyIndex);
//    if (originalStudent == null)
//    {
//        // handle property change
//        await AddDataAsync(student);
//        return;
//    }

//    originalStudent.UpdateInfo(student);
//}