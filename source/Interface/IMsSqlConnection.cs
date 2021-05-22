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

        SqlParameter SetParameter(string name, SqlDbType? type = null, int? size = null, bool? output = null);
        SqlParameter SetParameter(string name, string udt_type, bool? output = null);
    }
}
