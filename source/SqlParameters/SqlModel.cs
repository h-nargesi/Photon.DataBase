using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class SqlModel : Attribute { }
}
