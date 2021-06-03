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

	public enum StringType : byte
	{
		Char, NChar,
		Varchar, NVarchar, Binary, Varbinary,
	}
}
