using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;
using TallerMecanicoSOLID.Repositories;

namespace TallerMecanicoSOLID.Pages
{
    public class NuevoServicioModel : PageModel
    {
        private readonly IRepositorio<Servicio> _repositorio;

        public NuevoServicioModel(ConfiguracionBD configuracion)
        {
            RepositorioFactory factory =
                new RepositorioFactory(configuracion);

            _repositorio = factory.CrearRepositorioServicio();
        }

        [BindProperty]
        public Servicio Servicio { get; set; } = new Servicio 
        {
            Estado = true
        
        };

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Servicio.Nombre =
                Validaciones.SanitizarTexto(Servicio.Nombre, 120);

            Servicio.Descripcion =
                Validaciones.SanitizarTexto(Servicio.Descripcion, 255);

            Servicio.Nombre = Validaciones.Capitalizar(Servicio.Nombre);

            // Convertir el tipo seleccionado al valor aceptado por MySQL.
            switch (Servicio.Tipo?.Trim())
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
                    ModelState.AddModelError(
                        "Servicio.Tipo",
                        "Debe seleccionar un tipo de servicio válido.");
                    break;
            }

            ModelState.Clear();
            TryValidateModel(Servicio, nameof(Servicio));

            // Volver a comprobar el tipo después de limpiar ModelState.
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
                    "Debe seleccionar un tipo de servicio válido.");
            }

            if (!Validaciones.LetrasNumerosEspacios(Servicio.Nombre))
            {
                ModelState.AddModelError(
                    "Servicio.Nombre",
                    "El nombre solo puede contener letras, números y espacios.");
            }

            if (Servicio.Precio <= 0)
            {
                ModelState.AddModelError(
                    "Servicio.Precio",
                    "El precio debe ser mayor a 0.");
            }

            if (Servicio.DuracionEstimada <= 0)
            {
                ModelState.AddModelError(
                    "Servicio.DuracionEstimada",
                    "La duración estimada debe ser mayor a 0.");
            }

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
                int resultado = _repositorio.Crear(Servicio);

                if (resultado > 0)
                {
                    TempData["Mensaje"] =
                        "El servicio fue registrado correctamente.";

                    return RedirectToPage("/Servicios");
                }

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo registrar el servicio.");

                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error al guardar el servicio. " +
                    "Verifique los datos e intente nuevamente.");

                return Page();
            }
        }
    }
}
