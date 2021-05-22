using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;

namespace Photon.Database
{
    public interface IOledbConnection : IConnection
    {
        new OledbConnectionPath ConnectionString { get; set; }
        new IOledbConnection Clone();

        new OleDbParameterCollection Parameters { get; }

        OleDbParameter SetParameter(string name, OleDbType? type = null, int? size = null, bool? output = null);
    }
}
