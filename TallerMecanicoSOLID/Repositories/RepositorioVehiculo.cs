using MySql.Data.MySqlClient;
using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;

namespace TallerMecanicoSOLID.Repositories
{
    public class RepositorioVehiculo : IRepositorio<Vehiculo>
    {
        private readonly ConfiguracionBD _configuracion;

        public RepositorioVehiculo(ConfiguracionBD configuracion)
        {
            _configuracion = configuracion;
        }

        public List<Vehiculo> ListaEntidad()
        {
            List<Vehiculo> vehiculos = new List<Vehiculo>();

            string sql = @"
                SELECT v.id, v.cliente_id, v.placa, v.marca, v.modelo,
                       v.anio, v.color, v.tipo, v.kilometraje, v.estado,
                       v.creado_en,
                       CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre
                FROM vehiculo v
                INNER JOIN cliente c ON v.cliente_id = c.id
                WHERE v.eliminado = 0
                  AND c.eliminado = 0
                ORDER BY v.id DESC";

            using (MySqlConnection conexion = new MySqlConnection(_configuracion.ConnectionString))
            {
                conexion.Open();

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        vehiculos.Add(MapearVehiculo(lector));
                    }
                }
            }

            return vehiculos;
        }

        public Vehiculo? ObtenerId(int id)
        {
            Vehiculo? vehiculo = null;

            string sql = @"
                SELECT v.id, v.cliente_id, v.placa, v.marca, v.modelo,
                       v.anio, v.color, v.tipo, v.kilometraje, v.estado,
                       v.creado_en,
                       CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre
                FROM vehiculo v
                INNER JOIN cliente c ON v.cliente_id = c.id
                WHERE v.id = @id
                  AND v.eliminado = 0
                  AND c.eliminado = 0";

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
                            vehiculo = MapearVehiculo(lector);
                        }
                    }
                }
            }

            return vehiculo;
        }

        public List<Vehiculo> Buscar(string texto)
        {
            List<Vehiculo> vehiculos = new List<Vehiculo>();

            string sql = @"
                SELECT v.id, v.cliente_id, v.placa, v.marca, v.modelo,
                       v.anio, v.color, v.tipo, v.kilometraje, v.estado,
                       v.creado_en,
                       CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre
                FROM vehiculo v
                INNER JOIN cliente c ON v.cliente_id = c.id
                WHERE v.eliminado = 0
                  AND c.eliminado = 0
                  AND (
                      v.placa LIKE @texto
                      OR v.marca LIKE @texto
                      OR v.modelo LIKE @texto
                      OR v.tipo LIKE @texto
                      OR c.nombre LIKE @texto
                      OR c.apellido LIKE @texto
                      OR c.ci_nit LIKE @texto
                  )
                ORDER BY v.id DESC";

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
                            vehiculos.Add(MapearVehiculo(lector));
                        }
                    }
                }
            }

            return vehiculos;
        }

        public int Crear(Vehiculo entidad)
        {
            string sql = @"
                INSERT INTO vehiculo
                    (cliente_id, placa, marca, modelo, anio, color,
                     tipo, kilometraje, estado, eliminado)
                VALUES
                    (@cliente_id, @placa, @marca, @modelo, @anio, @color,
                     @tipo, @kilometraje, @estado, 0)";

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

        public int Editar(Vehiculo entidad)
        {
            string sql = @"
                UPDATE vehiculo
                SET cliente_id = @cliente_id,
                    placa = @placa,
                    marca = @marca,
                    modelo = @modelo,
                    anio = @anio,
                    color = @color,
                    tipo = @tipo,
                    kilometraje = @kilometraje,
                    estado = @estado
                WHERE id = @id
                  AND eliminado = 0";

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
            string sql = @"
                UPDATE vehiculo
                SET eliminado = 1
                WHERE id = @id
                  AND eliminado = 0";

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

        private static Vehiculo MapearVehiculo(MySqlDataReader lector)
        {
            return new Vehiculo
            {
                Id = Convert.ToInt32(lector["id"]),
                ClienteId = Convert.ToInt32(lector["cliente_id"]),
                Placa = lector["placa"].ToString()!,
                Marca = lector["marca"].ToString()!,
                Modelo = lector["modelo"].ToString()!,
                Anio = Convert.ToInt32(lector["anio"]),
                Color = lector["color"].ToString()!,
                Tipo = lector["tipo"].ToString()!,
                Kilometraje = Convert.ToInt32(lector["kilometraje"]),
                Estado = Convert.ToBoolean(lector["estado"]),
                CreadoEn = Convert.ToDateTime(lector["creado_en"]),
                ClienteNombre = lector["cliente_nombre"].ToString()!
            };
        }

        private static void AgregarParametros(MySqlCommand comando, Vehiculo entidad)
        {
            comando.Parameters.AddWithValue("@cliente_id", entidad.ClienteId);
            comando.Parameters.AddWithValue("@placa", entidad.Placa);
            comando.Parameters.AddWithValue("@marca", entidad.Marca);
            comando.Parameters.AddWithValue("@modelo", entidad.Modelo);
            comando.Parameters.AddWithValue("@anio", entidad.Anio);
            comando.Parameters.AddWithValue("@color", entidad.Color);
            comando.Parameters.AddWithValue("@tipo", entidad.Tipo);
            comando.Parameters.AddWithValue("@kilometraje", entidad.Kilometraje);
        }
    }
}