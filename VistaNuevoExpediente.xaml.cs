using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaNuevoExpediente : Wpf.Ui.Controls.FluentWindow
    {
        public VistaNuevoExpediente()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LblFechaActual.Text = DateTime.Now.ToString("dd/MM/yyyy");
            LblUsuarioActual.Text = SesionGlobal.Ficha ?? "Desconocido";

            ObtenerProximoFolio();
            CargarCatalogos();
            CargarUsuariosResponsables();
        }

        private void ObtenerProximoFolio()
        {
            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT MAX(Id_asunto) FROM Asuntos";
                    var result = command.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        LblProximoFolio.Text = "1";
                    else
                    {
                        long ultimoId = Convert.ToInt64(result);
                        LblProximoFolio.Text = (ultimoId + 1).ToString();
                    }
                }
            }
            catch (Exception)
            {
                LblProximoFolio.Text = "Error";
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void CargarCatalogos()
        {
            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                List<ItemCatalogo> listaSAP = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id_SAP, Descripcion_Sap FROM Cat_SAP";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaSAP.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbSAP.ItemsSource = listaSAP;

                List<ItemCatalogo> listaOrg = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id_organismo, Organismo FROM Organismos";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaOrg.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbOrganismo.ItemsSource = listaOrg;

                List<ItemCatalogo> listaDeptos = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT clave_depto, descripcion FROM Dep_personal";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaDeptos.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbDepartamento.ItemsSource = listaDeptos;

                List<ItemCatalogo> listaSec = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT SeccionSindical, Descripcion FROM AS_CatSecSind";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaSec.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbSecSindical.ItemsSource = listaSec;

                List<ItemCentroTrabajo> listaCentros = new List<ItemCentroTrabajo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT clave_centro, Desc_centroTrabajo, Id_SAP, id_organismo FROM AS_CatCentros";
                    using (var reader = cmd.ExecuteReader())
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
                CmbCentroTrabajo.ItemsSource = listaCentros;

                List<ItemCatalogo> listaDescCorta = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id_descripcionCorta, tipo_descripcion FROM Descripcion_corta";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaDescCorta.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbDescCorta.ItemsSource = listaDescCorta;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar catálogos: {ex.Message}");
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void CargarUsuariosResponsables()
        {
            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                List<ItemUsuario> listaUsuarios = new List<ItemUsuario>();
                string miTipo = "OPERADOR";
                string miFicha = SesionGlobal.Ficha ?? "admin";

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Ficha, nombre FROM usuario WHERE IFNULL(estatus, 1) = 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaUsuarios.Add(new ItemUsuario
                            {
                                IdFicha = reader.GetString(0),
                                Descripcion = $"{reader.GetString(0)} - {reader.GetString(1)}"
                            });
                        }
                    }

                    cmd.CommandText = "SELECT tipo FROM usuario WHERE Ficha = $ficha";
                    cmd.Parameters.AddWithValue("$ficha", miFicha);

                    var resultTipo = cmd.ExecuteScalar();
                    if (resultTipo != null && resultTipo != DBNull.Value)
                    {
                        miTipo = resultTipo.ToString().Trim().ToUpper();
                    }
                }

                CmbResponsable.ItemsSource = listaUsuarios;

                if (miTipo == "OPERADOR" || miTipo == "CONSULTOR")
                {
                    CmbResponsable.SelectedValue = miFicha;
                    CmbResponsable.IsEnabled = false;
                }
                else
                {
                    CmbResponsable.IsEnabled = true;
                    CmbResponsable.SelectedValue = miFicha;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar responsables: " + ex.Message);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void CmbResponsable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbResponsable.SelectedValue != null)
            {
                TxtFichaVisual.Text = CmbResponsable.SelectedValue.ToString();
            }
        }

        private void CmbCentroTrabajo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCentroTrabajo.SelectedItem is ItemCentroTrabajo centroSeleccionado)
            {
                if (centroSeleccionado.IdSap > 0)
                    CmbSAP.SelectedValue = centroSeleccionado.IdSap;

                if (centroSeleccionado.IdOrganismo > 0)
                    CmbOrganismo.SelectedValue = centroSeleccionado.IdOrganismo;
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // AHORA CmbDescCorta TAMBIÉN ES OBLIGATORIO Y REQUERIDO ANTES DE AVANZAR
            if (!DpFechaRecepcion.SelectedDate.HasValue ||
                string.IsNullOrWhiteSpace(TxtNombreOficio.Text) ||
                CmbResponsable.SelectedValue == null ||
                CmbDescCorta.SelectedValue == null ||
                !DpFechaCompromiso.SelectedDate.HasValue)
            {
                MessageBox.Show("Por favor, llena todos los campos obligatorios marcados con asterisco (*).", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Datos obligatorios
                string fechaRecepcion = DpFechaRecepcion.SelectedDate.Value.ToString("yyyy-MM-dd");
                string fechaCompromiso = DpFechaCompromiso.SelectedDate.Value.ToString("yyyy-MM-dd");
                string nombreOficio = TxtNombreOficio.Text.Trim();
                string tipoAsunto = "CASO";
                string fichaResponsable = CmbResponsable.SelectedValue.ToString();
                long idDescCorta = (long)CmbDescCorta.SelectedValue; // Obligatorio, ya no acepta nulos

                // Datos opcionales (Aceptan nulos sin tronar la BD)
                object fechaOficio = DpFechaOficio.SelectedDate.HasValue ? DpFechaOficio.SelectedDate.Value.ToString("yyyy-MM-dd") : DBNull.Value;
                object agenda = CmbAgenda.SelectedItem != null ? ((ComboBoxItem)CmbAgenda.SelectedItem).Content.ToString() : DBNull.Value;
                object instruccion = string.IsNullOrWhiteSpace(TxtInstruccion.Text) ? DBNull.Value : TxtInstruccion.Text.Trim();
                object observaciones = string.IsNullOrWhiteSpace(TxtObservaciones.Text) ? DBNull.Value : TxtObservaciones.Text.Trim();

                object idSap = CmbSAP.SelectedValue ?? DBNull.Value;
                object idOrg = CmbOrganismo.SelectedValue ?? DBNull.Value;
                object idCentro = CmbCentroTrabajo.SelectedValue ?? DBNull.Value;
                object idDepto = CmbDepartamento.SelectedValue ?? DBNull.Value;
                object idSecSindical = CmbSecSindical.SelectedValue ?? DBNull.Value;

                SqliteConnection conn = DatabaseConnection.GetConnection();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"
                INSERT INTO Asuntos (
                    Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, 
                    id_sap, clave_depto, Agenda, Sec_Sindical, Id_Organismo, 
                    clave_centroTrabajo, id_descripcionCorta, Id_estatus, Ficha, 
                    Instruccion, Observaciones, Fecha_Compromiso, Porcentaje_avance
                ) VALUES (
                    $fechaRec, $tipo, $nombreOfi, $fechaOfi, 
                    $sap, $depto, $agenda, $secSind, $org, 
                    $centro, $descCorta, 1, $ficha, 
                    $inst, $obs, $fechaComp, 0
                )";

                    command.Parameters.AddWithValue("$fechaRec", fechaRecepcion);
                    command.Parameters.AddWithValue("$tipo", tipoAsunto);
                    command.Parameters.AddWithValue("$nombreOfi", nombreOficio);
                    command.Parameters.AddWithValue("$fechaOfi", fechaOficio);
                    command.Parameters.AddWithValue("$agenda", agenda);
                    command.Parameters.AddWithValue("$fechaComp", fechaCompromiso);
                    command.Parameters.AddWithValue("$inst", instruccion);
                    command.Parameters.AddWithValue("$obs", observaciones);
                    command.Parameters.AddWithValue("$sap", idSap);
                    command.Parameters.AddWithValue("$depto", idDepto);
                    command.Parameters.AddWithValue("$secSind", idSecSindical);
                    command.Parameters.AddWithValue("$org", idOrg);
                    command.Parameters.AddWithValue("$centro", idCentro);

                    // La descripción corta se envía de forma segura
                    command.Parameters.AddWithValue("$descCorta", idDescCorta);

                    command.Parameters.AddWithValue("$ficha", fichaResponsable);

                    command.ExecuteNonQuery();
                }

                MessageBox.Show("¡Asunto registrado y asignado correctamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                VistaConsultaGeneral ventanaConsulta = new VistaConsultaGeneral();
                ventanaConsulta.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar en la Base de Datos:\n{ex.Message}", "Error DB", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public class ItemCatalogo
        {
            public long Id { get; set; }
            public string Descripcion { get; set; }
        }

        public class ItemCentroTrabajo : ItemCatalogo
        {
            public long IdSap { get; set; }
            public long IdOrganismo { get; set; }
        }

        public class ItemUsuario
        {
            public string IdFicha { get; set; }
            public string Descripcion { get; set; }
        }
    }
}