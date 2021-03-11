using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace Photon.Database
{
    public interface IMsSqlConnection : IConnection
    {
        event SqlInfoMessageEventHandler InfoMessage;
        new SQLConnectionPath ConnectionString { get; set; }
        new IMsSqlConnection Clone();

        new SqlParameterCollection Parameters { get; }

        new SqlParameter AddParameter(string name, bool output = false);
        SqlParameter AddParameter(string name, SqlDbType type, bool output = false);
        SqlParameter AddParameter(string name, SqlDbType type, int size, bool output = false);
        SqlParameter AddParameter(string name, string udt_type, bool output = false);
    }
}
