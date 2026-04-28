using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels.Base;
using Proyecto_GRRLN_expediente.modelos;
using Proyecto_GRRLN_expediente.db;
using System.Collections.Generic;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VistaEditarExpedienteViewModel : ViewModelBase
    {
        // ----------------------------------------------------
        // VARIABLES DE ESTADO Y DATOS ORIGINALES
        // ----------------------------------------------------
        private int _idAsunto;
        private int _avanceActual;
        private List<ItemCatalogo> _todosLosDeptos = new List<ItemCatalogo>();

        // ----------------------------------------------------
        // ACCIONES DE NAVEGACIÓN INYECTADAS POR LA VISTA
        // ----------------------------------------------------
        public Action CerrarVentanaAction { get; set; }
        public Action<int, int> AbrirAvanceAction { get; set; }
        public Action<int> AbrirBitacoraAction { get; set; }
        public Func<string> ConfirmarBorradoAction { get; set; }

        // ----------------------------------------------------
        // COLECCIONES PARA COMBOBOXES
        // ----------------------------------------------------
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

        // ----------------------------------------------------
        // PROPIEDADES BINDEABLES (UI)
        // ----------------------------------------------------
        private string _tituloVista;
        public string TituloVista { get => _tituloVista; set => SetProperty(ref _tituloVista, value); }

        private DateTime? _fechaRecepcion;
        public DateTime? FechaRecepcion
        {
            get => _fechaRecepcion;
            set 
            {
                if (SetProperty(ref _fechaRecepcion, value))
                {
                    OnPropertyChanged(nameof(MinFechaCompromiso));
                    // No forzamos null aquí para evitar loops en edición, la validación se hace en Guardar
                }
            }
        }
        
        public DateTime? MinFechaCompromiso => FechaRecepcion;

        private string _tipo;
        public string Tipo { get => _tipo; set => SetProperty(ref _tipo, value); }

        private string _nombreOficio;
        public string NombreOficio { get => _nombreOficio; set => SetProperty(ref _nombreOficio, value); }

        private DateTime? _fechaOficio;
        public DateTime? FechaOficio { get => _fechaOficio; set => SetProperty(ref _fechaOficio, value); }

        private long? _sapSeleccionado;
        public long? SapSeleccionado { get => _sapSeleccionado; set => SetProperty(ref _sapSeleccionado, value); }

        private long? _organismoSeleccionado;
        public long? OrganismoSeleccionado { get => _organismoSeleccionado; set => SetProperty(ref _organismoSeleccionado, value); }

        private ItemCatalogo _departamentoSeleccionado;
        public ItemCatalogo DepartamentoSeleccionado { get => _departamentoSeleccionado; set => SetProperty(ref _departamentoSeleccionado, value); }

        private string _agendaSeleccionada;
        public string AgendaSeleccionada { get => _agendaSeleccionada; set => SetProperty(ref _agendaSeleccionada, value); }

        private long? _secSindicalSeleccionada;
        public long? SecSindicalSeleccionada { get => _secSindicalSeleccionada; set => SetProperty(ref _secSindicalSeleccionada, value); }

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
        public long? DescCortaSeleccionada { get => _descCortaSeleccionada; set => SetProperty(ref _descCortaSeleccionada, value); }

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
        public string FichaVisual { get => _fichaVisual; set => SetProperty(ref _fichaVisual, value); }

        private bool _responsableEnabled;
        public bool ResponsableEnabled { get => _responsableEnabled; set => SetProperty(ref _responsableEnabled, value); }

        private DateTime? _fechaCompromiso;
        public DateTime? FechaCompromiso { get => _fechaCompromiso; set => SetProperty(ref _fechaCompromiso, value); }

        private string _instruccion;
        public string Instruccion { get => _instruccion; set => SetProperty(ref _instruccion, value); }

        private string _observaciones;
        public string Observaciones { get => _observaciones; set => SetProperty(ref _observaciones, value); }

        // ----------------------------------------------------
        // COMANDOS
        // ----------------------------------------------------
        public ICommand GuardarCommand { get; }
        public ICommand RegresarCommand { get; }
        public ICommand AvanceCommand { get; }
        public ICommand SeguimientoCommand { get; }
        public ICommand EliminarCommand { get; }

        public VistaEditarExpedienteViewModel(int idAsunto)
        {
            _idAsunto = idAsunto;
            TituloVista = $"EDITAR ASUNTO N° {_idAsunto}";

            GuardarCommand = new RelayCommand(ExecuteGuardar);
            RegresarCommand = new RelayCommand(ExecuteRegresar);
            AvanceCommand = new RelayCommand(ExecuteAvance);
            SeguimientoCommand = new RelayCommand(ExecuteSeguimiento);
            EliminarCommand = new RelayCommand(ExecuteEliminar);

            InicializarDatos();
        }

        private void InicializarDatos()
        {
            CargarCatalogos();
            CargarUsuariosResponsables();
            CargarDatosExpediente();
        }

        // ----------------------------------------------------
        // MÉTODOS DE BASE DE DATOS Y CARGA
        // ----------------------------------------------------
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
                            while (reader.Read()) ListaSAP.Add(new ItemCatalogo { Id = Convert.ToInt64(reader[0]), Descripcion = reader[1].ToString() });

                        cmd.CommandText = "SELECT id_organismo, Organismo FROM Organismos";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read()) ListaOrganismo.Add(new ItemCatalogo { Id = Convert.ToInt64(reader[0]), Descripcion = reader[1].ToString() });

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
                                    Id = Convert.ToInt64(reader[0]), 
                                    Descripcion = reader[1].ToString(),
                                    IdPadre = reader.IsDBNull(2) ? 0 : Convert.ToInt64(reader[2]),
                                    IdGerencia = reader.IsDBNull(3) ? 0 : Convert.ToInt64(reader[3])
                                });
                            }
                        }
                        ListaDepartamentos = new ObservableCollection<ItemCatalogo>(_todosLosDeptos);

                        cmd.CommandText = "SELECT SeccionSindical, Descripcion FROM AS_CatSecSind";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read()) ListaSecSindical.Add(new ItemCatalogo { Id = Convert.ToInt64(reader[0]), Descripcion = reader[1].ToString() });

                        cmd.CommandText = "SELECT clave_centro, Desc_centroTrabajo, Id_SAP, id_organismo FROM AS_CatCentros";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaCentrosTrabajo.Add(new ItemCentroTrabajo
                                {
                                    Id = Convert.ToInt64(reader[0]),
                                    Descripcion = reader[1].ToString(),
                                    IdSap = reader.IsDBNull(2) ? 0 : Convert.ToInt64(reader[2]),
                                    IdOrganismo = reader.IsDBNull(3) ? 0 : Convert.ToInt64(reader[3])
                                });
                            }
                        }

                        cmd.CommandText = "SELECT id_descripcionCorta, tipo_descripcion FROM Descripcion_corta";
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read()) ListaDescCorta.Add(new ItemCatalogo { Id = Convert.ToInt64(reader[0]), Descripcion = reader[1].ToString() });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error al cargar catálogos: {ex.Message}"); }
        }

        private void CargarUsuariosResponsables()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Ficha, nombre FROM usuario WHERE COALESCE(estatus, 1) = 1";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaResponsables.Add(new ItemUsuario
                                {
                                    IdFicha = reader[0].ToString(),
                                    Descripcion = $"{reader[0].ToString()} - {reader[1].ToString()}"
                                });
                            }
                        }

                        string miTipo = "OPERADOR";
                        string miFicha = SesionGlobal.Ficha ?? "admin";

                        cmd.CommandText = "SELECT tipo FROM usuario WHERE Ficha = @ficha";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ficha", miFicha);
                        var resultTipo = cmd.ExecuteScalar();
                        if (resultTipo != null && resultTipo != DBNull.Value)
                            miTipo = resultTipo.ToString().Trim().ToUpper();

                        if (miTipo == "OPERADOR" || miTipo == "CONSULTOR")
                        {
                            ResponsableEnabled = false;
                        }
                        else
                        {
                            ResponsableEnabled = true;
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error al cargar responsables: " + ex.Message); }
        }

        public void CargarDatosExpediente()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Fecha_recepcion, Tipo, Nombre_oficio, Fecha_oficio, Agenda, 
                                           Fecha_Compromiso, Instruccion, Observaciones, id_sap, clave_depto, 
                                           Sec_Sindical, Id_Organismo, clave_centroTrabajo, id_descripcionCorta, Porcentaje_avance,
                                           Ficha 
                                           FROM Asuntos WHERE Id_asunto = @id";
                        cmd.Parameters.AddWithValue("@id", _idAsunto);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                FechaRecepcion = reader.IsDBNull(0) ? (DateTime?)null : DateTime.Parse(reader[0].ToString());
                                Tipo = reader.IsDBNull(1) ? "CASO" : reader[1].ToString();
                                NombreOficio = reader.IsDBNull(2) ? "" : reader[2].ToString();
                                FechaOficio = reader.IsDBNull(3) ? (DateTime?)null : DateTime.Parse(reader[3].ToString());
                                AgendaSeleccionada = reader.IsDBNull(4) ? null : reader[4].ToString();
                                FechaCompromiso = reader.IsDBNull(5) ? (DateTime?)null : DateTime.Parse(reader[5].ToString());
                                Instruccion = reader.IsDBNull(6) ? "" : reader[6].ToString();
                                Observaciones = reader.IsDBNull(7) ? "" : reader[7].ToString();

                                SapSeleccionado = reader.IsDBNull(8) ? (long?)null : Convert.ToInt64(reader[8]);
                                SecSindicalSeleccionada = reader.IsDBNull(10) ? (long?)null : Convert.ToInt64(reader[10]);
                                OrganismoSeleccionado = reader.IsDBNull(11) ? (long?)null : Convert.ToInt64(reader[11]);
                                DescCortaSeleccionada = reader.IsDBNull(13) ? (long?)null : Convert.ToInt64(reader[13]);

                                _avanceActual = reader.IsDBNull(14) ? 0 : Convert.ToInt32(reader.GetValue(14));

                                if (!reader.IsDBNull(15))
                                {
                                    ResponsableSeleccionado = reader[15].ToString();
                                }

                                // Para los seleccionables de objeto: Centro de trabajo y Departamento
                                if (!reader.IsDBNull(12))
                                {
                                    long idCentro = Convert.ToInt64(reader[12]);
                                    var centro = ListaCentrosTrabajo.FirstOrDefault(c => c.Id == idCentro);
                                    if (centro != null)
                                    {
                                        _centroTrabajoSeleccionado = centro;
                                        OnPropertyChanged(nameof(CentroTrabajoSeleccionado));
                                        EjecutarLogicaCascada(); // Para pre-filtrar departamentos
                                    }
                                }

                                if (!reader.IsDBNull(9))
                                {
                                    long idDepto = Convert.ToInt64(reader[9]);
                                    var depto = ListaDepartamentos.FirstOrDefault(d => d.Id == idDepto);
                                    if (depto != null)
                                    {
                                        DepartamentoSeleccionado = depto;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error carga: {ex.Message}"); }
        }

        // ----------------------------------------------------
        // LÓGICA DE NEGOCIO (CASCADA)
        // ----------------------------------------------------
        private void EjecutarLogicaCascada()
        {
            if (CentroTrabajoSeleccionado != null)
            {
                if (CentroTrabajoSeleccionado.IdSap > 0)
                    SapSeleccionado = CentroTrabajoSeleccionado.IdSap;

                if (CentroTrabajoSeleccionado.IdOrganismo > 0)
                {
                    OrganismoSeleccionado = CentroTrabajoSeleccionado.IdOrganismo;

                    var idSeleccionadoPreviamente = DepartamentoSeleccionado?.Id;

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
                    }
                    else
                    {
                        ListaDepartamentos = new ObservableCollection<ItemCatalogo>(deptosFiltrados);
                    }

                    if (idSeleccionadoPreviamente.HasValue)
                    {
                        var match = ListaDepartamentos.FirstOrDefault(d => d.Id == idSeleccionadoPreviamente.Value);
                        if (match != null) DepartamentoSeleccionado = match;
                    }

                    if (DepartamentoSeleccionado == null && deptosFiltrados.Count == 1)
                    {
                        DepartamentoSeleccionado = deptosFiltrados[0];
                    }
                }
                else
                {
                    ListaDepartamentos = new ObservableCollection<ItemCatalogo>(_todosLosDeptos);
                }
            }
        }

        // ----------------------------------------------------
        // EJECUCIÓN DE COMANDOS
        // ----------------------------------------------------
        private void ExecuteAvance(object parameter)
        {
            AbrirAvanceAction?.Invoke(_idAsunto, _avanceActual);
        }

        private void ExecuteSeguimiento(object parameter)
        {
            AbrirBitacoraAction?.Invoke(_idAsunto);
        }

        private void ExecuteEliminar(object parameter)
        {
            string motivoBaja = ConfirmarBorradoAction?.Invoke();

            if (!string.IsNullOrEmpty(motivoBaja))
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        if (conn == null) return;
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"
                                UPDATE Asuntos SET Eliminado = 0 WHERE Id_asunto = @id;
                                INSERT INTO Seguimiento (num_Asunto, Descripcion, fecha_Seguimiento) 
                                VALUES (@id, @msg, @fecha);";

                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(_idAsunto));
                            cmd.Parameters.AddWithValue("@msg", $"EXPEDIENTE ELIMINADO. Motivo: {motivoBaja} (Eliminado por: {SesionGlobal.Nombre ?? "Usuario"})");
                            cmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Expediente eliminado con éxito y registrado en bitácora.", "Borrado Lógico", MessageBoxButton.OK, MessageBoxImage.Information);
                    CerrarVentanaAction?.Invoke();
                }
                catch (Exception ex) { MessageBox.Show($"Error al realizar el borrado lógico: {ex.Message}"); }
            }
        }

        private void ExecuteRegresar(object parameter)
        {
            CerrarVentanaAction?.Invoke();
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
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Asuntos SET 
                                                Fecha_recepcion = @fechaRec, 
                                                Nombre_oficio = @nombreOfi, 
                                                Fecha_oficio = @fechaOfi,
                                                id_sap = @sap, 
                                                clave_depto = @depto, 
                                                Agenda = @agenda, 
                                                Sec_Sindical = @secSind, 
                                                Id_Organismo = @org, 
                                                clave_centroTrabajo = @centro, 
                                                id_descripcionCorta = @descCorta,
                                                Ficha = @ficha, 
                                                Instruccion = @inst, 
                                                Observaciones = @obs, 
                                                Fecha_Compromiso = @fechaComp
                                                WHERE Id_asunto = @id";

                        command.Parameters.AddWithValue("@fechaRec", FechaRecepcion.Value.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@nombreOfi", NombreOficio.Trim());
                        command.Parameters.AddWithValue("@fechaOfi", FechaOficio.Value.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@agenda", AgendaSeleccionada);
                        command.Parameters.AddWithValue("@secSind", Convert.ToInt32(SecSindicalSeleccionada.Value));
                        command.Parameters.AddWithValue("@descCorta", Convert.ToInt32(DescCortaSeleccionada.Value));
                        command.Parameters.AddWithValue("@ficha", ResponsableSeleccionado);
                        command.Parameters.AddWithValue("@fechaComp", FechaCompromiso.Value.ToString("yyyy-MM-dd"));

                        object inst = string.IsNullOrWhiteSpace(Instruccion) ? DBNull.Value : Instruccion.Trim();
                        object obs = string.IsNullOrWhiteSpace(Observaciones) ? DBNull.Value : Observaciones.Trim();
                        command.Parameters.AddWithValue("@inst", inst);
                        command.Parameters.AddWithValue("@obs", obs);

                        command.Parameters.AddWithValue("@sap", Convert.ToInt32(SapSeleccionado.Value));
                        command.Parameters.AddWithValue("@org", Convert.ToInt32(OrganismoSeleccionado.Value));
                        command.Parameters.AddWithValue("@centro", Convert.ToInt32(CentroTrabajoSeleccionado.Id));
                        command.Parameters.AddWithValue("@depto", Convert.ToInt32(DepartamentoSeleccionado.Id));
                        command.Parameters.AddWithValue("@id", Convert.ToInt32(_idAsunto));

                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cambios guardados exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CerrarVentanaAction?.Invoke();
            }
            catch (Exception ex) { MessageBox.Show($"Error al actualizar: {ex.Message}"); }
        }
    }
}
