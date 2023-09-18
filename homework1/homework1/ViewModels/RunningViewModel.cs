using System;
using System.Reactive;
using System.Timers;
using ReactiveUI;

namespace homework1.ViewModels;
public class RunningViewModel : ViewModelBase
{
    private string _committedAns, _remainingTime, _remainingCount, _problem, _thisRes;

    public string CommittedAns
    {
        get => _committedAns;
        set => this.RaiseAndSetIfChanged(ref _committedAns, value);
    }

    public string RemainingTime
    {
        get => _remainingTime;
        set => this.RaiseAndSetIfChanged(ref _remainingTime, value);
    }

    public string RemainingCount
    {
        get => _remainingCount;
        set => this.RaiseAndSetIfChanged(ref _remainingCount, value);
    }

    public string Problem
    {
        get => _problem;
        set => this.RaiseAndSetIfChanged(ref _problem, value);
    }

    public string ThisRes
    {
        get => _thisRes;
        set => this.RaiseAndSetIfChanged(ref _thisRes, value);
    }

    public ReactiveCommand<Unit, Unit> CommitCommand { get; }

    public int SelectTime = 1, Count = 1;
    private readonly Random _random = new((int)DateTimeOffset.Now.Ticks);
    private int _trueAns, _remainingTimeInt, _remainingCountInt, _rightCount;
    private readonly Timer _timer = new(1000);

    private readonly Timer _timer2 = new(10);

    public Action<ViewModelBase> SwitchView = _ => { };

    public RunningViewModel()
    {
        _committedAns = _remainingTime = _remainingCount = _problem = _thisRes = string.Empty;
        _remainingCountInt = _rightCount = 0;
        _timer.AutoReset = false;
        _timer.Elapsed += (_, _) =>
        {
            --_remainingTimeInt;
            if (_remainingTimeInt == 0)
            {
                CheckAns(true);
            }
            PrintRemainingTime(_remainingTimeInt);
            _timer.Start();
        };
        CommitCommand = ReactiveCommand.Create(() =>
        {
            _timer.Stop();
            CheckAns(false);
        });
        _timer2.AutoReset = false;
        _timer2.Enabled = true;
        _timer2.Elapsed += (_, _) =>
        {
            _timer2.Stop();
            GiveProblem();
        };
        _timer2.Start();
    }

    private void PrintRemainingTime(int r)
    {
        RemainingTime = $"剩余 {r} 秒";
    }

    private void PrintThisRes(bool right)
    {
        ThisRes = right ? "答对了！" : "答错了！";
    }

    private void GiveProblem()
    {
        ++_remainingCountInt;
        if (_remainingCountInt > Count)
        {
            SwitchView(new ResultViewModel()
            {
                SwitchView = SwitchView,
                RightCount =  _rightCount,
                SumCount = Count,
                SelectTime = SelectTime,
            });
            return;
        }

        int a = _random.Next(0, 100), b = _random.Next(0, 100);
        if (_random.Next(2) == 0)
        {
            _trueAns = a + b;
            Problem = $"{a} + {b} = ?";
        }
        else
        {
            _trueAns = a - b;
            Problem = $"{a} - {b} = ?";
        }
        _remainingTimeInt = SelectTime;
        PrintRemainingTime(_remainingTimeInt);
        RemainingCount = $"当前为 {_remainingCountInt}/{Count} 题";
        _timer.Start();
    }

    public void CheckAns(bool isTimeout)
    {
        if (isTimeout || !int.TryParse(CommittedAns, out var res) || _trueAns != res)
        {
            PrintThisRes(false);
        }
        else
        {
            PrintThisRes(true);
            ++_rightCount;
        }
        GiveProblem();
    }
}
