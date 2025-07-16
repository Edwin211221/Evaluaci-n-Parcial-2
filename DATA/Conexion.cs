using MySql.Data.MySqlClient;

namespace ControlPagosUniversidad.DATA
{
    public class Conexion
    {
        private const string connectionString = "Server=localhost;Database=controlpagosuniversidad;Uid=root;Pwd=;";

        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(connectionString);
        }
    }
}