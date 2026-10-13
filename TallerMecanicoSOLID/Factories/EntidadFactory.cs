using TallerMecanicoSOLID.Models;     

namespace TallerMecanicoSOLID.Factories
{
    public abstract class EntidadFactory<T> 
        where T : EntidadBase
    {
        public abstract T Crear();
    }
}
