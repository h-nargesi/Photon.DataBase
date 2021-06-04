using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Photon.Database.Extensions
{
    public static class Extensions
    {
        public static Task<int> ExecuteNonQuerySafe(this IConnection connection, string command, object model = null)
        {
            if (model != null)
            {
                connection.Parameters.Clear();
                connection.SetParameters(model);
            }

            connection.CommandText = command;
            return connection.ExecuteNonQuerySafe();
        }
        public static Task<DbDataReader> ExecuteReaderSafe(this IConnection connection, string command, object model = null)
        {
            if (model != null)
            {
                connection.Parameters.Clear();
                connection.SetParameters(model);
            }

            connection.CommandText = command;
            return connection.ExecuteReaderSafe();
        }


        public static async Task<L> Literal<L>(this IConnection connection, string query, object model = null)
        {
            if (model != null)
            {
                connection.Parameters.Clear();
                connection.SetParameters(model);
            }

            connection.CommandText = query;
            using (var reader = await connection.ExecuteReaderSafe())
                if (await reader.ReadAsync()) return connection.GetValue<L>(0);
                else throw new Exception("Not found");
        }
        public static async Task<Nullable<L>> LiteralNullable<L>(this IConnection connection, string query, object model = null) where L : struct
        {
            if (model != null)
            {
                connection.Parameters.Clear();
                connection.SetParameters(model);
            }

            connection.CommandText = query;
            using (var reader = await connection.ExecuteReaderSafe())
                if (await reader.ReadAsync()) return connection.GetValue<Nullable<L>>(0);
                else return null;
        }
        public static async Task<List<L>> LiteralList<L>(this IConnection connection, string query, object model = null)
        {
            var list = new List<L>();

            if (model != null)
            {
                connection.Parameters.Clear();
                connection.SetParameters(model);
            }

            connection.CommandText = query;
            using (var reader = await connection.ExecuteReaderSafe())
                while (await reader.ReadAsync()) list.Add(connection.GetValue<L>(0));

            return list;
        }
        public static async Task<Dictionary<K, V>> LiteralGroup<K, V>(this IConnection connection, string query, object model = null)
        {
            var list = new Dictionary<K, V>();

            if (model != null)
            {
                connection.Parameters.Clear();
                connection.SetParameters(model);
            }

            connection.CommandText = query;
            using (var reader = await connection.ExecuteReaderSafe())
                while (await reader.ReadAsync()) list.Add(connection.GetValue<K>(0), connection.GetValue<V>(1));

            return list;
        }
        public static async Task<Dictionary<string, object>> LiteralObject(this IConnection connection, string query, object model = null)
        {
            var obj = new Dictionary<string, object>();

            if (model != null)
            {
                connection.Parameters.Clear();
                connection.SetParameters(model);
            }

            connection.CommandText = query;
            using (var reader = await connection.ExecuteReaderSafe())
                while (await reader.ReadAsync())
                    for (int i = 0; i < reader.FieldCount; i++)
                        obj.Add(reader.GetName(i), reader[i]);

            return obj;
        }

        public static Task<bool> Exists(this IConnection connection, string query, object model = null)
        {
            query = $@"select case when exists ({query}) then 1 else 0 end as ext";
            return Literal<bool>(connection, query, model);
        }
    }
}