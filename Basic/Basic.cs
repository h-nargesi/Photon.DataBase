using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Photon.DataBase
{
    //SDS: Sky Database Storing
    public enum DataBaseTypes { SQL, OleDB }

    public class ConnectionStringEventArgs : System.EventArgs
    {
        public ConnectionStringEventArgs(DataBaseTypes dbType)
        {
            DBType = dbType;
        }

        public readonly DataBaseTypes DBType;
    }
    public delegate void ConnectionStingSetHandler(object sender, ConnectionStringEventArgs e);

}
