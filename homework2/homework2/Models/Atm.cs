using System;
using homework2.common;

namespace homework2.Models;

internal class Atm
{
    private readonly Bank _bank;
    private readonly Random _random = new((int)DateTime.Now.Ticks);

    public Bank GetBank()
    {
        return _bank;
    }

    public struct BigMoneyFetchedArgs
    {
        public Account Account;
        public int Money;

        public BigMoneyFetchedArgs(Account account, int money)
        {
            Account = account;
            Money = money;
        }
    };

    public delegate void BigMoneyFetched(BigMoneyFetchedArgs args);

    public event BigMoneyFetched OnBigMoneyFetched;

    public void DepositMoney(CreditAccount account, int money)
    {
        if (money < 0)
        {
            throw new ArgumentException("Money can't be negative");
        }

        account.DepositMoney(money);
        _bank.DepositMoney(money);

    }

    public void WithdrawMoney(CreditAccount account, int money)
    {
        if (money < 0)
        {
            throw new ArgumentException("Money can't be negative");
        }

        if (money > 10000)
        {
            OnBigMoneyFetched?.Invoke(new BigMoneyFetchedArgs(account, money));
        }

        if (_random.Next(0, 100) < 10)
            throw new BadCashException($"Bad cash when {account} withdraw {money}$");
        account.WithdrawMoney(money);
        _bank.WithdrawMoney(money);
    }

    public Atm(Bank bank)
    {
        _bank = bank;
    }
}