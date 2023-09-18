using System;
using System.Reactive;
using System.Timers;
using ReactiveUI;

namespace homework1.ViewModels;
public class ResultViewModel:ViewModelBase
{
    private string _result = "";
    public int RightCount = 0, SumCount = 1, SelectTime;
    public string Result
    {
        get => _result;
        set => this.RaiseAndSetIfChanged(ref _result, value);
    }
    public ReactiveCommand<Unit,Unit> RemakeCommand { get; }
    public Action<ViewModelBase> SwitchView = _ => { };
    private readonly Timer _timer = new(10);

    public ResultViewModel()
    {
        _timer.AutoReset = false;
        _timer.Enabled = true;
        _timer.Elapsed += (_, _) =>
        {
            Result = $"答对了 {RightCount} 题，共 {SumCount} 题";
        };
        RemakeCommand = ReactiveCommand.Create(() =>
        {
            SwitchView(new StartViewModel()
            {
                SwitchView = SwitchView,
                SelectTime = SelectTime.ToString(),
                Count = SumCount.ToString(),
            });
            return;
        });
    }
}
