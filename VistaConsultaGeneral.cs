using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading; // NUEVO: Librería necesaria para el Temporizador (Debouncing)
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaConsultaGeneral : Wpf.Ui.Controls.FluentWindow
    {
        // NUEVO: Declaramos el temporizador
        private DispatcherTimer _searchTimer;

        public VistaConsultaGeneral()
        {
            InitializeComponent();

            // NUEVO: Configuramos el temporizador a 400 milisegundos
            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };
            _searchTimer.Tick += SearchTimer_Tick;

            CargarDatos();
        }

        // NUEVO: Cada que el usuario presiona una tecla, el temporizador se reinicia.
        // Así evitamos saturar la base de datos si escribe muy rápido.
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        // NUEVO: Cuando pasan los 400ms sin que el usuario escriba, se dispara la búsqueda.
        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            CargarDatos(TxtBuscar.Text);
        }

        private void CargarDatos(string filtro = "")
        {
            List<AsuntoGridModel> lista = new List<AsuntoGridModel>();
            SqliteConnection conn = DatabaseConnection.GetConnection();

            if (conn == null) return;

            try
            {
                using (var command = conn.CreateCommand())
                {
                    string query = @"
                        SELECT 
                            a.id_asunto, 
                            a.Nombre_oficio, 
                            a.Fecha_Compromiso, 
                            a.Porcentaje_avance,
                            c.tipo_descripcion,
                            g.Gerencia,
                            cs.Descripcion_Sap,
                            dp.descripcion AS Departamento
                        FROM Asuntos a
                        LEFT JOIN Descripcion_corta c ON a.id_descripcionCorta = c.id_descripcionCorta
                        LEFT JOIN Dep_personal dp ON a.clave_depto = dp.clave_depto
                        LEFT JOIN Subgerencia s ON dp.clave_subgerencia = s.Clave_subgerencia
                        LEFT JOIN AS_CatGerencia g ON s.clave_gerencia = g.Clave_gerencia
                        LEFT JOIN Cat_SAP cs ON a.id_sap = cs.Id_SAP
                        WHERE 1=1 ";

                    // MEJORA: Búsqueda Multi-Campo. Ahora busca en 5 columnas diferentes.
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        query += @" AND (
                                    a.Nombre_oficio LIKE $filtro OR 
                                    CAST(a.id_asunto AS TEXT) LIKE $filtro OR
                                    c.tipo_descripcion LIKE $filtro OR
                                    cs.Descripcion_Sap LIKE $filtro OR
                                    dp.descripcion LIKE $filtro
                                   )";
                        command.Parameters.AddWithValue("$filtro", "%" + filtro.Trim() + "%");
                    }

                    query += " ORDER BY a.id_asunto DESC";
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new AsuntoGridModel
                            {
                                Id = reader.GetInt32(0),
                                Asunto = reader.IsDBNull(1) ? "SIN NOMBRE" : reader.GetString(1),
                                FechaLimite = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Avance = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                DescripcionCortaTexto = reader.IsDBNull(4) ? "Sin descripción" : reader.GetString(4),
                                GerenciaTexto = reader.IsDBNull(5) ? "Sin gerencia" : reader.GetString(5),
                                Sap = reader.IsDBNull(6) ? "Sin SAP" : reader.GetString(6),
                                Departamento = reader.IsDBNull(7) ? "Sin Depto" : reader.GetString(7)
                            });
                        }
                    }
                }

                GridAsuntos.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer la base de datos: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }


        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnModificarAvance_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is AsuntoGridModel expSeleccionado)
            {
                VentanaActualizarAvance ventanaEmergente = new VentanaActualizarAvance(expSeleccionado.Id, expSeleccionado.Avance);

                if (ventanaEmergente.ShowDialog() == true)
                {
                    CargarDatos(TxtBuscar.Text);
                }
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is AsuntoGridModel expSeleccionado)
            {
                try
                {
                    VistaEditarExpediente ventanaEdicion = new VistaEditarExpediente(expSeleccionado.Id);
                    ventanaEdicion.ShowDialog();
                    CargarDatos(TxtBuscar.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al abrir la ventana de edición: {ex.Message}");
                }
            }
        }
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos(TxtBuscar.Text);
        }
    }

    public class AsuntoGridModel
    {
        public int Id { get; set; }
        public string Asunto { get; set; }
        public string FechaLimite { get; set; }
        public int Avance { get; set; }

        public string DescripcionCortaTexto { get; set; }
        public string GerenciaTexto { get; set; }

        public string Sap { get; set; }
        public string Departamento { get; set; }

        public string AvanceTexto => $"{Avance}%";

        public Brush ColorSemaforo
        {
            get
            {
                if (Avance >= 100) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00843D"));

                if (string.IsNullOrEmpty(FechaLimite) || FechaLimite == "NO DEFINIDA")
                    return new SolidColorBrush(Colors.Gray);

                if (DateTime.TryParse(FechaLimite, out DateTime fecha))
                {
                    TimeSpan diferencia = fecha.Date - DateTime.Now.Date;
                    int dias = (int)diferencia.TotalDays;

                    if (dias < 0) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8102E"));
                    if (dias <= 3) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F"));

                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
                }

                return new SolidColorBrush(Colors.Gray);
            }
        }

        public string TextoDias
        {
            get
            {
                if (Avance >= 100) return "Completado";

                if (string.IsNullOrEmpty(FechaLimite) || FechaLimite == "NO DEFINIDA")
                    return "Sin Fecha";

                if (DateTime.TryParse(FechaLimite, out DateTime fecha))
                {
                    TimeSpan diferencia = fecha.Date - DateTime.Now.Date;
                    int dias = (int)diferencia.TotalDays;

                    if (dias < 0) return $"Vencido ({-dias} d)";
                    if (dias == 0) return "Vence Hoy";
                    if (dias == 1) return "Vence Mañana";
                    return $"Faltan {dias} días";
                }

                return "Error";
            }
        }
    }
}