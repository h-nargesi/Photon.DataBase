using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Photon.Database
{
    public static class Program
    {
        private static IConnection Con;
        public static void Main(params string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("DataBase Connection Tester");
            Console.ForegroundColor = ConsoleColor.Gray;
            string order = Console.ReadLine(), command;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            try
            {
                while (order != null)
                {
                    int index = order.IndexOf(":");
                    if (index >= 0)
                    {
                        command = order[(index + 1)..].Trim();
                        order = order.Remove(index);
                    }
                    else command = null;

                    try
                    {
                        switch (order.ToLower())
                        {
                            case "quit": return;
                            case "state":
                                if (Con == null) Console.WriteLine("connection is not set.");
                                else
                                {
                                    Console.WriteLine("type: " + Con.TypeName());
                                    Console.WriteLine("con str: " + Con.ConnectionString);
                                    Console.WriteLine("connection: " + Con.State);
                                    Console.WriteLine("com str: " + Con.CommandText);
                                }
                                break;

                            case "type":
                                if (Con != null) Con.Dispose();
                                Con = command switch
                                {
                                    "sql" => new MsSqlConnection(),
                                    "oledb" => new OledbConnection(),
                                    "sqlite" => new SqliteConnection(),
                                    _ => throw new Exception("Invalid Command"),
                                };
                                break;

                            case "con":
                                string[] part_str = command.Split(',');
                                switch (Con.TypeName())
                                {
                                    case nameof(MsSqlConnection):
                                        Con.ConnectionString = part_str.Length switch
                                        {
                                            4 => new SQLConnectionPath(part_str[0], part_str[1], part_str[2], part_str[3]),
                                            2 => new SQLConnectionPath(part_str[0], part_str[1], null, null),
                                            _ => throw new Exception("Invalid 'con' syntax"),
                                        };
                                        break;
                                    case nameof(OledbConnection):
                                        Con.ConnectionString = part_str.Length switch
                                        {
                                            2 => new OledbConnectionPath(part_str[0], part_str[1]),
                                            3 => new OledbConnectionPath(part_str[0], part_str[1], part_str[2]),
                                            4 => new OledbConnectionPath(part_str[0], part_str[1], part_str[2], part_str[3]),
                                            _ => throw new Exception("Invalid 'con' syntax"),
                                        };
                                        break;
                                    case nameof(SqliteConnection):
                                        Con.ConnectionString = part_str.Length switch
                                        {
                                            3 => new SqliteConnectionPath(part_str[0], part_str[1], part_str[2]),
                                            1 => new SqliteConnectionPath(part_str[0], null, null, null),
                                            _ => throw new Exception("Invalid 'con' syntax"),
                                        };
                                        break;
                                }
                                break;

                            case "open": Con.Open(); break;

                            case "param":

                                index = command.IndexOf(":");
                                if (index >= 0)
                                {
                                    order = command.Remove(index);
                                    part_str = command[(index + 1)..].Trim().Split(',');
                                }
                                else throw new Exception("Invalid Command");

                                object value;
                                switch (order.ToLower())
                                {
                                    case "add":
                                        if (part_str.Length == 2)
                                            if (part_str[1] == "int")
                                                Con.SetParameter(part_str[0], System.Data.DbType.Int32);
                                            else if (part_str[1] == "string")
                                                Con.SetParameter(part_str[0], System.Data.DbType.String);
                                            else throw new Exception("Invalid Command");
                                        else if (part_str.Length == 3)
                                            if (part_str[1] == "int")
                                                Con.SetParameter(part_str[0], System.Data.DbType.Int32).Value = int.Parse(part_str[2]);
                                            else if (part_str[1] == "string")
                                                Con.SetParameter(part_str[0], System.Data.DbType.Int32).Value = part_str[2];
                                            else throw new Exception("Invalid Command");
                                        break;
                                    case "remove":
                                        Con.Parameters.RemoveAt(part_str[0]);
                                        break;
                                    case "get":
                                        Console.WriteLine(Con.Parameters[part_str[0]].Value);
                                        break;
                                    default:
                                        if (part_str.Length == 1) value = null;
                                        else if (part_str[0] == "int") value = int.Parse(part_str[1]);
                                        else if (part_str[0] == "string") value = part_str[1];
                                        else throw new Exception("Invalid Command");
                                        Con.Parameters[order].Value = value;
                                        break;
                                }
                                break;

                            case "ctype":
                                Con.CommandType = command switch
                                {
                                    "sp" => System.Data.CommandType.StoredProcedure,
                                    "tx" => System.Data.CommandType.Text,
                                    "tb" => System.Data.CommandType.TableDirect,
                                    _ => throw new Exception("Invalid Command"),
                                };
                                break;

                            case "com":
                                if (command == null) Console.WriteLine(Con.CommandText);
                                else Con.CommandText = command;
                                break;

                            case "exec": Console.WriteLine(Con.ExecuteNonQuery()); break;

                            case "exes": Console.WriteLine(Con.ExecuteScalar()); break;

                            case "exer":
                                using (var reader = Con.ExecuteReader())
                                {
                                    int count = Con.FieldCount;
                                    int counter = 0;
                                    while (Con.Read())
                                    {
                                        Console.WriteLine("line " + ++counter + ":");
                                        Console.Write(Con[0].ToString());
                                        for (int i = 1; i < count; i++)
                                            Console.Write("\t" + Con[i].ToString());
                                        Console.WriteLine();
                                    }
                                }
                                break;

                            #region Scan
                            //case "scan":
                            //    System.Data.DataTable tables; string[] restrictions;
                            //if (command == null) tables = Con.oledb_Con.GetSchema();
                            //else
                            //{
                            //    index = command.IndexOf(":");
                            //    if (index < 0) tables = Con.oledb_Con.GetSchema(command);
                            //    else
                            //    {
                            //        restrictions = command.Substring(index + 1).Split(',');
                            //        for (int i = 0; i < restrictions.Length; i++)
                            //            if (restrictions[i].Length == 0) restrictions[i] = null;
                            //        command = command.Remove(index++);
                            //        tables = Con.oledb_Con.GetSchema(command, restrictions);
                            //    }
                            //}
                            //Console.WriteLine(tables.TableName);
                            //foreach (System.Data.DataColumn column in tables.Columns)
                            //    Console.WriteLine(column);
                            //Console.WriteLine();
                            //for (int i = 0; i < tables.Rows.Count; i++)
                            //{
                            //    Console.Write("[" + i + "]");
                            //    for (int j = 0; j < tables.Columns.Count; j++)
                            //        Console.Write("-" + tables.Rows[i][j]);
                            //    Console.WriteLine();
                            //}
                            //    break;

                            //case "scano":
                            //    if (command == null) throw new Exception("Invalid Command");

                            //    index = command.IndexOf(":");
                            //    if (index < 0) restrictions = null;
                            //    else
                            //    {
                            //        restrictions = command.Substring(index + 1).Split(',');
                            //        for (int i = 0; i < restrictions.Length; i++)
                            //            if (restrictions[i].Length == 0) restrictions[i] = null;
                            //        command = command.Remove(index++);
                            //    }

                            //    Guid schema;
                            //    switch (command)
                            //    {
                            //        case "1":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Tables;
                            //            break;
                            //        case "2":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Columns;
                            //            break;
                            //        case "3":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Provider_Types;
                            //            break;
                            //        case "4":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Referential_Constraints;
                            //            break;
                            //        case "10":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Key_Column_Usage;
                            //            break;
                            //        case "9":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Primary_Keys;
                            //            break;
                            //        case "8":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Indexes;
                            //            break;
                            //        case "7":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys;
                            //            break;
                            //        case "5":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Constraint_Column_Usage;
                            //            break;
                            //        case "6":
                            //            schema = System.Data.OleDb.OleDbSchemaGuid.Check_Constraints;
                            //            break;
                            //        default: throw new Exception("Invalid Command");
                            //    }

                            //    tables = Con.oledb_Con.GetOleDbSchemaTable(schema, restrictions);

                            //    Console.WriteLine(tables.TableName);
                            //    foreach (System.Data.DataColumn column in tables.Columns)
                            //        Console.WriteLine(column);
                            //    Console.WriteLine();
                            //    for (int i = 0; i < tables.Rows.Count; i++)
                            //    {
                            //        Console.Write("[" + i + "]");
                            //        for (int j = 0; j < tables.Columns.Count; j++)
                            //            Console.Write("-" + tables.Rows[i][j]);
                            //        Console.WriteLine();
                            //    }
                            //    break;
                            #endregion

                            case "close":
                                switch (command)
                                {
                                    case "connection": Con.CloseConnection(); break;
                                    case "reader": Con.CloseReader(); break;
                                    default: throw new Exception("Invalid Command");
                                }
                                break;

                            case "help":
                                Console.WriteLine(@"
state
type:sql|oledb|sqlite
con:
    server,database,user,pass - oledb|sql
    path,pass - oledb
    server,database  - server
open
ctype:sp|tx|tb
param:
    add:    name,int|string
            name,int|string,vlaue
    remove:name
    get:name
    name:int,value
com:command_string
exec
exer
exes
scan
close:connection|reader
quit");
                                break;

                            case "": break;
                            default: throw new Exception("Invalid Order");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;

                        string space = "";
                        while (ex != null)
                        {
                            Console.WriteLine(space + ex.Message);
                            Console.WriteLine(ex.StackTrace);
                            ex = ex.InnerException;
                            space = "Cased By > ";
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                    order = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                }
            }
            finally { if (Con != null) Con.Dispose(); }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static string TypeName(this IConnection con)
        {
            return con.GetType().Name;
        }
    }
}
