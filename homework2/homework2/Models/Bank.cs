using System;

namespace homework2.Models
{
    internal class Bank
    {
        public string Name { get; private set; }
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

        public void ChangeName(string newName)
        {
            Name = newName;
        }

        public Bank(string name)
        {
            Name = name;
            _money = 0;
        }
    }
}
