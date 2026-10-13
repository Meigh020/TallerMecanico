
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;
using TallerMecanicoSOLID.Repositories;

namespace TallerMecanicoSOLID.Pages
{
    public class VehiculosModel : PageModel
    {
        private readonly IRepositorio<Vehiculo> _repositorio;
        private readonly IDataProtector _protector;

        public VehiculosModel(
            ConfiguracionBD configuracion,
            IDataProtectionProvider provider)
        {
            RepositorioFactory factory =
                new RepositorioFactory(configuracion);

            _repositorio = factory.CrearRepositorioVehiculo();

            _protector = provider.CreateProtector(
                "TallerMecanicoSOLID.Vehiculos");
        }

        public List<Vehiculo> ListaVehiculos { get; set; }
            = new List<Vehiculo>();

        [BindProperty(SupportsGet = true)]
        public string Buscar { get; set; } = "";

        public void OnGet()
        {
            if (string.IsNullOrWhiteSpace(Buscar))
            {
                ListaVehiculos = _repositorio.ListaEntidad();
            }
            else
            {
                Buscar = Validaciones.SanitizarTexto(Buscar, 100);
                ListaVehiculos = _repositorio.Buscar(Buscar);
            }
        }

        public string GenerarToken(int id)
        {
            return _protector.Protect(id.ToString());
        }

        public IActionResult OnPostEliminar(string token)
        {
            try
            {
                string textoId = _protector.Unprotect(token);
                int id = Convert.ToInt32(textoId);

                if (!Validaciones.EsIdValido(id))
                {
                    TempData["Error"] = "El identificador del vehículo no es válido.";
                    return RedirectToPage();
                }

                int resultado = _repositorio.Eliminar(id);

                if (resultado > 0)
                {
                    TempData["Mensaje"] =
                        "El vehículo fue eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] =
                        "No se encontró el vehículo o ya fue eliminado.";
                }
            }
            catch
            {
                TempData["Error"] =
                    "No se pudo procesar la eliminación del vehículo.";
            }

            return RedirectToPage();
        }
    }
}
