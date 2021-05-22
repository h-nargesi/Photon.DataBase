using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace Photon.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class OledbParam : SqlParam
    {
        public OledbParam(string name = null) : base(name) { }
        public OledbParam(string name, OleDbType type) : base(name)
        {
            Type = type;
        }
        public OledbParam(string name, OleDbType type, int size) : base(name, size)
        {
            Type = type;
        }
        public OledbParam(OleDbType type) : base()
        {
            Type = type;
        }
        public OledbParam(OleDbType type, int size) : base(null, size)
        {
            Type = type;
        }

        public OleDbType? Type { get; }
    }
}
