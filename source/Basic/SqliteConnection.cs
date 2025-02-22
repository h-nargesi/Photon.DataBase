﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Photon.Database.Extensions;
using Photon.Database.Procedures;

namespace Photon.Database
{
    public class SqliteConnection : Connection, ISqliteConnection
    {
        private readonly ProcedureRegister procedures;
        private readonly SQLiteConnection con;
        private readonly SQLiteCommand com;
        private CommandType command_type;
        private SqliteConnectionPath path;

        public override event ConnectionStingSetHandler ConnectionStringChange;

        private static SQLiteConnection Con(out SQLiteConnection con, string connection_string = null)
        {
            if (connection_string == null) con = new SQLiteConnection();
            else con = new SQLiteConnection(connection_string);
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
            procedures = new ProcedureRegister();
        }
        public SqliteConnection(ProcedureRegister procedures)
            : base(Con(out SQLiteConnection con), Com(out SQLiteCommand com))
        {
            this.con = con;
            this.com = com;
            this.procedures = procedures;
        }
        private SqliteConnection(SQLiteConnection con, ProcedureRegister procedures)
            : base(Con(out SQLiteConnection new_con, con.ConnectionString), Com(out SQLiteCommand com))
        {
            this.con = new_con;
            this.com = com;
            this.procedures = procedures;
        }

        public override IConnection Clone()
        {
            return (this as ISqliteConnection).Clone();
        }
        ISqliteConnection ISqliteConnection.Clone()
        {
            return new SqliteConnection(con, procedures);
        }

        public override CommandType CommandType
        {
            get { return command_type; }
            set { command_type = value; }
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

        public override object LastInsertedID
        {
            get { return con.LastInsertRowId; }
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


        #region Procedures
        public override int ExecuteNonQuery()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call<int>(CommandText, this).Result;
            else return base.ExecuteNonQuery();
        }
        public override DbDataReader ExecuteReader()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call< DbDataReader>(CommandText, this).Result;
            else return base.ExecuteReader();
        }
        public override object ExecuteScalar()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call<object>(CommandText, this).Result;
            else return base.ExecuteScalar();
        }

        public override Task<int> ExecuteNonQueryAsync()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call<int>(CommandText, this);
            else return base.ExecuteNonQueryAsync();
        }
        public override Task<DbDataReader> ExecuteReaderAsync()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call<DbDataReader>(CommandText, this);
            else return base.ExecuteReaderAsync();
        }
        public override Task<object> ExecuteScalarAsync()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call<object>(CommandText, this);
            else return base.ExecuteScalarAsync();
        }

        public override Task<int> ExecuteNonQuerySafe()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call<int>(CommandText, this);
            else return base.ExecuteNonQuerySafe();
        }
        public override Task<DbDataReader> ExecuteReaderSafe()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call<DbDataReader>(CommandText, this);
            else return base.ExecuteReaderSafe();
        }
        public override Task<object> ExecuteScalarSafe()
        {
            if (CommandType == CommandType.StoredProcedure)
                return procedures.Call<object>(CommandText, this);
            else return base.ExecuteScalarSafe();
        }
        #endregion


        #region Parameters:
        SQLiteParameterCollection ISqliteConnection.Parameters
        {
            get { return com.Parameters; }
        }

        protected override DbParameter SetParam(MemberInfo member, Type value_type)
        {
            var attribute = member.GetCustomAttribute<SqliteParam>();
            if (attribute == null) return null;

            string name = attribute.Name ?? member.Name;
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("$")) name = "$" + name;

            SQLiteParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new SQLiteParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            if (attribute.Size != null) parameter.Size = attribute.Size.Value;
            if (attribute.Type != null) parameter.DbType = attribute.Type.Value;
            else if (value_type != null && value_type != typeof(DbValue))
                parameter.DbType = value_type.GetDbType();

            return parameter;
        }
        protected override DbParameter SetFreeParam(MemberInfo member, Type value_type)
        {
            string name = member.Name;
            if (!name.StartsWith("$")) name = "$" + name;

            SQLiteParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new SQLiteParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            if (value_type != null && value_type != typeof(DbValue))
                parameter.DbType = value_type.GetDbType();

            return parameter;
        }

        protected override DbParameter SetParam(string name,
            object type = null, int? size = null, bool? output = null)
        {

            if (type is DbType db_type)
                return (this as ISqliteConnection).SetParameter(name, db_type, size, output);
            else if (type is Type sys_type)
                return (this as ISqliteConnection).SetParameter(name, sys_type.GetDbType(), size, output);
            else if (type is string str_type && Enum.TryParse(str_type, out db_type))
                return (this as ISqliteConnection).SetParameter(name, db_type, size, output);
            else return (this as ISqliteConnection).SetParameter(name, null, size, output);
        }

        SQLiteParameter ISqliteConnection.SetParameter(string name, DbType? type, int? size, bool? output)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("$")) name = "$" + name;

            SQLiteParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new SQLiteParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            if (type != null) parameter.DbType = type.Value;
            if (size != null) parameter.Size = size.Value;
            if (output != null) parameter.Direction = output.Value ?
                    ParameterDirection.InputOutput : ParameterDirection.Input;

            return parameter;
        }
        #endregion

    }
}
