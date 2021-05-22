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

        new SQLiteParameter SetParameter(string name, DbType? type = null, int? size = null, bool? output = null);
    }
}
