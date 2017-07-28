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
    public class TaxesController : IEntityController<Taxes>
    {
        private readonly SQLiteConnection _connection;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ILanguage _currentLanguage;

        public TaxesController(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
        }

        public void SetCurrentLanguage(ILanguage language)
        {
            _log.Info(string.Format(_currentLanguage.GetWord("LanguageSet"), "Taxes", language.Identifier));
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
            _log.Info(string.Format(_currentLanguage.GetWord("TableCreated"), "Taxes", result));
            _connection.Close();
            return result;
        }

        public List<Taxes> Get()
        {
            var list = new List<Taxes>();
            var sql = "SELECT * FROM Taxes";
            _connection.Open();
            using (var command = new SQLiteCommand(sql, _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var taxes = GetTaxesFromReader(reader);
                        list.Add(taxes);
                    }
                }
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedGet"), "Taxes", list));
            _connection.Close();
            return list;
        }

        public Taxes Get(long id)
        {
            Taxes tax = null;
            var sql = "SELECT * FROM Taxes WHERE Id = @Id";
            _connection.Open();
            using (var command = new SQLiteCommand(sql, _connection))
            {
                PrepareCommandSelect(command, id);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        tax = GetTaxesFromReader(reader);
                }
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedGetSingle"), "Taxes", tax));
            _connection.Close();
            return tax;
        }

        public ObservableCollection<Taxes> Get<TValue>(Expression<Func<Taxes, bool>> predicate = null,
            Expression<Func<Taxes, TValue>> orderBy = null)
        {
            if (predicate == null && orderBy == null)
                return GetCollection(Get());
            if (predicate != null && orderBy == null)
                return GetCollection(GetQueryable().Where(predicate).ToList());
            return GetCollection(predicate == null
                ? GetQueryable().OrderBy(orderBy).ToList()
                : GetQueryable().Where(predicate).OrderBy(orderBy).ToList());
        }

        public Taxes Get(Expression<Func<Taxes, bool>> predicate)
        {
            return GetQueryable().Where(predicate).FirstOrDefault();
        }

        public int Insert(Taxes entity)
        {
            int result;
            _connection.Open();
            using (var command = new SQLiteCommand(_connection))
            {
                PrepareCommandInsert(command, entity);
                result = command.ExecuteNonQuery();
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedInsert"), "Taxes", entity, result));
            _connection.Close();
            return result;
        }

        public int Update(Taxes entity)
        {
            int result;
            _connection.Open();
            using (var command = new SQLiteCommand(_connection))
            {
                PrepareCommandUpdate(command, entity);
                result = command.ExecuteNonQuery();
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedUpdate"), "Taxes", entity, result));
            _connection.Close();
            return result;
        }

        public int Delete(Taxes entity)
        {
            int result;
            _connection.Open();
            using (var command = new SQLiteCommand(_connection))
            {
                PrepareDeletCommand(command, entity);
                result = command.ExecuteNonQuery();
            }
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedDelete"), "Taxes", entity, result));
            _connection.Close();
            return result;
        }

        public int Count(Expression<Func<Taxes, bool>> predicate = null)
        {
            if (predicate == null)
            {
                var count = Get().Count;
                _log.Info(string.Format(_currentLanguage.GetWord("ExecutedCountSimple"), "Taxes", count));
                return count;
            }
            var count2 = GetQueryable().Where(predicate).Count();
            _log.Info(string.Format(_currentLanguage.GetWord("ExecutedCount"), "Taxes", predicate, count2));
            return count2;
        }

        private string GetCreateTableSQL()
        {
            return "CREATE TABLE Taxes (" +
                   "Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                   "MerchantId INTEGER NOT NULL," +
                   "CreatedAt TEXT NOT NULL," +
                   "Deleted BOOLEAN NOT NULL," +
                   "ModifiedAt TEXT NOT NULL," +
                   "DateTaxWasDue TEXT NOT NULL," +
                   "DueInEuro DOUBLE NOT NULL," +
                   "PayedInEuro DOUBLE NOT NULL)";
        }

        private void PrepareCommandSelect(SQLiteCommand command, long id)
        {
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id);
        }

        private Taxes GetTaxesFromReader(SQLiteDataReader reader)
        {
            return new Taxes
            {
                Id = Convert.ToInt64(reader["Id"].ToString()),
                MerchantId = Convert.ToInt64(reader["MerchantId"].ToString()),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"].ToString()),
                Deleted = Convert.ToBoolean(reader["Deleted"].ToString()),
                ModifiedAt = Convert.ToDateTime(reader["ModifiedAt"].ToString()),
                DateTaxWasDue = Convert.ToDateTime(reader["ModifiedAt"].ToString()),
                DueInEuro = Convert.ToDouble(reader["DueInEuro"].ToString()),
                PayedInEuro = Convert.ToDouble(reader["PayedInEuro"].ToString())
            };  
        }

        private ObservableCollection<Taxes> GetCollection(IEnumerable<Taxes> oldList)
        {
            var collection = new ObservableCollection<Taxes>();
            foreach (var item in oldList)
                collection.Add(item);
            return collection;
        }

        private void PrepareCommandInsert(SQLiteCommand command, Taxes taxes)
        {
            command.CommandText = "INSERT INTO Taxes (Id, MerchantId, CreatedAt, Deleted, ModifiedAt, DateTaxWasDue, " +
                                  "DueInEuro, PayedInEuro) VALUES (@Id, @MerchantId, @CreatedAt, @Deleted, @ModifiedAt, " +
                                  "@DateTaxWasDue, @DueInEuro, @PayedInEuro)";
            command.Prepare();
            AddParametersUpdateInsert(command, taxes);
        }

        private void AddParametersUpdateInsert(SQLiteCommand command, Taxes taxes)
        {
            command.Parameters.AddWithValue("@Id", taxes.Id);
            command.Parameters.AddWithValue("@MerchantId", taxes.MerchantId);
            command.Parameters.AddWithValue("@CreatedAt", taxes.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            command.Parameters.AddWithValue("@Deleted", taxes.Deleted);
            command.Parameters.AddWithValue("@ModifiedAt", taxes.ModifiedAt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            command.Parameters.AddWithValue("@DateTaxWasDue", taxes.DateTaxWasDue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            command.Parameters.AddWithValue("@DueInEuro", taxes.DueInEuro);
            command.Parameters.AddWithValue("@PayedInEuro", taxes.PayedInEuro);
        }

        private void PrepareCommandUpdate(SQLiteCommand command, Taxes taxes)
        {
            command.CommandText =
                "UPDATE Taxes SET MerchantId = @MerchantId, CreatedAt = @CreatedAt, Deleted = @Deleted, " +
                "ModifiedAt = @ModifiedAt, DateTaxWasDue = @DateTaxWasDue, DueInEuro = @DueInEuro, " +
                "PayedInEuro = @PayedInEuro  WHERE Id = @Id";
            command.Prepare();
            AddParametersUpdateInsert(command, taxes);
        }

        private void PrepareDeletCommand(SQLiteCommand command, Taxes taxes)
        {
            command.CommandText = "DELETE FROM Taxes WHERE Id = @Id";
            command.Prepare();
            command.Parameters.AddWithValue("@Id", taxes.Id);
        }

        private IQueryable<Taxes> GetQueryable()
        {
            return Get().AsQueryable();
        }
    }
}