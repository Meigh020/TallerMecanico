
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;
using TallerMecanicoSOLID.Repositories;

namespace TallerMecanicoSOLID.Pages
{
    public class NuevoVehiculoModel : PageModel
    {
        private readonly IRepositorio<Vehiculo> _repositorio;
        private readonly IRepositorio<Cliente> _repositorioCliente;

        public NuevoVehiculoModel(ConfiguracionBD configuracion)
        {
            RepositorioFactory factory =
                new RepositorioFactory(configuracion);

            _repositorio = factory.CrearRepositorioVehiculo();
            _repositorioCliente = factory.CrearRepositorioCliente();
        }

        [BindProperty]
        public Vehiculo Vehiculo { get; set; } = new Vehiculo
        {
            Estado = true
        };

        public List<Cliente> ClientesDisponibles { get; set; }
            = new List<Cliente>();

        public async Task OnGetAsync()
        {
            await CargarClientesAsync();
        }

        private async Task CargarClientesAsync()
        {
            List<Cliente> clientes =
                await _repositorioCliente.ListaEntidadAsync();

            ClientesDisponibles = clientes
                .Where(cliente => cliente.Estado)
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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

                if (cliente == null || !cliente.Estado)
                {
                    ModelState.AddModelError(
                        "Vehiculo.ClienteId",
                        "El cliente seleccionado no existe, está inactivo o fue eliminado.");
                }
            }

            if (!ModelState.IsValid)
            {
                await CargarClientesAsync();
                return Page();
            }

            try
            {
                int resultado =
                    await _repositorio.CrearAsync(Vehiculo);

                if (resultado > 0)
                {
                    TempData["Mensaje"] =
                        "El vehículo fue registrado correctamente.";

                    return RedirectToPage("/Vehiculos");
                }

                ModelState.AddModelError(
                    "",
                    "No se pudo registrar el vehículo.");

                await CargarClientesAsync();
                return Page();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(
                    "Vehiculo.Placa",
                    "Ya existe un vehículo registrado con esa placa.");

                await CargarClientesAsync();
                return Page();
            }
            catch (MySqlException)
            {
                ModelState.AddModelError(
                    "",
                    "No se pudo guardar el vehículo en la base de datos. Verifique los datos e inténtelo nuevamente.");

                await CargarClientesAsync();
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "Ocurrió un error inesperado al registrar el vehículo.");

                await CargarClientesAsync();
                return Page();
            }
        }
    }
}
