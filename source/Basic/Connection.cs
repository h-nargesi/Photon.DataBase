using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    public abstract class Connection : IConnection
    {
        #region Fields, Events, Contructor:
        private readonly DbConnection con;
        private readonly DbCommand com;
        private DbDataReader cor;
        private DbTransaction trx;
        private bool cor_is_reading;

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
        public abstract ConnectionPath ConnectionString { get; set; }
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
        public void CloseConnection()
        {
            Rollback();
            con.Close();
        }
        public void Close()
        {
            if (cor != null && !cor.IsClosed) cor.Close();
            Rollback();
            con.Close();
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
        public bool HasTransaction
        {
            get { return trx != null; }
        }
        public void Commit()
        {
            if (trx != null) {
                trx.Commit();
                trx = null;
            }
        }
        public void Rollback()
        {
            if (trx != null) {
                trx.Rollback();
                trx = null;
            }
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

        public int ExecuteNonQuery()
        {
            return com.ExecuteNonQuery();
        }
        public DbDataReader ExecuteReader()
        {
            cor_is_reading = false;
            cor = com.ExecuteReader();
            return cor;
        }
        public object ExecuteScalar()
        {
            return com.ExecuteScalar();
        }
        #endregion


        #region Parameters:
        public DbParameterCollection Parameters
        {
            get { return com.Parameters; }
        }
        public abstract DbParameter AddParameter(string name, bool output = false);
        public abstract DbParameter AddParameter(string name, DbType type, bool output = false);
        public abstract DbParameter AddParameter(string name, DbType type, int size, bool output = false);
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
        public bool IsReading
        {
            // is usefull for inner subject reading
            get { return cor != null && !cor.IsClosed && cor_is_reading; }
        }
        public int FieldCount
        {
            get
            {
                if (cor == null)
                    throw new DatabaseException("The command is not executed.");
                return cor.FieldCount;
            }
        }

        public bool Read()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            cor_is_reading = cor.Read();
            return cor_is_reading;
        }
        public bool NextResult()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            cor_is_reading = cor.NextResult();
            return cor_is_reading;
        }
        public void CloseReader()
        {
            cor_is_reading = false;
            if (cor != null) cor.Close();
        }

        public byte[] GetBytes(int index)
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            return cor.GetValue(index) as byte[];
        }
        public byte[] GetBytes(string index)
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            return cor.GetValue(cor.GetOrdinal(index)) as byte[];
        }
        public string GetName(int index)
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            return cor.GetName(index);
        }
        public int GetOrdinal(string name)
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            return cor.GetOrdinal(name);
        }

        public IEnumerator GetEnumerator()
        {
            if (cor == null)
                throw new DatabaseException("The command is not executed.");
            else return cor.GetEnumerator();
        }
        #endregion
    }
}