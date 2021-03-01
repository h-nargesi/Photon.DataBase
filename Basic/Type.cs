using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Photon.Database
{
    public static class SqlTypeComparetion
    {
        public static bool SingleTypeComparetion(SingleType stype, DbType dbtype)
        {
            switch (dbtype)
            {
                /*
                 * NOT SUPPORTED
                 * Byte
                 * Currency
                 * Object
                 * Single
                 * UInt16
                 * UInt32
                 * UInt64
                 * VarNumeric
                 * Xml
                 * DateTimeOffset
                 */
                /*
                 * STRING TYPE
                 * AnsiString
                 * Binary
                 * String
                 * AnsiStringFixedLength
                 * StringFixedLength
                 */
                case DbType.Boolean: return stype == SingleType.Boolean;
                case DbType.Date: return stype == SingleType.Date;
                case DbType.DateTime: return stype == SingleType.Date;
                case DbType.DateTime2: return stype == SingleType.Date;
                case DbType.Time: return stype == SingleType.Date;
                case DbType.Decimal: return stype == SingleType.Decimal;
                case DbType.Double: return stype == SingleType.Real;
                case DbType.Guid: return stype == SingleType.Guid;
                case DbType.Int16: return stype == SingleType.Short;
                case DbType.Int32: return stype == SingleType.Int;
                case DbType.Int64: return stype == SingleType.Long;
                case DbType.SByte: return stype == SingleType.Byte;
                default: return false;
            }
        }
        public static bool StringTypeComparetion(StringType stype, DbType dbtype)
        {
            switch (dbtype)
            {
                case DbType.AnsiString: return stype == StringType.Varchar;
                case DbType.Binary: return stype == StringType.Binary;
                case DbType.String: return stype == StringType.NVarchar;
                case DbType.AnsiStringFixedLength: return stype == StringType.Char;
                case DbType.StringFixedLength: return stype == StringType.NChar;
                default: return false;
            }
        }
    }

    public enum SingleType : byte
    {
        Date, Boolean, Variant,
        Byte, Int, Short, Long, Decimal,
        Real,
        Guid, Image
    }

    /*
        OleDbType                   SqlDbType                   DbType
        Empty = 0,
        TinyInt = 16,               TinyInt = 20,               SByte = 14,
        SmallInt = 2,               SmallInt = 16,              Int16 = 10,
        Integer = 3,                Int = 8,                    Int32 = 11,
        BigInt = 20,                BigInt = 0,                 Int64 = 12,
        UnsignedTinyInt = 17,                                   Byte = 2,
        UnsignedSmallInt = 18,                                  UInt16 = 18,
        UnsignedInt = 19,                                       UInt32 = 19,
        UnsignedBigInt = 21,                                    UInt64 = 20,
        Single = 4,                                             Single = 15,
        Double = 5,                 Real = 13,                  Double = 8,
                                    Float = 6,
        Decimal = 14,               Decimal = 5,                Decimal = 7,
        Currency = 6,               Money = 9,                  Currency = 4,
                                    SmallMoney = 17,

        DBDate = 133,               Date = 31,                  Date = 5,
        DBTime = 134,               Time = 32,                  Time = 17,
        DBTimeStamp = 135,          Timestamp = 19,
        Date = 7,                   DateTime = 4,               DateTime = 6,
                                    SmallDateTime = 15,
                                    DateTimeOffset = 34,        DateTimeOffset = 27,
                                                                DateTime2 = 26,

        BSTR = 8,
        IDispatch = 9,
        Error = 10,
        Boolean = 11,               Bit = 2,                    Boolean = 3,
        Variant = 12,               Variant = 23,
        IUnknown = 13,
        Filetime = 64,
        Guid = 72,                  UniqueIdentifier = 14,      Guid = 9,
        Binary = 128,               Binary = 1,                 Binary = 1,
        Char = 129,                 Char = 3,                   AnsiStringFixedLength = 22,
        WChar = 130,                NChar = 10,                 StringFixedLength = 23,
        Numeric = 131,
        PropVariant = 138,
        VarNumeric = 139,
        VarChar = 200,              VarChar = 22,               AnsiString = 0,
        LongVarChar = 201,          Text = 18,        
        VarWChar = 202,             NVarChar = 12,              String = 16,
        LongVarWChar = 203,         NText = 11,
        VarBinary = 204,            VarBinary = 21,
        LongVarBinary = 205,        Image = 7,
                                    Xml = 25,                   Xml = 25,
                                    Udt = 29,
                                    Structured = 30,
                                                                VarNumeric = 21,
                                                                Object = 13,
    */

    public enum StringType : byte
    {
        Char, NChar,
        Varchar, NVarchar, Binary, Varbinary,
    }
}
