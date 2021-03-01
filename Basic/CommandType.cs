using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.DataBase
{
    // Summary:
    //     Specifies how a command string is interpreted.
    public enum CommandType
    {
        // Summary:
        //     An SQL text command. (Default.)
        Text = 1,
        //
        // Summary:
        //     The name of a stored procedure.
        StoredProcedure = 4,
        //
        // Summary:
        //     The name of a table.
        TableDirect = 512,
    }
}
