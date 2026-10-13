using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Repositories
{
    /// <summary>
    /// Fábrica de repositorios (Factory Method).
    /// Permite obtener implementaciones de IRepositorio&lt;T&gt;
    /// a partir de la configuración de base de datos,
    /// sin que las páginas conozcan las clases concretas.
    /// </summary>
    public class RepositorioFactory
    {
        private readonly ConfiguracionBD _configuracion;

        public RepositorioFactory(ConfiguracionBD configuracion)
        {
            _configuracion = configuracion;
        }

        public IRepositorio<Cliente> CrearRepositorioCliente()
        {
            return new RepositorioCliente(_configuracion);
        }

        public IRepositorio<Vehiculo> CrearRepositorioVehiculo()
        {
            return new RepositorioVehiculo(_configuracion);
        }

        public IRepositorio<Servicio> CrearRepositorioServicio()
        {
            return new RepositorioServicio(_configuracion);
        }
    }
}
