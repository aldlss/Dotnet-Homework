using System;
using System.ComponentModel.DataAnnotations;

namespace homework6.Models;

public class School
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

public class PackageSchool: IListItem
{
    public static PackageSchool Package() {
        return new PackageSchool(new School());
    }
    public School Self { get; set; }
    string IListItem.ShowText
    {
        get => $"{Self.Name} {Self.Address}";
    }
    public PackageSchool(School school) {
        Self = school;
    }
}
