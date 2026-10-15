
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;
using TallerMecanicoSOLID.Repositories;

namespace TallerMecanicoSOLID.Pages
{
    public class EditarServicioModel : PageModel
    {
        private readonly IRepositorio<Servicio> _repositorio;
        private readonly IDataProtector _protector;

        public EditarServicioModel(
            ConfiguracionBD configuracion,
            IDataProtectionProvider provider)
        {
            RepositorioFactory factory =
                new RepositorioFactory(configuracion);

            _repositorio = factory.CrearRepositorioServicio();

            _protector = provider.CreateProtector(
                "TallerMecanicoSOLID.Servicios");
        }

        [BindProperty]
        public Servicio Servicio { get; set; } = new Servicio();

        [BindProperty]
        public string Token { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                int servicioId =
                    Convert.ToInt32(_protector.Unprotect(id));

                if (!Validaciones.EsIdValido(servicioId))
                {
                    return RedirectToPage("/Servicios");
                }

                Servicio? servicio =
                    await _repositorio.ObtenerIdAsync(servicioId);

                if (servicio == null)
                {
                    TempData["Error"] =
                        "El servicio no existe o está inactivo.";

                    return RedirectToPage("/Servicios");
                }

                Servicio = servicio;
                Token = id;

                return Page();
            }
            catch
            {
                TempData["Error"] =
                    "El enlace de edición no es válido.";

                return RedirectToPage("/Servicios");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Verificar el token antes de editar.
            int servicioId;

            try
            {
                servicioId =
                    Convert.ToInt32(_protector.Unprotect(Token));

                if (!Validaciones.EsIdValido(servicioId))
                {
                    return RedirectToPage("/Servicios");
                }

                Servicio.Id = servicioId;
            }
            catch
            {
                TempData["Error"] =
                    "El enlace de edición no es válido.";

                return RedirectToPage("/Servicios");
            }

            // 2. Normalizar los textos.
            Servicio.Nombre =
                Validaciones.SanitizarTexto(Servicio.Nombre, 120);

            Servicio.Descripcion =
                Validaciones.SanitizarTexto(
                    Servicio.Descripcion, 255);

            Servicio.Nombre =
                Validaciones.Capitalizar(Servicio.Nombre);

            Servicio.Tipo = Servicio.Tipo?.Trim() ?? "";

            Servicio.UnidadDuracion =
                Servicio.UnidadDuracion?.Trim() ?? "";

            // 3. Convertir el tipo al valor permitido por MySQL.
            switch (Servicio.Tipo)
            {
                case "Mantenimiento preventivo":
                case "preventivo":
                    Servicio.Tipo = "preventivo";
                    break;

                case "Mantenimiento correctivo":
                case "correctivo":
                    Servicio.Tipo = "correctivo";
                    break;

                case "Revisión simple":
                case "Revision simple":
                case "revision_simple":
                    Servicio.Tipo = "revision_simple";
                    break;

                default:
                    Servicio.Tipo = "";
                    break;
            }

            // 4. Volver a validar el modelo normalizado.
            ModelState.Clear();
            TryValidateModel(Servicio, nameof(Servicio));

            // 5. Validar el nombre.
            if (!Validaciones.LetrasNumerosEspacios(Servicio.Nombre))
            {
                ModelState.AddModelError(
                    "Servicio.Nombre",
                    "El nombre solo puede contener letras, números y espacios.");
            }

            // 6. Validar el tipo permitido por la base de datos.
            string[] tiposPermitidos =
            {
                "preventivo",
                "correctivo",
                "revision_simple"
            };

            if (string.IsNullOrWhiteSpace(Servicio.Tipo) ||
                !tiposPermitidos.Contains(Servicio.Tipo))
            {
                ModelState.AddModelError(
                    "Servicio.Tipo",
                    "Seleccione un tipo de servicio válido.");
            }

            // 7. Validar el precio.
            if (Servicio.Precio <= 0)
            {
                ModelState.AddModelError(
                    "Servicio.Precio",
                    "El precio debe ser mayor a 0.");
            }

            // 8. Validar la duración.
            if (Servicio.DuracionEstimada <= 0)
            {
                ModelState.AddModelError(
                    "Servicio.DuracionEstimada",
                    "La duración estimada debe ser mayor a 0.");
            }

            // 9. Validar la unidad de duración.
            if (string.IsNullOrWhiteSpace(Servicio.UnidadDuracion))
            {
                ModelState.AddModelError(
                    "Servicio.UnidadDuracion",
                    "Debe seleccionar una unidad de duración.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Comprobar que el servicio todavía está activo.
                Servicio? existente =
                    await _repositorio.ObtenerIdAsync(servicioId);

                if (existente == null)
                {
                    TempData["Error"] =
                        "El servicio no existe o está inactivo.";

                    return RedirectToPage("/Servicios");
                }

                int resultado =
                    await _repositorio.EditarAsync(Servicio);

                if (resultado > 0)
                {
                    TempData["Mensaje"] =
                        "El servicio fue actualizado correctamente.";

                    return RedirectToPage("/Servicios");
                }

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo actualizar el servicio. Verifique que siga activo.");

                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo actualizar el servicio. " +
                    "Verifique los datos e inténtelo nuevamente.");

                return Page();
            }
        }
    }
}
