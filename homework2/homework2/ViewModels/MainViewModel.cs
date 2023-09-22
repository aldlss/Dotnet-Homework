using System;
using System.Reactive;
using homework2.common;
using homework2.Models;
using ReactiveUI;

namespace homework2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _inputMoney, _outputLog, _inputName, _accountName;
    
    private readonly Atm _atm;
    private CreditAccount? _nowAccount = null;

    public string InputMoney
    {
        get => _inputMoney;
        set => this.RaiseAndSetIfChanged(ref _inputMoney, value);
    }

    public string OutputLog
    {
        get => _outputLog;
        set => this.RaiseAndSetIfChanged(ref _outputLog, value);
    }

    public string InputName
    {
        get => _inputName;
        set => this.RaiseAndSetIfChanged(ref _inputName, value);
    }

    public string AccountName
    {
        get => _accountName;
        set => this.RaiseAndSetIfChanged(ref _accountName, value);
    }

    public ReactiveCommand<Unit, Unit> DepositMoneyCommand { get; }
    public ReactiveCommand<Unit, Unit> WithdrawMoneyCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateAccountCommand { get; }

    public MainViewModel()
    {
        _atm = new Atm(new Bank("Satori Bank"));
        _atm.GetBank().DepositMoney(10000);
        _atm.OnBigMoneyFetched += (args => { OutputLog += $"Big money fetched: {args.Account.Name} {args.Money}\n"; });
        _inputMoney = _outputLog = _inputName = _accountName = string.Empty;

        DepositMoneyCommand = ReactiveCommand.Create(() =>
        {
            if (_nowAccount is null)
            {
                OutputLog += "Please create an account\n";
                return;
            }

            if (!int.TryParse(InputMoney, out var money))
            {
                OutputLog += "Please input a valid number\n";
                return;
            }

            try
            {
                _atm.DepositMoney(_nowAccount, money);
                OutputLog += $"Deposit {money}$ to {_nowAccount.Name}\n";
            }
            catch (Exception e)
            {
                OutputLog += $"{e.Message}\n";
            }
        });

        WithdrawMoneyCommand = ReactiveCommand.Create(() =>
        {
            if (_nowAccount is null)
            {
                OutputLog += "Please create an account\n";
                return;
            }

            if (!int.TryParse(InputMoney, out var money))
            {
                OutputLog += "Please input a valid number\n";
                return;
            }

            try
            {
                _atm.WithdrawMoney(_nowAccount, money);
                OutputLog += $"Withdraw {money}$ from {_nowAccount.Name}\n";
            }
            catch (BadCashException e)
            {
                OutputLog += $"Catch BadCashException {e.Message}\n";
            }
            catch (Exception e)
            {
                OutputLog += $"{e.Message}\n";
            }
        });

        CreateAccountCommand = ReactiveCommand.Create(() =>
        {
            if (string.IsNullOrEmpty(_inputName))
            {
                OutputLog += "Please input a valid name\n";
                return;
            }
            _nowAccount = new CreditAccount(_inputName);
            AccountName = $"当前账户为：{_nowAccount.Name}";
            OutputLog += $"Create account {_nowAccount.Name}\n";
        });
    }
}
