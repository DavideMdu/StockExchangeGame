﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using Languages.Interfaces;
using StockExchangeGame.Database.Models;

namespace StockExchangeGame.Database.Generic
{
    // ReSharper disable once UnusedMember.Global
    public class MerchantController : IEntityController<Merchant>
    {
        private readonly SQLiteConnection _connection;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ILanguage _currentLanguage;

        public MerchantController(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
        }

        public void SetCurrentLanguage(ILanguage language)
        {
            _log.Info(string.Format(_currentLanguage.GetWord("LanguageSet"), "Merchant", language.Identifier));
            _currentLanguage = language;
        }

        public ILanguage GetCurrentLanguage()
        {
            return _currentLanguage;
        }

        public int CreateTable()
        {
            int result;
            var sql = GetCreateTableSQL();
            _connection.Open();
            using (var command = new SQLiteCommand(sql, _connection))
            {
                result = command.ExecuteNonQuery();
            }
            _log.Info(string.Format(_currentLanguage.GetWord("TableCreated"), "Merchant", result));
            _connection.Close();
            return result;
        }

        public List<Merchant> Get()
        {
            var list = new List<Merchant>();
            var sql = "SELECT * FROM Merchant";
            _connection.Open();
            using (var command = new SQLiteCommand(sql, _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var merchant = GetMerchantFromReader(reader);
                        list.Add(merchant);
                    }
                }
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedGet"), "Merchant", list));
            _connection.Close();
            return list;
        }

        public Merchant Get(long id)
        {
            Merchant merchant = null;
            var sql = "SELECT * FROM Merchant WHERE Id = @Id";
            _connection.Open();
            using (var command = new SQLiteCommand(sql, _connection))
            {
                PrepareCommandSelect(command, id);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        merchant = GetMerchantFromReader(reader);
                }
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedGetSingle"), "Merchant", merchant));
            _connection.Close();
            return merchant;
        }

        public ObservableCollection<Merchant> Get<TValue>(Expression<Func<Merchant, bool>> predicate = null,
            Expression<Func<Merchant, TValue>> orderBy = null)
        {
            if (predicate == null && orderBy == null)
                return GetCollection(Get());
            if (predicate != null && orderBy == null)
                return GetCollection(GetQueryable().Where(predicate).ToList());
            return GetCollection(predicate == null
                ? GetQueryable().OrderBy(orderBy).ToList()
                : GetQueryable().Where(predicate).OrderBy(orderBy).ToList());
        }

        public Merchant Get(Expression<Func<Merchant, bool>> predicate)
        {
            return GetQueryable().Where(predicate).FirstOrDefault();
        }

        public int Insert(Merchant entity)
        {
            int result;
            _connection.Open();
            using (var command = new SQLiteCommand(_connection))
            {
                PrepareCommandInsert(command, entity);
                result = command.ExecuteNonQuery();
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedInsert"), "Merchant", entity, result));
            _connection.Close();
            return result;
        }

        public int Update(Merchant entity)
        {
            int result;
            _connection.Open();
            using (var command = new SQLiteCommand(_connection))
            {
                PrepareCommandUpdate(command, entity);
                result = command.ExecuteNonQuery();
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedUpdate"), "Merchant", entity, result));
            _connection.Close();
            return result;
        }

        public int Delete(Merchant entity)
        {
            int result;
            _connection.Open();
            using (var command = new SQLiteCommand(_connection))
            {
                PrepareDeletCommand(command, entity);
                result = command.ExecuteNonQuery();
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedDelete"), "Merchant", entity, result));
            _connection.Close();
            return result;
        }

        public int Count(Expression<Func<Merchant, bool>> predicate = null)
        {
            return predicate == null ? CountNoPredicate() : CountPredicate();
        }

        private int CountNoPredicate()
        {
            var count = Get().Count;
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedCountSimple"), "Merchant", count));
            return count;
        }

        private int CountPredicate(Expression<Func<Merchant, bool>> predicate = null)
        {
            var count2 = GetQueryable().Where(predicate).Count();
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedCount"), "Merchant", predicate, count2));
            return count2;
        }

        private string GetCreateTableSQL()
        {
            return "CREATE TABLE Merchant (" +
                   "Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                   "Name TEXT NOT NULL," +
                   "CreatedAt TEXT NOT NULL," +
                   "Deleted BOOLEAN NOT NULL," +
                   "ModifiedAt TEXT NOT NULL," +
                   "LiquidFundsInEuro DOUBLE NOT NULL)";
        }

        private void PrepareCommandSelect(SQLiteCommand command, long id)
        {
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id);
        }

        private Merchant GetMerchantFromReader(SQLiteDataReader reader)
        {
            return new Merchant
            {
                Id = Convert.ToInt64(reader["Id"].ToString()),
                Name = reader["Name"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"].ToString()),
                Deleted = Convert.ToBoolean(reader["Deleted"].ToString()),
                ModifiedAt = Convert.ToDateTime(reader["ModifiedAt"].ToString()),
            };
        }

        private ObservableCollection<Merchant> GetCollection(IEnumerable<Merchant> oldList)
        {
            var collection = new ObservableCollection<Merchant>();
            foreach (var item in oldList)
                collection.Add(item);
            return collection;
        }

        private void PrepareCommandInsert(SQLiteCommand command, Merchant merchant)
        {
            command.CommandText = "INSERT INTO Merchant (Id, Name, CreatedAt, Deleted, ModifiedAt, LiquidFundsInEuro) " +
                                  "VALUES (@Id, @Name, @CreatedAt, @Deleted, @ModifiedAt, @LiquidFundsInEuro)";
            command.Prepare();
            AddParametersUpdateInsert(command, merchant);
        }

        private void AddParametersUpdateInsert(SQLiteCommand command, Merchant merchant)
        {
            command.Parameters.AddWithValue("@Id", merchant.Id);
            command.Parameters.AddWithValue("@Name", merchant.Name);
            command.Parameters.AddWithValue("@CreatedAt", merchant.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            command.Parameters.AddWithValue("@Deleted", merchant.Deleted);
            command.Parameters.AddWithValue("@ModifiedAt", merchant.ModifiedAt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            command.Parameters.AddWithValue("@LiquidFundsInEuro", merchant.LiquidFundsInEuro);
        }

        private void PrepareCommandUpdate(SQLiteCommand command, Merchant merchant)
        {
            command.CommandText =
                "UPDATE Merchant SET Name = @Name, CreatedAt = @CreatedAt, Deleted = @Deleted, " +
                "ModifiedAt = @ModifiedAt, LiquidFundsInEuro = @LiquidFundsInEuro WHERE Id = @Id";
            command.Prepare();
            AddParametersUpdateInsert(command, merchant);
        }

        private void PrepareDeletCommand(SQLiteCommand command, Merchant merchant)
        {
            command.CommandText = "DELETE FROM Merchant WHERE Id = @Id";
            command.Prepare();
            command.Parameters.AddWithValue("@Id", merchant.Id);
        }

        private IQueryable<Merchant> GetQueryable()
        {
            return Get().AsQueryable();
        }
    }
}