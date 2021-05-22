using System;
using System.Collections.Generic;
using System.Data;

namespace Photon.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MsSqlParam : SqlParam
    {
        public MsSqlParam(string name = null) : base(name) { }
        public MsSqlParam(string name, SqlDbType type) : base(name)
        {
            Type = type;
        }
        public MsSqlParam(string name, SqlDbType type, int size) : base(name, size)
        {
            Type = type;
        }
        public MsSqlParam(SqlDbType type) : base()
        {
            Type = type;
        }
        public MsSqlParam(SqlDbType type, int size) : base(null, size)
        {
            Type = type;
        }

        public SqlDbType? Type { get; }
    }
}
