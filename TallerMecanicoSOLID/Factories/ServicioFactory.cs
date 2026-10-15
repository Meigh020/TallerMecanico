using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Factories
{
    public class ServicioFactory: EntidadFactory<Servicio>
    {
        public override Servicio Crear()
        {
            return new Servicio();
        }
    }
}
