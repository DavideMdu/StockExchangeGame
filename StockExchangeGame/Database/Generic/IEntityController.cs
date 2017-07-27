﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace StockExchangeGame.Database.Generic
{
    // ReSharper disable once UnusedMember.Global
    public interface IEntityController<T>
    {
        // ReSharper disable once UnusedMember.Global
        List<T> Get();

        // ReSharper disable once UnusedMember.Global
        T Get(long id);

        // ReSharper disable once UnusedMember.Global
        ObservableCollection<T> Get<TValue>(Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, TValue>> orderBy = null);

        // ReSharper disable once UnusedMember.Global
        T Get(Expression<Func<T, bool>> predicate);

        // ReSharper disable once UnusedMember.Global
        int Insert(T entity);

        // ReSharper disable once UnusedMember.Global
        int Update(T entity);

        // ReSharper disable once UnusedMember.Global
        int Delete(T bought);

        // ReSharper disable once UnusedMember.Global
        int Count(Expression<Func<T, bool>> predicate = null);
    }
}