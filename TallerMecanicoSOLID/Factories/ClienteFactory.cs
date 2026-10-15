using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Factories
{
    public class ClienteFactory : EntidadFactory<Cliente>
    {
        public override Cliente Crear()
        {
            return new Cliente();
            
        }
    }
}
