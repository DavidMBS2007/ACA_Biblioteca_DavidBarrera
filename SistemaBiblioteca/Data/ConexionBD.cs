using System.Configuration;
using Microsoft.Data.SqlClient;

namespace SistemaBiblioteca.Data
{
    /// <summary>
    /// Gestiona la cadena de conexión y la creación de conexiones a SQL Server.
    /// </summary>
    public static class ConexionBD
    {
        public static string CadenaConexion =>
            ConfigurationManager.ConnectionStrings["BibliotecaDB"]?.ConnectionString
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'BibliotecaDB' en App.config.");

        public static SqlConnection CrearConexion() => new SqlConnection(CadenaConexion);

        /// <summary>Verifica que la base de datos sea accesible.</summary>
        public static bool ProbarConexion(out string mensaje)
        {
            try
            {
                using var cn = CrearConexion();
                cn.Open();
                mensaje = "Conexión exitosa.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }
    }
}
