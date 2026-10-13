using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Interfaces
{
    public interface IRepositorio<T> where T : EntidadBase
    {
        List<T> ListaEntidad();
        T? ObtenerId(int id);
        List<T> Buscar(string texto);
        int Crear(T entidad);
        int Editar(T entidad);
        int Eliminar(int id);
    }
}
