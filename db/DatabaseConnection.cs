using System;
using System.IO;
using System.Diagnostics;
using Npgsql;
using System.Windows;

namespace Proyecto_GRRLN_expediente.db
{
    public class DatabaseConnection
    {
        public static string RutaConfirmada = "Supabase (PostgreSQL)";

        public static NpgsqlConnection GetConnection()
        {
            int maxRetries = 3;
            int delayMs = 1000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    string connectionString = "Host=aws-1-us-west-2.pooler.supabase.com;Database=postgres;Username=postgres.tmouqfcqelmnrhhpodkv;Password=BMPJITGgc3jokrSw;SSL Mode=Require;Trust Server Certificate=true";

                    var conn = new NpgsqlConnection(connectionString);
                    conn.Open();
                    return conn;
                }
                catch (Exception ex)
                {
                    if (i == maxRetries - 1) // Último intento fallido
                    {
                        MessageBox.Show($"Error persistente al conectar con Supabase (Intento {i + 1}/{maxRetries}).\n\nDetalle: {ex.Message}", "Error Crítico de Red", MessageBoxButton.OK, MessageBoxImage.Error);
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