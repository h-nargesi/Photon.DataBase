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

        public Task<T> Call<T>(string name, IConnection connection)
        {
            ProceduresHandler procedure;
            lock (procedures)
                procedure = procedures[name];

            if (!(procedure.Invoke(connection) is Task<T> task))
                throw new Exception("Invalid type of task result.");
            return task;
        }

        public void Register(string name, ProceduresHandler procedure)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (procedure == null) throw new ArgumentNullException(nameof(procedure));

            lock (procedures) procedures.Add(name, procedure);
        }
    }
}
