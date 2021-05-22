using System;
using System.Collections.Generic;
using System.Data;

namespace Photon.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SqlDirection : SqlParameterAttribute
    {
        public SqlDirection()
        {
            Direction = ParameterDirection.InputOutput;
        }
        public SqlDirection(bool isout)
        {
            Direction = isout ? ParameterDirection.InputOutput : ParameterDirection.Input;
        }
        public SqlDirection(ParameterDirection direction)
        {
            Direction = direction;
        }

        public ParameterDirection Direction { get; }
    }
}
