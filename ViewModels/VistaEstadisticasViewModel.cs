using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Proyecto_GRRLN_expediente.ViewModels.Base;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VistaEstadisticasViewModel : ViewModelBase
    {
        public Action CerrarVentanaAction { get; set; }
        public Action MostrarDetalleAnimationAction { get; set; }
        public Action OcultarDetalleAnimationAction { get; set; }

        private bool _isLoaded = false;
        private List<DetalleAsuntoModel> _asuntosActuales = new List<DetalleAsuntoModel>();
        private List<UsuarioRanking> _listaRankingGlobal = new List<UsuarioRanking>();

        // Filtros
        private DateTime _fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set
            {
                if (SetProperty(ref _fechaInicio, value)) ActualizarDashboard();
            }
        }

        private DateTime _fechaFin = DateTime.Now.Date;
        public DateTime FechaFin
        {
            get => _fechaFin;
            set
            {
                if (SetProperty(ref _fechaFin, value)) ActualizarDashboard();
            }
        }

        public ObservableCollection<string> ListaSaps { get; } = new ObservableCollection<string>();

        private string _sapSeleccionado;
        public string SapSeleccionado
        {
            get => _sapSeleccionado;
            set
            {
                if (SetProperty(ref _sapSeleccionado, value)) ActualizarDashboard();
            }
        }

        // Pestaña 1 - Estadísticas
        private int _totalAsuntos;
        public int TotalAsuntos { get => _totalAsuntos; set => SetProperty(ref _totalAsuntos, value); }

        private int _totalCompletados;
        public int TotalCompletados { get => _totalCompletados; set => SetProperty(ref _totalCompletados, value); }

        private int _totalProceso;
        public int TotalProceso { get => _totalProceso; set => SetProperty(ref _totalProceso, value); }

        private int _totalVencidos;
        public int TotalVencidos { get => _totalVencidos; set => SetProperty(ref _totalVencidos, value); }

        private double _promedioAvance;
        public double PromedioAvance { get => _promedioAvance; set => SetProperty(ref _promedioAvance, value); }

        private string _promedioAvanceTexto;
        public string PromedioAvanceTexto { get => _promedioAvanceTexto; set => SetProperty(ref _promedioAvanceTexto, value); }

        public ObservableCollection<GraficaDonaModel> DatosDona { get; } = new ObservableCollection<GraficaDonaModel>();
        public ObservableCollection<UsuarioEstadisticaModel> DatosRendimientoUsuarios { get; } = new ObservableCollection<UsuarioEstadisticaModel>();
        
        // Detalles de Pestaña 1
        private string _tituloDetalle;
        public string TituloDetalle { get => _tituloDetalle; set => SetProperty(ref _tituloDetalle, value); }

        public ObservableCollection<DetalleAsuntoModel> AsuntosDetalle { get; } = new ObservableCollection<DetalleAsuntoModel>();

        // Pestaña 2 - Productividad
        private int _totalUsuarios;
        public int TotalUsuarios { get => _totalUsuarios; set => SetProperty(ref _totalUsuarios, value); }

        private int _totalSesiones;
        public int TotalSesiones { get => _totalSesiones; set => SetProperty(ref _totalSesiones, value); }

        private string _promedioSesiones;
        public string PromedioSesiones { get => _promedioSesiones; set => SetProperty(ref _promedioSesiones, value); }

        private string _topUsuario;
        public string TopUsuario { get => _topUsuario; set => SetProperty(ref _topUsuario, value); }

        public ObservableCollection<UsuarioRanking> ListaRankingGlobal { get; } = new ObservableCollection<UsuarioRanking>();

        private UsuarioRanking _usuarioSeleccionado;
        public UsuarioRanking UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set
            {
                if (SetProperty(ref _usuarioSeleccionado, value))
                {
                    ActualizarRadarDesempeno(_usuarioSeleccionado);
                }
            }
        }

        private string _radarEmpleadoLabel;
        public string RadarEmpleadoLabel { get => _radarEmpleadoLabel; set => SetProperty(ref _radarEmpleadoLabel, value); }

        public ObservableCollection<RadarModel> DatosRadarEmpleado { get; } = new ObservableCollection<RadarModel>();
        public ObservableCollection<RadarModel> DatosRadarPromedio { get; } = new ObservableCollection<RadarModel>();


        // Comandos
        public ICommand RegresarCommand { get; }
        public ICommand MostrarDetalleCommand { get; }
        public ICommand CerrarDetalleCommand { get; }

        public VistaEstadisticasViewModel()
        {
            RegresarCommand = new RelayCommand(ExecuteRegresar);
            MostrarDetalleCommand = new RelayCommand(ExecuteMostrarDetalle);
            CerrarDetalleCommand = new RelayCommand(ExecuteCerrarDetalle);

            CargarFiltroSAPs();
            _isLoaded = true;
            ActualizarDashboard();
        }

        private void CargarFiltroSAPs()
        {
            ListaSaps.Clear();
            string estrato = SesionGlobal.Estrato?.ToUpper() ?? "";

            if (estrato == "GERENTE" || estrato == "JEFE" || estrato == "ADMINISTRADOR")
            {
                ListaSaps.Add("TODOS");
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        if (conn == null) return;
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = "SELECT DISTINCT Descripcion_Sap FROM Cat_SAP WHERE Descripcion_Sap IS NOT NULL";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read()) ListaSaps.Add(reader[0].ToString());
                        }
                    }
                }
                catch (Exception) { }
                
                _sapSeleccionado = "TODOS";
            }
            else
            {
                // Usuario de captura, solo ve su propio rendimiento
                ListaSaps.Add("MIS ASUNTOS");
                _sapSeleccionado = "MIS ASUNTOS";
            }

            OnPropertyChanged(nameof(SapSeleccionado));
        }

        private void ActualizarDashboard()
        {
            if (!_isLoaded) return;
            if (string.IsNullOrEmpty(SapSeleccionado)) return;

            CargarDatosEstadisticos(SapSeleccionado, FechaInicio, FechaFin);
            CargarEstadisticasPersonal();
        }

        private void CargarDatosEstadisticos(string filtroSap, DateTime inicio, DateTime fin)
        {
            _asuntosActuales.Clear();
            DatosRendimientoUsuarios.Clear();

            int total = 0, completados = 0, proceso = 0, vencidos = 0;
            int sumaAvance = 0;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    var cmd1 = conn.CreateCommand();
                    string queryAsuntos = @"
                        SELECT a.Id_asunto, dc.tipo_descripcion, a.Porcentaje_avance, a.Fecha_Compromiso, 
                               cs.Descripcion_Sap, a.Fecha_recepcion
                        FROM Asuntos a
                        LEFT JOIN Cat_SAP cs ON a.id_sap = cs.Id_SAP
                        LEFT JOIN Descripcion_corta dc ON a.id_descripcionCorta = dc.id_descripcionCorta 
                        WHERE a.Eliminado = 1 AND a.Fecha_recepcion >= @inicio AND a.Fecha_recepcion <= @fin ";

                    if (filtroSap == "MIS ASUNTOS")
                    {
                        queryAsuntos += " AND a.Ficha = @miFicha";
                        cmd1.Parameters.AddWithValue("@miFicha", SesionGlobal.Ficha ?? "");
                    }
                    else if (filtroSap != "TODOS")
                    {
                        queryAsuntos += " AND cs.Descripcion_Sap = @sapFiltro";
                        cmd1.Parameters.AddWithValue("@sapFiltro", filtroSap);
                    }

                    cmd1.CommandText = queryAsuntos;
                    cmd1.Parameters.AddWithValue("@inicio", inicio.ToString("yyyy-MM-dd"));
                    cmd1.Parameters.AddWithValue("@fin", fin.ToString("yyyy-MM-dd"));

                    using (var reader = cmd1.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            total++;
                            int id = Convert.ToInt32(reader[0]);

                            string descripcion = reader.IsDBNull(1) ? "Sin descripción" : reader.GetValue(1).ToString();
                            if (string.IsNullOrWhiteSpace(descripcion)) descripcion = "Sin descripción";

                            int avance = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader[2]);
                            string fechaComp = reader.IsDBNull(3) ? "" : reader[3].ToString();
                            string sap = reader.IsDBNull(4) ? "SIN SAP" : reader[4].ToString();
                            string fechaRecStr = reader.IsDBNull(5) ? "" : reader[5].ToString();

                            sumaAvance += avance;
                            bool estaVencido = false;

                            if (avance >= 100) completados++;
                            else
                            {
                                proceso++;
                                if (DateTime.TryParse(fechaComp, out DateTime f) && (f.Date - DateTime.Now.Date).TotalDays < 0)
                                {
                                    vencidos++;
                                    estaVencido = true;
                                }
                            }

                            _asuntosActuales.Add(new DetalleAsuntoModel
                            {
                                ID = id,
                                Descripcion = descripcion,
                                SAP = sap,
                                Avance = avance + "%",
                                Vencido = estaVencido ? "SÍ" : "NO",
                                FechaRegistro = string.IsNullOrEmpty(fechaRecStr) ? "Sin Fecha" : fechaRecStr
                            });
                        }
                    }

                    var cmdUsuarios = conn.CreateCommand();
                    string queryRendimiento = @"
                        SELECT 
                            u.nombre AS NombreUsuario,
                            SUM(CASE WHEN a.Porcentaje_avance >= 100 THEN 1 ELSE 0 END) AS Completados,
                            SUM(CASE WHEN a.Porcentaje_avance < 100 AND (a.Fecha_Compromiso >= TO_CHAR(CURRENT_DATE, 'YYYY-MM-DD') OR a.Fecha_Compromiso IS NULL OR a.Fecha_Compromiso = '') THEN 1 ELSE 0 END) AS EnProceso,
                            SUM(CASE WHEN a.Porcentaje_avance < 100 AND a.Fecha_Compromiso < TO_CHAR(CURRENT_DATE, 'YYYY-MM-DD') AND a.Fecha_Compromiso != '' THEN 1 ELSE 0 END) AS Vencidos
                        FROM Asuntos a
                        INNER JOIN usuario u ON a.Ficha = u.Ficha 
                        LEFT JOIN Cat_SAP cs ON a.id_sap = cs.Id_SAP
                        WHERE a.Eliminado = 1 ";

                    if (filtroSap == "MIS ASUNTOS")
                    {
                        queryRendimiento += " AND a.Ficha = @miFicha ";
                        cmdUsuarios.Parameters.AddWithValue("@miFicha", SesionGlobal.Ficha ?? "");
                    }
                    else if (filtroSap != "TODOS")
                    {
                        queryRendimiento += " AND cs.Descripcion_Sap = @sapFiltro ";
                        cmdUsuarios.Parameters.AddWithValue("@sapFiltro", filtroSap);
                    }

                    queryRendimiento += " GROUP BY u.nombre ORDER BY u.nombre ASC";
                    cmdUsuarios.CommandText = queryRendimiento;

                    using (var reader = cmdUsuarios.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DatosRendimientoUsuarios.Add(new UsuarioEstadisticaModel
                            {
                                NombreUsuario = reader["NombreUsuario"].ToString(),
                                Completados = Convert.ToInt32(reader["Completados"]),
                                EnProceso = Convert.ToInt32(reader["EnProceso"]),
                                Vencidos = Convert.ToInt32(reader["Vencidos"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error DB: " + ex.Message);
            }

            TotalAsuntos = total;
            TotalCompletados = completados;
            TotalProceso = proceso;
            TotalVencidos = vencidos;

            double promedioAvance = total > 0 ? (double)sumaAvance / total : 0;
            PromedioAvance = promedioAvance;
            PromedioAvanceTexto = Math.Round(promedioAvance, 1) + "%";

            DatosDona.Clear();
            var gruposSap = _asuntosActuales.GroupBy(a => a.SAP);

            foreach (var grupo in gruposSap)
            {
                int cantidadSap = grupo.Count();
                var asuntoUrgente = grupo.FirstOrDefault(a => a.Vencido == "SÍ") ?? grupo.FirstOrDefault();
                string descUrgente = asuntoUrgente != null ? $"Folio {asuntoUrgente.ID} - {asuntoUrgente.Descripcion}" : "Ninguno";

                int completadosSap = grupo.Count(a => a.Avance == "100%");
                double eficienciaSap = cantidadSap > 0 ? (completadosSap * 100.0) / cantidadSap : 0;

                DatosDona.Add(new GraficaDonaModel
                {
                    Nombre = grupo.Key,
                    Cantidad = cantidadSap,
                    AsuntoMasUrgente = descUrgente,
                    EficienciaComparativa = $"Resolución: {Math.Round(eficienciaSap, 1)}%"
                });
            }
        }

        private void CargarEstadisticasPersonal()
        {
            _listaRankingGlobal.Clear();
            ListaRankingGlobal.Clear();
            int totalUsuarios = 0;
            int totalSesiones = 0;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    using (var cmd = conn.CreateCommand())
                    {
                        string queryRanking = @"
                            SELECT u.Ficha, u.nombre, u.tipo, u.contador, 
                                   COUNT(a.Id_asunto) as AsuntosAsignados
                            FROM usuario u
                            LEFT JOIN Asuntos a ON u.Ficha = a.Ficha AND a.Eliminado = 1
                            WHERE u.estatus = 1";

                        string estrato = SesionGlobal.Estrato?.ToUpper() ?? "";
                        if (estrato != "GERENTE" && estrato != "JEFE" && estrato != "ADMINISTRADOR")
                        {
                            queryRanking += " AND u.Ficha = @miFicha ";
                            cmd.Parameters.AddWithValue("@miFicha", SesionGlobal.Ficha ?? "");
                        }

                        queryRanking += " GROUP BY u.Ficha, u.nombre, u.tipo, u.contador ORDER BY u.contador DESC";
                        cmd.CommandText = queryRanking;

                        using (var reader = cmd.ExecuteReader())
                        {
                            int posicion = 1;
                            while (reader.Read())
                            {
                                int sesiones = reader["contador"] != DBNull.Value ? Convert.ToInt32(reader["contador"]) : 0;
                                int asuntos = reader["AsuntosAsignados"] != DBNull.Value ? Convert.ToInt32(reader["AsuntosAsignados"]) : 0;

                                var usuarioRanking = new UsuarioRanking
                                {
                                    Posicion = posicion.ToString(),
                                    Ficha = reader["Ficha"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Tipo = reader["tipo"].ToString(),
                                    ContadorSesiones = sesiones,
                                    AsuntosGestionados = asuntos
                                };
                                
                                _listaRankingGlobal.Add(usuarioRanking);
                                ListaRankingGlobal.Add(usuarioRanking);

                                totalUsuarios++;
                                totalSesiones += sesiones;
                                posicion++;
                            }
                        }
                    }
                }

                TotalUsuarios = totalUsuarios;
                TotalSesiones = totalSesiones;

                if (totalUsuarios > 0)
                {
                    double promedio = (double)totalSesiones / totalUsuarios;
                    PromedioSesiones = Math.Round(promedio, 1).ToString();
                    var topUser = _listaRankingGlobal.First();
                    TopUsuario = $"{topUser.Nombre} ({topUser.ContadorSesiones})";

                    UsuarioSeleccionado = ListaRankingGlobal.FirstOrDefault();
                }
                else
                {
                    PromedioSesiones = "0";
                    TopUsuario = "Sin datos";
                    DatosRadarEmpleado.Clear();
                    DatosRadarPromedio.Clear();
                    RadarEmpleadoLabel = "Sin selección";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el ranking del personal: {ex.Message}", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ActualizarRadarDesempeno(UsuarioRanking usuario)
        {
            if (usuario == null || _listaRankingGlobal.Count == 0) return;

            double promedioSesiones = _listaRankingGlobal.Average(u => u.ContadorSesiones);
            double promedioAsuntos = _listaRankingGlobal.Average(u => u.AsuntosGestionados);

            double maxSesiones = _listaRankingGlobal.Max(u => u.ContadorSesiones) == 0 ? 1 : _listaRankingGlobal.Max(u => u.ContadorSesiones);
            double maxAsuntos = _listaRankingGlobal.Max(u => u.AsuntosGestionados) == 0 ? 1 : _listaRankingGlobal.Max(u => u.AsuntosGestionados);

            DatosRadarPromedio.Clear();
            DatosRadarPromedio.Add(new RadarModel { Metrica = "Uso de Sistema", Valor = (promedioSesiones / maxSesiones) * 100 });
            DatosRadarPromedio.Add(new RadarModel { Metrica = "Carga Operativa", Valor = (promedioAsuntos / maxAsuntos) * 100 });
            DatosRadarPromedio.Add(new RadarModel { Metrica = "Eficiencia", Valor = 50 });
            DatosRadarPromedio.Add(new RadarModel { Metrica = "Constancia", Valor = 50 });

            DatosRadarEmpleado.Clear();
            DatosRadarEmpleado.Add(new RadarModel { Metrica = "Uso de Sistema", Valor = (usuario.ContadorSesiones / maxSesiones) * 100 });
            DatosRadarEmpleado.Add(new RadarModel { Metrica = "Carga Operativa", Valor = (usuario.AsuntosGestionados / maxAsuntos) * 100 });
            DatosRadarEmpleado.Add(new RadarModel { Metrica = "Eficiencia", Valor = usuario.AsuntosGestionados > 0 ? 75 : 0 });
            DatosRadarEmpleado.Add(new RadarModel { Metrica = "Constancia", Valor = usuario.ContadorSesiones > 0 ? 80 : 0 });

            RadarEmpleadoLabel = usuario.Nombre;
        }

        private void ExecuteRegresar(object parameter)
        {
            CerrarVentanaAction?.Invoke();
        }

        private void ExecuteMostrarDetalle(object parameter)
        {
            if (parameter is string tipo)
            {
                IEnumerable<DetalleAsuntoModel> datosFiltrados = _asuntosActuales;
                switch (tipo)
                {
                    case "TOTAL":
                        TituloDetalle = "Detalle: Total de Asuntos";
                        break;
                    case "COMPLETADOS":
                        TituloDetalle = "Detalle: Asuntos Completados";
                        datosFiltrados = _asuntosActuales.Where(a => a.Avance == "100%");
                        break;
                    case "PROCESO":
                        TituloDetalle = "Detalle: Asuntos en Proceso";
                        datosFiltrados = _asuntosActuales.Where(a => a.Avance != "100%");
                        break;
                    case "VENCIDOS":
                        TituloDetalle = "Detalle: Asuntos Vencidos (Requieren atención)";
                        datosFiltrados = _asuntosActuales.Where(a => a.Vencido == "SÍ");
                        break;
                }

                AsuntosDetalle.Clear();
                foreach (var d in datosFiltrados)
                {
                    AsuntosDetalle.Add(d);
                }

                MostrarDetalleAnimationAction?.Invoke();
            }
        }

        private void ExecuteCerrarDetalle(object parameter)
        {
            OcultarDetalleAnimationAction?.Invoke();
        }
    }

    // Modelos
    public class GraficaDonaModel
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public string AsuntoMasUrgente { get; set; }
        public string EficienciaComparativa { get; set; }
    }

    public class UsuarioEstadisticaModel
    {
        public string NombreUsuario { get; set; }
        public int Completados { get; set; }
        public int EnProceso { get; set; }
        public int Vencidos { get; set; }
    }

    public class DetalleAsuntoModel
    {
        public int ID { get; set; }
        public string SAP { get; set; }
        public string Descripcion { get; set; }
        public string FechaRegistro { get; set; }
        public string Avance { get; set; }
        public string Vencido { get; set; }
    }

    public class UsuarioRanking
    {
        public string Posicion { get; set; }
        public string Ficha { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public int ContadorSesiones { get; set; }
        public int AsuntosGestionados { get; set; }
    }

    public class RadarModel
    {
        public string Metrica { get; set; }
        public double Valor { get; set; }
    }
}
