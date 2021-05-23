using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    /*
        Access 97/2000/XP/2003 Database Connection String using Microsoft Jet Driver (DSN less)
        Provider=Microsoft.Jet.OLEDB.4.0;Data Source=<path>;User Id=;Password=;"

        SQL Server Database Connection String using Microsoft SQL Server OLEDB Driver (DSN less)
        Provider=sqloledb;Data Source=SQLServerName;Initial Catalog=DBaseName;User Id=sa;Password=password;

        Oracle Database Connection String using Oracle OLEDB Driver (DSN less)
        Provider=OraOLEDB.Oracle;"Data Source=OracleDBase;User Id=user;Password=password;

        Excel 97/2000/XP/2003 Database Connection String using Microsoft Jet Driver 4.0 (DSN less)
        Provider=Microsoft.Jet.OLEDB.4.0;Data Source=<pATH>;Extended Properties=;Excel 8.0;HDR=Yes;

        Comma Separated Values (.csv) Connection String using Microsoft Jet Driver 4.0 (DSN less)
        Provider=Microsoft.Jet.OLEDB.4.0;Data Source=;Extended Properties=;HDR=Yes;FMT=Delimited;

        Microsoft Index Server Connection String using MS Index Server Driver (DSN less)
        Provider=msidxs;Data source=myCatalog;
    */

    public interface IConnectionPath
    {
        IConnectionPath Copy();
    }

    public delegate void ConnectionStingSetHandler(object sender, EventArgs e);
}
