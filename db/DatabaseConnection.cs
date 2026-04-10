using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using System.Windows;

namespace Proyecto_GRRLN_expediente.db
{
    public class DatabaseConnection
    {
        public static string RutaConfirmada = "";

        public static SqliteConnection GetConnection()
        {
            try
            {
                // 1. Usamos el radar para encontrar dónde está la base de datos
                string dbPath = EncontrarBaseDeDatos();

                if (dbPath == null)
                {
                    MessageBox.Show("No pude encontrar el archivo PEMEXDB.db por ninguna parte. Revisa que no le hayas cambiado el nombre y que esté en tu proyecto.", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                RutaConfirmada = dbPath;

                // 2. Conectamos
                string connectionString = $"Data Source={dbPath};Mode=ReadWrite;";

                // ¡LA MAGIA AQUÍ! Creamos una conexión NUEVA y FRESCA cada vez que se solicita
                var conn = new SqliteConnection(connectionString);
                conn.Open();

                return conn;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de SQLite al conectar.\n\nArchivo que intentó abrir:\n{RutaConfirmada}\n\nDetalle: {ex.Message}", "Error de BD", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // ==========================================
        // EL RADAR: Busca el archivo hacia arriba
        // ==========================================
        private static string EncontrarBaseDeDatos()
        {
            string directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(directorioActual);

            for (int i = 0; i < 6; i++)
            {
                if (dir == null) break;

                string posibleRuta = Path.Combine(dir.FullName, "db", "PEMEXDB.db");
                if (File.Exists(posibleRuta)) return posibleRuta;

                string posibleRutaSuelto = Path.Combine(dir.FullName, "PEMEXDB.db");
                if (File.Exists(posibleRutaSuelto)) return posibleRutaSuelto;

                dir = dir.Parent;
            }

            return null;
        }

        public static void CloseConnection()
        {
          
        }
    }
}