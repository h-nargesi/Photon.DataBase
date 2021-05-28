using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

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

        public override IConnectionPath ConnectionString
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

        public override T GetValue<T>(int index)
        {
            return (T)SafeValue(typeof(T), this[index]);
        }
        public override T GetValue<T>(string index)
        {
            return (T)SafeValue(typeof(T), this[index]);
        }
        private object SafeValue(Type type, object value)
        {
            if (value is DBNull) return null;
            else if (value is long long_val)
            {
                if (type == typeof(int) || type == typeof(int?)) return (int)long_val;
                else if (type == typeof(short) || type == typeof(short?)) return (short)long_val;
                else if (type == typeof(byte) || type == typeof(byte?)) return (byte)long_val;
                else if (type == typeof(bool) || type == typeof(bool?)) return long_val != 0;
                else if (type == typeof(DateTime) || type == typeof(DateTime?))
                    return new DateTime(long_val);
                else return long_val;
            }
            else if (value is double double_val)
            {
                if (type == typeof(float) || type == typeof(float?))
                    return (float)double_val;
                else return double_val;
            }

            return value;
        }

        #region Parameters:
        SQLiteParameterCollection ISqliteConnection.Parameters
        {
            get { return com.Parameters; }
        }

        protected override DbParameter SetParam(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<SqliteParam>();
            if (attribute == null) return null;

            string name = attribute.Name ?? member.Name;
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("@")) name = "@" + name;

            SQLiteParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else parameter = new SQLiteParameter() { ParameterName = name };

            if (attribute.Type != null) parameter.DbType = attribute.Type.Value;
            if (attribute.Size != null) parameter.Size = attribute.Size.Value;

            return parameter;
        }

        protected override DbParameter SetParam(string name,
            object type = null, int? size = null, bool? output = null)
        {
            if (type is DbType db_type || type is string str_type && Enum.TryParse(str_type, out db_type))
                return (this as ISqliteConnection).SetParameter(name, db_type, size, output);
            else return (this as ISqliteConnection).SetParameter(name, null, size, output);
        }

        SQLiteParameter ISqliteConnection.SetParameter(string name, DbType? type, int? size, bool? output)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("@")) name = "@" + name;

            SQLiteParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else parameter = new SQLiteParameter() { ParameterName = name };

            if (type != null) parameter.DbType = type.Value;
            if (size != null) parameter.Size = size.Value;
            if (output != null) parameter.Direction = output.Value ?
                    ParameterDirection.InputOutput : ParameterDirection.Input;

            return parameter;
        }
        #endregion

    }
}
