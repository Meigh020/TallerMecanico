using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Factories
{
    public class VehiculoFactory : EntidadFactory<Vehiculo>
    {
        public override Vehiculo Crear()
        {
            return new Vehiculo();  

        }
    }
}
