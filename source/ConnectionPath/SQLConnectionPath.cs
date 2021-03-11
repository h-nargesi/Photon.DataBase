using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
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
