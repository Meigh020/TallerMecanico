
using MySql.Data.MySqlClient;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Repositories
{
    public class RepositorioServicio : IRepositorio<Servicio>
    {
        private readonly ConfiguracionBD _configuracion;

        public RepositorioServicio(ConfiguracionBD configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<List<Servicio>> ListaEntidadAsync()
        {
            List<Servicio> servicios = new List<Servicio>();

            string sql = @"SELECT id, nombre, tipo, descripcion, precio,
                                  duracion_estimada, unidad_duracion, estado
                           FROM servicio
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
                        servicios.Add(MapearServicio(lector));
                    }
                }
            }

            return servicios;
        }

        public async Task<Servicio?> ObtenerIdAsync(int id)
        {
            Servicio? servicio = null;

            string sql = @"SELECT id, nombre, tipo, descripcion, precio,
                                  duracion_estimada, unidad_duracion, estado
                           FROM servicio
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
                            servicio = MapearServicio(lector);
                        }
                    }
                }
            }

            return servicio;
        }

        public async Task<List<Servicio>> BuscarAsync(string texto)
        {
            List<Servicio> servicios = new List<Servicio>();

            string sql = @"SELECT id, nombre, tipo, descripcion, precio,
                                  duracion_estimada, unidad_duracion, estado
                           FROM servicio
                           WHERE eliminado = 0
                           AND (nombre LIKE @texto
                                OR tipo LIKE @texto
                                OR descripcion LIKE @texto)
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
                            servicios.Add(MapearServicio(lector));
                        }
                    }
                }
            }

            return servicios;
        }

        public async Task<int> CrearAsync(Servicio entidad)
        {
            string sql = @"INSERT INTO servicio
                               (nombre, tipo, descripcion, precio,
                                duracion_estimada, unidad_duracion, estado, eliminado)
                           VALUES
                               (@nombre, @tipo, @descripcion, @precio,
                                @duracion_estimada, @unidad_duracion, @estado, 0)";

            using (MySqlConnection conexion =
                   new MySqlConnection(_configuracion.ConnectionString))
            {
                await conexion.OpenAsync();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, entidad);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado);

                    return await comando.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> EditarAsync(Servicio entidad)
        {
            string sql = @"UPDATE servicio
                           SET nombre = @nombre,
                               tipo = @tipo,
                               descripcion = @descripcion,
                               precio = @precio,
                               duracion_estimada = @duracion_estimada,
                               unidad_duracion = @unidad_duracion,
                               estado = @estado
                           WHERE id = @id AND eliminado = 0";

            using (MySqlConnection conexion =
                   new MySqlConnection(_configuracion.ConnectionString))
            {
                await conexion.OpenAsync();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, entidad);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado);
                    comando.Parameters.AddWithValue("@id", entidad.Id);

                    return await comando.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> EliminarAsync(int id)
        {
            string sql = @"UPDATE servicio
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

        private static Servicio MapearServicio(MySqlDataReader lector)
        {
            return new Servicio
            {
                Id = Convert.ToInt32(lector["id"]),
                Nombre = lector["nombre"].ToString()!,
                Tipo = lector["tipo"].ToString()!,
                Descripcion = lector["descripcion"].ToString()!,
                Precio = Convert.ToDecimal(lector["precio"]),
                DuracionEstimada = Convert.ToInt32(lector["duracion_estimada"]),
                UnidadDuracion = lector["unidad_duracion"].ToString()!,
                Estado = Convert.ToBoolean(lector["estado"])
            };
        }

        private static void AgregarParametros(
            MySqlCommand comando,
            Servicio entidad)
        {
            comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
            comando.Parameters.AddWithValue("@tipo", entidad.Tipo);
            comando.Parameters.AddWithValue("@descripcion", entidad.Descripcion);
            comando.Parameters.AddWithValue("@precio", entidad.Precio);
            comando.Parameters.AddWithValue(
                "@duracion_estimada", entidad.DuracionEstimada);
            comando.Parameters.AddWithValue(
                "@unidad_duracion", entidad.UnidadDuracion);
        }
    }
}
