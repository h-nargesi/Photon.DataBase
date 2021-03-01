using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;

namespace Photon.DataBase
{
    public interface IConnection : IEnumerable, IDisposable
    {
        event ConnectionStingSetHandler ConnectionStringChange;
        event EventHandler DataBaseTypeChange;

        IConnection Clone();

        void Open();
        int ExecuteNonQuery();
        void ExecuteReader();
        object ExecuteScalar();
        bool Read();
        void CloseConnection();
        void CloseReader();
        void Close();

        byte[] GetBytes(string index);
        byte[] GetBytes(int index);
        string GetName(int index);
        int GetOrdinal(string name);

        object this[int index] { get; }
        object this[string index] { get; }
        GetSpesialTypes Values { get; }

        string CommandText { set; get; }
        CommandType CommandType { set; get; }
        DbParameterCollection Parameters { get; }
        ConnectionPath ConnectionString { set; get; }
        DataBaseTypes DBType { set; get; }
        bool ReaderIsClosed { get; }
        ConnectionState State { get; }
        int FieldCount { get; }

        void AddParameter(DbParameter parameter);

        SqlParameter AddSqlParameter(string name, bool isOut = false);
        SqlParameter AddSqlParameter(string name, SqlDbType type, int? size, bool isOut = false);
        SqlParameter AddSqlParameter(string name, string UdtTypeName, bool isOut = false);

        DbParameter AddParameter(string name, bool isOut = false);
        DbParameter AddParameter(string name, SingleType type, bool isOut = false);
        DbParameter AddParameter(string name, StringType type, int size, bool isOut = false);
        DbParameter AddParameter(string name, string UdtTypeName, bool isOut = false);
    }
}
