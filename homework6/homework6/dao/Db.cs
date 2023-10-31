using System.Collections.Generic;
using System.Linq;
using homework6.Models;
using Microsoft.EntityFrameworkCore;

namespace homework6.Dao;

public class Db: DbContext {
    private readonly string _dbPath;
    private DbSet<Models.Class> Classes { get; set; }
    private DbSet<Models.School> Schools { get; set; }
    private DbSet<Models.Student> Students { get; set; }
    private Db(string dbPath) {
        _dbPath = dbPath;
    }

    public static Db GetInstance(string dbPath) {
        var db = new Db(dbPath);
        if (!db.Database.EnsureCreated()) {
            db.Database.Migrate();
        }
        return db;
    }

    public void AddSchool(Models.School school) {
        Schools.Add(school);
        SaveChanges();
    }

    public void AddClass(Models.Class @class) {
        Classes.Add(@class);
        SaveChanges();
    }

    public void AddStudent(Models.Student student) {
        Students.Add(student);
        SaveChanges();
    }

    public void UpdateSchool(Models.School school) {
        Schools.Update(school);
        SaveChanges();
    }

    public void UpdateClass(Models.Class @class) {
        Classes.Update(@class);
        SaveChanges();
    }

    public void UpdateStudent(Models.Student student) {
        Students.Update(student);
        SaveChanges();
    }

    public void DeleteSchool(Models.School school) {
        Schools.Remove(school);
        SaveChanges();
    }

    public void DeleteClass(Models.Class @class) {
        Classes.Remove(@class);
        SaveChanges();
    }

    public void DeleteStudent(Models.Student student) {
        Students.Remove(student);
        SaveChanges();
    }

    public List<PackageSchool> GetSchools() {
        return Schools.Select(school => new PackageSchool(school)).ToList();
    }

    public List<PackageClass> GetClasses() {
        return Classes.Select(@class => new PackageClass(@class)).ToList();
    }

    public List<PackageStudent> GetStudents() {
        return Students.Select(student => new PackageStudent(student)).ToList();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }
}