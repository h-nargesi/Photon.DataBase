using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Photon.Database.Extensions
{
    public static class TypeConvert
    {
        public static SqlDbType GetSqlDbType(this DbType type)
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

        public static OleDbType GetOleDbType(this DbType type)
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
        
    }
}