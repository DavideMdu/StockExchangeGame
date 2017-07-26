﻿using System;

namespace StockExchangeGame.Database.Models
{
    // ReSharper disable once UnusedMember.Global
    public class Merchant : AbstractEntity
    {
        private double _liquidFunds;
        private string _name;

        public Merchant()
        {
            CreatedAt = DateTime.Now;
            ModifiedAt = DateTime.Now;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once UnusedMember.Global
        public string Name
        {
            get => _name;
            set
            {
                if (value.Equals(_name))
                    return;
                _name = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once UnusedMember.Global
        public double LiquidFunds
        {
            get => _liquidFunds;
            set
            {
                if (value.Equals(_liquidFunds))
                    return;
                _liquidFunds = value;
                OnPropertyChanged();
            }
        }
    }
}