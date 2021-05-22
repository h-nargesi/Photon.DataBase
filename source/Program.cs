#if DEBUG
/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Sky.Launcher;

namespace Photon.DataBase
{
    public class Program
    {
        static Connection Con = new Connection(DataBaseTypes.OleDB);
        public static void Main(params string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("DataBase Connection Tester");
            Console.ForegroundColor = ConsoleColor.Gray;
            string order = Console.ReadLine(), command;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            while (order != "quit")
            {
                int index = order.IndexOf(":");
                if (index >= 0)
                {
                    command = order.Substring(index + 1).Trim();
                    order = order.Remove(index);
                }
                else command = null;

                try
                {
                    switch (order)
                    {
                        case "state":
                            Console.WriteLine("type: " + Con.DBType);
                            Console.WriteLine("con str: " + Con.ConnectionString);
                            Console.WriteLine("connection: " + Con.State);
                            Console.WriteLine("com str: " + Con.CommandText);
                            break;
                        case "type":
                            switch (command)
                            {
                                case "sql": Con.DBType = DataBaseTypes.SQL; break;
                                case "oledb": Con.DBType = DataBaseTypes.OleDB; break;
                                default: throw new Exception("Invalid Command");
                            }
                            break;

                        case "con":
                            string[] part_str = command.Split(',');
                            switch (Con.DBType)
                            {
                                case DataBaseTypes.SQL:
                                    switch (part_str.Length)
                                    {
                                        case 4:
                                            Con.ConnectionString = new SQLConnectionPath(part_str[0], part_str[1], part_str[2], part_str[3]);
                                            break;
                                        case 2:
                                            Con.ConnectionString = new SQLConnectionPath(part_str[0], part_str[1], null, null);
                                            break;
                                        default: throw new Exception("Invalid 'con' syntax");
                                    }
                                    break;
                                case DataBaseTypes.OleDB:
                                    switch (part_str.Length)
                                    {
                                        case 2:
                                            Con.ConnectionString = new OleDBConnectionPath(part_str[0], part_str[1]);
                                            break;
                                        case 3:
                                            Con.ConnectionString = new OleDBConnectionPath(part_str[0], part_str[1], part_str[2]);
                                            break;
                                        case 4:
                                            Con.ConnectionString = new OleDBConnectionPath(part_str[0], part_str[1], part_str[2], part_str[3]);
                                            break;

                                        default: throw new Exception("Invalid 'con' syntax");
                                    }
                                    break;
                            }
                            break;

                        case "open": Con.Open(); break;

                        case "param":

                            index = command.IndexOf(":");
                            if (index >= 0)
                            {
                                order = command.Remove(index);
                                part_str = command.Substring(index + 1).Trim().Split(',');
                            }
                            else throw new Exception("Invalid Command");

                            object value;
                            switch (order)
                            {
                                case "add":
                                    switch (Con.DBType)
                                    {
                                        case DataBaseTypes.SQL:
                                            if (part_str.Length == 2)
                                                if (part_str[1] == "int")
                                                    Con.Parameters.Add(
                                                        new System.Data.SqlClient.SqlParameter(
                                                            part_str[0], System.Data.SqlDbType.Int));
                                                else if (part_str[1] == "string")
                                                    Con.Parameters.Add(
                                                        new System.Data.SqlClient.SqlParameter(
                                                            part_str[0], System.Data.SqlDbType.NVarChar));
                                                else throw new Exception("Invalid Command");
                                            else if (part_str.Length == 3)
                                                if (part_str[1] == "int")
                                                    Con.Parameters.Add(
                                                        new System.Data.SqlClient.SqlParameter(
                                                            part_str[0], int.Parse(part_str[2])));
                                                else if (part_str[1] == "string")
                                                    Con.Parameters.Add(
                                                        new System.Data.SqlClient.SqlParameter(
                                                            part_str[0], part_str[2]));
                                                else throw new Exception("Invalid Command");
                                            break;
                                        case DataBaseTypes.OleDB:
                                            if (part_str.Length == 2)
                                                if (part_str[1] == "int")
                                                    Con.Parameters.Add(
                                                        new System.Data.OleDb.OleDbParameter(
                                                            part_str[0], System.Data.OleDb.OleDbType.Integer));
                                                else if (part_str[1] == "string")
                                                    Con.Parameters.Add(
                                                        new System.Data.OleDb.OleDbParameter(
                                                            part_str[0], System.Data.OleDb.OleDbType.VarChar));
                                                else throw new Exception("Invalid Command");
                                            else if (part_str.Length == 3)
                                                if (part_str[1] == "int")
                                                    Con.Parameters.Add(
                                                        new System.Data.OleDb.OleDbParameter(
                                                            part_str[0], int.Parse(part_str[2])));
                                                else if (part_str[1] == "string")
                                                    Con.Parameters.Add(
                                                        new System.Data.OleDb.OleDbParameter(
                                                            part_str[0], part_str[2]));
                                                else throw new Exception("Invalid Command");
                                            break;
                                    }
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
                            switch (command)
                            {
                                case "sp": Con.CommandType = CommandType.StoredProcedure;
                                    break;
                                case "tx": Con.CommandType = CommandType.Text;
                                    break;
                                case "tb": Con.CommandType = CommandType.TableDirect;
                                    break;
                                default: throw new Exception("Invalid Command");
                            }
                            break;

                        case "com": Con.CommandText = command; break;

                        case "exec": Console.WriteLine(Con.ExecuteNonQuery()); break;

                        case "exer":
                            Con.ExecuteReader();
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
                            Console.WriteLine("state:");
                            Console.WriteLine("type:sql/oledb");
                            Console.WriteLine("con:");
                            Console.WriteLine("\tserver,database,user,pass - oledb/sql");
                            Console.WriteLine("\tpath,pass - oledb");
                            Console.WriteLine("\tserver,database  - server");
                            Console.WriteLine("open");
                            Console.WriteLine("ctype:sp/tx/tb");
                            Console.WriteLine("param:");
                            Console.WriteLine("\tadd:\tname,int/string");
                            Console.WriteLine("\t\tname,int/string,vlaue");
                            Console.WriteLine("\tremove:\tname");
                            Console.WriteLine("\tget:name");
                            Console.WriteLine("\tname:int,value");
                            Console.WriteLine("com:command_string");
                            Console.WriteLine("exec");
                            Console.WriteLine("exer");
                            Console.WriteLine("scan");
                            Console.WriteLine("close:connection/reader");
                            Console.WriteLine("quit");
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
                        Console.WriteLine(space = ex.Message);
                        ex = ex.InnerException;
                        space += " ";
                    }
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                order = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        //class 
    }
}
*/
#endif