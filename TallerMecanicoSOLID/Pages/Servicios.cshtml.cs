
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;
using TallerMecanicoSOLID.Repositories;

namespace TallerMecanicoSOLID.Pages
{
    public class ServiciosModel : PageModel
    {
        private readonly IRepositorio<Servicio> _repositorio;
        private readonly IDataProtector _protector;

        public ServiciosModel(
            ConfiguracionBD configuracion,
            IDataProtectionProvider provider)
        {
            RepositorioFactory factory =
                new RepositorioFactory(configuracion);

            _repositorio = factory.CrearRepositorioServicio();

            _protector = provider.CreateProtector(
                "TallerMecanicoSOLID.Servicios");
        }

        public List<Servicio> ListaServicios { get; set; }
            = new List<Servicio>();

        [BindProperty(SupportsGet = true)]
        public string Buscar { get; set; } = "";

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Buscar))
            {
                ListaServicios =
                    await _repositorio.ListaEntidadAsync();
            }
            else
            {
                ListaServicios =
                    await _repositorio.BuscarAsync(Buscar);
            }
        }

        public string GenerarToken(int id)
        {
            return _protector.Protect(id.ToString());
        }

        public async Task<IActionResult> OnPostEliminarAsync(
            string token)
        {
            try
            {
                string textoId = _protector.Unprotect(token);
                int id = Convert.ToInt32(textoId);

                int resultado =
                    await _repositorio.EliminarAsync(id);

                if (resultado > 0)
                {
                    TempData["Mensaje"] =
                        "El servicio fue eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] =
                        "No se encontró el servicio o ya fue eliminado.";
                }
            }
            catch
            {
                TempData["Error"] =
                    "No se pudo procesar la eliminación del servicio.";
            }

            return RedirectToPage();
        }
    }
}
