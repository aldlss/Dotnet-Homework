using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace homework6.Models;

[PrimaryKey(nameof(Id), nameof(SchoolName))]
public class Student {
    [Key]
    public int Idd { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string ClassName { get; set; }
    public string SchoolName { get; set; }
}

public class PackageStudent:IListItem {
    static public PackageStudent Package() {
        return new PackageStudent(new Student());
    }
    public Student Self { get; set; }
    public string ShowText {
        get => $"{Self.Id} {Self.Name} {Self.SchoolName} {Self.SchoolName}";
    }
    public PackageStudent(Student student) {
        Self = student;
    }
}