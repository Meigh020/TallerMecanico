
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Pages
{
    public class ClientesModel : PageModel
    {
        private readonly IRepositorio<Cliente> _repositorio;
        private readonly IDataProtector _protector;

        public ClientesModel(
            IRepositorio<Cliente> repositorio,
            IDataProtectionProvider provider)
        {
            _repositorio = repositorio;
            _protector = provider.CreateProtector(
                "TallerMecanicoSOLID.Clientes");
        }

        public List<Cliente> ListaClientes { get; set; }
            = new List<Cliente>();

        [BindProperty(SupportsGet = true)]
        public string Buscar { get; set; } = "";

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Buscar))
            {
                ListaClientes =
                    await _repositorio.ListaEntidadAsync();
            }
            else
            {
                ListaClientes =
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
                        "El cliente fue eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] =
                        "No se encontró el cliente o ya fue eliminado.";
                }
            }
            catch
            {
                TempData["Error"] =
                    "No se pudo procesar la eliminación del cliente.";
            }

            return RedirectToPage();
        }
    }
}
