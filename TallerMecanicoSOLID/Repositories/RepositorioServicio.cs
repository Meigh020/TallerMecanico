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

        public List<Servicio> ListaEntidad()
        {
            List<Servicio> servicios = new List<Servicio>();

            string sql = @"SELECT id, nombre, tipo, descripcion, precio,
                                  duracion_estimada, unidad_duracion, estado
                           FROM servicio
                           WHERE eliminado = 0
                           ORDER BY id DESC";

            using (MySqlConnection conexion = new MySqlConnection(_configuracion.ConnectionString))
            {
                conexion.Open();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        servicios.Add(MapearServicio(lector));
                    }
                }
            }

            return servicios;
        }

        public Servicio? ObtenerId(int id)
        {
            Servicio? servicio = null;

            string sql = @"SELECT id, nombre, tipo, descripcion, precio,
                                  duracion_estimada, unidad_duracion, estado
                           FROM servicio
                           WHERE id = @id AND eliminado = 0";

            using (MySqlConnection conexion = new MySqlConnection(_configuracion.ConnectionString))
            {
                conexion.Open();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            servicio = MapearServicio(lector);
                        }
                    }
                }
            }

            return servicio;
        }

        public List<Servicio> Buscar(string texto)
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

            using (MySqlConnection conexion = new MySqlConnection(_configuracion.ConnectionString))
            {
                conexion.Open();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@texto", "%" + texto + "%");

                    using (MySqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            servicios.Add(MapearServicio(lector));
                        }
                    }
                }
            }

            return servicios;
        }

        public int Crear(Servicio entidad)
        {
            string sql = @"INSERT INTO servicio
                               (nombre, tipo, descripcion, precio,
                                duracion_estimada, unidad_duracion, estado, eliminado)
                           VALUES
                               (@nombre, @tipo, @descripcion, @precio,
                                @duracion_estimada, @unidad_duracion, @estado, 0)";

            using (MySqlConnection conexion = new MySqlConnection(_configuracion.ConnectionString))
            {
                conexion.Open();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, entidad);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado);

                    return comando.ExecuteNonQuery();
                }
            }
        }

        public int Editar(Servicio entidad)
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

            using (MySqlConnection conexion = new MySqlConnection(_configuracion.ConnectionString))
            {
                conexion.Open();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, entidad);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado);
                    comando.Parameters.AddWithValue("@id", entidad.Id);

                    return comando.ExecuteNonQuery();
                }
            }
        }

        public int Eliminar(int id)
        {
            string sql = @"UPDATE servicio
                           SET eliminado = 1
                           WHERE id = @id AND eliminado = 0";

            using (MySqlConnection conexion = new MySqlConnection(_configuracion.ConnectionString))
            {
                conexion.Open();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    return comando.ExecuteNonQuery();
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

        private static void AgregarParametros(MySqlCommand comando, Servicio entidad)
        {
            comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
            comando.Parameters.AddWithValue("@tipo", entidad.Tipo);
            comando.Parameters.AddWithValue("@descripcion", entidad.Descripcion);
            comando.Parameters.AddWithValue("@precio", entidad.Precio);
            comando.Parameters.AddWithValue("@duracion_estimada", entidad.DuracionEstimada);
            comando.Parameters.AddWithValue("@unidad_duracion", entidad.UnidadDuracion);
        }
    }
}