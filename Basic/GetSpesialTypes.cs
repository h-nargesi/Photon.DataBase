using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;

namespace Photon.DataBase
{
    public class GetSpesialTypes
    {
        private DbDataReader cor;
        public GetSpesialTypes(DbDataReader cor)
        {
            this.cor = cor;
        }

        public bool IsDBNull(string index)
        {
            return cor.IsDBNull(cor.GetOrdinal(index));
        }
        public bool IsDBNull(int index)
        {
            return cor.IsDBNull(index);
        }

        public bool GetBoolean(string index)
        {
            return cor.GetBoolean(cor.GetOrdinal(index));
        }
        public bool GetBoolean(int index)
        {
            return cor.GetBoolean(index);
        }

        public byte GetByte(string index)
        {
            return cor.GetByte(cor.GetOrdinal(index));
        }
        public byte GetByte(int index)
        {
            return cor.GetByte(index);
        }

        public char GetChar(string index)
        {
            return cor.GetChar(cor.GetOrdinal(index));
        }
        public char GetChar(int index)
        {
            return cor.GetChar(index);
        }

        public DateTime GetDateTime(string index)
        {
            return cor.GetDateTime(cor.GetOrdinal(index));
        }
        public DateTime GetDateTime(int index)
        {
            return cor.GetDateTime(index);
        }

        public decimal GetDecimal(string index)
        {
            return cor.GetDecimal(cor.GetOrdinal(index));
        }
        public decimal GetDecimal(int index)
        {
            return cor.GetDecimal(index);
        }

        public double GetDouble(string index)
        {
            return cor.GetDouble(cor.GetOrdinal(index));
        }
        public double GetDouble(int index)
        {
            return cor.GetDouble(index);
        }

        public float GetFloat(string index)
        {
            return cor.GetFloat(cor.GetOrdinal(index));
        }
        public float GetFloat(int index)
        {
            return cor.GetFloat(index);
        }

        public Guid GetGuid(string index)
        {
            return cor.GetGuid(cor.GetOrdinal(index));
        }
        public Guid GetGuid(int index)
        {
            return cor.GetGuid(index);
        }

        public short GetInt16(string index)
        {
            return cor.GetInt16(cor.GetOrdinal(index));
        }
        public short GetInt16(int index)
        {
            return cor.GetInt16(index);
        }

        public int GetInt32(string index)
        {
            return cor.GetInt32(cor.GetOrdinal(index));
        }
        public int GetInt32(int index)
        {
            return cor.GetInt32(index);
        }

        public long GetInt64(string index)
        {
            return cor.GetInt64(cor.GetOrdinal(index));
        }
        public long GetInt64(int index)
        {
            return cor.GetInt64(index);
        }

        public string GetString(string index)
        {
            return cor.GetString(cor.GetOrdinal(index));
        }
        public string GetString(int index)
        {
            return cor.GetString(index);
        }
    }
}
