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

        new OleDbParameter AddParameter(string name, bool output = false);
        OleDbParameter AddParameter(string name, OleDbType type, bool output = false);
        OleDbParameter AddParameter(string name, OleDbType type, int size, bool output = false);
    }
}
