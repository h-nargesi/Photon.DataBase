﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Photon.Database.Extensions;

namespace Photon.Database
{
    public class OledbConnection : Connection, IOledbConnection
    {
        private readonly OleDbConnection con;
        private readonly OleDbCommand com;
        private OledbConnectionPath path;

        public override event ConnectionStingSetHandler ConnectionStringChange;

        private static OleDbConnection Con(out OleDbConnection con, string connection_string = null)
        {
            if (connection_string == null) con = new OleDbConnection();
            else con = new OleDbConnection(connection_string);
            return con;
        }
        private static OleDbCommand Com(out OleDbCommand com)
        {
            com = new OleDbCommand();
            return com;
        }

        public OledbConnection() : base(Con(out OleDbConnection con), Com(out OleDbCommand com))
        {
            this.con = con;
            this.com = com;
        }
        private OledbConnection(OleDbConnection con) : 
            base(Con(out OleDbConnection new_con, con.ConnectionString), Com(out OleDbCommand com))
        {
            this.con = new_con;
            this.com = com;
        }

        public override IConnection Clone()
        {
            return (this as IOledbConnection).Clone();
        }
        IOledbConnection IOledbConnection.Clone()
        {
            return new OledbConnection(con);
        }

        public override IConnectionPath ConnectionString
        {
            get { return path; }
            set
            {
                if (value is OledbConnectionPath oledb_path)
                    SetConnectionString(oledb_path);

                else throw new ArgumentException(
                    nameof(value), "Invalid type of connection-string.");
            }
        }
        OledbConnectionPath IOledbConnection.ConnectionString
        {
            get { return path; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                SetConnectionString(value);
            }
        }
        private void SetConnectionString(OledbConnectionPath con_str)
        {
            path = (OledbConnectionPath)con_str.Copy();
            con.ConnectionString = path.ToString();

            ConnectionStringChange?.Invoke(this, new EventArgs());
        }

        public override object LastInsertedID => throw new NotImplementedException();


        #region Parameters:
        OleDbParameterCollection IOledbConnection.Parameters
        {
            get { return com.Parameters; }
        }

        protected override DbParameter SetParam(MemberInfo member, Type value_type)
        {
            var attribute = member.GetCustomAttribute<OledbParam>();
            if (attribute == null) return null;

            string name = attribute.Name ?? member.Name;
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("@")) name = "@" + name;

            OleDbParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new OleDbParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            if (attribute.Size != null) parameter.Size = attribute.Size.Value;
            if (attribute.Type != null) parameter.OleDbType = attribute.Type.Value;
            else if (value_type != null && value_type != typeof(DbValue))
                parameter.OleDbType = value_type.GetDbType().GetOleDbType();

            return parameter;
        }
        protected override DbParameter SetFreeParam(MemberInfo member, Type value_type)
        {
            string name = member.Name;
            if (!name.StartsWith("@")) name = "@" + name;

            OleDbParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new OleDbParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            if (value_type != null && value_type != typeof(DbValue))
                parameter.OleDbType = value_type.GetDbType().GetOleDbType();

            return parameter;
        }

        protected override DbParameter SetParam(string name,
            object type = null, int? size = null, bool? output = null)
        {
            if (type is OleDbType oledb_type) return SetParameter(name, oledb_type, size, output);
            else if (type is DbType db_type) return SetParameter(name, db_type.GetOleDbType(), size, output);
            else if (type is Type sys_type) return SetParameter(name, sys_type.GetDbType().GetOleDbType(), size, output);
            else if (type is string str_type && Enum.TryParse(str_type, out oledb_type))
                return SetParameter(name, oledb_type, size, output);
            else return SetParameter(name, null, size, output);
        }

        public OleDbParameter SetParameter(string name,
            OleDbType? type = null, int? size = null, bool? output = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("@")) name = "@" + name;

            OleDbParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new OleDbParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            if (type != null) parameter.OleDbType = type.Value;
            if (size != null) parameter.Size = size.Value;
            if (output != null) parameter.Direction = output.Value ?
                    ParameterDirection.InputOutput : ParameterDirection.Input;

            return parameter;
        }
        #endregion

    }
}
