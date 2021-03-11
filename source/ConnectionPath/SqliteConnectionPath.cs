using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    public class SqliteConnectionPath : ConnectionPath
    {
        public SqliteConnectionPath(string path) : this(path, null) {}
        
        public SqliteConnectionPath(string path, string password) {
            if (String.IsNullOrEmpty(path))
                throw new InvalidDataException("The path is null or empty.");
            this.path = path;
            this.password = password;
        }

        public readonly string path, password;
        
        public override ConnectionPath Copy()
        {
            return new SqliteConnectionPath(path, password);
        }

        public override string ToString()
        {
            string result = $"Data Source={path};";
            if (password != null) result += $"Password={password};";
            return result;
        }
    }
}
