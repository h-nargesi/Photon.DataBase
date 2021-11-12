using System.Threading.Tasks;
namespace Photon.Database.Procedures
{
    public delegate Task ProceduresHandler(IConnection connection);
}