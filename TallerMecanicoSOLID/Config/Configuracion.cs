namespace TallerMecanicoSOLID.Configuracion
{
    public class ConfiguracionBD
    {
        private static ConfiguracionBD? instancia;

        public string ConnectionString { get; private set; }

        private ConfiguracionBD(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static ConfiguracionBD ObtenerInstancia(string connectionString)
        {
            if (instancia == null)
            {
                instancia = new ConfiguracionBD(connectionString);
            }

            return instancia;
        }
    }
}