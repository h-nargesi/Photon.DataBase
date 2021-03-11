using System;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;

namespace Photon.Database
{
    public interface ISqliteConnection : IConnection
    {
        new SqliteConnectionPath ConnectionString { get; set; }
        new ISqliteConnection Clone();

        new SQLiteParameterCollection Parameters { get; }

        new SQLiteParameter AddParameter(string name, bool output = false);
        new SQLiteParameter AddParameter(string name, DbType type, bool output = false);
        new SQLiteParameter AddParameter(string name, DbType type, int size, bool output = false);
    }
}
