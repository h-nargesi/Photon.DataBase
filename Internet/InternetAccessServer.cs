#if DEBUG && INTERNET
using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.DataBase.Connection
{
    public class InternetAccessServer
    {
        public InternetAccessServer(Connection DataBase)
        {
            this.data_base = DataBase;
        }

        Connection data_base;

        public bool ExecuteCommand(string Command)
        {
            try
            {
                data_base.Open();
                data_base.CommandText = Command;
                data_base.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }
        public bool ExecuteNonQuery(string Query)
        {
            try
            {
                data_base.Open();
                data_base.CommandText = Query;
                data_base.ExecuteReader();
                return true;
            }
            catch { return false; }
        }

        public Connection DataBase
        {
            get { return data_base; }
        }

        public const char Record = '\uFFEE';
        public const char Feild = '\uFFDE';
        public const char Value = '\uFFCE';
    }
}
#endif