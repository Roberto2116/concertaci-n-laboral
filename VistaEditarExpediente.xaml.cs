using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaEditarExpediente : Wpf.Ui.Controls.FluentWindow
    {
        private int _idAsunto;
        private int _avanceActual; // Para mandarlo a la ventana de actualización

        public VistaEditarExpediente(int idAsunto)
        {
            InitializeComponent();
            _idAsunto = idAsunto;
            TxtTituloVista.Text = $"EDITAR ASUNTO N° {_idAsunto}";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarCatalogos();
            CargarDatosExpediente();
        }

        // ==========================================
        // 1. CARGA DE DATOS Y CATÁLOGOS
        // ==========================================
        private void CargarCatalogos()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    List<ItemCatalogo> listaSAP = new List<ItemCatalogo>();
                    List<ItemCatalogo> listaOrg = new List<ItemCatalogo>();
                    List<ItemCatalogo> listaDeptos = new List<ItemCatalogo>();
                    List<ItemCatalogo> listaSec = new List<ItemCatalogo>();
                    List<ItemCentroTrabajo> listaCentros = new List<ItemCentroTrabajo>();
                    List<ItemCatalogo> listaDescCorta = new List<ItemCatalogo>();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Id_SAP, Descripcion_Sap FROM Cat_SAP";
                        using (var reader = cmd.ExecuteReader()) while (reader.Read()) listaSAP.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });

                        cmd.CommandText = "SELECT id_organismo, Organismo FROM Organismos";
                        using (var reader = cmd.ExecuteReader()) while (reader.Read()) listaOrg.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });

                        cmd.CommandText = "SELECT clave_depto, descripcion FROM Dep_personal";
                        using (var reader = cmd.ExecuteReader()) while (reader.Read()) listaDeptos.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });

                        cmd.CommandText = "SELECT SeccionSindical, Descripcion FROM AS_CatSecSind";
                        using (var reader = cmd.ExecuteReader()) while (reader.Read()) listaSec.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });

                        cmd.CommandText = "SELECT clave_centro, Desc_centroTrabajo, Id_SAP, id_organismo FROM AS_CatCentros";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaCentros.Add(new ItemCentroTrabajo
                                {
                                    Id = reader.GetInt64(0),
                                    Descripcion = reader.GetString(1),
                                    IdSap = reader.IsDBNull(2) ? 0 : reader.GetInt64(2),
                                    IdOrganismo = reader.IsDBNull(3) ? 0 : reader.GetInt64(3)
                                });
                            }
                        }

                        cmd.CommandText = "SELECT id_descripcionCorta, tipo_descripcion FROM Descripcion_corta";
                        using (var reader = cmd.ExecuteReader()) while (reader.Read()) listaDescCorta.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                    }

                    CmbSAP.ItemsSource = listaSAP;
                    CmbOrganismo.ItemsSource = listaOrg;
                    CmbDepartamento.ItemsSource = listaDeptos;
                    CmbSecSindical.ItemsSource = listaSec;
                    CmbCentroTrabajo.ItemsSource = listaCentros;
                    CmbDescCorta.ItemsSource = listaDescCorta;
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error catálogos: {ex.Message}"); }
            finally { DatabaseConnection.CloseConnection(); }
        }

        private void CargarDatosExpediente()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, Agenda, 
                                           Fecha_Compromiso, Instruccion, Observaciones, id_sap, clave_depto, 
                                           Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Porcentaje_avance 
                                           FROM Asuntos WHERE Id_asunto = $id";
                        cmd.Parameters.AddWithValue("$id", _idAsunto);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DpFechaRecepcion.SelectedDate = reader.IsDBNull(0) ? (DateTime?)null : DateTime.Parse(reader.GetString(0));
                                DpFechaOficio.SelectedDate = reader.IsDBNull(3) ? (DateTime?)null : DateTime.Parse(reader.GetString(3));
                                DpFechaCompromiso.SelectedDate = reader.IsDBNull(5) ? (DateTime?)null : DateTime.Parse(reader.GetString(5));
                                TxtNombreOficio.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                TxtInstruccion.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                TxtObservaciones.Text = reader.IsDBNull(7) ? "" : reader.GetString(7);

                                string tipo = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                foreach (ComboBoxItem item in CmbTipo.Items) if (item.Content.ToString() == tipo) CmbTipo.SelectedItem = item;

                                string agenda = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                foreach (ComboBoxItem item in CmbAgenda.Items) if (item.Content.ToString() == agenda) CmbAgenda.SelectedItem = item;

                                CmbSAP.SelectedValue = reader.IsDBNull(8) ? (long?)null : reader.GetInt64(8);
                                CmbDepartamento.SelectedValue = reader.IsDBNull(9) ? (long?)null : reader.GetInt64(9);
                                CmbSecSindical.SelectedValue = reader.IsDBNull(10) ? (long?)null : reader.GetInt64(10);
                                CmbOrganismo.SelectedValue = reader.IsDBNull(11) ? (long?)null : reader.GetInt64(11);
                                CmbCentroTrabajo.SelectedValue = reader.IsDBNull(12) ? (long?)null : reader.GetInt64(12);
                                CmbDescCorta.SelectedValue = reader.IsDBNull(13) ? (long?)null : reader.GetInt64(13);

                                _avanceActual = reader.IsDBNull(14) ? 0 : Convert.ToInt32(reader.GetValue(14));
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error carga: {ex.Message}"); }
            finally { DatabaseConnection.CloseConnection(); }
        }

        // ==========================================
        // 2. LÓGICA DE LOS BOTONES DE CABECERA
        // ==========================================

        private void BtnAvanceEdicion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pasamos los 2 parámetros que pide tu ventana
                VentanaActualizarAvance win = new VentanaActualizarAvance(_idAsunto, _avanceActual);
                if (win.ShowDialog() == true) CargarDatosExpediente();
            }
            catch (Exception ex) { MessageBox.Show($"Error al abrir avance: {ex.Message}"); }
        }

        private void BtnSeguimientoEdicion_Click(object sender, RoutedEventArgs e)
        {
            try { new VistaBitacora(_idAsunto).ShowDialog(); }
            catch (Exception ex) { MessageBox.Show($"Error bitácora: {ex.Message}"); }
        }

        // AQUÍ ESTÁ EL MÉTODO QUE TE DABA ERROR:
        private void BtnEliminarEdicion_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"¿Deseas eliminar permanentemente el Expediente N° {_idAsunto}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Si usas una ventana de confirmación extra:
                ConfirmarBorrado confirm = new ConfirmarBorrado();
                if (confirm.ShowDialog() == true) EjecutarBorrado(_idAsunto);
            }
        }

        private void EjecutarBorrado(int id)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Seguimiento WHERE num_Asunto = $id; DELETE FROM Asuntos WHERE Id_asunto = $id;";
                        cmd.Parameters.AddWithValue("$id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Expediente eliminado con éxito.");
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show($"Error al eliminar: {ex.Message}"); }
            finally { DatabaseConnection.CloseConnection(); }
        }

        // ==========================================
        // 3. GUARDADO Y OTROS
        // ==========================================
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Asuntos SET Fecha_recepcion = $fechaRec, Nombre_oficio = $nombreOfi, 
                                               id_sap = $sap, clave_centroTrabajo = $centro WHERE Id_asunto = $id";

                        command.Parameters.AddWithValue("$fechaRec", DpFechaRecepcion.SelectedDate?.ToString("yyyy-MM-dd") ?? "");
                        command.Parameters.AddWithValue("$nombreOfi", TxtNombreOficio.Text.Trim());
                        command.Parameters.AddWithValue("$sap", CmbSAP.SelectedValue ?? 0);
                        command.Parameters.AddWithValue("$centro", CmbCentroTrabajo.SelectedValue ?? 0);
                        command.Parameters.AddWithValue("$id", _idAsunto);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cambios guardados.");
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show($"Error al guardar: {ex.Message}"); }
        }

        private void CmbCentroTrabajo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCentroTrabajo.SelectedItem is ItemCentroTrabajo centro)
            {
                if (centro.IdSap > 0) CmbSAP.SelectedValue = centro.IdSap;
                if (centro.IdOrganismo > 0) CmbOrganismo.SelectedValue = centro.IdOrganismo;
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e) => this.Close();
    }

    // MODELOS
    public class ItemCatalogo { public long Id { get; set; } public string Descripcion { get; set; } }
    public class ItemCentroTrabajo { public long Id { get; set; } public string Descripcion { get; set; } public long IdSap { get; set; } public long IdOrganismo { get; set; } }
}