using System.Reactive;
using ReactiveUI;

namespace homework4.Models
{
    public class FileSystemItem
    {
        public string Name { get; }
        public string Path { get; }
        public FileSystemEntryType Type { get; }
        public ReactiveCommand <Unit, Unit> DoubleClickCommand { get; set; }
        public FileSystemItem(string name, string path, FileSystemEntryType type, ReactiveCommand<Unit, Unit> doubleClickCommand)
        {
            Name = name;
            Path = path;
            Type = type;
            DoubleClickCommand = doubleClickCommand;
        }

        public FileSystemItem(FileSystemNode node, ReactiveCommand<Unit, Unit> doubleClickCommand)
        {
            Name = node.Name;
            Path = node.Path;
            Type = node.Type;
            DoubleClickCommand = doubleClickCommand;
        }
    }
}
