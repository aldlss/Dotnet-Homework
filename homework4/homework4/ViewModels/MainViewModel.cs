using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using homework4.Models;
using ReactiveUI;

namespace homework4.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _nowPath;
    private ObservableCollection<FileSystemNode> _fileSystemEntriesTree;
    private List<FileSystemItem> _fileSystemItemList = new();
    private FileSystemNode? _selectedNode;

    public string NowPath
    {
        get => _nowPath;
        set => this.RaiseAndSetIfChanged(ref _nowPath, value);
    }

    public ObservableCollection<FileSystemNode> FileSystemEntriesTree
    {
        get => _fileSystemEntriesTree;
        set => this.RaiseAndSetIfChanged(ref _fileSystemEntriesTree, value);
    }

    public FileSystemNode? SelectedNode
    {
        get => _selectedNode;
        set => this.RaiseAndSetIfChanged(ref _selectedNode, value);
    }

    public List<FileSystemItem> FileSystemItemList
    {
        get => _fileSystemItemList;
        set => this.RaiseAndSetIfChanged(ref _fileSystemItemList, value);
    }

    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<Unit, Unit> MoveUpCommand { get; }

    public MainViewModel()
    {
        _nowPath = "D:\\";
        var dNode = new FileSystemNode("D:", "D:\\", FileSystemEntryType.Directory);
        AddAllSubEntriesToNode(dNode);
        var eNode = new FileSystemNode("E:", "E:\\", FileSystemEntryType.Directory);
        AddAllSubEntriesToNode(eNode);
        var fNode = new FileSystemNode("F:", "F:\\", FileSystemEntryType.Directory);
        AddAllSubEntriesToNode(fNode);
        _fileSystemEntriesTree = new ObservableCollection<FileSystemNode> { dNode, eNode, fNode };
        _selectedNode = dNode;
        this.WhenAnyValue(x => x.SelectedNode, (FileSystemNode? node) => node).Subscribe(node =>
        {
            if(node is null) return;
            if (node.Type == FileSystemEntryType.File) return;
            OpenAFolder(node.Path, node.Type);

            if (node.Children!.Count >= 1) return;
            AddAllSubEntriesToNode(node);
        });
        RefreshCommand = ReactiveCommand.Create(() =>
        {
            OpenAFolder(NowPath, FileSystemEntryType.Directory);
        });
        MoveUpCommand = ReactiveCommand.Create(() =>
        {
            var upPath = Path.GetDirectoryName(NowPath) ?? "";
            if (upPath == string.Empty) return;
            OpenAFolder(upPath, FileSystemEntryType.Directory);
        });
    }


    private DateTime _lastClickTime = DateTime.Now;
    private string _lastSelectedItem = "";

    public void DoubleClickCommand(string name, string path, FileSystemEntryType type)
    {
        var nowTime = DateTime.Now;
        var delta = (nowTime - _lastClickTime).TotalSeconds;
        if (_lastSelectedItem != path || delta > 0.5)
        {
            _lastClickTime = nowTime;
            _lastSelectedItem = path;
            return;
        }

        SelectedNode = null;
        if (type == FileSystemEntryType.File)
        {
            OpenAFile(path, type);
        }
        else
        {
            OpenAFolder(path, type);
        }
    }

    private void OpenAFile(string path, FileSystemEntryType type)
    {
        if(type == FileSystemEntryType.Directory) return;
        var ext = Path.GetExtension(path);
        using Process process = new();
        process.StartInfo.UseShellExecute = false;
        if (ext == ".txt" || ext == ".log" || ext == ".ini")
        {
            process.StartInfo.FileName = "notepad.exe";
            process.StartInfo.Arguments = path;
        }
        else if (ext == ".exe")
        {
            process.StartInfo.FileName = path;
        }
        else if (ext == ".cpp" || ext == ".py" || ext == ".cs" || ext == ".go" || ext == ".tsx")
        {
            process.StartInfo.FileName = "code";
            process.StartInfo.Arguments = path;
        }
        else
        {
            return;
        }
        process.Start();
    }

    private ReactiveCommand<Unit, Unit> CreateDoubleClickCommand(string name, string path, FileSystemEntryType type)
    {
        return ReactiveCommand.Create(() => { DoubleClickCommand(name, path, type); });
    }

    private void OpenAFolder(string path, FileSystemEntryType type)
    {
        if (type == FileSystemEntryType.File) return;
        NowPath = path;
        var folderList = Directory.GetFileSystemEntries(path);
        var list = new List<FileSystemItem>();
        foreach (var thisPath in folderList)
        {
            var name = Path.GetFileName(thisPath);
            if (name.StartsWith("$")) continue;
            list.Add(File.Exists(thisPath)
                ? new FileSystemItem($"[-] {name}", thisPath, FileSystemEntryType.File,
                    CreateDoubleClickCommand(name, thisPath, FileSystemEntryType.File))
                : new FileSystemItem($"[d] {name}", thisPath, FileSystemEntryType.Directory,
                    CreateDoubleClickCommand(name, thisPath, FileSystemEntryType.Directory)));
        }
        FileSystemItemList = list;
    }

    public void AddAllSubEntriesToNode(FileSystemNode node)
    {
        var folderList = Directory.GetFileSystemEntries(node.Path);
        if (folderList.Length == 0)
        {
            var fakeChild = new FileSystemNode("", "", FileSystemEntryType.File);
            node.AddChild(fakeChild);
            return;
        }
        foreach (var thisPath in folderList)
        {
            var name = Path.GetFileName(thisPath);
            if(name.StartsWith("$")) continue;
            node.AddChild(File.Exists(thisPath)
                ? new FileSystemNode($"[-] {name}", thisPath, FileSystemEntryType.File)
                : new FileSystemNode($"[d] {name}", thisPath, FileSystemEntryType.Directory));
        }
    }
}
