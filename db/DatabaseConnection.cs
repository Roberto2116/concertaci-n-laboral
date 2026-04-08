using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace Proyecto_GRRLN_expediente.db
{
    public class DatabaseConnection
    {
        private static SqliteConnection _connection;

     
        private static readonly string DbPath = @"C:\Users\Thinkpad\Desktop\Practicas\Proyecto_GRRLN_expediente\db\PEMEXDB.db";

        private static readonly string ConnectionString = $"Data Source={DbPath}";

        public static SqliteConnection GetConnection()
        {
            try
            {
                if (_connection == null)
                {
                    _connection = new SqliteConnection(ConnectionString);
                    _connection.Open();
                    Debug.WriteLine(">>> Conexión abierta en: " + DbPath);
                }
                else if (_connection.State != System.Data.ConnectionState.Open)
                {
                    _connection.Open();
                }
            }
            catch (Exception ex)
            {
                // Este mensaje de error te confirmará exactamente la ruta que está intentando abrir
                System.Windows.MessageBox.Show($"Error al abrir la base de datos.\n\n" +
                    $"Buscando en: {DbPath}\n\nDetalle: {ex.Message}");
            }

            return _connection;
        }

        public static void CloseConnection()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
                Debug.WriteLine(">>> Conexión cerrada.");
            }
        }
    }
}