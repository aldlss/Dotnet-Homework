using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Platform.Storage;
using homework3.Views;
using ReactiveUI;

namespace homework3.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _fileRoute, _originLineCount, _lineCount;

    public string FileRoute
    {
        get => _fileRoute;
        set => this.RaiseAndSetIfChanged(ref _fileRoute, value);
    }

    public ReactiveCommand<Unit, Unit> SelectFileCommand { get; }

    private List<AnalyzeResult> _wordCount = new(), _originWordCount = new();

    public List<AnalyzeResult> WordCount
    {
        get => _wordCount;
        set => this.RaiseAndSetIfChanged(ref _wordCount, value);
    }

    public List<AnalyzeResult> OriginWordCount
    {
        get => _originWordCount;
        set => this.RaiseAndSetIfChanged(ref _originWordCount, value);
    }

    public string OriginLineCount
    {
        get => _originLineCount;
        set => this.RaiseAndSetIfChanged(ref _originLineCount, value);
    }

    public string LineCount
    {
        get => _lineCount;
        set => this.RaiseAndSetIfChanged(ref _lineCount, value);
    }

    public MainViewModel()
    {
        SelectFileCommand = ReactiveCommand.Create(SelectFile);
    }

    private async void SelectFile()
    {
        var result = await MainWindow.Instance.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            FileTypeFilter = new FilePickerFileType[]
            {
                new("C# File")
                {
                    Patterns = new[] { "*.cs" }
                }
            }
        });

        if (result.Count > 0)
        {
            FileRoute = result[0].Path.AbsolutePath;
            AnalyzeCsharpFile(result[0]);
        }
        else FileRoute = string.Empty;
    }

    private async void AnalyzeCsharpFile(IStorageFile file)
    {
        var stream = await file.OpenReadAsync();
        StreamReader streamReader = new(stream);
        int originLineCount = 0, lineCount = 0;
        Dictionary<string, int>originWordCount = new(), wordCount = new();
        var parseLine = (string line, bool isComment) =>
        {
            bool word = false;
            string wordBuffer = string.Empty;
            foreach(char c in line)
            {
                if (char.IsLetterOrDigit(c))
                {
                    word = true;
                    wordBuffer += c;
                }
                else
                {
                    if (word)
                    {
                        if (originWordCount.ContainsKey(wordBuffer))
                            ++originWordCount[wordBuffer];
                        else originWordCount.Add(wordBuffer, 1);
                        if(!isComment)
                        {
                            if (wordCount.ContainsKey(wordBuffer))
                                ++wordCount[wordBuffer];
                            else wordCount.Add(wordBuffer, 1);
                        }
                        wordBuffer = string.Empty;
                        word = false;
                    }
                }
            }
        };
        while (!streamReader.EndOfStream)
        {
            var line = await streamReader.ReadLineAsync();
            if (line is null) continue;
            line = line.Trim();
            ++originLineCount;
            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("//"))
                parseLine(line[2..], true);
            ++lineCount;
            parseLine(line, false);
        }
        streamReader.Close();
        stream.Close();
        OriginLineCount = $"行数：{originLineCount}";
        LineCount = $"行数：{lineCount}";
        var temp = originWordCount.Select(x => new AnalyzeResult()
        {
            Word = x.Key,
            Count = x.Value
        }).ToList();
        temp.Sort((a, b) => -a.Count.CompareTo(b.Count));
        OriginWordCount = temp;
        temp = wordCount.Select(x => new AnalyzeResult()
        {
            Word = x.Key,
            Count = x.Value
        }).ToList();
        temp.Sort((a, b) => -a.Count.CompareTo(b.Count));
        WordCount = temp;
    }
}

public class AnalyzeResult
{
    public string Word { get; set; } = string.Empty;
    public int Count { get; set; }
}