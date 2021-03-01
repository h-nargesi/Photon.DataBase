using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    public class Connection : IConnection, IEnumerable, IDisposable
    {
        #region Fields, Events:
        private DbConnection con;
        private DbCommand com;
        private DbDataReader cor;
        private bool cor_is_reading;
        private DbDataAdapter adr;
        private ConnectionPath path;

        private DatabaseTypes dbType;

        public event ConnectionStingSetHandler ConnectionStringChange;
        public event EventHandler DatabaseTypeChange;

        public event SqlInfoMessageEventHandler InfoMessage
        {
            add
            {
                SqlConnection sql = (SqlConnection)con;
                if (sql != null) sql.InfoMessage += value;
                //com.Parameters.
                else throw new ArgumentException("The connection type mus be 'sql connection'", "Info Message");
            }
            remove
            {
                SqlConnection sql = (SqlConnection)con;
                if (sql != null) sql.InfoMessage -= value;
                else throw new ArgumentException("The connection type mus be 'sql connection'", "Info Message");
            }
        }
        public event EventHandler Disposed
        {
            add { con.Disposed += value; }
            remove { con.Disposed -= value; }
        }
        #endregion


        public Connection(DatabaseTypes DBType)
        {
            dbType = DBType;

            switch (dbType)
            {
                case DatabaseTypes.OleDB:
                    con = new OleDbConnection();
                    com = new OleDbCommand();
                    break;
                case DatabaseTypes.SQL:
                    con = new SqlConnection();
                    com = new SqlCommand();
                    break;
                case DatabaseTypes.SQLite:
                    con = new SQLiteConnection();
                    com = new SQLiteCommand();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        "DBType", "Database type is not valid.");
            }
        }


        #region Properties:
        public object this[int index]
        {
            get
            {
                if (cor == null)
                    throw new DatabaseException("The command not executed.");
                return cor[index];
            }
        }
        public object this[string index]
        {
            get
            {
                if (cor == null)
                    throw new DatabaseException("The command not executed.");
                return cor[index];
            }
        }

        public string CommandText
        {
            get { return com.CommandText; }
            set { com.CommandText = value; }
        }
        public int CommandTimeout
        {
            get { return com.CommandTimeout; }
            set { com.CommandTimeout = value; }
        }
        public CommandType CommandType
        {
            get { return com.CommandType; }
            set { com.CommandType = value; }
        }
        public DbParameterCollection Parameters
        {
            get { return com.Parameters; }
        }

        //I Connection
        public ConnectionPath ConnectionString
        {
            get { return path; }
            set
            {
                path = value.Copy();
                con.ConnectionString = path.ToString();
                if (ConnectionStringChange != null)
                    this.ConnectionStringChange(this, new ConnectionStringEventArgs(dbType));
            }
        }

        //I DB Connection
        public int ConnectionTimeout
        {
            get { return con.ConnectionTimeout; }
        }
        public string Database
        {
            get { return con.Database; }
        }

        public DatabaseTypes DBType
        {
            set
            {
                if (dbType != value)
                {
                    dbType = value;
                    if (cor != null && !cor.IsClosed)
                        cor.Close();
                    if (con != null) con.Close();
                    path = null;
                    switch (value)
                    {
                        case DatabaseTypes.OleDB:
                            con = new OleDbConnection();
                            com = new OleDbCommand();
                            break;
                        case DatabaseTypes.SQL:
                            con = new SqlConnection();
                            com = new SqlCommand();
                            break;
                        case DatabaseTypes.SQLite:
                            con = new SQLiteConnection();
                            com = new SQLiteCommand();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                "DBType", "Database type is not valid.");
                    }
                    if (DatabaseTypeChange != null)
                        this.DatabaseTypeChange(this, new EventArgs());
                }
            }
            get { return dbType; }
        }
        public bool ReaderIsClosed
        {
            get { return cor == null || cor.IsClosed; }
        }
        public bool IsReading
        {
            // is usefull for inner subject reading
            get { return cor != null && !cor.IsClosed && cor_is_reading; }
        }
        public ConnectionState State
        {
            get { return con.State; }
        }
        public int FieldCount
        {
            get
            {
                if (cor == null)
                    throw new DatabaseException("The command not executed.");
                return cor.FieldCount;
            }
        }

        public DbDataAdapter Adapter
        {
            get
            {
                switch (dbType)
                {
                    case DatabaseTypes.OleDB:
                        adr = new OleDbDataAdapter();
                        break;
                    case DatabaseTypes.SQL:
                        adr = new SqlDataAdapter();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            "DBType", "Database type is not valid.");
                }

                return adr;
            }
        }
        #endregion


        #region Parameters:
        public DbParameter AddSqlParameter(string name, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter()
            {
                ParameterName = name
            };
            if (isOut) param.Direction = ParameterDirection.InputOutput;

            com.Parameters.Add(param);
            return param;
        }
        public DbParameter AddSqlParameter(string name, SqlDbType type, int? size, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = type
            };
            if (isOut) param.Direction = ParameterDirection.InputOutput;
            if (size.HasValue) param.Size = size.Value;

            com.Parameters.Add(param);
            return param;
        }
        public DbParameter AddSqlParameter(string name, string UdtTypeName, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = UdtTypeName,
            };
            if (isOut) param.Direction = ParameterDirection.InputOutput;

            com.Parameters.Add(param);
            return param;
        }

        public DbParameter AddOleDbParameter(string name, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            OleDbParameter param = new OleDbParameter()
            {
                ParameterName = name
            };
            if (isOut) param.Direction = ParameterDirection.InputOutput;

            com.Parameters.Add(param);
            return param;
        }
        public DbParameter AddOleDbParameter(string name, OleDbType type, int? size, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            OleDbParameter param = new OleDbParameter
            {
                ParameterName = name,
                OleDbType = type
            };
            if (isOut) param.Direction = ParameterDirection.InputOutput;
            if (size.HasValue) param.Size = size.Value;

            com.Parameters.Add(param);
            return param;
        }

        public DbParameter AddParameter(string name, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            DbParameter param = null;
            switch (dbType)
            {
                case DatabaseTypes.SQL:
                    param = new SqlParameter();
                    break;
                case DatabaseTypes.OleDB:
                    param = new OleDbParameter();
                    break;
                case DatabaseTypes.SQLite:
                    param = new SQLiteParameter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        "DBType", "Database type is not valid.");
            }
            
            if (param == null) return null;

            param.ParameterName = name;
            if (isOut) param.Direction = ParameterDirection.InputOutput;
            com.Parameters.Add(param);

            return param;
        }
        public DbParameter AddParameter(string name, SingleType type, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            DbParameter param = null;
            switch (dbType)
            {
                case DatabaseTypes.SQL:
                    param = new SqlParameter();
                    switch (type)
                    {
                        case SingleType.Boolean:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Bit;
                            break;
                        case SingleType.Byte:
                            ((SqlParameter)param).SqlDbType = SqlDbType.TinyInt;
                            break;
                        case SingleType.Date:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Date;
                            break;
                        case SingleType.Int:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Int;
                            break;
                        case SingleType.Long:
                            ((SqlParameter)param).SqlDbType = SqlDbType.BigInt;
                            break;
                        case SingleType.Decimal:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Decimal;
                            break;
                        case SingleType.Real:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Real;
                            break;
                        case SingleType.Short:
                            ((SqlParameter)param).SqlDbType = SqlDbType.SmallInt;
                            break;
                        case SingleType.Variant:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Variant;
                            break;
                        case SingleType.Guid:
                            ((SqlParameter)param).SqlDbType = SqlDbType.UniqueIdentifier;
                            break;
                        case SingleType.Image:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Image;
                            break;
                    }
                    break;
                case DatabaseTypes.OleDB:
                    param = new OleDbParameter();
                    switch (type)
                    {
                        case SingleType.Boolean:
                            ((OleDbParameter)param).OleDbType = OleDbType.Boolean;
                            break;
                        case SingleType.Byte:
                            ((OleDbParameter)param).OleDbType = OleDbType.TinyInt;
                            break;
                        case SingleType.Date:
                            ((OleDbParameter)param).OleDbType = OleDbType.Date;
                            break;
                        case SingleType.Int:
                            ((OleDbParameter)param).OleDbType = OleDbType.Integer;
                            break;
                        case SingleType.Long:
                            ((OleDbParameter)param).OleDbType = OleDbType.BigInt;
                            break;
                        case SingleType.Decimal:
                            ((OleDbParameter)param).OleDbType = OleDbType.Decimal;
                            break;
                        case SingleType.Real:
                            ((OleDbParameter)param).OleDbType = OleDbType.Double;
                            break;
                        case SingleType.Short:
                            ((OleDbParameter)param).OleDbType = OleDbType.SmallInt;
                            break;
                        case SingleType.Variant:
                            ((OleDbParameter)param).OleDbType = OleDbType.Variant;
                            break;
                        case SingleType.Guid:
                            ((OleDbParameter)param).OleDbType = OleDbType.Guid;
                            break;
                    }
                    break;
                case DatabaseTypes.SQLite:
                    param = new SQLiteParameter();
                    switch (type)
                    {
                        case SingleType.Boolean:
                            param.DbType = DbType.Boolean;
                            break;
                        case SingleType.Byte:
                            param.DbType = DbType.Byte;
                            break;
                        case SingleType.Date:
                            param.DbType = DbType.Date;
                            break;
                        case SingleType.Int:
                            param.DbType = DbType.Int32;
                            break;
                        case SingleType.Long:
                            param.DbType = DbType.Int64;
                            break;
                        case SingleType.Decimal:
                            param.DbType = DbType.Decimal;
                            break;
                        case SingleType.Real:
                            param.DbType = DbType.Double;
                            break;
                        case SingleType.Short:
                            param.DbType = DbType.Int16;
                            break;
                        case SingleType.Variant:
                            param.DbType = DbType.VarNumeric;
                            break;
                        case SingleType.Guid:
                            param.DbType = DbType.Guid;
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        "DBType", "Database type is not valid.");
            }

            if (param == null) return null;

            param.ParameterName = name;
            if (isOut) param.Direction = ParameterDirection.InputOutput;
            com.Parameters.Add(param);

            return param;
        }
        public DbParameter AddParameter(string name, StringType type, int size, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            DbParameter param = null;
            switch (dbType)
            {
                case DatabaseTypes.SQL:
                    param = new SqlParameter();
                    switch (type)
                    {
                        case StringType.NChar:
                            ((SqlParameter)param).SqlDbType = SqlDbType.NChar;
                            break;
                        case StringType.Char:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Char;
                            break;
                        case StringType.NVarchar:
                            ((SqlParameter)param).SqlDbType = SqlDbType.NVarChar;
                            break;
                        case StringType.Varchar:
                            ((SqlParameter)param).SqlDbType = SqlDbType.VarChar;
                            break;
                        case StringType.Binary:
                            ((SqlParameter)param).SqlDbType = SqlDbType.Binary;
                            break;
                        case StringType.Varbinary:
                            ((SqlParameter)param).SqlDbType = SqlDbType.VarBinary;
                            break;
                    }
                    break;
                case DatabaseTypes.OleDB:
                    param = new OleDbParameter();
                    switch (type)
                    {
                        case StringType.NChar:
                            ((OleDbParameter)param).OleDbType = OleDbType.WChar;
                            break;
                        case StringType.Char:
                            ((OleDbParameter)param).OleDbType = OleDbType.Char;
                            break;
                        case StringType.NVarchar:
                            ((OleDbParameter)param).OleDbType = OleDbType.VarWChar;
                            break;
                        case StringType.Varchar:
                            ((OleDbParameter)param).OleDbType = OleDbType.VarChar;
                            break;
                        case StringType.Binary:
                            ((OleDbParameter)param).OleDbType = OleDbType.Binary;
                            break;
                        case StringType.Varbinary:
                            ((OleDbParameter)param).OleDbType = OleDbType.VarBinary;
                            break;
                    }
                    break;
                case DatabaseTypes.SQLite:
                    param = new SQLiteParameter();
                    switch (type)
                    {
                        case StringType.NChar:
                        case StringType.Char:
                        case StringType.NVarchar:
                        case StringType.Varchar:
                            param.DbType = DbType.String;
                            break;
                        case StringType.Binary:
                        case StringType.Varbinary:
                            param.DbType = DbType.Binary;
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        "DBType", "Database type is not valid.");
            }

            if (param == null) return null;

            param.Size = size;
            param.ParameterName = name;
            if (isOut) param.Direction = ParameterDirection.InputOutput;
            com.Parameters.Add(param);

            return param;
        }
        public DbParameter AddParameter(string name, string UdtTypeName, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            DbParameter param = null;
            switch (dbType)
            {
                case DatabaseTypes.SQL:
                    param = new SqlParameter();
                    ((SqlParameter)param).SqlDbType = SqlDbType.Udt;
                    ((SqlParameter)param).UdtTypeName = UdtTypeName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        "DBType", "Database type is not valid.");
            }

            if (param == null) return null;

            param.ParameterName = name;
            if (isOut) param.Direction = ParameterDirection.InputOutput;
            com.Parameters.Add(param);

            return param;
        }
        #endregion


        #region Method:
        public IConnection Clone()
        {
            return new Connection(dbType)
            {
                ConnectionString = ConnectionString
            };
        }

        public void Open()
        {
            con.Open();
            com.Connection = con;
        }
        public int ExecuteNonQuery()
        {
            return com.ExecuteNonQuery();
        }
        public DbDataReader ExecuteReader()
        {
            cor_is_reading = false;
            cor = com.ExecuteReader();
            return cor;
        }
        public object ExecuteScalar()
        {
            return com.ExecuteScalar();
        }
        public bool Read()
        {
            if (cor == null)
                throw new DatabaseException("The command not executed.");
            cor_is_reading = cor.Read();
            return cor_is_reading;
        }
        public bool NextResult()
        {
            if (cor == null)
                throw new DatabaseException("The command not executed.");
            cor_is_reading = cor.NextResult();
            return cor_is_reading;
        }
        public void CloseConnection()
        {
            con.Close();
        }
        public void CloseReader()
        {
            cor_is_reading = false;
            if (cor != null) cor.Close();
        }
        public void Dispose()
        {
            if (cor != null) cor.Dispose();
            if (com != null) com.Dispose();
            if (con != null) con.Dispose();
        }

        public byte[] GetBytes(int index)
        {
            if (cor == null)
                throw new DatabaseException("The command not executed.");
            return cor.GetValue(index) as byte[];
        }
        public byte[] GetBytes(string index)
        {
            if (cor == null)
                throw new DatabaseException("The command not executed.");
            return cor.GetValue(cor.GetOrdinal(index)) as byte[];
        }
        public string GetName(int index)
        {
            if (cor == null)
                throw new DatabaseException("The command not executed.");
            return cor.GetName(index);
        }
        public int GetOrdinal(string name)
        {
            if (cor == null)
                throw new DatabaseException("The command not executed.");
            return cor.GetOrdinal(name);
        }
        public IEnumerator GetEnumerator()
        {
            if (cor == null)
                throw new DatabaseException("The command not executed.");
            else return cor.GetEnumerator();
        }

        //I DB Connection
        public void Close()
        {
            if (cor != null && !cor.IsClosed)
                cor.Close();
            con.Close();
        }
        #endregion
    }
}
