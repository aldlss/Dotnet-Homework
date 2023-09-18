using System;
using System.Text;
using ReactiveUI;

namespace homework1.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _needNumber, _ansNumber;
    private ViewModelBase _currentViewModel;

    public string NeedNumber
    {
        get => _needNumber;
        set => this.RaiseAndSetIfChanged(ref _needNumber, value);
    }

    public string AnsNumber
    {
        get => _ansNumber;
        set => this.RaiseAndSetIfChanged(ref _ansNumber, value);
    }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainViewModel()
    {
        _needNumber = _ansNumber = string.Empty;

        this.WhenAnyValue(x => x.NeedNumber).Subscribe(needNumber =>
        {
            if (!int.TryParse(needNumber, out int number))
            {
                AnsNumber = "请输入整数";
                return;
            }

            if (number <= 1)
            {
                AnsNumber = "请输入大于 1 的数字";
                return;
            }

            StringBuilder sb = new();
            sb.Append("答案为：1");
            if ((number & 1) == 0)
            {
                sb.Append(",2");
            }
            for (int i = 3; i <= double.Sqrt(number); i += 2)
            {
                if (number % i == 0)
                {
                    sb.Append($",{i}");
                }
            }
            sb.Append($",{number}");
            AnsNumber = sb.ToString();
        });

        _currentViewModel = new StartViewModel()
        {
            SwitchView = @base => CurrentViewModel = @base,
        };
    }

}
