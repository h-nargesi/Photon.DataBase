using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Photon.Database
{
    public interface IConnection : IEnumerable, IDisposable
    {
        #region Events:
        event ConnectionStingSetHandler ConnectionStringChange;
        event EventHandler Disposed;
        #endregion


        #region Connection:
        ConnectionPath ConnectionString { get; set; }
        int ConnectionTimeout { get; }
        string Database { get; }
        ConnectionState State { get; }
        IConnection Clone();
        void Open();
        void CloseConnection();
        void Close();
        #endregion


        #region Transaction:
        void BeginTransaction();
        bool HasTransaction { get; }
        void Commit();
        void Rollback();
        #endregion


        #region SQL Command:
        string CommandText { get; set; }
        int CommandTimeout { get; set; }
        CommandType CommandType { get; set; }
        DbParameterCollection Parameters { get; }

        int ExecuteNonQuery();
        DbDataReader ExecuteReader();
        object ExecuteScalar();
        #endregion


        #region Parameters:
        DbParameter AddParameter(string name, bool output = false);
        DbParameter AddParameter(string name, DbType type, bool output = false);
        DbParameter AddParameter(string name, DbType type, int size, bool output = false);
        #endregion


        #region Reading Data:
        object this[int index] { get; }
        object this[string index] { get; }

        bool ReaderIsClosed { get; }
        bool IsReading { get; }
        int FieldCount { get; }

        bool Read();
        bool NextResult();
        void CloseReader();

        byte[] GetBytes(int index);
        byte[] GetBytes(string index);
        string GetName(int index);
        int GetOrdinal(string name);
        #endregion
    }
}
