using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaConsultas : Wpf.Ui.Controls.FluentWindow
    {
        public VistaConsultas()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarCatalogosFiltros();
        }

        private void CargarCatalogosFiltros()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    // 1. Cargar Estatus
                    List<ItemFiltro> listaEstatus = new List<ItemFiltro> { new ItemFiltro { Id = -1, Descripcion = "TODOS LOS ESTADOS" } };
                    var cmdE = conn.CreateCommand();
                    cmdE.CommandText = "SELECT id_estatus, tipo_estatus FROM Estatus";
                    using (var reader = cmdE.ExecuteReader())
                        while (reader.Read()) listaEstatus.Add(new ItemFiltro { Id = reader.GetInt32(0), Descripcion = reader.GetString(1) });
                    CmbEstado.ItemsSource = listaEstatus;
                    CmbEstado.SelectedIndex = 0;

                    // 2. Cargar Sedes (SAP)
                    List<ItemFiltro> listaSedes = new List<ItemFiltro> { new ItemFiltro { Id = -1, Descripcion = "TODAS LAS SEDES" } };
                    var cmdS = conn.CreateCommand();
                    cmdS.CommandText = "SELECT Id_SAP, Descripcion_Sap FROM Cat_SAP";
                    using (var reader = cmdS.ExecuteReader())
                        while (reader.Read()) listaSedes.Add(new ItemFiltro { Id = reader.GetInt32(0), Descripcion = reader.GetString(1) });
                    CmbSede.ItemsSource = listaSedes;
                    CmbSede.SelectedIndex = 0;
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error al cargar filtros: {ex.Message}"); }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            List<ResultadoConsultaModel> listaResultados = new List<ResultadoConsultaModel>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    var command = conn.CreateCommand();
                    // SE AGREGÓ EL JOIN CON Descripcion_corta
                    string sql = @"
                        SELECT a.Id_asunto, a.Nombre_oficio, dc.tipo_descripcion, s.Descripcion_Sap, e.tipo_estatus, a.Fecha_Compromiso
                        FROM Asuntos a
                        LEFT JOIN Cat_SAP s ON a.id_sap = s.Id_SAP
                        LEFT JOIN Estatus e ON a.Id_estatus = e.id_estatus
                        LEFT JOIN Descripcion_corta dc ON a.id_descripcionCorta = dc.id_descripcionCorta
                        WHERE 1=1";

                    if (!string.IsNullOrWhiteSpace(TxtBusqueda.Text))
                    {
                        sql += " AND (a.Nombre_oficio LIKE $texto OR dc.tipo_descripcion LIKE $texto OR CAST(a.Id_asunto AS TEXT) LIKE $texto)";
                        command.Parameters.AddWithValue("$texto", "%" + TxtBusqueda.Text.Trim() + "%");
                    }

                    if (CmbEstado.SelectedValue != null && (int)CmbEstado.SelectedValue != -1)
                    {
                        sql += " AND a.id_estatus = $idEstado";
                        command.Parameters.AddWithValue("$idEstado", CmbEstado.SelectedValue);
                    }

                    if (CmbSede.SelectedValue != null && (int)CmbSede.SelectedValue != -1)
                    {
                        sql += " AND a.id_sap = $idSede";
                        command.Parameters.AddWithValue("$idSede", CmbSede.SelectedValue);
                    }

                    command.CommandText = sql + " ORDER BY a.Id_asunto DESC";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaResultados.Add(new ResultadoConsultaModel
                            {
                                Id = reader.GetInt32(0),
                                TituloAsunto = reader.IsDBNull(1) ? "SIN TÍTULO" : reader.GetString(1),
                                CategoriaCorta = reader.IsDBNull(2) ? "General" : reader.GetString(2),
                                SedeNombre = reader.IsDBNull(3) ? "N/A" : reader.GetString(3),
                                EstatusNombre = reader.IsDBNull(4) ? "N/A" : reader.GetString(4),
                                FechaLimite = reader.IsDBNull(5) ? "" : reader.GetString(5)
                            });
                        }
                    }
                }
                lvResultados.ItemsSource = listaResultados;
            }
            catch (Exception ex) { MessageBox.Show($"Error en la búsqueda: {ex.Message}"); }
        }

        // --- NUEVO: EVENTO PARA ABRIR EL EXPEDIENTE AL HACER DOBLE CLIC ---
        private void lvResultados_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lvResultados.SelectedItem is ResultadoConsultaModel seleccionado)
            {
                try
                {
                    // Abrimos la ventana de edición pasando el ID
                    VistaEditarExpediente ventanaEdicion = new VistaEditarExpediente(seleccionado.Id);
                    ventanaEdicion.ShowDialog();

                    // Al cerrar la ventana, refrescamos la búsqueda por si hubo cambios
                    BtnBuscar_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar abrir el expediente. Verifica que VistaEditarExpediente reciba un entero en su constructor.\nDetalle: " + ex.Message);
                }
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            TxtBusqueda.Clear();
            CmbEstado.SelectedIndex = 0;
            CmbSede.SelectedIndex = 0;
            lvResultados.ItemsSource = null;
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e) => this.Close();
    }

    // --- MODELOS DE DATOS ---
    public class ItemFiltro { public int Id { get; set; } public string Descripcion { get; set; } }

    public class ResultadoConsultaModel
    {
        public int Id { get; set; }
        public string TituloAsunto { get; set; }   // El que aparece en grande
        public string CategoriaCorta { get; set; } // El que aparece en pequeño al lado
        public string SedeNombre { get; set; }
        public string EstatusNombre { get; set; }
        public string FechaLimite { get; set; }
    }

    public class SemaforoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var modelo = value as ResultadoConsultaModel;
            if (modelo == null) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));

            if (!string.IsNullOrEmpty(modelo.EstatusNombre) && (modelo.EstatusNombre.ToUpper().Contains("COMPLETADO") || modelo.EstatusNombre.ToUpper().Contains("ATENDIDO")))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008C4A"));

            if (string.IsNullOrWhiteSpace(modelo.FechaLimite)) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));

            DateTime f;
            if (DateTime.TryParse(modelo.FechaLimite, out f))
            {
                TimeSpan t = f.Date - DateTime.Now.Date;
                if (t.TotalDays < 0) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                if (t.TotalDays <= 3) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E67E22"));
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"));
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}