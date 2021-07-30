using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Photon.Database.Extensions;

namespace Photon.Database
{
    public class MsSqlConnection : Connection, IMsSqlConnection
    {
        private readonly SqlConnection con;
        private readonly SqlCommand com;
        private SQLConnectionPath path;

        public override event ConnectionStingSetHandler ConnectionStringChange;
        public event SqlInfoMessageEventHandler InfoMessage
        {
            add { con.InfoMessage += value; }
            remove { con.InfoMessage -= value; }
        }

        private static SqlConnection Con(out SqlConnection con)
        {
            con = new SqlConnection();
            return con;
        }
        private static SqlCommand Com(out SqlCommand com)
        {
            com = new SqlCommand();
            return com;
        }

        public MsSqlConnection() : base(Con(out SqlConnection con), Com(out SqlCommand com))
        {
            this.con = con;
            this.com = com;
        }
        private MsSqlConnection(SqlConnection con) : base(con, Com(out SqlCommand com))
        {
            this.con = con;
            this.com = com;
        }

        public override IConnection Clone()
        {
            return ((IMsSqlConnection)this).Clone();
        }
        IMsSqlConnection IMsSqlConnection.Clone()
        {
            return new MsSqlConnection(con);
        }

        public override IConnectionPath ConnectionString
        {
            get { return path; }
            set
            {
                if (value is SQLConnectionPath sql_path)
                    SetConnectionString(sql_path);

                else throw new ArgumentException(
                    nameof(value), "Invalid type of connection-string.");
            }
        }
        SQLConnectionPath IMsSqlConnection.ConnectionString
        {
            get { return path; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                SetConnectionString(value);
            }
        }
        private void SetConnectionString(SQLConnectionPath con_str)
        {
            path = (SQLConnectionPath)con_str.Copy();
            con.ConnectionString = path.ToString();

            ConnectionStringChange?.Invoke(this, new EventArgs());
        }

        public override object LastInsertedID => throw new NotImplementedException();


        #region Parameters:
        SqlParameterCollection IMsSqlConnection.Parameters
        {
            get { return com.Parameters; }
        }

        protected override DbParameter SetParam(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<MsSqlParam>();
            if (attribute == null) return null;

            string name = attribute.Name ?? member.Name;
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new SqlParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            if (attribute.Type != null) parameter.SqlDbType = attribute.Type.Value;
            if (attribute.Size != null) parameter.Size = attribute.Size.Value;

            return parameter;
        }
        protected override DbParameter SetFreeParam(MemberInfo member)
        {
            string name = member.Name;
            if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new SqlParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            return parameter;
        }

        protected override DbParameter SetParam(string name,
            object type = null, int? size = null, bool? output = null)
        {
            if (type is SqlDbType sql_type) return SetParameter(name, sql_type, size, output);
            else if (type is DbType db_type) return SetParameter(name, db_type.GetSqlDbType(), size, output);
            else if (type is string str_type && Enum.TryParse(str_type, out sql_type))
                return SetParameter(name, sql_type, size, output);
            else return SetParameter(name, null, size, output);
        }

        public SqlParameter SetParameter(string name,
            SqlDbType? type = null, int? size = null, bool? output = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            else if (!name.StartsWith("@")) name = "@" + name;

            SqlParameter parameter;
            // find the exists parameter
            if (com.Parameters.Contains(name)) parameter = com.Parameters[name];
            // create new if not exists
            else
            {
                parameter = new SqlParameter() { ParameterName = name };
                com.Parameters.Add(parameter);
            }

            if (type != null) parameter.SqlDbType = type.Value;
            if (size != null) parameter.Size = size.Value;
            if (output != null) parameter.Direction = output.Value ?
                    ParameterDirection.InputOutput : ParameterDirection.Input;

            return parameter;
        }

        public SqlParameter SetParameter(string name, string udt_type, bool? output)
        {
            SqlParameter parameter = SetParameter(name, type: SqlDbType.Udt, output: output);

            parameter.UdtTypeName = udt_type;
            if (output != null) parameter.Direction = output.Value ?
                    ParameterDirection.InputOutput : ParameterDirection.Input;

            return parameter;
        }

        #endregion
    }
}
