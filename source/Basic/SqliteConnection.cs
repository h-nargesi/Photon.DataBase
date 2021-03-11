using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    public class SqliteConnection : Connection, ISqliteConnection
    {
        private readonly SQLiteConnection con;
        private readonly SQLiteCommand com;
        private SqliteConnectionPath path;

        public override event ConnectionStingSetHandler ConnectionStringChange;

        private static SQLiteConnection Con(out SQLiteConnection con)
        {
            con = new SQLiteConnection();
            return con;
        }
        private static SQLiteCommand Com(out SQLiteCommand com)
        {
            com = new SQLiteCommand();
            return com;
        }

        public SqliteConnection() : base(Con(out SQLiteConnection con), Com(out SQLiteCommand com))
        {
            this.con = con;
            this.com = com;
        }
        private SqliteConnection(SQLiteConnection con) : base(con, Com(out SQLiteCommand com))
        {
            this.con = con;
            this.com = com;
        }

        public override IConnection Clone()
        {
            return ((IOledbConnection)this).Clone();
        }
        ISqliteConnection ISqliteConnection.Clone()
        {
            return new SqliteConnection(con);
        }

        public override ConnectionPath ConnectionString
        {
            get { return path; }
            set
            {
                if (value is SqliteConnectionPath sqlite_path)
                    SetConnectionString(sqlite_path);

                else throw new ArgumentException(
                    nameof(value), "Invalid type of connection-string.");
            }
        }
        SqliteConnectionPath ISqliteConnection.ConnectionString
        {
            get { return path; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                SetConnectionString(value);
            }
        }
        private void SetConnectionString(SqliteConnectionPath con_str)
        {
            path = (SqliteConnectionPath)con_str.Copy();
            con.ConnectionString = path.ToString();

            ConnectionStringChange?.Invoke(this, new EventArgs());
        }


        #region Parameters:
        SQLiteParameterCollection ISqliteConnection.Parameters
        {
            get { return com.Parameters; }
        }
        SQLiteParameter ISqliteConnection.AddParameter(string name, bool output)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SQLiteParameter param = new SQLiteParameter()
            {
                ParameterName = name,
                Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
            };

            com.Parameters.Add(param);
            return param;
        }
        SQLiteParameter ISqliteConnection.AddParameter(string name, DbType type, bool output)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SQLiteParameter param = new SQLiteParameter
            {
                ParameterName = name,
                DbType = type,
                Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
            };

            com.Parameters.Add(param);
            return param;
        }
        SQLiteParameter ISqliteConnection.AddParameter(string name, DbType type, int size, bool output)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SQLiteParameter param = new SQLiteParameter
            {
                ParameterName = name,
                DbType = type,
                Size = size,
                Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
            };

            com.Parameters.Add(param);
            return param;
        }

        public override DbParameter AddParameter(string name, bool output = false)
        {
            return ((ISqliteConnection)this).AddParameter(name, output);
        }
        public override DbParameter AddParameter(string name, DbType type, bool output = false)
        {
            return ((ISqliteConnection)this).AddParameter(name, type, output);
        }
        public override DbParameter AddParameter(string name, DbType type, int size, bool output = false)
        {
            return ((ISqliteConnection)this).AddParameter(name, type, size, output);
        }
        #endregion

    }
}
