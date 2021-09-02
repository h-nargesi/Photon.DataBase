using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.Database
{
    public class SqliteConnectionPath : IConnectionPath
    {
        public SqliteConnectionPath(string path, string version = null, string password = null, bool? foreign_keys = true)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new InvalidDataException("The path is null or empty.");
            this.path = path;
            this.version = version;
            this.password = password;
            this.foreign_keys = foreign_keys;
        }

        public readonly string path, version, password;
        public readonly bool? foreign_keys;

        public IConnectionPath Copy()
        {
            return new SqliteConnectionPath(path, version, password);
        }

        public override string ToString()
        {
            string result = $"Data Source={path};";
            if (version != null) result += $"Version={version};";
            if (password != null) result += $"Password={password};";
            if (foreign_keys != null) result += $"foreign keys={foreign_keys.ToString().ToLower()}";
            return result;
        }
    }
}
