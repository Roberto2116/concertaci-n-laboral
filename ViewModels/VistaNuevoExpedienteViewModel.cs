using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;
using Proyecto_GRRLN_expediente.modelos;
using Proyecto_GRRLN_expediente.ViewModels.Base;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VistaNuevoExpedienteViewModel : ViewModelBase
    {
        private List<ItemCatalogo> _todosLosDeptos = new List<ItemCatalogo>();

        // Acciones para navegación inyectadas desde la vista
        public Action CerrarVentanaAction { get; set; }
        public Action AbrirConsultaAction { get; set; }

        #region Propiedades de UI (Labels)
        private string _fechaActual;
        public string FechaActual
        {
            get => _fechaActual;
            set => SetProperty(ref _fechaActual, value);
        }

        private string _usuarioActual;
        public string UsuarioActual
        {
            get => _usuarioActual;
            set => SetProperty(ref _usuarioActual, value);
        }

        private string _proximoFolio;
        public string ProximoFolio
        {
            get => _proximoFolio;
            set => SetProperty(ref _proximoFolio, value);
        }
        #endregion

        #region Colecciones para ComboBox
        public ObservableCollection<ItemCatalogo> ListaSAP { get; set; } = new ObservableCollection<ItemCatalogo>();
        public ObservableCollection<ItemCatalogo> ListaOrganismo { get; set; } = new ObservableCollection<ItemCatalogo>();
        
        private ObservableCollection<ItemCatalogo> _listaDepartamentos = new ObservableCollection<ItemCatalogo>();
        public ObservableCollection<ItemCatalogo> ListaDepartamentos
        {
            get => _listaDepartamentos;
            set => SetProperty(ref _listaDepartamentos, value);
        }

        public ObservableCollection<ItemCatalogo> ListaSecSindical { get; set; } = new ObservableCollection<ItemCatalogo>();
        public ObservableCollection<ItemCentroTrabajo> ListaCentrosTrabajo { get; set; } = new ObservableCollection<ItemCentroTrabajo>();
        public ObservableCollection<ItemCatalogo> ListaDescCorta { get; set; } = new ObservableCollection<ItemCatalogo>();
        public ObservableCollection<ItemUsuario> ListaResponsables { get; set; } = new ObservableCollection<ItemUsuario>();
        public ObservableCollection<string> ListaAgendas { get; set; } = new ObservableCollection<string> { "ADMINISTRATIVA", "SINDICAL" };
        #endregion

        #region Valores Seleccionados y Entradas
        private DateTime? _fechaRecepcion;
        public DateTime? FechaRecepcion
        {
            get => _fechaRecepcion;
            set 
            {
                if (SetProperty(ref _fechaRecepcion, value))
                {
                    OnPropertyChanged(nameof(MinFechaCompromiso));
                    if (FechaCompromiso.HasValue && FechaRecepcion.HasValue && FechaCompromiso.Value < FechaRecepcion.Value)
                    {
                        FechaCompromiso = null;
                    }
                }
            }
        }

        public DateTime? MinFechaCompromiso => FechaRecepcion;

        private string _nombreOficio;
        public string NombreOficio
        {
            get => _nombreOficio;
            set => SetProperty(ref _nombreOficio, value);
        }

        private DateTime? _fechaOficio;
        public DateTime? FechaOficio
        {
            get => _fechaOficio;
            set => SetProperty(ref _fechaOficio, value);
        }

        private long? _sapSeleccionado;
        public long? SapSeleccionado
        {
            get => _sapSeleccionado;
            set => SetProperty(ref _sapSeleccionado, value);
        }

        private long? _organismoSeleccionado;
        public long? OrganismoSeleccionado
        {
            get => _organismoSeleccionado;
            set => SetProperty(ref _organismoSeleccionado, value);
        }

        private ItemCatalogo _departamentoSeleccionado;
        public ItemCatalogo DepartamentoSeleccionado
        {
            get => _departamentoSeleccionado;
            set => SetProperty(ref _departamentoSeleccionado, value);
        }

        private string _agendaSeleccionada;
        public string AgendaSeleccionada
        {
            get => _agendaSeleccionada;
            set => SetProperty(ref _agendaSeleccionada, value);
        }

        private long? _secSindicalSeleccionada;
        public long? SecSindicalSeleccionada
        {
            get => _secSindicalSeleccionada;
            set => SetProperty(ref _secSindicalSeleccionada, value);
        }

        private ItemCentroTrabajo _centroTrabajoSeleccionado;
        public ItemCentroTrabajo CentroTrabajoSeleccionado
        {
            get => _centroTrabajoSeleccionado;
            set
            {
                if (SetProperty(ref _centroTrabajoSeleccionado, value))
                {
                    EjecutarLogicaCascada();
                }
            }
        }

        private long? _descCortaSeleccionada;
        public long? DescCortaSeleccionada
        {
            get => _descCortaSeleccionada;
            set => SetProperty(ref _descCortaSeleccionada, value);
        }

        private string _responsableSeleccionado;
        public string ResponsableSeleccionado
        {
            get => _responsableSeleccionado;
            set
            {
                if (SetProperty(ref _responsableSeleccionado, value))
                {
                    FichaVisual = value;
                }
            }
        }

        private string _fichaVisual;
        public string FichaVisual
        {
            get => _fichaVisual;
            set => SetProperty(ref _fichaVisual, value);
        }

        private bool _responsableEnabled = true;
        public bool ResponsableEnabled
        {
            get => _responsableEnabled;
            set => SetProperty(ref _responsableEnabled, value);
        }

        private DateTime? _fechaCompromiso;
        public DateTime? FechaCompromiso
        {
            get => _fechaCompromiso;
            set => SetProperty(ref _fechaCompromiso, value);
        }

        private string _instruccion;
        public string Instruccion
        {
            get => _instruccion;
            set => SetProperty(ref _instruccion, value);
        }

        private string _observaciones;
        public string Observaciones
        {
            get => _observaciones;
            set => SetProperty(ref _observaciones, value);
        }
        #endregion

        public ICommand GuardarCommand { get; }
        public ICommand RegresarCommand { get; }

        public VistaNuevoExpedienteViewModel()
        {
            GuardarCommand = new RelayCommand(ExecuteGuardar);
            RegresarCommand = new RelayCommand(ExecuteRegresar);

            InicializarDatos();
        }

        private void InicializarDatos()
        {
            FechaActual = DateTime.Now.ToString("dd/MM/yyyy");
            UsuarioActual = SesionGlobal.Ficha ?? "Desconocido";

            ObtenerProximoFolio();
            CargarCatalogos();
            CargarUsuariosResponsables();
        }

        private void ObtenerProximoFolio()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT MAX(Id_asunto) FROM Asuntos";
                        var result = command.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            ProximoFolio = "1";
                        else
                        {
                            long ultimoId = Convert.ToInt64(result);
                            ProximoFolio = (ultimoId + 1).ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
                ProximoFolio = "Error";
            }
        }

        private void CargarCatalogos()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Id_SAP, Descripcion_Sap FROM Cat_SAP";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read()) ListaSAP.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });

                        cmd.CommandText = "SELECT id_organismo, Organismo FROM Organismos";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read()) ListaOrganismo.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });

                        cmd.CommandText = @"
                            SELECT d.clave_depto, d.descripcion, d.clave_subgerencia, s.clave_gerencia 
                            FROM Dep_personal d 
                            LEFT JOIN Subgerencia s ON d.clave_subgerencia = s.Clave_subgerencia";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _todosLosDeptos.Add(new ItemCatalogo 
                                { 
                                    Id = reader.GetInt64(0), 
                                    Descripcion = reader.GetString(1),
                                    IdPadre = reader.IsDBNull(2) ? 0 : reader.GetInt64(2),
                                    IdGerencia = reader.IsDBNull(3) ? 0 : reader.GetInt64(3)
                                });
                            }
                        }
                        ListaDepartamentos = new ObservableCollection<ItemCatalogo>(_todosLosDeptos);

                        cmd.CommandText = "SELECT SeccionSindical, Descripcion FROM AS_CatSecSind";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read()) ListaSecSindical.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });

                        cmd.CommandText = "SELECT clave_centro, Desc_centroTrabajo, Id_SAP, id_organismo FROM AS_CatCentros";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaCentrosTrabajo.Add(new ItemCentroTrabajo
                                {
                                    Id = reader.GetInt64(0),
                                    Descripcion = reader.GetString(1),
                                    IdSap = reader.IsDBNull(2) ? 0 : reader.GetInt64(2),
                                    IdOrganismo = reader.IsDBNull(3) ? 0 : reader.GetInt64(3)
                                });
                            }
                        }

                        cmd.CommandText = "SELECT id_descripcionCorta, tipo_descripcion FROM Descripcion_corta";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read()) ListaDescCorta.Add(new ItemCatalogo { Id = reader.GetInt64(0), Descripcion = reader.GetString(1) });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar catálogos: {ex.Message}");
            }
        }

        private void CargarUsuariosResponsables()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    string miTipo = "OPERADOR";
                    string miFicha = SesionGlobal.Ficha ?? "admin";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Ficha, nombre FROM usuario WHERE IFNULL(estatus, 1) = 1";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaResponsables.Add(new ItemUsuario
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

                    if (miTipo == "OPERADOR" || miTipo == "CONSULTOR")
                    {
                        ResponsableSeleccionado = miFicha;
                        ResponsableEnabled = false;
                    }
                    else
                    {
                        ResponsableEnabled = true;
                        ResponsableSeleccionado = miFicha;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar responsables: " + ex.Message);
            }
        }

        private void EjecutarLogicaCascada()
        {
            if (CentroTrabajoSeleccionado != null)
            {
                if (CentroTrabajoSeleccionado.IdSap > 0)
                    SapSeleccionado = CentroTrabajoSeleccionado.IdSap;

                if (CentroTrabajoSeleccionado.IdOrganismo > 0)
                {
                    OrganismoSeleccionado = CentroTrabajoSeleccionado.IdOrganismo;

                    var deptosFiltrados = _todosLosDeptos
                        .Where(d => d.IdPadre == CentroTrabajoSeleccionado.IdOrganismo)
                        .ToList();

                    if (deptosFiltrados.Count == 0 && CentroTrabajoSeleccionado.IdSap > 0)
                    {
                        deptosFiltrados = _todosLosDeptos
                            .Where(d => d.IdGerencia == CentroTrabajoSeleccionado.IdSap)
                            .ToList();
                    }

                    if (deptosFiltrados.Count == 0)
                    {
                        ListaDepartamentos = new ObservableCollection<ItemCatalogo>(_todosLosDeptos);
                        DepartamentoSeleccionado = null;
                    }
                    else
                    {
                        ListaDepartamentos = new ObservableCollection<ItemCatalogo>(deptosFiltrados);
                        if (deptosFiltrados.Count == 1)
                        {
                            DepartamentoSeleccionado = deptosFiltrados[0];
                        }
                        else
                        {
                            DepartamentoSeleccionado = null;
                        }
                    }
                }
                else
                {
                    ListaDepartamentos = new ObservableCollection<ItemCatalogo>(_todosLosDeptos);
                }
            }
        }

        private void ExecuteGuardar(object parameter)
        {
            if (!FechaRecepcion.HasValue ||
                !FechaOficio.HasValue ||
                string.IsNullOrWhiteSpace(NombreOficio) ||
                SapSeleccionado == null ||
                OrganismoSeleccionado == null ||
                CentroTrabajoSeleccionado == null ||
                DepartamentoSeleccionado == null ||
                string.IsNullOrWhiteSpace(AgendaSeleccionada) ||
                SecSindicalSeleccionada == null ||
                DescCortaSeleccionada == null ||
                string.IsNullOrWhiteSpace(ResponsableSeleccionado) ||
                !FechaCompromiso.HasValue)
            {
                MessageBox.Show("Por favor, llena todos los campos obligatorios (*).", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (FechaCompromiso.Value < FechaRecepcion.Value)
            {
                MessageBox.Show("¡Lógica inválida! La Fecha de Compromiso no puede ser anterior a la Fecha de Recepción.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string tipoAsunto = "CASO";
                object agenda = AgendaSeleccionada;
                object instruccion = string.IsNullOrWhiteSpace(Instruccion) ? DBNull.Value : Instruccion.Trim();
                object observaciones = string.IsNullOrWhiteSpace(Observaciones) ? DBNull.Value : Observaciones.Trim();

                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

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

                        command.Parameters.AddWithValue("$fechaRec", FechaRecepcion.Value.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("$tipo", tipoAsunto);
                        command.Parameters.AddWithValue("$nombreOfi", NombreOficio.Trim());
                        command.Parameters.AddWithValue("$fechaOfi", FechaOficio.Value.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("$agenda", agenda);
                        command.Parameters.AddWithValue("$fechaComp", FechaCompromiso.Value.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("$inst", instruccion);
                        command.Parameters.AddWithValue("$obs", observaciones);
                        command.Parameters.AddWithValue("$sap", SapSeleccionado.Value);
                        command.Parameters.AddWithValue("$depto", DepartamentoSeleccionado.Id);
                        command.Parameters.AddWithValue("$secSind", SecSindicalSeleccionada.Value);
                        command.Parameters.AddWithValue("$org", OrganismoSeleccionado.Value);
                        command.Parameters.AddWithValue("$centro", CentroTrabajoSeleccionado.Id);
                        command.Parameters.AddWithValue("$descCorta", DescCortaSeleccionada.Value);
                        command.Parameters.AddWithValue("$ficha", ResponsableSeleccionado);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("¡Asunto registrado y asignado correctamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Disparamos la acción inyectada por la Vista para manejar la navegación
                AbrirConsultaAction?.Invoke();
                CerrarVentanaAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar en la Base de Datos:\n{ex.Message}", "Error DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteRegresar(object parameter)
        {
            CerrarVentanaAction?.Invoke();
        }
    }
}
