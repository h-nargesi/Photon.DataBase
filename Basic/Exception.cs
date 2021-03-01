using System;
using System.Collections.Generic;
using System.Text;

namespace Photon.DataBase
{
    public class InvalidDataException : Exception
    {
        public InvalidDataException() : base("Invalid data") { }
        public InvalidDataException(string parameter)
            : base("Invalid data in " + parameter) { }
    }
}
