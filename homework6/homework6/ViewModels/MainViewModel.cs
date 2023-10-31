using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using homework6.Dao;
using homework6.Models;
using ReactiveUI;

namespace homework6.ViewModels;

public class MainViewModel : ViewModelBase
{
    private enum ShowData {
        School,
        Class,
        Student,
        None,
    }
    private ShowData _showData = ShowData.None;
    private readonly string _dbPath = @"F:\code\homework\dotnet\homework6\homework6.sqlite";
    private string _log="";
    private readonly Db _db;
    private List<string> _listItems = new();

    public string Log
    {
        get => _log;
        set => this.RaiseAndSetIfChanged(ref _log, value);
    }
    public List<string> ListItems
    {
        get => _listItems;
        set => this.RaiseAndSetIfChanged(ref _listItems, value);
    }

    public ObservableCollection<string> SelectedItem { get; } = new();

    public MainViewModel()
    {
        _db = Db.GetInstance(_dbPath);
        GetAllSchoolsCommand = ReactiveCommand.Create(GetAllSchools);
        GetAllClassesCommand = ReactiveCommand.Create(GetAllClasses);
        GetAllStudentsCommand = ReactiveCommand.Create(GetAllStudents);
        DeleteCommand = ReactiveCommand.Create(Delete);
        AddSchoolCommand = ReactiveCommand.Create(AddSchool);
        UpdateSchoolCommand = ReactiveCommand.Create(UpdateSchool);
        AddClassCommand = ReactiveCommand.Create(AddClass);
        UpdateClassCommand = ReactiveCommand.Create(UpdateClass);
        AddStudentCommand = ReactiveCommand.Create(AddStudent);
        UpdateStudentCommand = ReactiveCommand.Create(UpdateStudent);
        SelectedItem.WhenAnyValue(collection => collection.Count).Subscribe((count) =>
        {
            if (count == 0) return;
            switch (_showData)
            {
                case ShowData.School:
                    PackageSchool? packageSchool = GetSchoolItem(SelectedItem[0]);
                    if (packageSchool == null)
                    {
                        return;
                    }

                    SchoolName = packageSchool.Self.Name;
                    SchoolAddress = packageSchool.Self.Address;
                    break;
                case ShowData.Class:
                    PackageClass? packageClass = GetClassItem(SelectedItem[0]);
                    if (packageClass == null)
                    {
                        return;
                    }

                    ClassName = packageClass.Self.Name;
                    ClassMajor = packageClass.Self.Major;
                    ClassSchoolName = packageClass.Self.SchoolName;
                    break;
                case ShowData.Student:
                    PackageStudent? packageStudent = GetStudentItem(SelectedItem[0]);
                    if (packageStudent == null)
                    {
                        return;
                    }

                    StudentName = packageStudent.Self.Name;
                    StudentClassName = packageStudent.Self.ClassName;
                    StudentSchoolName = packageStudent.Self.SchoolName;
                    StudentId = packageStudent.Self.Id;
                    break;
            }
        });
    }

    public ReactiveCommand<Unit, Unit> GetAllSchoolsCommand { get; }
    private void GetAllSchools() {
        try
        {
            Log += $"Get All Schools\n";
            _showData = ShowData.School;
            var schools = _db.GetSchools();
            var listSchools = new List<IListItem>(schools);
            ListItems = listSchools.Select(s => s.ShowText).ToList();
        }
        catch (Exception e)
        {
            Log += $"{e.Message}\n";
        }
    }

    public ReactiveCommand<Unit, Unit> GetAllClassesCommand { get; }
    private void GetAllClasses() {
        Log += $"Get All Classes\n";
        _showData = ShowData.Class;
        ListItems = _db.GetClasses().Select(s => s.ShowText).ToList();
    }

    public ReactiveCommand<Unit, Unit> GetAllStudentsCommand { get; }

    private void GetAllStudents() {
        Log += $"Get All Students\n";
        _showData = ShowData.Student;
        ListItems = _db.GetStudents().Select(s => s.ShowText).ToList();
    }

    private PackageSchool? GetSchoolItem(string i)
    {
        foreach (var item in _db.GetSchools())
        {
            if (((IListItem)item).ShowText == i)
            {
                return item;
            }
        }
        return null;
    }

    private PackageClass? GetClassItem(string i)
    {
        foreach (var item in _db.GetClasses())
        {
            if (((IListItem)item).ShowText == i)
            {
                return item;
            }
        }
        return null;
    }

    private PackageStudent? GetStudentItem(string i)
    {
        foreach (var item in _db.GetStudents())
        {
            if (((IListItem)item).ShowText == i)
            {
                return item;
            }
        }
        return null;
    }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    private void Delete() {
        if (SelectedItem.Count == 0 || _showData == ShowData.None) {
            return;
        }
        try
        {
            switch (_showData)
            {
                case ShowData.School:
                    Log += $"Delete School\n";
                    PackageSchool? packageSchool = GetSchoolItem(SelectedItem[0]);
                    if (packageSchool == null)
                    {
                        return;
                    }
                    _db.DeleteSchool(packageSchool.Self);
                    GetAllSchools();
                    break;
                case ShowData.Class:
                    Log += $"Delete Class\n";
                    PackageClass? packageClass = GetClassItem(SelectedItem[0]);
                    if (packageClass == null)
                    {
                        return;
                    }
                    _db.DeleteClass(packageClass.Self);
                    GetAllClasses();
                    break;
                case ShowData.Student:
                    Log += $"Delete Student\n";
                    PackageStudent? packageStudent = GetStudentItem(SelectedItem[0]);
                    if (packageStudent == null)
                    {
                        return;
                    }
                    _db.DeleteStudent(packageStudent.Self);
                    GetAllStudents();
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Log += $"{e.Message}\n";
        }
    }

# region School
    private string _schoolName = "", _schoolAddress = "";
    public string SchoolName
    {
        get => _schoolName;
        set => this.RaiseAndSetIfChanged(ref _schoolName, value);
    }
    public string SchoolAddress
    {
        get => _schoolAddress;
        set => this.RaiseAndSetIfChanged(ref _schoolAddress, value);
    }

    public ReactiveCommand<Unit, Unit> AddSchoolCommand { get; }
    private void AddSchool() {
        Log += $"Add School\n";
        try
        {
            _db.AddSchool(new School()
            {
                Name = _schoolName,
                Address = _schoolAddress,
            });
            GetAllSchools();
        }
        catch (Exception e)
        {
            Log += $"{e.Message}\n";
        }
    }

    public ReactiveCommand<Unit, Unit> UpdateSchoolCommand { get; }
    private void UpdateSchool() {
        if (SelectedItem.Count == 0) {
            return;
        }
        Log += $"Update School\n";
        try
        {
            PackageSchool? packageSchool = GetSchoolItem(SelectedItem[0]);
            if (packageSchool == null)
            {
                return;
            }
            packageSchool.Self.Name = _schoolName;
            packageSchool.Self.Address = _schoolAddress;
            _db.UpdateSchool(packageSchool.Self);
            GetAllSchools();
        }
        catch (Exception e)
        {
            Log += $"{e.Message}\n";
        }
    }
#endregion

#region Class
    private string _className = "", _classMajor = "", _classSchoolName = "";
    public string ClassName
    {
        get => _className;
        set => this.RaiseAndSetIfChanged(ref _className, value);
    }
    public string ClassMajor
    {
        get => _classMajor;
        set => this.RaiseAndSetIfChanged(ref _classMajor, value);
    }
    public string ClassSchoolName
    {
        get => _classSchoolName;
        set => this.RaiseAndSetIfChanged(ref _classSchoolName, value);
    }

    public ReactiveCommand<Unit, Unit> AddClassCommand { get; }
    private void AddClass() {
        Log += $"Add Class\n";
        try
        {
            _db.AddClass(new Class()
            {
                Name = _className,
                Major = _classMajor,
                SchoolName = _classSchoolName,
            });
            GetAllClasses();
        }
        catch (Exception e)
        {
            Log += $"{e.Message}\n";
        }
    }

    public ReactiveCommand<Unit, Unit> UpdateClassCommand { get; }
    private void UpdateClass() {
        if (SelectedItem.Count == 0) {
            return;
        }
        Log += $"Update Class\n";
        try
        {
            PackageClass? packageClass = GetClassItem(SelectedItem[0]);
            if (packageClass == null)
            {
                return;
            }
            packageClass.Self.Name = _className;
            packageClass.Self.Major = _classMajor;
            packageClass.Self.SchoolName = _classSchoolName;
            _db.UpdateClass(packageClass.Self);
            GetAllClasses();
        }
        catch (Exception e)
        {
            Log += $"{e.Message}\n";
        }
    }
#endregion

#region Student
    private string _studentName = "",  _studentClassName = "", _studentSchoolName = "", _studentId = "";
    public string StudentName
    {
        get => _studentName;
        set => this.RaiseAndSetIfChanged(ref _studentName, value);
    }
    public string StudentClassName
    {
        get => _studentClassName;
        set => this.RaiseAndSetIfChanged(ref _studentClassName, value);
    }
    public string StudentSchoolName
    {
        get => _studentSchoolName;
        set => this.RaiseAndSetIfChanged(ref _studentSchoolName, value);
    }
    public string StudentId
    {
        get => _studentId;
        set => this.RaiseAndSetIfChanged(ref _studentId, value);
    }

    public ReactiveCommand<Unit, Unit> AddStudentCommand { get; }
    private void AddStudent() {
        Log += $"Add Student\n";
        try
        {
            _db.AddStudent(new Student()
            {
                Name = _studentName,
                ClassName = _studentClassName,
                SchoolName = _studentSchoolName,
                Id = _studentId,
            });
            GetAllStudents();
        }
        catch (Exception e)
        {
            Log += $"{e.Message}\n";
        }
    }

    public ReactiveCommand<Unit, Unit> UpdateStudentCommand { get; }
    private void UpdateStudent() {
        Log += $"Update Student\n";
        if (SelectedItem.Count == 0) {
            return;
        }
        try
        {
            PackageStudent? packageStudent = GetStudentItem(SelectedItem[0]);
            if (packageStudent == null)
            {
                return;
            }
            packageStudent.Self.Name = _studentName;
            packageStudent.Self.ClassName = _studentClassName;
            packageStudent.Self.SchoolName = _studentSchoolName;
            _db.UpdateStudent(packageStudent.Self);
            GetAllStudents();
        }
        catch (Exception e)
        {
            Log += $"{e.Message}\n";
        }
    }
#endregion
}
