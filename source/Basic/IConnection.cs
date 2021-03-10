using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Photon.Database
{
    public interface IConnection : IEnumerable, IDisposable
    {
        event ConnectionStingSetHandler ConnectionStringChange;
        event EventHandler DatabaseTypeChange;

        IConnection Clone();

        void Open();
        int ExecuteNonQuery();
        DbDataReader ExecuteReader();
        object ExecuteScalar();
        bool Read();
        bool NextResult();
        void CloseConnection();
        void CloseReader();
        void Close();

        void BeginTransaction();
        bool HasTransaction { get; }
        void Commit();
        void Rollback();

        byte[] GetBytes(string index);
        byte[] GetBytes(int index);
        string GetName(int index);
        int GetOrdinal(string name);

        object this[int index] { get; }
        object this[string index] { get; }

        string CommandText { set; get; }
        int CommandTimeout { get; set; }
        CommandType CommandType { set; get; }
        DbParameterCollection Parameters { get; }
        ConnectionPath ConnectionString { set; get; }
        DatabaseTypes DBType { set; get; }
        bool ReaderIsClosed { get; }
        ConnectionState State { get; }
        int FieldCount { get; }

        DbParameter AddSqlParameter(string name, bool isOut = false);
        DbParameter AddSqlParameter(string name, SqlDbType type, int? size, bool isOut = false);
        DbParameter AddSqlParameter(string name, string UdtTypeName, bool isOut = false);

        DbParameter AddOleDbParameter(string name, bool isOut = false);
        DbParameter AddOleDbParameter(string name, OleDbType type, int? size, bool isOut = false);

        DbParameter AddParameter(string name, bool isOut = false);
        DbParameter AddParameter(string name, SingleType type, bool isOut = false);
        DbParameter AddParameter(string name, StringType type, int size, bool isOut = false);
        DbParameter AddParameter(string name, string UdtTypeName, bool isOut = false);
    }
}
