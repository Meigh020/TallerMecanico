
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRepositorio<Vehiculo> _repositorio;

        public IndexModel(IRepositorio<Vehiculo> repositorio)
        {
            _repositorio = repositorio;
        }

        public List<Vehiculo> Vehiculos { get; set; }
            = new List<Vehiculo>();

        public int TotalVehiculos { get; set; }

        public int TotalEnReparacion { get; set; }

        public int TotalListosEntrega { get; set; }

        public int TotalEntregasHoy { get; set; }

        public async Task OnGetAsync()
        {
            Vehiculos = await _repositorio.ListaEntidadAsync();

            TotalVehiculos = Vehiculos.Count;
        }
    }
}
