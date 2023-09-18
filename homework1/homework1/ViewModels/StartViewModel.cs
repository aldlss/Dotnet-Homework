using System;
using System.Reactive;
using ReactiveUI;

namespace homework1.ViewModels;
public class StartViewModel:ViewModelBase
{
    private string _selectTime = "1", _count = "1";
    private bool isError = false;

    public string SelectTime
    {
        get => _selectTime;
        set => this.RaiseAndSetIfChanged(ref _selectTime, value);
    }

    public string Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }

    public bool IsError
    {
        get => isError;
        set => this.RaiseAndSetIfChanged(ref isError, value);
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }

    public Action<ViewModelBase> SwitchView = _ => {};

    public StartViewModel()
    {
        this.WhenAnyValue(x => x.SelectTime, x => x.Count).Subscribe(onNext:(tuple) =>
        {
            var selectTime = tuple.Item1;
            var count = tuple.Item2;
            if (!int.TryParse(selectTime, out int time) || !int.TryParse(count, out int c))
            {
                IsError = true;
                return;
            }

            if (time <= 0 || c <= 0)
            {
                IsError = true;
                return;
            }

            IsError = false;
        });

        StartCommand = ReactiveCommand.Create(() =>
        {
            if(IsError) return;
            SwitchView(new RunningViewModel()
            {
                SwitchView = SwitchView,
                SelectTime = int.Parse(SelectTime),
                Count = int.Parse(Count),
            });
            return;
        });
    }
}
