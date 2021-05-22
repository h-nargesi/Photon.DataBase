using System;
using System.Collections.Generic;
using System.Data;

namespace Photon.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SqliteParam : SqlParam
    {
        public SqliteParam(string name = null) : base(name) { }
        public SqliteParam(string name, DbType type) : base(name)
        {
            Type = type;
        }
        public SqliteParam(string name, DbType type, int size) : base(name, size)
        {
            Type = type;
        }
        public SqliteParam(DbType type) : base()
        {
            Type = type;
        }
        public SqliteParam(DbType type, int size) : base(null, size)
        {
            Type = type;
        }

        public DbType? Type { get; }
    }
}
