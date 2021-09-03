using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Photon.Database
{
    public abstract class Connection : IConnection
    {
        #region Fields, Events, Contructor:
        private readonly DbConnection con; // connector
        private readonly DbCommand com; // handler
        private DbDataReader cor; // data_reader
        private DbTransaction trx; // transaction

        public abstract event ConnectionStingSetHandler ConnectionStringChange;
        public event EventHandler Disposed
        {
            add { con.Disposed += value; }
            remove { con.Disposed -= value; }
        }

        public Connection(DbConnection con, DbCommand com)
        {
            this.con = con ?? throw new ArgumentNullException(nameof(con));
            this.com = com ?? throw new ArgumentNullException(nameof(com));
        }
        #endregion


        #region Connection:
        public abstract IConnectionPath ConnectionString { get; set; }
        public int ConnectionTimeout
        {
            get { return con.ConnectionTimeout; }
        }
        public string Database
        {
            get { return con.Database; }
        }
        public ConnectionState State
        {
            get { return con.State; }
        }
        public abstract IConnection Clone();
        public void Open()
        {
            con.Open();
            com.Connection = con;
        }
        public async Task OpenAsync()
        {
            await con.OpenAsync();
            com.Connection = con;
        }
        public async Task OpenSafe()
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
            {
                await con.OpenAsync();
                com.Connection = con;
            }
        }
        public void CloseConnection()
        {
            Rollback();
            con.Close();
        }
        public async Task CloseConnectionAsync()
        {
            await RollbackAsync();
            con.Close();
        }
        public void Close()
        {
            if (cor != null && !cor.IsClosed) cor.Close();
            Rollback();
            con.Close();
        }
        public async Task CloseAsync()
        {
            if (cor != null && !cor.IsClosed) await cor.CloseAsync();
            await RollbackAsync();
            await con.CloseAsync();
        }

        public void Dispose()
        {
            if (cor != null) cor.Dispose();
            Rollback();
            com.Dispose();
            con.Dispose();
        }
        #endregion


        #region Transaction:
        public void BeginTransaction()
        {
            trx = con.BeginTransaction();
        }
        public async Task BeginTransactionAsync()
        {
            trx = await con.BeginTransactionAsync();
        }
        public bool HasTransaction
        {
            get { return trx != null; }
        }
        public void Commit()
        {
            if (trx != null)
            {
                trx.Commit();
                trx = null;
            }
        }
        public Task CommitAsync()
        {
            if (trx != null)
            {
                var t = trx;
                trx = null;
                return t.CommitAsync();
            }
            else return Task.CompletedTask;
        }
        public void Rollback()
        {
            if (trx != null)
            {
                trx.Rollback();
                trx = null;
            }
        }
        public Task RollbackAsync()
        {
            if (trx != null)
            {
                var t = trx;
                trx = null;
                return t.RollbackAsync();
            }
            else return Task.CompletedTask;
        }
        #endregion


        #region SQL Command:
        public string CommandText
        {
            get { return com.CommandText; }
            set { com.CommandText = value; }
        }
        public int CommandTimeout
        {
            get { return com.CommandTimeout; }
            set { com.CommandTimeout = value; }
        }
        public CommandType CommandType
        {
            get { return com.CommandType; }
            set { com.CommandType = value; }
        }
        public abstract object LastInsertedID { get; }

        public virtual int ExecuteNonQuery()
        {
            return com.ExecuteNonQuery();
        }
        public virtual DbDataReader ExecuteReader()
        {
            cor = com.ExecuteReader();
            return cor;
        }
        public virtual object ExecuteScalar()
        {
            return com.ExecuteScalar();
        }

        public virtual Task<int> ExecuteNonQueryAsync()
        {
            return com.ExecuteNonQueryAsync();
        }
        public virtual async Task<DbDataReader> ExecuteReaderAsync()
        {
            cor = await com.ExecuteReaderAsync();
            return cor;
        }
        public virtual Task<object> ExecuteScalarAsync()
        {
            return com.ExecuteScalarAsync();
        }

        public virtual async Task<int> ExecuteNonQuerySafe()
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                await OpenAsync();

            return com.ExecuteNonQueryAsync().Result;
        }
        public virtual async Task<DbDataReader> ExecuteReaderSafe()
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                await OpenAsync();

            cor = await com.ExecuteReaderAsync();
            return cor;
        }
        public virtual async Task<object> ExecuteScalarSafe()
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                await OpenAsync();

            return com.ExecuteScalarAsync().Result;
        }
        #endregion


        #region Reading Data:
        public object this[int index]
        {
            get { return cor?[index]; }
        }
        public object this[string index]
        {
            get { return cor?[index]; }
        }

        public bool ReaderIsClosed
        {
            get { return cor == null || cor.IsClosed; }
        }
        public int FieldCount
            => cor?.FieldCount ?? throw new DatabaseException("The command is not executed.");

        public bool Read()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            return cor.Read();
        }
        public bool NextResult()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            return cor.NextResult();
        }
        public void CloseReader()
        {
            cor?.Close();
        }

        public Task<bool> ReadAsync()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            return cor.ReadAsync();
        }
        public Task<bool> NextResultAsync()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            return cor.NextResultAsync();
        }
        public Task CloseReaderAsync()
        {
            if (cor != null) return cor.CloseAsync();
            else return Task.CompletedTask;
        }

        public byte[] GetBytes(int index)
        {
            return cor?.GetValue(index) as byte[];
        }
        public byte[] GetBytes(string index)
        {
            return cor?.GetValue(cor.GetOrdinal(index)) as byte[];
        }
        public string GetName(int index)
        {
            return cor?.GetName(index);
        }
        public int GetOrdinal(string name)
        {
            return cor?.GetOrdinal(name) ?? -1;
        }

        public virtual T GetValue<T>(int index)
        {
            return (T)cor?[index];
        }
        public virtual T GetValue<T>(string index)
        {
            return (T)cor?[index];
        }

        public Dictionary<string, int> GetColumns()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");

            var result = new Dictionary<string, int>();
            for (int i = 0; i < cor.FieldCount; i++)
                result.Add(cor.GetName(i), i);

            return result;
        }

        public IEnumerator GetEnumerator()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            else return cor.GetEnumerator();
        }
        #endregion


        #region Parameters:
        public DbParameterCollection Parameters
        {
            get { return com.Parameters; }
        }

        protected abstract DbParameter SetParam(string name,
            object type = null, int? size = null, bool? output = null);
        protected abstract DbParameter SetParam(MemberInfo member);
        protected abstract DbParameter SetFreeParam(MemberInfo member);

        public DbParameter SetParameter(string name, DbType? type, int? size = null, bool? output = null)
        {
            return SetParam(name: name, type: type, size: size, output: output);
        }

        /// <summary>
        /// To set the sql parameter as anonymous type
        /// if parameter exists in sql command handler, it will replace.
        /// </summary>
        /// <param name="name">The name of parameter</param>
        /// <param name="parameter">The sql-parameter</param>
        /// <returns>It returns this object to continue call other method</returns>
        public IConnection SetParameter(DbParameter parameter)
        {
            // check the paramters
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter), "Can not insert null parameter.");

            // remove exist parameter
            if (com.Parameters.Contains(parameter.ParameterName))
                com.Parameters.RemoveAt(parameter.ParameterName);

            // insert new parameter
            com.Parameters.Add(parameter);

            // continue
            return this;
        }
        /// <summary>
        /// To set the sql parameter as anonymous type
        /// if parameter exists in sql command handler, it will edit.
        /// </summary>
        /// <param name="name">The name of parameter</param>
        /// <param name="definition">
        /// is a anonymous type that precent several paramters
        /// {
        ///     Type = {SqlDbType|OledbType|SqliteType...},
        ///     IsOut = {bool},
        ///     Direction = {ParameterDirection},
        ///     Size = {int},
        ///     Value = {object}
        /// }
        /// </param>
        /// <returns>It returns this object to continue call other method</returns>
        public IConnection SetParameter(string name, object definition)
        {
            // check the paramters
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Can not insert parameter without name.");
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), "Can not insert null parameter.");

            // find type and propertoies
            var deftype = definition.GetType();

            object value;
            PropertyInfo pro;

            // Type {DbType}
            pro = deftype.GetProperty("Type");
            if (pro == null) value = null;
            else value = pro.GetValue(definition);

            // Parameter
            DbParameter parameter = SetParam(name, value);

            // Direction {ParameterDirection}
            pro = deftype.GetProperty("Direction");
            if (pro != null)
            {
                value = pro.GetValue(definition);
                if (value is ParameterDirection _val) parameter.Direction = _val;
                else if (value is string)
                    if (Enum.TryParse(value.ToString(), out _val))
                        parameter.Direction = _val;
            }

            // IsOut {bool}
            pro = deftype.GetProperty("IsOut");
            if (pro != null)
            {
                value = pro.GetValue(definition);
                if (value is bool _val)
                    parameter.Direction = _val ? ParameterDirection.InputOutput : ParameterDirection.Input;
                else if (value is string)
                    if (bool.TryParse(value.ToString(), out _val))
                        parameter.Direction = _val ? ParameterDirection.InputOutput : ParameterDirection.Input;
            }

            // Size {int}
            pro = deftype.GetProperty("Size");
            if (pro != null)
            {
                value = pro.GetValue(definition);
                if (value is int _val) parameter.Size = _val;
                else if (value is string)
                    if (int.TryParse(value.ToString(), out _val)) parameter.Size = _val;
            }

            // Value {object}
            pro = deftype.GetProperty("Value");
            if (pro != null) value = pro.GetValue(definition);
            if (value is DbValue db_value)
            {
                parameter.Value = db_value.Value ?? DBNull.Value;
                parameter.Value = db_value.Type;
            }
            else parameter.Value = value ?? DBNull.Value;

            return this;
        }

        /// <summary>
        /// To set parameters from a model
        /// if parameter exists in sql command handler, it will edit.
        /// </summary>
        /// <param name="model">an object with 'Sqlmodel' attribute</param>
        /// <returns>It returns this object to continue call other method</returns>
        public IConnection SetParameters(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // is model an sql-model
            var type = model.GetType();
            if (type.GetCustomAttribute<SqlModel>() == null)
                throw new Exception("Invalid sql-model object to set parameters.");

            // parse properties
            foreach (var property in type.GetProperties())
                SetParameter(property, property.GetValue(model));

            // parse fields
            foreach (var field in type.GetFields())
                SetParameter(field, field.GetValue(model));

            // continue
            return this;
        }
        /// <summary>
        /// To set parameters from a model with filter some feilds
        /// if parameter exists in sql command handler, it will edit.
        /// </summary>
        /// <param name="model">an object with 'Sqlmodel' attribute</param>
        /// <param name="filters">filter the properties or feilds of model</param>
        /// <returns>It returns this object to continue call other method</returns>
        public IConnection SetParameters(object model, HashSet<string> filters)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (filters.Count < 1) return this;

            // is model an sql-model
            var type = model.GetType();
            if (type.GetCustomAttribute<SqlModel>() == null)
                throw new Exception("Invalid sql-model object to set parameters.");

            // parse properties
            foreach (var property in type.GetProperties())
                if (filters.Contains(property.Name) &&
                    SetParameter(property, property.GetValue(model)) != null)
                {
                    filters.Remove(property.Name);
                    if (filters.Count < 1) return this;
                }

            if (filters.Count < 1) return this;

            // parse fields
            foreach (var field in type.GetFields())
                if (filters.Contains(field.Name) &&
                    SetParameter(field, field.GetValue(model)) != null)
                {
                    filters.Remove(field.Name);
                    if (filters.Count < 1) break;
                }

            // continue
            return this;
        }

        /// <summary>
        /// To set parameters from a anonymous model
        /// if parameter exists in sql command handler, it will edit.
        /// </summary>
        /// <param name="model">an object which can be anonymous type</param>
        /// <returns>It returns this object to continue call other method</returns>
        public IConnection SetFreeParameters(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // is model an sql-model
            var type = model.GetType();

            // parse properties
            foreach (var property in type.GetProperties())
                SetFreeParameter(property, property.GetValue(model));

            // parse fields
            foreach (var field in type.GetFields())
                SetFreeParameter(field, field.GetValue(model));

            // continue
            return this;
        }

        private DbParameter SetParameter(MemberInfo member, object value)
        {
            DbParameter parameter = SetParam(member);
            if (parameter == null) return null;

            // check value
            if (value == null) parameter.Value = DBNull.Value;
            else if (value is DbValue db_value)
            {
                parameter.Value = db_value.Value ?? DBNull.Value;
                parameter.Value = db_value.Type;
            }
            else
            {
                var sql_list = member.GetCustomAttribute<SqlList>();
                if (sql_list != null)
                    if (value is Array array) parameter.Value = GetSqlListValue(array);
                    else parameter.Value = value;
            }

            // check direction
            var dir = member.GetCustomAttribute<SqlDirection>();
            if (dir != null) parameter.Direction = dir.Direction;

            return parameter;
        }
        private DbParameter SetFreeParameter(MemberInfo member, object value)
        {
            DbParameter parameter = SetFreeParam(member);
            if (parameter == null) return null;

            // check value
            if (value is DbValue db_value)
            {
                parameter.Value = db_value.Value ?? DBNull.Value;
                parameter.Value = db_value.Type;
            }
            else parameter.Value = value ?? DBNull.Value;

            return parameter;
        }

        public static DataTable GetSqlListValue(Array values)
        {
            var table = new DataTable();
            foreach (var property in values.GetType().GetElementType().GetProperties())
                if (property.GetCustomAttribute<SqlParam>() != null)
                    table.Columns.Add(new DataColumn(property.Name, property.PropertyType));

            foreach (var val in values)
            {
                var row = table.NewRow();
                foreach (var property in val.GetType().GetProperties())
                    row[property.Name] = property.GetValue(val);
                table.Rows.Add(row);
            }

            return table;
        }
        #endregion
    }
}