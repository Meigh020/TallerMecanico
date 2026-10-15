
using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Interfaces
{
    public interface IRepositorio<T> where T : EntidadBase
    {
        Task<List<T>> ListaEntidadAsync();

        Task<T?> ObtenerIdAsync(int id);

        Task<List<T>> BuscarAsync(string texto);

        Task<int> CrearAsync(T entidad);

        Task<int> EditarAsync(T entidad);

        Task<int> EliminarAsync(int id);
    }
}
