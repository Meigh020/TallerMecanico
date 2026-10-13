using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Factories
{
    public class ServicioFactory: EntidadFactory<Servicio>
    {
        public override Servicio Crear()
        {
            return new Servicio
            {
                Nombre = "Nombre del servicio",
                Tipo = "Tipo del servicio",
                Descripcion = "Descripción del servicio",
                Precio = 100.0m,
                DuracionEstimada = 4,
                UnidadDuracion = "dias"

            };
        }
    }
}
