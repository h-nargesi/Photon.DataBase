using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Photon.Database.Extensions
{
    public static class TypeConvert
    {
        /*
            OleDbType					SqlDbType					DbType
            Empty = 0,
            TinyInt = 16,				            				SByte = 14,
            SmallInt = 2,				SmallInt = 16,				Int16 = 10,
            Integer = 3,				Int = 8,					Int32 = 11,
            BigInt = 20,				BigInt = 0,					Int64 = 12,
            UnsignedTinyInt = 17,		TinyInt = 20,				Byte = 2,
            UnsignedSmallInt = 18,									UInt16 = 18,
            UnsignedInt = 19,										UInt32 = 19,
            UnsignedBigInt = 21,									UInt64 = 20,
            Single = 4,												Single = 15,
            Double = 5,					Real = 13,					Double = 8,
                                        Float = 6,
            Decimal = 14,				Decimal = 5,				Decimal = 7,
            Currency = 6,				Money = 9,					Currency = 4,
                                        SmallMoney = 17,
            VarNumeric = 139,										VarNumeric = 21,

            DBDate = 133,				Date = 31,					Date = 5,
            DBTime = 134,				Time = 32,					Time = 17,
            DBTimeStamp = 135,			Timestamp = 19,
            Date = 7,					DateTime = 4,				DateTime = 6,
                                        SmallDateTime = 15,
                                        DateTimeOffset = 34,		DateTimeOffset = 27,
                                        DateTime2 = 33,				DateTime2 = 26,

            BSTR = 8,
            IDispatch = 9,
            Error = 10,
            Boolean = 11,				Bit = 2,					Boolean = 3,
            Variant = 12,				Variant = 23,
            IUnknown = 13,
            Filetime = 64,
            Guid = 72,					UniqueIdentifier = 14,		Guid = 9,
            Binary = 128,				Binary = 1,					Binary = 1,
            VarBinary = 204,			VarBinary = 21,
            LongVarBinary = 205,		Image = 7,
            Char = 129,					Char = 3,					AnsiStringFixedLength = 22,
            WChar = 130,				NChar = 10,					StringFixedLength = 23,
            Numeric = 131,
            PropVariant = 138,
            VarChar = 200,				VarChar = 22,				AnsiString = 0,
            LongVarChar = 201,			Text = 18,		
            VarWChar = 202,				NVarChar = 12,				String = 16,
            LongVarWChar = 203,			NText = 11,
                                        Xml = 25,					Xml = 25,
                                        Udt = 29,
                                        Structured = 30,
                                                                    Object = 13,
        */
        public static SqlDbType GetSqlDbType(this DbType type)
        {
            return type switch
            {
                //
                // Summary:
                //     An integral type representing signed 8-bit integers with values between -128
                //     and 127.
                DbType.Byte => SqlDbType.TinyInt,
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

        public static DbType GetDbType(this SqlDbType type)
        {
            return type switch
            {
                //
                // Summary:
                //     System.Int64. A 64-bit signed integer.
                SqlDbType.BigInt => DbType.Int64,
                //
                // Summary:
                //     System.Array of type System.Byte. A fixed-length stream of binary data ranging
                //     between 1 and 8,000 bytes.
                SqlDbType.Binary => DbType.Binary,
                //
                // Summary:
                //     System.Boolean. An unsigned numeric value that can be 0, 1, or null.
                SqlDbType.Bit => DbType.Boolean,
                //
                // Summary:
                //     System.String. A fixed-length stream of non-Unicode characters ranging between
                //     1 and 8,000 characters.
                SqlDbType.Char => DbType.AnsiStringFixedLength,
                //
                // Summary:
                //     System.DateTime. Date and time data ranging in value from January 1, 1753 to
                //     December 31, 9999 to an accuracy of 3.33 milliseconds.
                SqlDbType.DateTime => DbType.DateTime,
                //
                // Summary:
                //     System.Decimal. A fixed precision and scale numeric value between -10 38 -1 and
                //     10 38 -1.
                SqlDbType.Decimal => DbType.Decimal,
                //
                // Summary:
                //     System.Double. A floating point number within the range of -1.79E +308 through
                //     1.79E +308.
                // SqlDbType.Float => DbType.Double,
                //
                // Summary:
                //     System.Array of type System.Byte. A variable-length stream of binary data ranging
                //     from 0 to 2 31 -1 (or 2,147,483,647) bytes.
                // SqlDbType.Image => DbType.Binary,
                //
                // Summary:
                //     System.Int32. A 32-bit signed integer.
                SqlDbType.Int => DbType.Int32,
                //
                // Summary:
                //     System.Decimal. A currency value ranging from -2 63 (or -9,223,372,036,854,775,808)
                //     to 2 63 -1 (or +9,223,372,036,854,775,807) with an accuracy to a ten-thousandth
                //     of a currency unit.
                SqlDbType.Money => DbType.Currency,
                //
                // Summary:
                //     System.String. A fixed-length stream of Unicode characters ranging between 1
                //     and 4,000 characters.
                SqlDbType.NChar => DbType.StringFixedLength,
                //
                // Summary:
                //     System.String. A variable-length stream of Unicode data with a maximum length
                //     of 2 30 - 1 (or 1,073,741,823) characters.
                // SqlDbType.NText => DbType.String,
                //
                // Summary:
                //     System.String. A variable-length stream of Unicode characters ranging between
                //     1 and 4,000 characters. Implicit conversion fails if the string is greater than
                //     4,000 characters. Explicitly set the object when working with strings longer
                //     than 4,000 characters. Use System.Data.SqlDbType.NVarChar when the database column
                //     is nvarchar(max).
                SqlDbType.NVarChar => DbType.String,
                //
                // Summary:
                //     System.Single. A floating point number within the range of -3.40E +38 through
                //     3.40E +38.
                SqlDbType.Real => DbType.Double,
                //
                // Summary:
                //     System.Guid. A globally unique identifier (or GUID).
                SqlDbType.UniqueIdentifier => DbType.Guid,
                //
                // Summary:
                //     System.DateTime. Date and time data ranging in value from January 1, 1900 to
                //     June 6, 2079 to an accuracy of one minute.
                // SqlDbType.SmallDateTime => DbType.DateTime,
                //
                // Summary:
                //     System.Int16. A 16-bit signed integer.
                SqlDbType.SmallInt => DbType.Int16,
                //
                // Summary:
                //     System.Decimal. A currency value ranging from -214,748.3648 to +214,748.3647
                //     with an accuracy to a ten-thousandth of a currency unit.
                // SqlDbType.SmallMoney => DbType.Currency,
                //
                // Summary:
                //     System.String. A variable-length stream of non-Unicode data with a maximum length
                //     of 2 31 -1 (or 2,147,483,647) characters.
                // SqlDbType.Text => DbType.AnsiString,
                //
                // Summary:
                //     System.Array of type System.Byte. Automatically generated binary numbers, which
                //     are guaranteed to be unique within a database. timestamp is used typically as
                //     a mechanism for version-stamping table rows. The storage size is 8 bytes.
                // SqlDbType.Timestamp => DbType.DateTime,
                //
                // Summary:
                //     System.Byte. An 8-bit unsigned integer.
                SqlDbType.TinyInt => DbType.Byte,
                //
                // Summary:
                //     System.Array of type System.Byte. A variable-length stream of binary data ranging
                //     between 1 and 8,000 bytes. Implicit conversion fails if the byte array is greater
                //     than 8,000 bytes. Explicitly set the object when working with byte arrays larger
                //     than 8,000 bytes.
                // SqlDbType.VarBinary => DbType.Binary,
                //
                // Summary:
                //     System.String. A variable-length stream of non-Unicode characters ranging between
                //     1 and 8,000 characters. Use System.Data.SqlDbType.VarChar when the database column
                //     is varchar(max).
                SqlDbType.VarChar => DbType.AnsiString,
                //
                // Summary:
                //     System.Object. A special data type that can contain numeric, string, binary,
                //     or date data as well as the SQL Server values Empty and Null, which is assumed
                //     if no other type is declared.
                // SqlDbType.Variant => ?,
                //
                // Summary:
                //     An XML value. Obtain the XML as a string using the System.Data.SqlClient.SqlDataReader.GetValue(System.Int32)
                //     method or System.Data.SqlTypes.SqlXml.Value property, or as an System.Xml.XmlReader
                //     by calling the System.Data.SqlTypes.SqlXml.CreateReader method.
                SqlDbType.Xml => DbType.Xml,
                //
                // Summary:
                //     A SQL Server user-defined type (UDT).
                // SqlDbType.Udt => ?,
                //
                // Summary:
                //     A special data type for specifying structured data contained in table-valued
                //     parameters.
                // SqlDbType.Structured => ?,
                //
                // Summary:
                //     Date data ranging in value from January 1,1 AD through December 31, 9999 AD.
                SqlDbType.Date => DbType.Date,
                //
                // Summary:
                //     Time data based on a 24-hour clock. Time value range is 00:00:00 through 23:59:59.9999999
                //     with an accuracy of 100 nanoseconds. Corresponds to a SQL Server time value.
                SqlDbType.Time => DbType.Time,
                //
                // Summary:
                //     Date and time data. Date value range is from January 1,1 AD through December
                //     31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy
                //     of 100 nanoseconds.
                SqlDbType.DateTime2 => DbType.DateTime2,
                //
                // Summary:
                //     Date and time data with time zone awareness. Date value range is from January
                //     1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999
                //     with an accuracy of 100 nanoseconds. Time zone value range is -14:00 through
                //     +14:00.
                SqlDbType.DateTimeOffset => DbType.DateTimeOffset,
                //
                // Summary:
                //     Other is not supported.
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
    }
}