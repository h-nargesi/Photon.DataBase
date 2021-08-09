using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photon.Database.Procedures
{
    public class ProcedureRegister
    {
        public ProcedureRegister()
        {
            procedures = new Dictionary<string, ProceduresHandler>();
        }

        private readonly Dictionary<string, ProceduresHandler> procedures;

        public object Call(string name, IConnection connection)
        {
            ProceduresHandler procedure;
            lock (procedures)
                procedure = procedures[name];

            return procedure.Invoke(connection);
        }

        public void Register(string name, ProceduresHandler procedure)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (procedure == null) throw new ArgumentNullException(nameof(procedure));
            procedures.Add(name, procedure);
        }
    }
}
