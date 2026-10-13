
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;
using TallerMecanicoSOLID.Repositories;
namespace TallerMecanicoSOLID.Pages
{
    public class NuevoClienteModel : PageModel
    {
        private readonly IRepositorio<Cliente> _repositorio;

        public NuevoClienteModel(ConfiguracionBD configuracion)
        {
            RepositorioFactory factory =
                new RepositorioFactory(configuracion);

            _repositorio = factory.CrearRepositorioCliente();
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = new Cliente 
        {
            Estado = true

        };

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Normalizar los datos antes de validarlos.
            Cliente.Nombre =
                Validaciones.Capitalizar(
                    Validaciones.SanitizarTexto(Cliente.Nombre, 80));

            Cliente.Apellido =
                Validaciones.Capitalizar(
                    Validaciones.SanitizarTexto(Cliente.Apellido, 80));

            Cliente.CiNit =
                Validaciones.SanitizarTexto(Cliente.CiNit, 20).Trim();

            Cliente.Telefono = Cliente.Telefono?.Trim() ?? "";

            Cliente.Email =
                Validaciones.SanitizarTexto(Cliente.Email, 120)
                    .Trim()
                    .ToLowerInvariant();

            Cliente.Direccion =
                Validaciones.SanitizarTexto(Cliente.Direccion, 200);

            // Volver a validar el modelo después de normalizar.
            ModelState.Clear();
            TryValidateModel(Cliente, nameof(Cliente));

            // Validaciones específicas del negocio.
            if (!Validaciones.ValidarTelefono(Cliente.Telefono))
            {
                ModelState.AddModelError(
                    "Cliente.Telefono",
                    "El teléfono contiene caracteres no permitidos o supera los 20 caracteres.");
            }

            if (!Validaciones.ValidarEmail(Cliente.Email))
            {
                ModelState.AddModelError(
                    "Cliente.Email",
                    "Ingrese un correo electrónico con un formato válido.");
            }
            if (!Validaciones.SoloLetras(Cliente.Nombre))
            {
                ModelState.AddModelError(
                    "Cliente.Nombre",
                    "El nombre solo puede contener letras y espacios.");
            }

            if (!Validaciones.SoloLetras(Cliente.Apellido))
            {
                ModelState.AddModelError(
                    "Cliente.Apellido",
                    "El apellido solo puede contener letras y espacios.");
            }

            if (!Validaciones.SoloNumeros(Cliente.CiNit))
            {
                ModelState.AddModelError(
                    "Cliente.CiNit",
                    "El CI/NIT debe contener únicamente números.");
            }

            if (string.IsNullOrWhiteSpace(Cliente.Telefono))
            {
                ModelState.AddModelError(
                    "Cliente.Telefono",
                    "El teléfono es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(Cliente.Direccion))
            {
                ModelState.AddModelError(
                    "Cliente.Direccion",
                    "La dirección es obligatoria.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }


            try
            {
                int resultado = _repositorio.Crear(Cliente);

                if (resultado > 0)
                {
                    TempData["Mensaje"] =
                        "El cliente fue registrado correctamente.";

                    return RedirectToPage("/Clientes");
                }

                ModelState.AddModelError(
                    "",
                    "No se pudo registrar el cliente. Inténtelo nuevamente.");

                return Page();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(
                    "Cliente.CiNit",
                    "Ya existe un cliente registrado con ese CI/NIT.");

                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "Ocurrió un error al guardar el cliente. Verifique los datos e inténtelo nuevamente.");

                return Page();
            }
        }
    }
}
