using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Factories
{
    public class ClienteFactory : EntidadFactory<Cliente>
    {
        public override Cliente Crear()
        {
            return new Cliente
            {
                Nombre = "Nombre del cliente",
                Apellido = "Apellido del cliente",
                CiNit = "123456789",
                Telefono = "555-1234",
                Email = ""
            };
        }
    }
}
