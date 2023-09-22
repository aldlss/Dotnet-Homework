using System;

namespace homework2.Models
{
    internal class Account
    {
        public string Name { get; private set; }

        public void ChangeName(string newName)
        {
            Name = newName;
        }

        public Account(string name)
        {
            Name = name;
        }
    }

    internal class CreditAccount : Account
    {
        private int _money;

        public void DepositMoney(int money)
        {
            if (money < 0)
            {
                throw new ArgumentException("Money can't be negative");
            }
            _money += money;
        }

        public void WithdrawMoney(int money)
        {
            if (money < 0)
            {
                throw new ArgumentException("Money can't be negative");
            }

            if (money > _money)
            {
                throw new ArgumentException("Not enough money");
            }
            _money -= money;
        }

        public int CheckMoney()
        {
            return _money;
        }

        public CreditAccount(string name) : base(name)
        {
            _money = 0;
        }
    }
}
