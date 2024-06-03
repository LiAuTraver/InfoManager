using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InfoManager.Services;
using System.Linq;

namespace InfoManager.ViewModels;

public partial class DataViewModel : ObservableRecipient
{
    private readonly Students _students = new(null, null);

    public ObservableCollection<Student> Source { get; } = new();

    public IRelayCommand SortByIdCommand { get; }
    public IRelayCommand SortByAverageCommand { get; }
    public IRelayCommand SortByNameCommand { get; }
    public DataViewModel()
    {
        SortByIdCommand = new RelayCommand(SortById);
        SortByAverageCommand = new RelayCommand(SortByAverage);
        SortByNameCommand = new RelayCommand(SortByName);
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is not string newFilePath)
        {
            return;
        }

        var data = await _students.GetGridDataAsync(newFilePath);
        if (data is null)
        {
            return;
        }
        Source.Clear();
        foreach (var student in data)
        {
            Source.Add(student);
        }
        SortById(); // Default sort by ID
    }

    private void SortById()
    {
        var sortedData = Source.OrderBy(student =>
        {
            if (int.TryParse(student.Id, out var numericId))
            {
                return numericId;
            }
            return int.MaxValue; // Handle non-numeric IDs by putting them at the end
        }).ThenBy(student => student.Id).ToList();

        UpdateSource(sortedData);
    }

    private void UpdateSource(List<Student> sortedData)
    {
        Source.Clear();
        foreach (var student in sortedData)
        {
            Source.Add(student);
        }
    }

    private void SortByAverage()
    {
        var sortedData = Source.OrderBy(student => student.Average).ToList();
        UpdateSource(sortedData);
    }

    private void SortByName()
    {
        var sortedData = Source.OrderBy(student => student.Name).ToList();
        UpdateSource(sortedData);
    }

    public async Task AddDataAsync(Student student)
    {
        _students.AddStudent(student);
        Source.Add(student);
        await Task.CompletedTask;
    }

    public string GetPath()
    {
        return _students.FilePath;
    }
}