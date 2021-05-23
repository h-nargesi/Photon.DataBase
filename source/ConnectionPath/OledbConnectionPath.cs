using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    public class OledbConnectionPath : IConnectionPath
    {
        public OledbConnectionPath(string Path, string Password)
        {
            string errors = null;

            if (Path != null && Path.IndexOf(';') > -1) errors += "," + OledbConnectionString[1];
            if (Password != null && Password.IndexOf(';') > -1) errors += "," + OledbConnectionString[6];

            if (errors != null)
                throw new InvalidDataException(errors.Remove(0, 1));

            this.path = Path;
            this.pass = Password;
            if (Path != null)
            {
                if (Path.ToLower().EndsWith(".accdb"))
                    this.provider = "Microsoft.ACE.OLEDB.12.0";
                else if (Path.ToLower().EndsWith(".mdb"))
                    this.provider = "Microsoft.Jet.OLEDB.4.0";
                else this.provider = "Microsoft.Jet.OLEDB.4.0";
            }
            else this.provider = "Microsoft.Jet.OLEDB.4.0";
        }
        public OledbConnectionPath(string Provider, string Path, string Password)
        {
            string errors = null;

            if (Provider != null && Provider.IndexOf(';') > -1) errors += "," + OledbConnectionString[0];
            if (Path != null && Path.IndexOf(';') > -1) errors += "," + OledbConnectionString[1];
            if (Password != null && Password.IndexOf(';') > -1) errors += "," + OledbConnectionString[6];

            if (errors != null)
                throw new InvalidDataException(errors.Remove(0, 1));

            this.path = Path;
            this.pass = Password;
            if (Provider == null && Path != null)
            {
                if (Path.ToLower().EndsWith(".accdb"))
                    this.provider = "Microsoft.ACE.OLEDB.12.0";
                else if (Path.ToLower().EndsWith(".mdb"))
                    this.provider = "Microsoft.Jet.OLEDB.4.0";
                else this.provider = "Microsoft.Jet.OLEDB.4.0";
            }
            else this.provider = Provider;
        }
        public OledbConnectionPath(string Server, string DataBase, string Username, string Password)
        {
            string errors = null;

            if (Server != null && Server.IndexOf(';') > -1) errors += "," + OledbConnectionString[1];
            if (DataBase != null && DataBase.IndexOf(';') > -1) errors += "," + OledbConnectionString[2];
            if (Username != null && Username.IndexOf(';') > -1) errors += "," + OledbConnectionString[5];
            if (Password != null && Password.IndexOf(';') > -1) errors += "," + OledbConnectionString[6];

            if (errors != null)
                throw new InvalidDataException(errors.Remove(0, 1));

            this.provider = "SQLOLEDB";
            this.path = Server;
            this.db = DataBase;
            this.user = Username;
            this.pass = Password;
        }

        readonly string provider, path, db, user, pass;
        readonly bool security = true;

        public string Provider { get { return provider; } }
        public string Path { get { return path; } }
        public string DataBase { get { return db; } }
        public string Username { get { return user; } }
        public string Password { get { return pass; } }

        public IConnectionPath Copy()
        {
            if (db == null)
                return new OledbConnectionPath(provider, path, pass);
            else return new OledbConnectionPath(path, db, user, pass);
        }
        public override string ToString()
        {
            string ans = OledbConnectionString[0] + "=" + provider + ";";
            ans += OledbConnectionString[1] + "=" + path + ";";
            if (db != null)
            {
                ans += OledbConnectionString[2] + "=" + db + ";";
                ans += OledbConnectionString[5] + "=" + user + ";";
            }
            else
            {
                ans += OledbConnectionString[3] + "=" + security + ";";
                ans += OledbConnectionString[4] + " ";
            }
            ans += OledbConnectionString[6] + "=" + pass + ";";
            return ans;
        }

        public readonly string[] OledbConnectionString = new string[7] {
            "Provider", "Data Source", "Initial Catalog", "Persist Security Info",
            "Jet OLEDB:Database","User ID", "Password" };
    }
}
