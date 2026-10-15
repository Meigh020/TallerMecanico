
using MySql.Data.MySqlClient;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Repositories
{
    public class RepositorioCliente : IRepositorio<Cliente>
    {
        private readonly ConfiguracionBD _configuracion;

        public RepositorioCliente(ConfiguracionBD configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<List<Cliente>> ListaEntidadAsync()
        {
            List<Cliente> clientes = new List<Cliente>();

            string sql = @"SELECT id, nombre, apellido, ci_nit, telefono,
                                  email, direccion, estado, fecha_registro
                           FROM cliente
                           WHERE eliminado = 0
                           ORDER BY id DESC";

            using (MySqlConnection conexion =
                   new MySqlConnection(_configuracion.ConnectionString))
            {
                await conexion.OpenAsync();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                using (MySqlDataReader lector =
                       (MySqlDataReader)await comando.ExecuteReaderAsync())
                {
                    while (await lector.ReadAsync())
                    {
                        clientes.Add(MapearCliente(lector));
                    }
                }
            }

            return clientes;
        }

        public async Task<Cliente?> ObtenerIdAsync(int id)
        {
            Cliente? cliente = null;

            string sql = @"SELECT id, nombre, apellido, ci_nit, telefono,
                                  email, direccion, estado, fecha_registro
                           FROM cliente
                           WHERE id = @id AND eliminado = 0";

            using (MySqlConnection conexion =
                   new MySqlConnection(_configuracion.ConnectionString))
            {
                await conexion.OpenAsync();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader lector =
                           (MySqlDataReader)await comando.ExecuteReaderAsync())
                    {
                        if (await lector.ReadAsync())
                        {
                            cliente = MapearCliente(lector);
                        }
                    }
                }
            }

            return cliente;
        }

        public async Task<List<Cliente>> BuscarAsync(string texto)
        {
            List<Cliente> clientes = new List<Cliente>();

            string sql = @"SELECT id, nombre, apellido, ci_nit, telefono,
                                  email, direccion, estado, fecha_registro
                           FROM cliente
                           WHERE eliminado = 0
                           AND (nombre LIKE @texto
                                OR apellido LIKE @texto
                                OR ci_nit LIKE @texto
                                OR email LIKE @texto)
                           ORDER BY id DESC";

            using (MySqlConnection conexion =
                   new MySqlConnection(_configuracion.ConnectionString))
            {
                await conexion.OpenAsync();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@texto", "%" + texto + "%");

                    using (MySqlDataReader lector =
                           (MySqlDataReader)await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            clientes.Add(MapearCliente(lector));
                        }
                    }
                }
            }

            return clientes;
        }

        public async Task<int> CrearAsync(Cliente entidad)
        {
            string sql = @"INSERT INTO cliente
                               (nombre, apellido, ci_nit, telefono, email,
                                direccion, estado, eliminado)
                           VALUES
                               (@nombre, @apellido, @ci_nit, @telefono, @email,
                                @direccion, @estado, 0)";

            using (MySqlConnection conexion =
                   new MySqlConnection(_configuracion.ConnectionString))
            {
                await conexion.OpenAsync();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@apellido", entidad.Apellido);
                    comando.Parameters.AddWithValue("@ci_nit", entidad.CiNit);
                    comando.Parameters.AddWithValue("@telefono", entidad.Telefono);
                    comando.Parameters.AddWithValue("@email", entidad.Email);
                    comando.Parameters.AddWithValue("@direccion", entidad.Direccion);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado);

                    return await comando.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> EditarAsync(Cliente entidad)
        {
            string sql = @"UPDATE cliente
                           SET nombre = @nombre,
                               apellido = @apellido,
                               ci_nit = @ci_nit,
                               telefono = @telefono,
                               email = @email,
                               direccion = @direccion,
                               estado = @estado
                           WHERE id = @id AND eliminado = 0";

            using (MySqlConnection conexion =
                   new MySqlConnection(_configuracion.ConnectionString))
            {
                await conexion.OpenAsync();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", entidad.Id);
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@apellido", entidad.Apellido);
                    comando.Parameters.AddWithValue("@ci_nit", entidad.CiNit);
                    comando.Parameters.AddWithValue("@telefono", entidad.Telefono);
                    comando.Parameters.AddWithValue("@email", entidad.Email);
                    comando.Parameters.AddWithValue("@direccion", entidad.Direccion);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado);

                    return await comando.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> EliminarAsync(int id)
        {
            string sql = @"UPDATE cliente
                           SET eliminado = 1
                           WHERE id = @id AND eliminado = 0";

            using (MySqlConnection conexion =
                   new MySqlConnection(_configuracion.ConnectionString))
            {
                await conexion.OpenAsync();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    return await comando.ExecuteNonQueryAsync();
                }
            }
        }

        private static Cliente MapearCliente(MySqlDataReader lector)
        {
            return new Cliente
            {
                Id = Convert.ToInt32(lector["id"]),
                Nombre = lector["nombre"].ToString()!,
                Apellido = lector["apellido"].ToString()!,
                CiNit = lector["ci_nit"].ToString()!,
                Telefono = lector["telefono"].ToString()!,
                Email = lector["email"].ToString()!,
                Direccion = lector["direccion"].ToString()!,
                Estado = Convert.ToBoolean(lector["estado"]),
                FechaRegistro = Convert.ToDateTime(lector["fecha_registro"])
            };
        }
    }
}
