namespace InfoManager.Services;

public sealed class Student(string name, string id, List<double> grades)
{
    public string Name
    {
        get;
        set;
    } = name;

    public string Id
    {
        get;
        set;
    } = id;

    public List<double> Grades
    {
        get;
        set;
    } = grades;

    public string GradesString
    {
        get => string.Join(", ", Grades);
        set => Grades = value.Split(',').Select(double.Parse).ToList();
    }

    public double Average => Grades.Any() ? Grades.Average() : 0;
}