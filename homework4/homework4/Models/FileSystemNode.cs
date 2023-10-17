using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace homework4.Models
{
    public enum FileSystemEntryType
    {
        File,
        Directory
    }

    public class FileSystemNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public FileSystemEntryType Type { get; }

        public ObservableCollection<FileSystemNode>? Children { get; }
        public Dictionary<string, FileSystemNode>? DictionaryMap { get; }

        public FileSystemNode(string name, string path, FileSystemEntryType type)
        {
            Name = name;
            Path = path;
            Type = type;
            if (type == FileSystemEntryType.Directory)
            {
                Children = new ObservableCollection<FileSystemNode>();
                DictionaryMap = new Dictionary<string, FileSystemNode>();
            }
        }

        public void AddChild(FileSystemNode child)
        {
            if (Type == FileSystemEntryType.File)
            {
                return;
                //throw new InvalidOperationException("Cannot add child to a file");
            }

            DictionaryMap!.Add(child.Name, child);
            Children!.Add(child);
        }

        public void AddChildren(FileSystemNode[] children)
        {
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        public void RemoveChild(string name)
        {
            if (Type == FileSystemEntryType.File)
            {
                return;
                //throw new InvalidOperationException("Cannot remove child from a file");
            }

            if (DictionaryMap!.ContainsKey(name))
            {
                Children!.Remove(DictionaryMap[name]);
                DictionaryMap.Remove(name);
            }
        }

        public void ClearChildren()
        {
            if (Type == FileSystemEntryType.File)
            {
                return;
            }

            Children!.Clear();
            DictionaryMap!.Clear();
        }
    }
}
