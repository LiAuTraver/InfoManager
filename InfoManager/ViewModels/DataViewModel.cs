using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using InfoManager.Models;
using InfoManager.Services;

namespace InfoManager.ViewModels;

public class DataViewModel : ObservableRecipient, INotifyPropertyChanged
{
    private StudentService _studentService = new(null, true);

    public ObservableCollection<Student> Source
    {
        get;
    } = [];

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is not string newFilePath)
        {
            return;
        }

        if (newFilePath != _studentService.FilePath)
        {
            _studentService = new StudentService(newFilePath, true);
        }

        var data = await _studentService.GetGridDataAsync(newFilePath);
        if (data is null)
        {
            return;
        }

        Source.Clear();
        foreach (var student in data)
        {
            Source.Add(student);
        }

        // Default sort by ID
        SortData("Id", true);
    }

    public void SortData(string propertyName, bool isAscending)
    {
        if (propertyName != string.Empty)
        {
            _currentSortPropertyName = propertyName;
        }
        else
        {
            propertyName = _currentSortPropertyName;
        }

        var sortedData = StudentSortService.Sort(Source, propertyName, isAscending);
        Source.Clear();
        foreach (var student in sortedData)
        {
            Source.Add(student);
        }
    } // ReSharper disable MemberCanBePrivate.Global
    public async Task<bool> AddDataAsync(Student pendingAppendStudent)
    {
        _studentService.AddStudent(pendingAppendStudent);
        Source.Add(pendingAppendStudent); // source must be called to update the UI
        await Task.CompletedTask;
        return true;
    }

    private string _currentSortPropertyName = "Id";

    public async Task<bool> AddDataAsync(string id, string name, string grades)
    {
        if (_studentService.FindStudent(id) is not null)
        {
            return false;
        }

        return await AddDataAsync(new Student(id, name, grades));
    }

    // failed, also the name `IsDataChanged` is not appropriate for a nullable return type
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
            await _studentService.RestoreData();
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