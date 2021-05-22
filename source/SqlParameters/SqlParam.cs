using System;
using System.Collections.Generic;
using System.Data;

namespace Photon.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class SqlParam : SqlParameterAttribute
    {
        public SqlParam(string name = null, int? size = null)
        {
            Name = name;
            Size = size;
        }

        public string Name { get; }
        public int? Size { get; }
    }
}
