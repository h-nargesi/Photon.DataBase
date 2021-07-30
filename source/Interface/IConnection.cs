using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Photon.Database
{
    public interface IConnection : IEnumerable, IDisposable
    {
        #region Events:
        event ConnectionStingSetHandler ConnectionStringChange;
        event EventHandler Disposed;
        #endregion


        #region Connection:
        IConnectionPath ConnectionString { get; set; }
        int ConnectionTimeout { get; }
        string Database { get; }
        ConnectionState State { get; }
        IConnection Clone();
        void Open();
        Task OpenAsync();
        Task OpenSafe();
        void CloseConnection();
        Task CloseConnectionAsync();
        void Close();
        Task CloseAsync();
        #endregion


        #region Transaction:
        void BeginTransaction();
        Task BeginTransactionAsync();
        bool HasTransaction { get; }
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
        #endregion


        #region SQL Command:
        string CommandText { get; set; }
        int CommandTimeout { get; set; }
        CommandType CommandType { get; set; }
        object LastInsertedID { get; }

        int ExecuteNonQuery();
        DbDataReader ExecuteReader();
        object ExecuteScalar();

        Task<int> ExecuteNonQueryAsync();
        Task<DbDataReader> ExecuteReaderAsync();
        Task<object> ExecuteScalarAsync();

        Task<int> ExecuteNonQuerySafe();
        Task<DbDataReader> ExecuteReaderSafe();
        Task<object> ExecuteScalarSafe();
        #endregion


        #region Reading Data:
        object this[int index] { get; }
        object this[string index] { get; }

        bool ReaderIsClosed { get; }
        int FieldCount { get; }

        bool Read();
        bool NextResult();
        void CloseReader();

        Task<bool> ReadAsync();
        Task<bool> NextResultAsync();
        Task CloseReaderAsync();

        byte[] GetBytes(int index);
        byte[] GetBytes(string index);
        string GetName(int index);
        int GetOrdinal(string name);
        T GetValue<T>(int index);
        T GetValue<T>(string index);
        
        Dictionary<string, int> GetColumns();
        #endregion


        #region Parameters:
        DbParameterCollection Parameters { get; }

        DbParameter SetParameter(string name, DbType? type, int? size = null, bool? output = null);

        /// <summary>
        /// To set the sql parameter as anonymous type
        /// if parameter exists in sql command handler, it will replace.
        /// </summary>
        /// <param name="name">The name of parameter</param>
        /// <param name="parameter">The sql-parameter</param>
        /// <returns>It returns this object to continue call other method</returns>
        IConnection SetParameter(DbParameter parameter);
        /// <summary>
        /// To set the sql parameter as anonymous type
        /// if parameter exists in sql command handler, it will edit.
        /// </summary>
        /// <param name="name">The name of parameter</param>
        /// <param name="definition">
        /// is a anonymous type that precent several paramters
        /// {
        ///     Type = {SqlDbType|OledbType|SqliteType...},
        ///     IsOut = {bool},
        ///     Direction = {ParameterDirection},
        ///     Size = {int},
        ///     Value = {object}
        /// }
        /// </param>
        /// <returns>It returns this object to continue call other method</returns>
        IConnection SetParameter(string name, object definition);

        /// <summary>
        /// To set parameters from a model
        /// if parameter exists in sql command handler, it will edit.
        /// </summary>
        /// <param name="model">an object with 'Sqlmodel' attribute</param>
        /// <returns>It returns this object to continue call other method</returns>
        IConnection SetParameters(object model);
        /// <summary>
        /// To set parameters from a model with filter some feilds
        /// if parameter exists in sql command handler, it will edit.
        /// </summary>
        /// <param name="model">an object with 'Sqlmodel' attribute</param>
        /// <param name="filters">filter the properties or feilds of model</param>
        /// <returns>It returns this object to continue call other method</returns>
        IConnection SetParameters(object model, HashSet<string> filters);

        /// <summary>
        /// To set parameters from a anonymous model
        /// if parameter exists in sql command handler, it will edit.
        /// </summary>
        /// <param name="model">an object which can be anonymous type</param>
        /// <returns>It returns this object to continue call other method</returns>
        IConnection SetFreeParameters(object model);
        #endregion
    }
}
