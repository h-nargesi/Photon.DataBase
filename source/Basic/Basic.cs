using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Photon.Database
{
    //SDS: Sky Database Storing
    public enum DatabaseTypes { SQL, OleDB, SQLite }

    public class ConnectionStringEventArgs : System.EventArgs
    {
        public ConnectionStringEventArgs(DatabaseTypes dbType)
        {
            DBType = dbType;
        }

        public readonly DatabaseTypes DBType;
    }
    public delegate void ConnectionStingSetHandler(object sender, ConnectionStringEventArgs e);

}
