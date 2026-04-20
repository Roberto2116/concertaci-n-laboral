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
        private int _avanceActual;
        private List<ItemCatalogo> _todosLosDeptos = new List<ItemCatalogo>();

        public VistaEditarExpediente(int idAsunto)
        {
            InitializeComponent();
            _idAsunto = idAsunto;
            TxtTituloVista.Text = $"EDITAR ASUNTO N° {_idAsunto}";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarCatalogos();
            CargarUsuariosResponsables(); // Agregado para llenar el combobox de responsables
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
 
                        cmd.CommandText = @"
                            SELECT d.clave_depto, d.descripcion, d.clave_subgerencia, s.clave_gerencia 
                            FROM Dep_personal d 
                            LEFT JOIN Subgerencia s ON d.clave_subgerencia = s.Clave_subgerencia";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaDeptos.Add(new ItemCatalogo 
                                { 
                                    Id = reader.GetInt64(0), 
                                    Descripcion = reader.GetString(1),
                                    IdPadre = reader.IsDBNull(2) ? 0 : reader.GetInt64(2),
                                    IdGerencia = reader.IsDBNull(3) ? 0 : reader.GetInt64(3)
                                });
                            }
                        }
                        _todosLosDeptos = listaDeptos;

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

        private void CargarUsuariosResponsables()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    List<ItemUsuario> listaUsuarios = new List<ItemUsuario>();
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
                    }
                    CmbResponsable.ItemsSource = listaUsuarios;

                    // Permisos de edición de responsable
                    string miTipo = "OPERADOR";
                    string miFicha = SesionGlobal.Ficha ?? "admin";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT tipo FROM usuario WHERE Ficha = $ficha";
                        cmd.Parameters.AddWithValue("$ficha", miFicha);
                        var resultTipo = cmd.ExecuteScalar();
                        if (resultTipo != null && resultTipo != DBNull.Value)
                            miTipo = resultTipo.ToString().Trim().ToUpper();
                    }

                    if (miTipo == "OPERADOR" || miTipo == "CONSULTOR")
                    {
                        CmbResponsable.IsEnabled = false;
                    }
                    else
                    {
                        CmbResponsable.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error al cargar responsables: " + ex.Message); }
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
                        // Agregué Ficha al final de la consulta
                        cmd.CommandText = @"SELECT Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, Agenda, 
                                           Fecha_Compromiso, Instruccion, Observaciones, id_sap, clave_depto, 
                                           Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Porcentaje_avance,
                                           Ficha 
                                           FROM Asuntos WHERE Id_asunto = $id";
                        cmd.Parameters.AddWithValue("$id", _idAsunto);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DpFechaRecepcion.SelectedDate = reader.IsDBNull(0) ? (DateTime?)null : DateTime.Parse(reader.GetString(0));
                                TxtTipo.Text = reader.IsDBNull(1) ? "CASO" : reader.GetString(1);
                                TxtNombreOficio.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                DpFechaOficio.SelectedDate = reader.IsDBNull(3) ? (DateTime?)null : DateTime.Parse(reader.GetString(3));

                                string agenda = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                foreach (ComboBoxItem item in CmbAgenda.Items) if (item.Content.ToString() == agenda) CmbAgenda.SelectedItem = item;

                                DpFechaCompromiso.SelectedDate = reader.IsDBNull(5) ? (DateTime?)null : DateTime.Parse(reader.GetString(5));
                                TxtInstruccion.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                TxtObservaciones.Text = reader.IsDBNull(7) ? "" : reader.GetString(7);

                                CmbSAP.SelectedValue = reader.IsDBNull(8) ? (long?)null : reader.GetInt64(8);
                                CmbDepartamento.SelectedValue = reader.IsDBNull(9) ? (long?)null : reader.GetInt64(9);
                                CmbSecSindical.SelectedValue = reader.IsDBNull(10) ? (long?)null : reader.GetInt64(10);
                                CmbOrganismo.SelectedValue = reader.IsDBNull(11) ? (long?)null : reader.GetInt64(11);
                                CmbCentroTrabajo.SelectedValue = reader.IsDBNull(12) ? (long?)null : reader.GetInt64(12);
                                CmbDescCorta.SelectedValue = reader.IsDBNull(13) ? (long?)null : reader.GetInt64(13);

                                _avanceActual = reader.IsDBNull(14) ? 0 : Convert.ToInt32(reader.GetValue(14));

                                // Cargar Responsable
                                if (!reader.IsDBNull(15))
                                {
                                    CmbResponsable.SelectedValue = reader.GetString(15);
                                    TxtFichaVisual.Text = reader.GetString(15);
                                }

                                // --- RESTRICCIÓN INICIAL AL CARGAR ---
                                if (DpFechaRecepcion.SelectedDate.HasValue)
                                {
                                    DpFechaCompromiso.DisplayDateStart = DpFechaRecepcion.SelectedDate.Value;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error carga: {ex.Message}"); }
            finally { DatabaseConnection.CloseConnection(); }
        }

        // ==========================================
        // 2. LÓGICA DE LOS BOTONES DE CABECERA Y COMBOS
        // ==========================================

        private void CmbResponsable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbResponsable.SelectedValue != null)
            {
                TxtFichaVisual.Text = CmbResponsable.SelectedValue.ToString();
            }
        }

        private void CmbCentroTrabajo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCentroTrabajo.SelectedItem is ItemCentroTrabajo centro)
            {
                if (centro.IdSap > 0) CmbSAP.SelectedValue = centro.IdSap;

                if (centro.IdOrganismo > 0)
                {
                    CmbOrganismo.SelectedValue = centro.IdOrganismo;

                    // --- ESTRATEGIA DE FILTRADO HÍBRIDO EN CASCADA ---
                    var idSeleccionadoPreviamente = CmbDepartamento.SelectedValue;

                    // Intento 1: Filtrado estricto por Subgerencia (IdPadre)
                    var deptosFiltrados = _todosLosDeptos
                        .Where(d => d.IdPadre == centro.IdOrganismo)
                        .ToList();

                    // Intento 2: Si no hay resultados, filtramos por Gerencia (SAP)
                    if (deptosFiltrados.Count == 0 && centro.IdSap > 0)
                    {
                        deptosFiltrados = _todosLosDeptos
                            .Where(d => d.IdGerencia == centro.IdSap)
                            .ToList();
                    }

                    // Intento 3: Fallback de seguridad - Si sigue vacío, mostramos todo
                    if (deptosFiltrados.Count == 0)
                    {
                        CmbDepartamento.ItemsSource = _todosLosDeptos;
                    }
                    else
                    {
                        CmbDepartamento.ItemsSource = deptosFiltrados;
                    }

                    // Restauramos la selección si el departamento sigue siendo válido para el nuevo origen
                    if (idSeleccionadoPreviamente != null)
                    {
                        // Buscamos si el ID seleccionado está en la lista actual (ya sea filtrada o completa)
                        CmbDepartamento.SelectedValue = idSeleccionadoPreviamente;
                    }

                    // Si no hay nada seleccionado y solo hay una opción lógica, auto-seleccionamos
                    if (CmbDepartamento.SelectedValue == null && deptosFiltrados.Count == 1)
                    {
                        CmbDepartamento.SelectedItem = deptosFiltrados[0];
                    }
                }
                else
                {
                    CmbDepartamento.ItemsSource = _todosLosDeptos;
                }
            }
        }

        private void BtnAvanceEdicion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

        private void BtnEliminarEdicion_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"¿Deseas eliminar permanentemente el Expediente N° {_idAsunto}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Si usas ConfirmarBorrado, descomenta estas líneas y quita el EjecutarBorrado directo
                // ConfirmarBorrado confirm = new ConfirmarBorrado();
                // if (confirm.ShowDialog() == true) EjecutarBorrado(_idAsunto);

                EjecutarBorrado(_idAsunto);
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
        // 3. GUARDADO (ACTUALIZADO COMPLETO)
        // ==========================================
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!DpFechaRecepcion.SelectedDate.HasValue ||
                !DpFechaOficio.SelectedDate.HasValue ||
                string.IsNullOrWhiteSpace(TxtNombreOficio.Text) ||
                CmbSAP.SelectedValue == null ||
                CmbOrganismo.SelectedValue == null ||
                CmbCentroTrabajo.SelectedValue == null ||
                CmbDepartamento.SelectedValue == null ||
                CmbAgenda.SelectedValue == null ||
                CmbSecSindical.SelectedValue == null ||
                CmbDescCorta.SelectedValue == null ||
                CmbResponsable.SelectedValue == null ||
                !DpFechaCompromiso.SelectedDate.HasValue)
            {
                MessageBox.Show("Por favor, llena todos los campos obligatorios (*).", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- VALIDACIÓN EXTRA DE SEGURIDAD (FECHAS) ---
            if (DpFechaCompromiso.SelectedDate.Value < DpFechaRecepcion.SelectedDate.Value)
            {
                MessageBox.Show("¡Lógica inválida! La Fecha de Compromiso no puede ser anterior a la Fecha de Recepción.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    using (var command = conn.CreateCommand())
                    {
                        // AHORA SE ACTUALIZAN TODOS LOS CAMPOS
                        command.CommandText = @"UPDATE Asuntos SET 
                                                Fecha_recepcion = $fechaRec, 
                                                Nombre_oficio = $nombreOfi, 
                                                Fecha_oficio = $fechaOfi,
                                                id_sap = $sap, 
                                                clave_depto = $depto, 
                                                Agenda = $agenda, 
                                                Sec_Sindical = $secSind, 
                                                Id_Organismo = $org, 
                                                clave_centroTrabajo = $centro, 
                                                id_descripcionCorta = $descCorta,
                                                Ficha = $ficha, 
                                                Instruccion = $inst, 
                                                Observaciones = $obs, 
                                                Fecha_Compromiso = $fechaComp
                                                WHERE Id_asunto = $id";

                        command.Parameters.AddWithValue("$fechaRec", DpFechaRecepcion.SelectedDate.Value.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("$nombreOfi", TxtNombreOficio.Text.Trim());
                        command.Parameters.AddWithValue("$fechaOfi", DpFechaOficio.SelectedDate.HasValue ? DpFechaOficio.SelectedDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);

                        object agenda = CmbAgenda.SelectedItem != null ? ((ComboBoxItem)CmbAgenda.SelectedItem).Content.ToString() : DBNull.Value;
                        command.Parameters.AddWithValue("$agenda", agenda);

                        command.Parameters.AddWithValue("$secSind", CmbSecSindical.SelectedValue ?? DBNull.Value);
                        command.Parameters.AddWithValue("$descCorta", CmbDescCorta.SelectedValue);

                        command.Parameters.AddWithValue("$ficha", CmbResponsable.SelectedValue.ToString());
                        command.Parameters.AddWithValue("$fechaComp", DpFechaCompromiso.SelectedDate.Value.ToString("yyyy-MM-dd"));

                        command.Parameters.AddWithValue("$inst", string.IsNullOrWhiteSpace(TxtInstruccion.Text) ? DBNull.Value : TxtInstruccion.Text.Trim());
                        command.Parameters.AddWithValue("$obs", string.IsNullOrWhiteSpace(TxtObservaciones.Text) ? DBNull.Value : TxtObservaciones.Text.Trim());

                        command.Parameters.AddWithValue("$sap", CmbSAP.SelectedValue ?? DBNull.Value);
                        command.Parameters.AddWithValue("$org", CmbOrganismo.SelectedValue ?? DBNull.Value);
                        command.Parameters.AddWithValue("$centro", CmbCentroTrabajo.SelectedValue ?? DBNull.Value);
                        command.Parameters.AddWithValue("$depto", CmbDepartamento.SelectedValue ?? DBNull.Value);

                        command.Parameters.AddWithValue("$id", _idAsunto);

                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cambios guardados exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show($"Error al actualizar: {ex.Message}"); }
            finally { DatabaseConnection.CloseConnection(); }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void DpFechaRecepcion_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DpFechaRecepcion.SelectedDate.HasValue)
            {
                // Bloqueamos físicamente los días anteriores a la recepción en el calendario de compromiso
                DpFechaCompromiso.DisplayDateStart = DpFechaRecepcion.SelectedDate.Value;

                // Si ya había una fecha de compromiso elegida y ahora es menor a la recepción, la limpiamos
                if (DpFechaCompromiso.SelectedDate.HasValue && DpFechaCompromiso.SelectedDate.Value < DpFechaRecepcion.SelectedDate.Value)
                {
                    DpFechaCompromiso.SelectedDate = null;
                }
            }
            else
            {
                DpFechaCompromiso.DisplayDateStart = null;
            }
        }

        // MODELOS
        public class ItemCatalogo { public long Id { get; set; } public string Descripcion { get; set; } public long IdPadre { get; set; } public long IdGerencia { get; set; } }
        public class ItemCentroTrabajo : ItemCatalogo { public long IdSap { get; set; } public long IdOrganismo { get; set; } }
        public class ItemUsuario { public string IdFicha { get; set; } public string Descripcion { get; set; } }
    }
}