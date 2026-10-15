
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
    public class EditarVehiculoModel : PageModel
    {
        private readonly IRepositorio<Vehiculo> _repositorio;
        private readonly IRepositorio<Cliente> _repositorioCliente;
        private readonly IDataProtector _protector;

        public EditarVehiculoModel(
            ConfiguracionBD configuracion,
            IDataProtectionProvider provider)
        {
            RepositorioFactory factory =
                new RepositorioFactory(configuracion);

            _repositorio = factory.CrearRepositorioVehiculo();
            _repositorioCliente = factory.CrearRepositorioCliente();

            _protector = provider.CreateProtector(
                "TallerMecanicoSOLID.Vehiculos");
        }

        [BindProperty]
        public Vehiculo Vehiculo { get; set; } = new Vehiculo();

        [BindProperty]
        public string Token { get; set; } = "";

        public List<Cliente> ClientesDisponibles { get; set; }
            = new List<Cliente>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            int vehiculoId;

            try
            {
                vehiculoId =
                    Convert.ToInt32(_protector.Unprotect(id));

                if (!Validaciones.EsIdValido(vehiculoId))
                {
                    return RedirectToPage("/Vehiculos");
                }
            }
            catch
            {
                TempData["Error"] =
                    "El enlace de edición no es válido.";

                return RedirectToPage("/Vehiculos");
            }

            Vehiculo? vehiculo =
                await _repositorio.ObtenerIdAsync(vehiculoId);

            if (vehiculo == null)
            {
                TempData["Error"] =
                    "El vehículo no existe o está inactivo.";

                return RedirectToPage("/Vehiculos");
            }

            Vehiculo = vehiculo;
            Token = id;

            await CargarClientesAsync();

            // Si el cliente asociado fue desactivado, se permite
            // mostrar el vehículo, pero no se ofrece como cliente activo.
            return Page();
        }

        private async Task CargarClientesAsync()
        {
            ClientesDisponibles =
                await _repositorioCliente.ListaEntidadAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int vehiculoId;

            try
            {
                vehiculoId =
                    Convert.ToInt32(_protector.Unprotect(Token));

                if (!Validaciones.EsIdValido(vehiculoId))
                {
                    return RedirectToPage("/Vehiculos");
                }

                Vehiculo.Id = vehiculoId;
            }
            catch
            {
                TempData["Error"] =
                    "El enlace de edición no es válido.";

                return RedirectToPage("/Vehiculos");
            }

            Vehiculo.Placa =
                Validaciones.FormatearPlaca(Vehiculo.Placa);

            Vehiculo.Marca =
                Validaciones.SanitizarTexto(Vehiculo.Marca, 60);

            Vehiculo.Modelo =
                Validaciones.SanitizarTexto(Vehiculo.Modelo, 60);

            Vehiculo.Color =
                Validaciones.SanitizarTexto(Vehiculo.Color, 40);

            Vehiculo.Tipo =
                Validaciones.SanitizarTexto(Vehiculo.Tipo, 40);

            ModelState.Clear();
            TryValidateModel(Vehiculo, nameof(Vehiculo));

            if (!Validaciones.ValidarPlaca(Vehiculo.Placa))
            {
                ModelState.AddModelError(
                    "Vehiculo.Placa",
                    "La placa debe contener 3 o 4 números seguidos de 3 letras. Ejemplo: 5214BEX.");
            }

            if (!Validaciones.ValidarMarca(Vehiculo.Marca))
            {
                ModelState.AddModelError(
                    "Vehiculo.Marca",
                    "La marca solo puede contener letras, espacios y guiones.");
            }

            if (!Validaciones.ValidarModelo(Vehiculo.Modelo))
            {
                ModelState.AddModelError(
                    "Vehiculo.Modelo",
                    "El modelo solo puede contener letras, números, espacios y guiones.");
            }

            if (!Validaciones.ValidarAnio(Vehiculo.Anio))
            {
                ModelState.AddModelError(
                    "Vehiculo.Anio",
                    "Ingrese un año entre 1901 y el próximo año.");
            }

            if (string.IsNullOrWhiteSpace(Vehiculo.Color) ||
                Vehiculo.Color.Length > 40 ||
                !Validaciones.LetrasNumerosEspacios(Vehiculo.Color))
            {
                ModelState.AddModelError(
                    "Vehiculo.Color",
                    "Ingrese un color válido de hasta 40 caracteres.");
            }

            string[] tiposPermitidos =
            {
                "Automóvil",
                "Camioneta",
                "Motocicleta",
                "Camión",
                "Otro"
            };

            if (!tiposPermitidos.Contains(Vehiculo.Tipo))
            {
                ModelState.AddModelError(
                    "Vehiculo.Tipo",
                    "Seleccione un tipo de vehículo válido.");
            }

            if (!Validaciones.ValidarKilometraje(Vehiculo.Kilometraje))
            {
                ModelState.AddModelError(
                    "Vehiculo.Kilometraje",
                    "El kilometraje debe estar entre 0 y 2.000.000.");
            }

            if (!Validaciones.EsIdValido(Vehiculo.ClienteId))
            {
                ModelState.AddModelError(
                    "Vehiculo.ClienteId",
                    "Debe seleccionar un cliente válido.");
            }
            else
            {
                Cliente? cliente =
                    await _repositorioCliente.ObtenerIdAsync(
                        Vehiculo.ClienteId);

                if (cliente == null)
                {
                    ModelState.AddModelError(
                        "Vehiculo.ClienteId",
                        "El cliente seleccionado no existe o está inactivo.");
                }
            }

            if (!ModelState.IsValid)
            {
                await CargarClientesAsync();
                return Page();
            }

            try
            {
                Vehiculo? existente =
                    await _repositorio.ObtenerIdAsync(vehiculoId);

                if (existente == null)
                {
                    TempData["Error"] =
                        "El vehículo no existe o está inactivo.";

                    return RedirectToPage("/Vehiculos");
                }

                int resultado =
                    await _repositorio.EditarAsync(Vehiculo);

                if (resultado > 0)
                {
                    TempData["Mensaje"] =
                        "El vehículo fue actualizado correctamente.";

                    return RedirectToPage("/Vehiculos");
                }

                ModelState.AddModelError(
                    "",
                    "No se pudo actualizar el vehículo. Verifique que siga activo.");

                await CargarClientesAsync();
                return Page();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(
                    "Vehiculo.Placa",
                    "Ya existe otro vehículo registrado con esa placa.");

                await CargarClientesAsync();
                return Page();
            }
            catch (MySqlException)
            {
                ModelState.AddModelError(
                    "",
                    "No se pudo actualizar el vehículo en la base de datos.");

                await CargarClientesAsync();
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "Ocurrió un error inesperado al actualizar el vehículo.");

                await CargarClientesAsync();
                return Page();
            }
        }
    }
}
