using System;
using System.Collections.Generic;
using System.Data;

namespace Photon.Database
{
    public struct DbValue
    {
        public object Value { get; set; }
        public DbType Type { get; set; }
    }
}
