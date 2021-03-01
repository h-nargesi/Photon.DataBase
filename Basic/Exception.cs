using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.DataBase
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message) { }
        public DatabaseException(string message, Exception inner) : base(message, inner) { }
    }
    public class InvalidDataException : DatabaseException
    {
        public InvalidDataException() : base("Invalid data") { }
        public InvalidDataException(string parameter)
            : base("Invalid data in " + parameter) { }
    }
}
