
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;
using TallerMecanicoSOLID.Repositories;

namespace TallerMecanicoSOLID.Pages
{
    public class EditarClienteModel : PageModel
    {
        private readonly IRepositorio<Cliente> _repositorio;
        private readonly IDataProtector _protector;

        public EditarClienteModel(
            ConfiguracionBD configuracion,
            IDataProtectionProvider provider)
        {
            RepositorioFactory factory =
                new RepositorioFactory(configuracion);

            _repositorio = factory.CrearRepositorioCliente();

            _protector = provider.CreateProtector(
                "TallerMecanicoSOLID.Clientes");
        }

        [BindProperty]
        public string NumeroCi { get; set; } = "";

        [BindProperty]
        public string ExtensionCi { get; set; } = "";

        [BindProperty]
        public Cliente Cliente { get; set; } = new Cliente();

        [BindProperty]
        public string Token { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(string id)
        {
            int clienteId;

            try
            {
                clienteId =
                    Convert.ToInt32(_protector.Unprotect(id));
            }
            catch
            {
                TempData["Error"] =
                    "El enlace de edición no es válido.";

                return RedirectToPage("/Clientes");
            }

            Cliente? cliente =
                await _repositorio.ObtenerIdAsync(clienteId);

            if (cliente == null)
            {
                TempData["Error"] =
                    "El cliente no existe o ya fue eliminado.";

                return RedirectToPage("/Clientes");
            }

            Cliente = cliente;
            string[] partesCi = (cliente.CiNit ?? "").Split('-', 2);
            NumeroCi = partesCi[0];
            ExtensionCi = partesCi.Length == 2 ? partesCi[1] : "";
            Token = id;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int clienteId;

            // Comprobar el token antes de modificar el registro.
            try
            {
                clienteId =
                    Convert.ToInt32(_protector.Unprotect(Token));

                if (clienteId <= 0)
                {
                    return RedirectToPage("/Clientes");
                }

                Cliente.Id = clienteId;
            }
            catch
            {
                TempData["Error"] =
                    "El enlace de edición no es válido.";

                return RedirectToPage("/Clientes");
            }

            // Normalizar los datos recibidos.
            Cliente.Nombre =
                Validaciones.Capitalizar(
                    Validaciones.SanitizarTexto(Cliente.Nombre, 80));

            Cliente.Apellido =
                Validaciones.Capitalizar(
                    Validaciones.SanitizarTexto(Cliente.Apellido, 80));

            NumeroCi = (NumeroCi ?? "").Trim();
            ExtensionCi = (ExtensionCi ?? "").Trim().ToUpperInvariant();
            string[] extensionesCi = { "CB", "LP", "SC", "OR", "PT", "TJ", "CH", "BE", "PA" };
            Cliente.CiNit = ExtensionCi.Length > 0
                ? $"{NumeroCi}-{ExtensionCi}"
                : NumeroCi;

            Cliente.Telefono = Cliente.Telefono?.Trim() ?? "";

            Cliente.Email =
                Validaciones.SanitizarTexto(Cliente.Email, 120)
                    .Trim()
                    .ToLowerInvariant();

            Cliente.Direccion =
                Validaciones.SanitizarTexto(Cliente.Direccion, 200);

            // Validar otra vez después de normalizar.
            ModelState.Clear();
            TryValidateModel(Cliente, nameof(Cliente));

            if (ExtensionCi.Length > 0 && !extensionesCi.Contains(ExtensionCi))
            {
                ModelState.AddModelError("ExtensionCi", "Seleccione una extensión válida.");
            }

            if (!Validaciones.ValidarTelefono(Cliente.Telefono))
            {
                ModelState.AddModelError(
                    "Cliente.Telefono",
                    "El teléfono contiene caracteres no permitidos o supera los 8 caracteres.");
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

            if (!Validaciones.ValidarCiNit(Cliente.CiNit))
            {
                ModelState.AddModelError(
                    "NumeroCi",
                    "Ingrese un número de CI válido, con extensión opcional. Ejemplo: 8815391-CB.");
            }
            if (!Validaciones.SoloNumeros(NumeroCi))
            {
                ModelState.AddModelError("NumeroCi", "El número de CI debe contener solo dígitos.");
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
                // Conservar el token para volver a enviar el formulario.
                return Page();
            }

            try
            {
                // Comprobar que el registro sigue activo.
                Cliente? existente =
                    await _repositorio.ObtenerIdAsync(clienteId);

                if (existente == null)
                {
                    TempData["Error"] =
                        "El cliente no existe o ya fue eliminado.";

                    return RedirectToPage("/Clientes");
                }

                int resultado =
                    await _repositorio.EditarAsync(Cliente);

                if (resultado > 0)
                {
                    TempData["Mensaje"] =
                        "El cliente fue actualizado correctamente.";

                    return RedirectToPage("/Clientes");
                }

                ModelState.AddModelError(
                    "",
                    "No se pudo actualizar el cliente. Verifique que siga activo.");

                return Page();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(
                    "Cliente.CiNit",
                    "Ya existe otro cliente registrado con ese CI/NIT.");

                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "Ocurrió un error al actualizar el cliente. Inténtelo nuevamente.");

                return Page();
            }
        }
    }
}
