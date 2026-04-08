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
            ObtenerProximoFolio();
            CargarCatalogos();
            CargarUsuariosResponsables(); // AHORA SÍ CON LA VALIDACIÓN CORRECTA
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

                // 1. Cargar SAPs
                List<ItemCatalogo> listaSAP = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id_SAP, Descripcion_Sap FROM Cat_SAP";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaSAP.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbSAP.ItemsSource = listaSAP;

                // 2. Cargar Organismos
                List<ItemCatalogo> listaOrg = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id_organismo, Organismo FROM Organismos";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaOrg.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbOrganismo.ItemsSource = listaOrg;

                // 3. Cargar Departamentos
                List<ItemCatalogo> listaDeptos = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT clave_depto, descripcion FROM Dep_personal";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaDeptos.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbDepartamento.ItemsSource = listaDeptos;

                // 4. Cargar Secciones Sindicales
                List<ItemCatalogo> listaSec = new List<ItemCatalogo>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT SeccionSindical, Descripcion FROM AS_CatSecSind";
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) listaSec.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                }
                CmbSecSindical.ItemsSource = listaSec;

                // 5. Cargar Centros de Trabajo
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

                // 6. Cargar Descripción Corta
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

        // ==============================================================================
        // CARGAR USUARIOS PARA ASIGNACIÓN (CORREGIDO PARA USAR SesionGlobal.Ficha)
        // ==============================================================================
        private void CargarUsuariosResponsables()
        {
            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                List<ItemUsuario> listaUsuarios = new List<ItemUsuario>();
                string miTipo = "OPERADOR"; // Por defecto asumimos el menor permiso

                // Usamos la variable SesionGlobal que tú manejas en tu proyecto
                string miFicha = SesionGlobal.Ficha ?? "admin";

                using (var cmd = conn.CreateCommand())
                {
                    // 1. Traemos a todos los usuarios activos
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

                    // 2. BUSCAMOS TU ROL DIRECTAMENTE EN LA BASE DE DATOS (100% SEGURO)
                    cmd.CommandText = "SELECT tipo FROM usuario WHERE Ficha = $ficha";
                    cmd.Parameters.AddWithValue("$ficha", miFicha);

                    var resultTipo = cmd.ExecuteScalar();
                    if (resultTipo != null && resultTipo != DBNull.Value)
                    {
                        miTipo = resultTipo.ToString().Trim().ToUpper();
                    }
                }

                CmbResponsable.ItemsSource = listaUsuarios;

                // 3. APLICAMOS LA REGLA SEGÚN EL ROL ENCONTRADO
                if (miTipo == "OPERADOR" || miTipo == "CONSULTOR")
                {
                    // Bloqueado: Solo se lo puede asignar a sí mismo
                    CmbResponsable.SelectedValue = miFicha;
                    CmbResponsable.IsEnabled = false;
                }
                else
                {
                    // Desbloqueado: Eres Administrador, Gerente, Jefe... Puedes elegir.
                    CmbResponsable.IsEnabled = true;
                    CmbResponsable.SelectedValue = miFicha; // Lo deja en tu nombre por comodidad, pero puedes abrirlo
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
            if (!DpFechaRecepcion.SelectedDate.HasValue || !DpFechaOficio.SelectedDate.HasValue || !DpFechaCompromiso.SelectedDate.HasValue)
            {
                MessageBox.Show("Por favor, selecciona todas las fechas obligatorias.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtNombreOficio.Text) || CmbTipo.SelectedItem == null || CmbAgenda.SelectedItem == null ||
                CmbSAP.SelectedValue == null || CmbOrganismo.SelectedValue == null || CmbCentroTrabajo.SelectedValue == null ||
                CmbDepartamento.SelectedValue == null || CmbSecSindical.SelectedValue == null || CmbDescCorta.SelectedValue == null ||
                CmbResponsable.SelectedValue == null)
            {
                MessageBox.Show("Por favor, llena todos los campos obligatorios de los menús desplegables y asegúrate de elegir un Responsable.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string fechaRecepcion = DpFechaRecepcion.SelectedDate.Value.ToString("yyyy-MM-dd");
                string fechaOficio = DpFechaOficio.SelectedDate.Value.ToString("yyyy-MM-dd");
                string fechaCompromiso = DpFechaCompromiso.SelectedDate.Value.ToString("yyyy-MM-dd");

                string nombreOficio = TxtNombreOficio.Text.Trim();
                string tipoAsunto = ((ComboBoxItem)CmbTipo.SelectedItem).Content.ToString();
                string agenda = ((ComboBoxItem)CmbAgenda.SelectedItem).Content.ToString();
                string instruccion = TxtInstruccion.Text?.Trim();
                string observaciones = TxtObservaciones.Text?.Trim();

                long idSap = (long)CmbSAP.SelectedValue;
                long idOrg = (long)CmbOrganismo.SelectedValue;
                long idCentro = (long)CmbCentroTrabajo.SelectedValue;
                long idDepto = (long)CmbDepartamento.SelectedValue;
                long idSecSindical = (long)CmbSecSindical.SelectedValue;
                long idDescCorta = (long)CmbDescCorta.SelectedValue;

                string fichaResponsable = CmbResponsable.SelectedValue.ToString();

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

                    command.Parameters.AddWithValue("$inst", string.IsNullOrEmpty(instruccion) ? DBNull.Value : instruccion);
                    command.Parameters.AddWithValue("$obs", string.IsNullOrEmpty(observaciones) ? DBNull.Value : observaciones);

                    command.Parameters.AddWithValue("$sap", idSap);
                    command.Parameters.AddWithValue("$depto", idDepto);
                    command.Parameters.AddWithValue("$secSind", idSecSindical);
                    command.Parameters.AddWithValue("$org", idOrg);
                    command.Parameters.AddWithValue("$centro", idCentro);
                    command.Parameters.AddWithValue("$descCorta", idDescCorta);

                    // AQUI INYECTAMOS AL RESPONSABLE QUE SELECCIONÓ EL ADMIN
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