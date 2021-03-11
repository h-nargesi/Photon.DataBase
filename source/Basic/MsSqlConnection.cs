using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    public class MsSqlConnection : Connection, IMsSqlConnection
    {
        private readonly SqlConnection con;
        private readonly SqlCommand com;
        private SQLConnectionPath path;

        public override event ConnectionStingSetHandler ConnectionStringChange;
        public event SqlInfoMessageEventHandler InfoMessage
        {
            add { con.InfoMessage += value; }
            remove { con.InfoMessage -= value; }
        }

        private static SqlConnection Con(out SqlConnection con)
        {
            con = new SqlConnection();
            return con;
        }
        private static SqlCommand Com(out SqlCommand com)
        {
            com = new SqlCommand();
            return com;
        }

        public MsSqlConnection() : base(Con(out SqlConnection con), Com(out SqlCommand com))
        {
            this.con = con;
            this.com = com;
        }
        private MsSqlConnection(SqlConnection con) : base(con, Com(out SqlCommand com))
        {
            this.con = con;
            this.com = com;
        }

        public override IConnection Clone()
        {
            return ((IMsSqlConnection)this).Clone();
        }
        IMsSqlConnection IMsSqlConnection.Clone()
        {
            return  new MsSqlConnection(con);
        }

        public override ConnectionPath ConnectionString
        {
            get { return path; }
            set
            {
                if (value is SQLConnectionPath sql_path)
                    SetConnectionString(sql_path);

                else throw new ArgumentException(
                    nameof(value), "Invalid type of connection-string.");
            }
        }
        SQLConnectionPath IMsSqlConnection.ConnectionString
        {
            get { return path; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                SetConnectionString(value);
            }
        }
        private void SetConnectionString(SQLConnectionPath con_str)
        {
            path = (SQLConnectionPath)con_str.Copy();
            con.ConnectionString = path.ToString();

            ConnectionStringChange?.Invoke(this, new EventArgs());
        }


        #region Parameters:
        SqlParameterCollection IMsSqlConnection.Parameters
        {
            get { return com.Parameters; }
        }
        SqlParameter IMsSqlConnection.AddParameter(string name, bool output)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter()
            {
                ParameterName = name,
                Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
            };

            com.Parameters.Add(param);
            return param;
        }
        SqlParameter IMsSqlConnection.AddParameter(string name, SqlDbType type, bool output)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = type,
                Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
            };

            com.Parameters.Add(param);
            return param;
        }
        SqlParameter IMsSqlConnection.AddParameter(string name, SqlDbType type, int size, bool output)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = type,
                Size = size,
                Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
            };

            com.Parameters.Add(param);
            return param;
        }
        SqlParameter IMsSqlConnection.AddParameter(string name, string udt_type, bool output)
        {
            name = name.TrimStart();
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = udt_type,
                Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
            };

            com.Parameters.Add(param);
            return param;
        }

        private static SqlDbType type_of(DbType type)
        {
            return type switch
            {
                //
                // Summary:
                //     An integral type representing signed 8-bit integers with values between -128
                //     and 127.
                DbType.SByte => SqlDbType.TinyInt,
                //
                // Summary:
                //     An integral type representing signed 16-bit integers with values between -32768
                //     and 32767.
                DbType.Int16 => SqlDbType.SmallInt,
                //
                // Summary:
                //     An integral type representing signed 32-bit integers with values between -2147483648
                //     and 2147483647.
                DbType.Int32 => SqlDbType.Int,
                //
                // Summary:
                //     An integral type representing signed 64-bit integers with values between -9223372036854775808
                //     and 9223372036854775807.
                DbType.Int64 => SqlDbType.BigInt,
                //
                // Summary:
                //     A floating point type representing values ranging from approximately 5.0 x 10
                //     -324 to 1.7 x 10 308 with a precision of 15-16 digits.
                DbType.Double => SqlDbType.Real,
                //
                // Summary:
                //     A simple type representing values ranging from 1.0 x 10 -28 to approximately
                //     7.9 x 10 28 with 28-29 significant digits.
                DbType.Decimal => SqlDbType.Decimal,
                //
                // Summary:
                //     A currency value ranging from -2 63 (or -922,337,203,685,477.5808) to 2 63 -1
                //     (or +922,337,203,685,477.5807) with an accuracy to a ten-thousandth of a currency
                //     unit.
                DbType.Currency => SqlDbType.Money,
                //
                // Summary:
                //     A type representing a date value.
                DbType.Date => SqlDbType.Date,
                //
                // Summary:
                //     A type representing a SQL Server DateTime value. If you want to use a SQL Server
                //     time value, use System.Data.SqlDbType.Time.
                DbType.Time => SqlDbType.Time,
                //
                // Summary:
                //     A type representing a date and time value.
                DbType.DateTime => SqlDbType.DateTime,
                //
                // Summary:
                //     Date and time data with time zone awareness. Date value range is from January
                //     1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999
                //     with an accuracy of 100 nanoseconds. Time zone value range is -14:00 through
                //     +14:00.
                DbType.DateTimeOffset => SqlDbType.DateTimeOffset,
                //
                // Summary:
                //     Date and time data. Date value range is from January 1,1 AD through December
                //     31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy
                //     of 100 nanoseconds.
                DbType.DateTime2 => SqlDbType.DateTime2,
                //
                // Summary:
                //     A simple type representing Boolean values of true or false.
                DbType.Boolean => SqlDbType.Bit,
                //
                // Summary:
                //     A globally unique identifier (or GUID).
                DbType.Guid => SqlDbType.UniqueIdentifier,
                //
                // Summary:
                //     A variable-length stream of binary data ranging between 1 and 8,000 bytes.
                DbType.Binary => SqlDbType.Binary,
                //
                // Summary:
                //     A fixed-length stream of non-Unicode characters.
                DbType.AnsiStringFixedLength => SqlDbType.Char,
                //
                // Summary:
                //     A fixed-length string of Unicode characters.
                DbType.StringFixedLength => SqlDbType.NChar,
                //
                // Summary:
                //     A variable-length stream of non-Unicode characters ranging between 1 and 8,000
                //     characters.
                DbType.AnsiString => SqlDbType.VarChar,
                //
                // Summary:
                //     A type representing Unicode character strings.
                DbType.String => SqlDbType.NVarChar,
                //
                // Summary:
                //     A parsed representation of an XML document or fragment.
                DbType.Xml => SqlDbType.Xml,
                //
                // Summary:
                //     Other is not supported.
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }

        public override DbParameter AddParameter(string name, bool output = false)
        {
            return ((IMsSqlConnection)this).AddParameter(name, output);
        }
        public override DbParameter AddParameter(string name, DbType type, bool output = false)
        {
            return ((IMsSqlConnection)this).AddParameter(name, type_of(type), output);
        }
        public override DbParameter AddParameter(string name, DbType type, int size, bool output = false)
        {
            return ((IMsSqlConnection)this).AddParameter(name, type_of(type), size, output);
        }
        #endregion
    }
}
