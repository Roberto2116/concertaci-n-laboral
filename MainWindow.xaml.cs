using System;
using System.Windows;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            // Al iniciar, cargamos la VistaInicio (el tablero de avisos)
            ContenedorPrincipal.Content = new VistaInicio();

            // AGREGADO: Suscribimos la ventana al evento de cierre para capturar la "X"
            this.Closing += MainWindow_Closing;
        }

        // ==========================================================
        // LÓGICA DE BITÁCORA: REGISTRAR SALIDA
        // ==========================================================
        private void RegistrarSalidaBitacora()
        {
            // Solo actuamos si tenemos un ID de sesión válido
            if (SesionGlobal.IdSesionActual <= 0) return;

            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                using (var command = conn.CreateCommand())
                {
                    // Actualizamos la columna fecha_salida del registro que abrimos en el Login
                    command.CommandText = "UPDATE Bitacora_Sesiones SET Fecha_Salida = $fecha WHERE Id_Sesion = $id";
                    command.Parameters.AddWithValue("$fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("$id", SesionGlobal.IdSesionActual);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Log silencioso para no interrumpir el cierre del programa
                Console.WriteLine("Error al registrar salida: " + ex.Message);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Se ejecuta al cerrar con la "X"
            RegistrarSalidaBitacora();
        }

        private void BtnNuevoAvisoGlobal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VistaCrearAviso ventanaAviso = new VistaCrearAviso();
                ventanaAviso.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el formulario de avisos: {ex.Message}");
            }
        }

        private void BtnExpedientes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VistaConsultaGeneral ventanaConsulta = new VistaConsultaGeneral();
                ventanaConsulta.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el seguimiento: {ex.Message}", "Error de Navegación");
            }
        }

        private void BtnNuevoCaso_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VistaNuevoExpediente ventanaNuevo = new VistaNuevoExpediente();
                ventanaNuevo.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el registro: {ex.Message}");
            }
        }

        private void Btnconsultas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VistaConsultas ventanaConsultas = new VistaConsultas();
                ventanaConsultas.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el módulo de consultas: {ex.Message}");
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            // AGREGADO: Registrar salida antes del Shutdown
            RegistrarSalidaBitacora();
            DatabaseConnection.CloseConnection();
            Application.Current.Shutdown();
        }

        private void BtnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            VistaEstadisticas ventanaEstadisticas = new VistaEstadisticas();
            ventanaEstadisticas.ShowDialog();
        }

        private void BtnAdministracion_Click(object sender, RoutedEventArgs e)
        {
            VentanaAdministrador adminWindow = new VentanaAdministrador();
            adminWindow.ShowDialog();
        }
    }
}