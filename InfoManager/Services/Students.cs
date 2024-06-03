namespace InfoManager.Services;
public class Students
{
    private readonly char _delimiter;

    public string FilePath
    {
        get;
        private set;
    }
    private const string DefaultFilePath = @"N:\Extracted\StudentScoreManagementSystem\StudentScoreManagementSystem\data\student.txt";
    private readonly List<Student> _students = [];

    public Students(string? filePath, char? delimiter)
    {
        _delimiter = delimiter ?? ' ';
        FilePath = filePath ?? DefaultFilePath;
        if (!File.Exists(FilePath))
        {
            throw new FileNotFoundException("File does not exist.");
        }
    }

    public void AddStudent(Student student)
    {
        _students.Add(student);
    }

    private async Task ProcessStudentsAsync(string? filePath)
    {
        filePath ??= FilePath;
        FilePath = filePath;
        var students = await GetGridDataAsync(null);
        if (students == null)
        {
            throw new Exception("No students found.");
        }
    }

    public static async Task<Students> CreateAsync(string? filePath, char? delimiter)
    {
        var students = new Students(filePath, delimiter);
        await students.ProcessStudentsAsync(null);
        return students;
    }

    public async Task<IEnumerable<Student>?> GetGridDataAsync(string? filePath)
    {
        filePath ??= FilePath;
        FilePath = filePath;
        _students.Clear();

        using (var streamReader = new StreamReader(filePath))
        {
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
                var grades = studentData.Skip(2).Select(double.Parse).ToList();
                _students.Add(new Student(name, id, grades));
                studentData = [];
                data = await streamReader.ReadLineAsync();
            }
        }

        return _students;
    }
}