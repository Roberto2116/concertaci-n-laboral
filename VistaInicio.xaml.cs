using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;
// ELIMINADO: using Wpf.Ui.Controls; (Esto causaba el error de MessageBox y Button ambiguos)

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaInicio : UserControl
    {
        private DispatcherTimer timerMensajes;

        public VistaInicio()
        {
            InitializeComponent();
            CargarBandejaAvisos();
            IniciarTemporizador();
            this.Unloaded += VistaInicio_Unloaded;
        }

        private void IniciarTemporizador()
        {
            timerMensajes = new DispatcherTimer();
            timerMensajes.Interval = TimeSpan.FromSeconds(10);
            timerMensajes.Tick += TimerMensajes_Tick;
            timerMensajes.Start();
        }

        private void TimerMensajes_Tick(object sender, EventArgs e)
        {
            CargarBandejaAvisos();
        }

        private void VistaInicio_Unloaded(object sender, RoutedEventArgs e)
        {
            if (timerMensajes != null)
            {
                timerMensajes.Stop();
                timerMensajes.Tick -= TimerMensajes_Tick;
            }
        }

        private void CargarBandejaAvisos()
        {
            string miFicha = SesionGlobal.Ficha;
            string miDepto = SesionGlobal.ClaveDepto;

            List<AvisoModel> listaAvisos = new List<AvisoModel>();
            SqliteConnection conn = DatabaseConnection.GetConnection();

            try
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance 
                        FROM Mensaje 
                        WHERE IFNULL(Archivado, 0) = 0 AND ( 
                             (Tipo_alcance = 'PERSONAL' AND Ficha_destino = $ficha)
                          OR (Tipo_alcance = 'DEPTO' AND clave_depto_destino = $depto)
                          OR (Tipo_alcance = 'SAP')
                        )
                        ORDER BY fecha_posteo DESC;";

                    command.Parameters.AddWithValue("$ficha", miFicha ?? "");
                    command.Parameters.AddWithValue("$depto", miDepto ?? "");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaAvisos.Add(new AvisoModel
                            {
                                IdMensaje = reader.GetInt32(0),        // Columna 0: Id_mensaje
                                Emisor = "De: " + reader.GetString(1), // Columna 1: ficha
                                TextoMensaje = reader.GetString(2),    // Columna 2: mensaje_enviado
                                Fecha = reader.GetString(3),           // Columna 3: fecha_posteo
                                TipoAlcance = reader.GetString(4)      // Columna 4: Tipo_alcance
                            });
                        }
                    }
                }

                icAvisos.ItemsSource = listaAvisos;

                if (listaAvisos.Count == 0)
                {
                    icAvisos.Visibility = Visibility.Collapsed;
                    PanelSinAvisos.Visibility = Visibility.Visible;
                }
                else
                {
                    icAvisos.Visibility = Visibility.Visible;
                    PanelSinAvisos.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error silencioso al cargar avisos: {ex.Message}");
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void BtnArchivar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is AvisoModel mensajeSeleccionado)
            {
                try
                {
                    SqliteConnection conn = DatabaseConnection.GetConnection();
                    if (conn == null) return;

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "UPDATE Mensaje SET Archivado = 1 WHERE Id_mensaje = $id";
                        command.Parameters.AddWithValue("$id", mensajeSeleccionado.IdMensaje);
                        command.ExecuteNonQuery();
                    }

                    CargarBandejaAvisos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al archivar el mensaje:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DatabaseConnection.CloseConnection();
                }
            }
        }

        private void BtnArchivarTodo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                string miFicha = SesionGlobal.Ficha;
                string miDepto = SesionGlobal.ClaveDepto;

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Mensaje 
                        SET Archivado = 1 
                        WHERE IFNULL(Archivado, 0) = 0 AND ( 
                             (Tipo_alcance = 'PERSONAL' AND Ficha_destino = $ficha)
                          OR (Tipo_alcance = 'DEPTO' AND clave_depto_destino = $depto)
                          OR (Tipo_alcance = 'SAP')
                        )";

                    command.Parameters.AddWithValue("$ficha", miFicha ?? "");
                    command.Parameters.AddWithValue("$depto", miDepto ?? "");
                    command.ExecuteNonQuery();
                }

                CargarBandejaAvisos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar la bandeja:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }
    }

    public class AvisoModel
    {
        public int IdMensaje { get; set; }
        public string Emisor { get; set; }
        public string TextoMensaje { get; set; }
        public string Fecha { get; set; }
        public string TipoAlcance { get; set; }
        public int Archivado { get; set; }

        // CORRECCIÓN MAGISTRAL: Mandar el icono como "string" evita tener que importar Wpf.Ui.Controls en este archivo
        public string AvisoSymbol => TipoAlcance == "PERSONAL" ? "Person24" : (TipoAlcance == "DEPTO" ? "Building24" : "Megaphone24");

        public string TextoEtiqueta
        {
            get
            {
                if (TipoAlcance == "PERSONAL") return "AVISO DIRECTO";
                if (TipoAlcance == "DEPTO") return "AVISO DEPARTAMENTAL";
                return "COMUNICADO GENERAL";
            }
        }

        public string HexColor
        {
            get
            {
                if (TipoAlcance == "PERSONAL") return "#9B59B6"; // Morado
                if (TipoAlcance == "DEPTO") return "#3498DB";    // Azul
                return "#EF4444";                                // Rojo para global
            }
        }

        public Brush ColorEtiqueta
        {
            get
            {
                if (TipoAlcance == "PERSONAL") return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B59B6"));
                if (TipoAlcance == "DEPTO") return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
            }
        }
    }
}