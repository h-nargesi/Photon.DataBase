using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.DataBase
{
    public abstract class ConnectionPath
    {
        public abstract ConnectionPath Copy();
    }

    public class OleDBConnectionPath : ConnectionPath
    {
        public OleDBConnectionPath(string Path, string Password)
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
        public OleDBConnectionPath(string Provider, string Path, string Password)
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
        public OleDBConnectionPath(string Server, string DataBase, string Username, string Password)
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

        public override ConnectionPath Copy()
        {
            if (db == null)
                return new OleDBConnectionPath(provider, path, pass);
            else return new OleDBConnectionPath(path, db, user, pass);
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

        /*
            Access 97/2000/XP/2003 Database Connection String using Microsoft Jet Driver (DSN less)
            Provider=Microsoft.Jet.OLEDB.4.0;Data Source=<path>;User Id=;Password=;"

            SQL Server Database Connection String using Microsoft SQL Server OLEDB Driver (DSN less)
            Provider=sqloledb;Data Source=SQLServerName;Initial Catalog=DBaseName;User Id=sa;Password=password;

            Oracle Database Connection String using Oracle OLEDB Driver (DSN less)
            Provider=OraOLEDB.Oracle;"Data Source=OracleDBase;User Id=user;Password=password;

            Excel 97/2000/XP/2003 Database Connection String using Microsoft Jet Driver 4.0 (DSN less)
            Provider=Microsoft.Jet.OLEDB.4.0;Data Source=<pATH>;Extended Properties=;Excel 8.0;HDR=Yes;

            Comma Separated Values (.csv) Connection String using Microsoft Jet Driver 4.0 (DSN less)
            Provider=Microsoft.Jet.OLEDB.4.0;Data Source=;Extended Properties=;HDR=Yes;FMT=Delimited;

            Microsoft Index Server Connection String using MS Index Server Driver (DSN less)
            Provider=msidxs;Data source=myCatalog;
        */
    }

    public class SQLConnectionPath : ConnectionPath
    {
        public SQLConnectionPath(string Server, string DataBase,
            string Username, string Password)
        {
            string errors = null;

            if (Server != null && Server.IndexOf(';') > -1) errors += "," + SqlConnectionString[0];
            if (DataBase != null && DataBase.IndexOf(';') > -1) errors += "," + SqlConnectionString[1];
            if (Username != null && Username.IndexOf(';') > -1) errors += "," + SqlConnectionString[4];
            if (Password != null && Password.IndexOf(';') > -1) errors += "," + SqlConnectionString[5];

            if (errors != null) throw new InvalidDataException(errors.Remove(0, 1));

            this.server = Server;
            this.db = DataBase;
            this.user = Username;
            this.pass = Password;
            this.integrity = user == null;
        }
        public SQLConnectionPath(string Server, string DataBase, string Username,
            string Password, string FileAttachment)
        {
            string errors = null;

            if (Server != null && Server.IndexOf(';') > -1) errors += "," + SqlConnectionString[0];
            if (DataBase != null && DataBase.IndexOf(';') > -1) errors += "," + SqlConnectionString[1];
            if (Username != null && Username.IndexOf(';') > -1) errors += "," + SqlConnectionString[4];
            if (Password != null && Password.IndexOf(';') > -1) errors += "," + SqlConnectionString[5];
            if (FileAttachment != null && FileAttachment.IndexOf(';') > -1) errors += "," + SqlConnectionString[2];

            if (errors != null) throw new InvalidDataException(errors.Remove(0, 1));

            this.server = Server;
            this.db = DataBase;
            this.user = Username;
            this.pass = Password;
            this.integrity = user == null;
            this.attach = FileAttachment;
        }
        public SQLConnectionPath(string Server, string DataBase, string Username,
            string Password, string FileAttachment, bool Encrypt, int Timeout)
        {
            string errors = null;

            if (Server != null && Server.IndexOf(';') > -1) errors += "," + SqlConnectionString[0];
            if (DataBase != null && DataBase.IndexOf(';') > -1) errors += "," + SqlConnectionString[1];
            if (Username != null && Username.IndexOf(';') > -1) errors += "," + SqlConnectionString[4];
            if (Password != null && Password.IndexOf(';') > -1) errors += "," + SqlConnectionString[5];
            if (FileAttachment != null && FileAttachment.IndexOf(';') > -1) errors += "," + SqlConnectionString[2];

            if (errors != null) throw new InvalidDataException(errors.Remove(0, 1));

            this.server = Server;
            this.db = DataBase;
            this.user = Username;
            this.pass = Password;
            this.integrity = user == null;
            this.attach = FileAttachment;
            this.encrypt = Encrypt;
            this.timeout = Timeout;
        }
        public SQLConnectionPath(string Server, string DataBase, string Username, string Password,
            string FileAttachment, bool Encrypt, int Timeout, bool UserInterface)
        {
            string errors = null;

            if (Server != null && Server.IndexOf(';') > -1) errors += "," + SqlConnectionString[0];
            if (DataBase != null && DataBase.IndexOf(';') > -1) errors += "," + SqlConnectionString[1];
            if (Username != null && Username.IndexOf(';') > -1) errors += "," + SqlConnectionString[4];
            if (Password != null && Password.IndexOf(';') > -1) errors += "," + SqlConnectionString[5];
            if (FileAttachment != null && FileAttachment.IndexOf(';') > -1) errors += "," + SqlConnectionString[2];

            if (errors != null) throw new InvalidDataException(errors.Remove(0, 1));

            this.server = Server;
            this.db = DataBase;
            this.user = Username;
            this.pass = Password;
            this.integrity = user == null;
            this.attach = FileAttachment;
            this.encrypt = Encrypt;
            this.timeout = Timeout;
            this.inface = UserInterface;
        }

        readonly string server, db,
            attach, user, pass;
        readonly bool integrity, inface, encrypt;
        readonly int timeout = -1;

        public string Server { get { return server; } }
        public string DataBase { get { return db; } }
        public bool Integrity { get { return integrity; } }
        public string Username { get { return user; } }
        public string Password { get { return pass; } }
        public string AttachDbFilename { get { return attach; } }

        public override ConnectionPath Copy()
        {
            return new SQLConnectionPath(
                server, db, user, pass, attach,
                encrypt, timeout, inface);
        }
        public override string ToString()
        {
            string ans = SqlConnectionString[0] + "=" + server + ";";
            if (db != null) ans += SqlConnectionString[1] + "=" + db + ";";
            if (attach != null) ans += SqlConnectionString[2] + "=" + attach + ";";
            if (integrity) ans += SqlConnectionString[3] + "=True;";
            else
            {
                ans += SqlConnectionString[4] + "=" + user + ";";
                ans += SqlConnectionString[5] + "=" + pass + ";";
            }
            if (inface) ans += SqlConnectionString[6] + "=" + inface + ";";
            if (timeout >= 0) ans += SqlConnectionString[7] + "=" + timeout + ";";
            if (encrypt) ans += SqlConnectionString[8] + "=" + encrypt + ";";
            return ans;
        }

        public readonly string[] SqlConnectionString = new string[9] {
            "Data Source", "Initial Catalog", "AttachDBFilename",
            "Integrated Security", "User ID", "Password", "User Instance",
            "Connection Timeout", "Encrypt"};

    }
}
