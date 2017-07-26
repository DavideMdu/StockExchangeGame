﻿using System;

namespace StockExchangeGame.Database.Models
{
    // ReSharper disable once UnusedMember.Global
    public class DummyCompany : AbstractEntity
    {
        private bool _active;

        private long _merchantId;

        private string _name;

        private double _sumInEuro;

        public DummyCompany()
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
        public long MerchantId
        {
            get => _merchantId;
            set
            {
                if (value.Equals(_merchantId))
                    return;
                _merchantId = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once UnusedMember.Global
        public double SumInEuro
        {
            get => _sumInEuro;
            set
            {
                if (value.Equals(_sumInEuro))
                    return;
                _sumInEuro = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once UnusedMember.Global
        public bool Active
        {
            get => _active;
            set
            {
                if (value.Equals(_active))
                    return;
                _active = value;
                OnPropertyChanged();
            }
        }
    }
}