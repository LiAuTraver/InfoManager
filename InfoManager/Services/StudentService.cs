using InfoManager.Models;

namespace InfoManager.Services;

using Newtonsoft.Json;

// methods part
public partial class StudentService
{
    public void AddStudent(Student student)
    {
        _students.Add(student);
    }

    public void DeleteStudent(Student student)
    {
        // remove student from list
        _students.Remove(student);
    }

    public Student? FindStudent(string id)
    {
        return _students.Find(student => student.Id == id);
    }

    public Student? FindStudent(int index)
    {
        return _students.Find(student => student.MyIndex == index);
    }

    private async Task ProcessStudentsAsync(string? filePath)
    {
        filePath ??= FilePath;
        FilePath = filePath;
        await GetGridDataAsync(null);
        if (_students.Count == 0)
        {
            throw new InvalidOperationException("No students found.");
        }
    }

    public static async Task<StudentService> CreateAsync(string? filePath, char? delimiter)
    {
        var students = new StudentService(filePath, delimiter);
        await students.ProcessStudentsAsync(null);
        return students;
    }

    public async Task<IEnumerable<Student>?> GetGridDataAsync(string? filePath)
    {
        filePath ??= FilePath;
        FilePath = filePath;
        _students.Clear();
        if (_isJson is not true)
        {
            return await GetGridDataAsyncNotJson(filePath);
        }
        else
        {
            return await GetGridDataAsyncIsJson(filePath);
        }
    }

    private async Task<IEnumerable<Student>> GetGridDataAsyncIsJson(string filePath)
    {
        lock (FileLock)
        {
            var jsonData = File.ReadAllText(filePath) ?? throw new FileNotFoundException("File does not exist.");
            var students = JsonConvert.DeserializeObject<List<Student>>(jsonData) ??
                           throw new JsonException("Failed to parse JSON");
            _students.AddRange(students);
        }

        SaveCopyOf(filePath);
        await Task.CompletedTask;
        return _students;
    }

    private async Task<IEnumerable<Student>?> GetGridDataAsyncNotJson(string filePath)
    {
        // `using` a IDisposable object,
        // so it will be disposed when the block ends(like local variables in C/C++),
        // rather than waiting for the GC to collect it
        using var streamReader = new StreamReader(filePath);
        // Reset the current peek position to the beginning of the file
        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

        // Discard first line
        await streamReader.ReadLineAsync();

        var data = await streamReader.ReadLineAsync();
        string[] studentData = [];
        while (data != null)
        {
            studentData = studentData.Concat(data.Split(_delimiter).Where(x => !string.IsNullOrEmpty(x))).ToArray();
            if (studentData.Length < 3)
            {
                data = await streamReader.ReadLineAsync();
                continue;
            }

            var id = studentData[0];
            var name = studentData[1];
            var grades = studentData.Skip(2).Select(double.Parse).ToList() ?? throw new FormatException("ParseError");
            _students.Add(new Student(name, id, grades));
            studentData = [];
            data = await streamReader.ReadLineAsync();
        }

        SaveCopyOf(filePath);
        return _students;
    }

    private void SaveCopyOf(string? newCopyFilePath)
    {
        // FIXME: lock is not working

        // lock (FileLock)
        // {
        // File.Copy(FilePath, newCopyFilePath, true);
        // File.SetAttributes(newCopyFilePath, FileAttributes.Hidden);
        // }
    }

    public async Task SaveStudentsAsync(string? filePath)
    {
        filePath ??= FilePath;
        FilePath = filePath;
        lock (FileLock)
        {
            if (_isJson is not true)
            {
                SaveDataNotJson(filePath);
            }
            else
            {
                SaveDataJson(filePath);
            }
            SaveCopyOf(filePath);
        }
        await Task.CompletedTask;
    }


    private void SaveDataJson(string filePath)
    {
        var jsonData = JsonConvert.SerializeObject(_students);
        // lock (FileLock)
        // {
            File.WriteAllText(filePath, jsonData);
        // }
    }

    private void SaveDataNotJson(string filePath)
    {
        // lock (FileLock)
        // {
            using var streamWriter = new StreamWriter(filePath);
            streamWriter.WriteLine("ID,Grade,Name");
            foreach (var student in _students)
            {
                streamWriter.WriteLine($"{student.Id},{string.Join(_delimiter, student.Grades)},{student.Name}");
            }
        // }
    }

    // not working
    public bool? IsStudentInfoChanged(Student student)
    {
        var originalStudent = FindStudent(student.MyIndex);
        if (originalStudent is null)
        {
            return null;
        }

        if (originalStudent == student)
        {
            return false;
        }

        var index = _students.FindIndex(s => s.MyIndex == student.MyIndex);
        _students[index] = student;
        return true;
    }

    public async Task RestoreData()
    {
        _students.Clear();
        // FIXME: lock is not working
        //File.Copy(BackupFilePath, FilePath, true);
        await GetGridDataAsync(FilePath);
    }
}

// ctor and vars part
public partial class StudentService
{
    private readonly char _delimiter;
    private readonly bool _isJson;

    public string FilePath
    {
        get;
        set;
    }

    private static readonly string DefaultFilePath = Path.Combine(AppContext.BaseDirectory, "data.json");
    private static readonly string BackupFilePath = Path.Combine(AppContext.BaseDirectory, "data.bak");
    private readonly List<Student> _students = [];
    private static readonly object FileLock = new();

    public StudentService(string? filePath, char? delimiter)
    {
        _delimiter = delimiter ?? ' ';
        FilePath = filePath ?? DefaultFilePath;
        _isJson = false;
    }

    public StudentService(string? filePath, bool isJson)
    {
        _isJson = isJson;
        FilePath = filePath ?? DefaultFilePath;
        _delimiter = ' ';
    }
}

// WRONG: in code below, originalStudent is a local reference to the student object in _students,
// so changing originalStudent will not change the student object in _students
// it will only change the local reference to that new student object

// if(FindStudent(student.MyIndex) is not { } originalStudent)
// {
//     // means student is not found; shouldn't happen
//     Console.WriteLine(@"Oops! You Shouldn't be here.");
//     return null;
// }
//
// if (student == originalStudent)
// {
//     return false;
// }
// // update student info
// originalStudent = student.Clone() as Student;
// return true;