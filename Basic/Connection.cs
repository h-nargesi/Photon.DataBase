using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Photon.DataBase
{
    public class Connection : IConnection, IEnumerable, IDisposable
    {
        #region Fields, Events:

        private DbConnection con;
        private DbCommand com;
        private DbDataReader cor;
        private bool cor_is_reading;
        private DbDataAdapter adr;
        private GetSpesialTypes get_value;
        private ConnectionPath path;

        private DataBaseTypes dbType;

        public event ConnectionStingSetHandler ConnectionStringChange;
        public event EventHandler DataBaseTypeChange;

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


        public Connection(DataBaseTypes DBType)
        {
            dbType = DBType;

            switch (dbType)
            {
                case DataBaseTypes.OleDB:
                    con = new OleDbConnection();
                    com = new OleDbCommand();
                    break;
                case DataBaseTypes.SQL:
                    con = new SqlConnection();
                    com = new SqlCommand();
                    break;
                default:
                    throw new ArgumentOutOfRangeException
                        ("DBType", "DataBase type is only OleDB or SQL");
            }
        }


        #region Properties:

        public object this[int index]
        {
            get { return cor[index]; }
        }
        public object this[string index]
        {
            get { return cor[index]; }
        }
        public GetSpesialTypes Values
        {
            get { return get_value; }
        }

        public string CommandText
        {
            get { return com.CommandText; }
            set { com.CommandText = value; }
        }
        public CommandType CommandType
        {
            get { return (CommandType)(int)com.CommandType; }
            set { com.CommandType = (System.Data.CommandType)(int)value; }
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

        public DataBaseTypes DBType
        {
            set
            {
                if (value > DataBaseTypes.OleDB)
                    throw new ArgumentOutOfRangeException("DBType", "DataBase type is only OleDB or SQL");
                if (dbType != value)
                {
                    dbType = value;
                    if (cor != null && !cor.IsClosed)
                        cor.Close();
                    if (con != null) con.Close();
                    path = null;
                    switch (value)
                    {
                        case DataBaseTypes.OleDB:
                            con = new OleDbConnection();
                            com = new OleDbCommand();
                            break;
                        case DataBaseTypes.SQL:
                            con = new SqlConnection();
                            com = new SqlCommand();
                            break;
                    }
                    if (DataBaseTypeChange != null)
                        this.DataBaseTypeChange(this, new EventArgs());
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
            get { return cor == null ? 0 : cor.FieldCount; }
        }

        public DbDataAdapter Adapter
        {
            get
            {
                switch (dbType)
                {
                    case DataBaseTypes.OleDB:
                        adr = new OleDbDataAdapter();
                        break;
                    case DataBaseTypes.SQL:
                        adr = new SqlDataAdapter();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException
                            ("DBType", "DataBase type is only OleDB or SQL");
                }

                return adr;
            }
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

        public void AddParameter(DbParameter parameter)
        {
            com.Parameters.Add(parameter);
        }

        public SqlParameter AddSqlParameter(string name, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter
            {
                ParameterName = name
            };
            if (isOut) param.Direction = ParameterDirection.InputOutput;

            com.Parameters.Add(param);
            return param;
        }
        public SqlParameter AddSqlParameter(string name, SqlDbType type, int? size, bool isOut = false)
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
        public SqlParameter AddSqlParameter(string name, string UdtTypeName, bool isOut = false)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = UdtTypeName
            };
            if (isOut) param.Direction = ParameterDirection.InputOutput;

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
                case DataBaseTypes.SQL:
                    param = new SqlParameter();
                    break;
                case DataBaseTypes.OleDB:
                    param = new OleDbParameter();
                    break;
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
                case DataBaseTypes.SQL:
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
                case DataBaseTypes.OleDB:
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
                case DataBaseTypes.SQL:
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
                case DataBaseTypes.OleDB:
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
                case DataBaseTypes.SQL:
                    param = new SqlParameter();
                    ((SqlParameter)param).SqlDbType = SqlDbType.Udt;
                    ((SqlParameter)param).UdtTypeName = UdtTypeName;
                    break;
                case DataBaseTypes.OleDB:
                    //param = new OleDbParameter();
                    //((OleDbParameter)param).OleDbType = OleDbType.VarWChar;
                    throw new Exception("Invalid Udt for Oledb");
            }

            if (param == null) return null;

            param.ParameterName = name;
            if (isOut) param.Direction = ParameterDirection.InputOutput;
            com.Parameters.Add(param);

            return param;
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
        public void ExecuteReader()
        {
            cor_is_reading = false;
            cor = com.ExecuteReader();
            get_value = new GetSpesialTypes(cor);
        }
        public object ExecuteScalar()
        {
            return com.ExecuteScalar();
        }
        public bool Read()
        {
            cor_is_reading = cor.Read();
            return cor_is_reading;
        }
        public void CloseConnection()
        {
            con.Close();
        }
        public void CloseReader()
        {
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
            return cor.GetValue(index) as byte[];
        }
        public byte[] GetBytes(string index)
        {
            return cor.GetValue(cor.GetOrdinal(index)) as byte[];
        }
        public string GetName(int index)
        {
            return cor.GetName(index);
        }
        public int GetOrdinal(string name)
        {
            return cor.GetOrdinal(name);
        }
        public IEnumerator GetEnumerator()
        {
            if (cor == null) return null;
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
