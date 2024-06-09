﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0660, CS0661
using System.ComponentModel;
using Newtonsoft.Json;

namespace InfoManager.Models;

public sealed class Student : INotifyPropertyChanged, ICloneable, IEquatable<Student>
{
    private static int Index
    {
        get;
        set;
    }

    public int MyIndex
    {
        get;
    }

    [JsonConstructor] // needed for deserialization otherwise it will throw an error because there are two ctors which take 3 parameters
    public Student(string name, string id, List<double> grades)
    {
        Name = name;
        Id = id;
        Grades = grades;
        Index++;
        MyIndex = Index;
    }

    public Student(string name, string id, string gradeString)
    {
        Name = name;
        Id = id;
        GradesString = gradeString;
        Index++;
        MyIndex = Index;
    }

    public string Name
    {
        get;
        set;
    }

    public string Id
    {
        get;
        set;
    }

    private List<double> _grades;

    public List<double> Grades
    {
        get => _grades;
        set
        {
            _grades = value;
            GradesString = string.Join(", ", _grades);
        }
    }

    private string _gradesString;

    public string GradesString
    {
        get => _gradesString;
        set
        {
            _gradesString = value;
            _grades = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => double.Parse(s.Trim()))
                .ToList();
        }
    }


    public double Average => Grades.Any() ? Grades.Average() : 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    // private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    // {
    //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    // }

    // public void UpdateInfo(Student? student)
    // {
    //     if (student == null)
    //     {
    //         return;
    //     }
    //
    //     if (student != this)
    //     {
    //         Name = student.Name;
    //         Id = student.Id;
    //         Grades = student.Grades;
    //     }
    //
    //     // dunno whether it's redundant or not
    //     GradesString = string.Join(", ", Grades);
    // }

    public object Clone()
    {
        // deep copy
        return new Student(Name, Id, Grades.ToList());
    }

    public static bool operator ==(Student? left, Student? right) => Equals(left, right);

    public static bool operator !=(Student? left, Student? right) => !(left == right);

    public bool Equals(Student? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null)
        {
            return false;
        }

        return _grades.Equals(other._grades) && _gradesString == other._gradesString && MyIndex == other.MyIndex &&
               Name == other.Name && Id == other.Id;
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public override bool Equals(object? obj)
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        return Equals(obj is Student student ? student : null);
    }
}