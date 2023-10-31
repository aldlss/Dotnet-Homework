using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace homework6.Models;

public class Class {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Major { get; set; }
    public string SchoolName { get; set; }
}

public class PackageClass:IListItem {
    static public PackageClass Package() {
        return new PackageClass(new Class());
    }
    public Class Self { get; set; }
    public string ShowText {
        get => $"{Self.Name} {Self.Major} {Self.SchoolName}";
    }

    public PackageClass(Class @class) {
        Self = @class;
    }
}