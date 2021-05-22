using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Photon.Database
{
    public class OledbConnection : Connection, IOledbConnection
    {
        private readonly OleDbConnection con;
        private readonly OleDbCommand com;
        private OledbConnectionPath path;

        public override event ConnectionStingSetHandler ConnectionStringChange;

        private static OleDbConnection Con(out OleDbConnection con)
        {
            con = new OleDbConnection();
            return con;
        }
        private static OleDbCommand Com(out OleDbCommand com)
        {
            com = new OleDbCommand();
            return com;
        }

        public OledbConnection() : base(Con(out OleDbConnection con), Com(out OleDbCommand com))
        {
            this.con = con;
            this.com = com;
        }
        private OledbConnection(OleDbConnection con) : base(con, Com(out OleDbCommand com))
        {
            this.con = con;
            this.com = com;
        }

        public override IConnection Clone()
        {
            return ((IOledbConnection)this).Clone();
        }
        IOledbConnection IOledbConnection.Clone()
        {
            return new OledbConnection(con);
        }

        public override ConnectionPath ConnectionString
        {
            get { return path; }
            set
            {
                if (value is OledbConnectionPath oledb_path)
                    SetConnectionString(oledb_path);

                else throw new ArgumentException(
                    nameof(value), "Invalid type of connection-string.");
            }
        }
        OledbConnectionPath IOledbConnection.ConnectionString
        {
            get { return path; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                SetConnectionString(value);
            }
        }
        private void SetConnectionString(OledbConnectionPath con_str)
        {
            path = (OledbConnectionPath)con_str.Copy();
            con.ConnectionString = path.ToString();

            ConnectionStringChange?.Invoke(this, new EventArgs());
        }


        #region Parameters:
        OleDbParameterCollection IOledbConnection.Parameters
        {
            get { return com.Parameters; }
        }

        protected override DbParameter SetParam(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<OledbParam>();
            if (attribute == null) return null;

            string name = attribute.Name ?? member.Name;
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("@")) name = "@" + name;

            OleDbParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else parameter = new OleDbParameter() { ParameterName = name };

            if (attribute.Type != null) parameter.OleDbType = attribute.Type.Value;
            if (attribute.Size != null) parameter.Size = attribute.Size.Value;

            return parameter;
        }
        
        protected override DbParameter SetParam(string name,
            object type = null, int? size = null, bool? output = null)
        {
            if (type is OleDbType oledb_type) return SetParameter(name, oledb_type, size, output);
            else if (type is DbType db_type) return SetParameter(name, Typeof(db_type), size, output);
            else if (type is string str_type && Enum.TryParse(str_type, out oledb_type))
                return SetParameter(name, oledb_type, size, output);
            else return SetParameter(name, null, size, output);
        }

        public OleDbParameter SetParameter(string name,
            OleDbType? type = null, int? size = null, bool? output = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("@")) name = "@" + name;

            OleDbParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else parameter = new OleDbParameter() { ParameterName = name };

            if (type != null) parameter.OleDbType = type.Value;
            if (size != null) parameter.Size = size.Value;
            if (output != null) parameter.Direction = output.Value ?
                    ParameterDirection.InputOutput : ParameterDirection.Input;

            return parameter;
        }

        private static OleDbType Typeof(DbType type)
        {
            return type switch
            {
                //
                // Summary:
                //     An integral type representing signed 8-bit integers with values between -128
                //     and 127.
                DbType.SByte => OleDbType.TinyInt,
                //
                // Summary:
                //     An integral type representing signed 16-bit integers with values between -32768
                //     and 32767.
                DbType.Int16 => OleDbType.SmallInt,
                //
                // Summary:
                //     An integral type representing signed 32-bit integers with values between -2147483648
                //     and 2147483647.
                DbType.Int32 => OleDbType.Integer,
                //
                // Summary:
                //     An integral type representing signed 64-bit integers with values between -9223372036854775808
                //     and 9223372036854775807.
                DbType.Int64 => OleDbType.BigInt,
                //
                // Summary:
                //     An 8-bit unsigned integer ranging in value from 0 to 255.
                DbType.Byte => OleDbType.UnsignedTinyInt,
                //
                // Summary:
                //     An integral type representing unsigned 16-bit integers with values between 0
                //     and 65535.
                DbType.UInt16 => OleDbType.UnsignedSmallInt,
                //
                // Summary:
                //     An integral type representing unsigned 32-bit integers with values between 0
                //     and 4294967295.
                DbType.UInt32 => OleDbType.UnsignedInt,
                //
                // Summary:
                //     An integral type representing unsigned 64-bit integers with values between 0
                //     and 18446744073709551615.
                DbType.UInt64 => OleDbType.UnsignedBigInt,
                //
                // Summary:
                //     A floating point type representing values ranging from approximately 1.5 x 10
                //     -45 to 3.4 x 10 38 with a precision of 7 digits.
                DbType.Single => OleDbType.Single,
                //
                // Summary:
                //     A floating point type representing values ranging from approximately 5.0 x 10
                //     -324 to 1.7 x 10 308 with a precision of 15-16 digits.
                DbType.Double => OleDbType.Double,
                //
                // Summary:
                //     A simple type representing values ranging from 1.0 x 10 -28 to approximately
                //     7.9 x 10 28 with 28-29 significant digits.
                DbType.Decimal => OleDbType.Decimal,
                //
                // Summary:
                //     A currency value ranging from -2 63 (or -922,337,203,685,477.5808) to 2 63 -1
                //     (or +922,337,203,685,477.5807) with an accuracy to a ten-thousandth of a currency
                //     unit.
                DbType.Currency => OleDbType.Currency,
                //
                // Summary:
                //     A variable-length numeric value.
                DbType.VarNumeric => OleDbType.VarNumeric,
                //
                // Summary:
                //     A type representing a date value.
                DbType.Date => OleDbType.DBDate,
                //
                // Summary:
                //     A type representing a date and time value.
                DbType.DateTime => OleDbType.Date,
                //
                // Summary:
                //     A type representing a SQL Server DateTime value. If you want to use a SQL Server
                //     time value, use System.Data.SqlDbType.Time.
                DbType.Time => OleDbType.DBTime,
                //
                // Summary:
                //     A simple type representing Boolean values of true or false.
                DbType.Boolean => OleDbType.Boolean,
                //
                // Summary:
                //     A globally unique identifier (or GUID).
                DbType.Guid => OleDbType.Guid,
                //
                // Summary:
                //     A variable-length stream of binary data ranging between 1 and 8,000 bytes.
                DbType.Binary => OleDbType.Binary,
                //
                // Summary:
                //     A variable-length stream of non-Unicode characters ranging between 1 and 8,000
                //     characters.
                DbType.AnsiString => OleDbType.Char,
                //
                // Summary:
                //     A fixed-length stream of non-Unicode characters.
                DbType.AnsiStringFixedLength => OleDbType.VarChar,
                //
                // Summary:
                //     A fixed-length string of Unicode characters.
                DbType.StringFixedLength => OleDbType.WChar,
                //
                // Summary:
                //     A type representing Unicode character strings.
                DbType.String => OleDbType.VarWChar,
                //
                // Summary:
                //     Other is not supported.
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
        #endregion

    }
}
