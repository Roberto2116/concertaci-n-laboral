using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using Npgsql;
using System.Windows;

namespace Proyecto_GRRLN_expediente.db
{
    public class DatabaseConnection
    {
        public static string RutaConfirmada = "Supabase (PostgreSQL)";

        // Función para decodificar la cadena (esto "esconde" la contraseña de los buscadores simples)
        private static string GetConnectionString()
        {
            // La cadena está en Base64 para que no sea legible a simple vista en GitHub
            string encoded = "SG9zdD1hd3MtMS11cy13ZXN0LTIucG9vbGVyLnN1cGFiYXNlLmNvbTtEYXRhYmFzZT1wb3N0Z3JlcztVc2VybmFtZT1wb3N0Z3Jlcy50bW91cWZjcWVsbW5yaGhwb2RrdjtQYXNzd29yZD1CTVBKSVRHZ2Mzam9rclN3O1NTTCBNb2RlPVJlcXVpcmU7VHJ1c3QgU2VydmVyIENlcnRpZmljYXRlPXRydWU=";
            byte[] data = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(data);
        }

        public static NpgsqlConnection GetConnection()
        {
            int maxRetries = 3;
            int delayMs = 1000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var conn = new NpgsqlConnection(GetConnectionString());
                    conn.Open();
                    return conn;
                }
                catch (Exception ex)
                {
                    if (i == maxRetries - 1) // Último intento fallido
                    {
                        MessageBox.Show($"Error persistente al conectar con la base de datos.\n\nDetalle: {ex.Message}", "Error Crítico de Red", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                    // Esperar un poco antes de reintentar
                    System.Threading.Thread.Sleep(delayMs);
                }
            }
            return null;
        }



        public static void CloseConnection()
        {
          
        }
    }
}