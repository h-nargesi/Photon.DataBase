using System;
using System.Collections.Generic;
using System.Data;

namespace Photon.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SqlList : SqlParameterAttribute { }
}
