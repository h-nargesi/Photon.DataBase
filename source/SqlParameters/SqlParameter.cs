using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SqlParameterAttribute : Attribute { }
}
