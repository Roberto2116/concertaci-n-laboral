using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Globalization;
using Wpf.Ui.Controls;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

// SOLUCIÓN DE AMBIGÜEDAD
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaEstadisticas : FluentWindow
    {
        private bool _isLoaded = false;
        private List<DetalleAsuntoModel> _asuntosActuales = new List<DetalleAsuntoModel>();
        private List<UsuarioRanking> _listaRankingGlobal = new List<UsuarioRanking>();

        public VistaEstadisticas()
        {
            InitializeComponent();

            DpInicio.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DpFin.SelectedDate = DateTime.Now.Date;

            CargarFiltroSAPs();
            _isLoaded = true;
            ActualizarDashboard();
        }

        private void CargarFiltroSAPs()
        {
            List<string> listaSaps = new List<string> { "TODOS" };
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT DISTINCT Descripcion_Sap FROM Cat_SAP WHERE Descripcion_Sap IS NOT NULL";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) listaSaps.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception) { }
            finally { DatabaseConnection.CloseConnection(); }

            CmbFiltroSap.ItemsSource = listaSaps;
            CmbFiltroSap.SelectedIndex = 0;
        }

        private void Filtros_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            ActualizarDashboard();
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded || e.OriginalSource != MainTabControl) return;

            if (MainTabControl.SelectedIndex == 0)
            {
                // Pestaña 1 (Asuntos): Muestra los filtros
                if (PanelFiltros != null) PanelFiltros.Visibility = Visibility.Visible;
            }
            else if (MainTabControl.SelectedIndex == 1)
            {
                // Pestaña 2 (Personal): Oculta los filtros porque ya no hay gráfica de tiempo
                if (PanelFiltros != null) PanelFiltros.Visibility = Visibility.Collapsed;
            }
        }

        private void ActualizarDashboard()
        {
            if (CmbFiltroSap.SelectedItem == null || !DpInicio.SelectedDate.HasValue || !DpFin.SelectedDate.HasValue) return;

            string sapSeleccionado = CmbFiltroSap.SelectedItem.ToString();
            DateTime fechaInicio = DpInicio.SelectedDate.Value.Date;
            DateTime fechaFin = DpFin.SelectedDate.Value.Date.AddDays(1).AddTicks(-1);

            CargarDatosEstadisticos(sapSeleccionado, fechaInicio, fechaFin);
            CargarEstadisticasPersonal();
        }

        // ==========================================================
        // LÓGICA: PESTAÑA 2 - RANKING DE PERSONAL
        // ==========================================================
        private void CargarEstadisticasPersonal()
        {
            _listaRankingGlobal.Clear();
            int totalUsuarios = 0;
            int totalSesiones = 0;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT u.Ficha, u.nombre, u.tipo, u.contador, 
                                   COUNT(a.Id_asunto) as AsuntosAsignados
                            FROM usuario u
                            LEFT JOIN Asuntos a ON u.Ficha = a.Ficha
                            WHERE u.estatus = 1
                            GROUP BY u.Ficha, u.nombre, u.tipo, u.contador
                            ORDER BY u.contador DESC";

                        using (var reader = cmd.ExecuteReader())
                        {
                            int posicion = 1;
                            while (reader.Read())
                            {
                                int sesiones = reader["contador"] != DBNull.Value ? Convert.ToInt32(reader["contador"]) : 0;
                                int asuntos = reader["AsuntosAsignados"] != DBNull.Value ? Convert.ToInt32(reader["AsuntosAsignados"]) : 0;

                                _listaRankingGlobal.Add(new UsuarioRanking
                                {
                                    Posicion = posicion.ToString(),
                                    Ficha = reader["Ficha"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Tipo = reader["tipo"].ToString(),
                                    ContadorSesiones = sesiones,
                                    AsuntosGestionados = asuntos
                                });

                                totalUsuarios++;
                                totalSesiones += sesiones;
                                posicion++;
                            }
                        }
                    }
                }

                GridRankingPersonal.ItemsSource = _listaRankingGlobal;

                LblTotalUsuarios.Text = totalUsuarios.ToString();
                LblTotalSesiones.Text = totalSesiones.ToString();

                if (totalUsuarios > 0)
                {
                    double promedio = (double)totalSesiones / totalUsuarios;
                    LblPromedioSesiones.Text = Math.Round(promedio, 1).ToString();
                    var topUser = _listaRankingGlobal.First();
                    LblTopUsuario.Text = $"{topUser.Nombre} ({topUser.ContadorSesiones})";

                    GridRankingPersonal.SelectedIndex = 0;
                }
                else
                {
                    LblPromedioSesiones.Text = "0";
                    LblTopUsuario.Text = "Sin datos";
                    RadarEmpleado.ItemsSource = null;
                    RadarPromedio.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el ranking del personal: {ex.Message}", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        // DIBUJAR RADAR AL SELECCIONAR EMPLEADO
        private void GridRankingPersonal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridRankingPersonal.SelectedItem is UsuarioRanking usuarioSeleccionado)
            {
                ActualizarRadarDesempeno(usuarioSeleccionado);
            }
        }

        // LÓGICA PARA EL RADAR DE DESEMPEÑO
        private void ActualizarRadarDesempeno(UsuarioRanking usuario)
        {
            if (_listaRankingGlobal.Count == 0) return;

            double promedioSesiones = _listaRankingGlobal.Average(u => u.ContadorSesiones);
            double promedioAsuntos = _listaRankingGlobal.Average(u => u.AsuntosGestionados);

            double maxSesiones = _listaRankingGlobal.Max(u => u.ContadorSesiones) == 0 ? 1 : _listaRankingGlobal.Max(u => u.ContadorSesiones);
            double maxAsuntos = _listaRankingGlobal.Max(u => u.AsuntosGestionados) == 0 ? 1 : _listaRankingGlobal.Max(u => u.AsuntosGestionados);

            var datosPromedio = new List<RadarModel>
            {
                new RadarModel { Metrica = "Uso de Sistema", Valor = (promedioSesiones / maxSesiones) * 100 },
                new RadarModel { Metrica = "Carga Operativa", Valor = (promedioAsuntos / maxAsuntos) * 100 },
                new RadarModel { Metrica = "Eficiencia", Valor = 50 },
                new RadarModel { Metrica = "Constancia", Valor = 50 }
            };

            var datosEmpleado = new List<RadarModel>
            {
                new RadarModel { Metrica = "Uso de Sistema", Valor = (usuario.ContadorSesiones / maxSesiones) * 100 },
                new RadarModel { Metrica = "Carga Operativa", Valor = (usuario.AsuntosGestionados / maxAsuntos) * 100 },
                new RadarModel { Metrica = "Eficiencia", Valor = usuario.AsuntosGestionados > 0 ? 75 : 0 },
                new RadarModel { Metrica = "Constancia", Valor = usuario.ContadorSesiones > 0 ? 80 : 0 }
            };

            RadarPromedio.ItemsSource = datosPromedio;
            RadarEmpleado.ItemsSource = datosEmpleado;
            RadarEmpleado.Label = usuario.Nombre;
        }

        // ==========================================================
        // LÓGICA: PESTAÑA 1 - ESTADÍSTICAS DE ASUNTOS (INTACTA)
        // ==========================================================
        private void CargarDatosEstadisticos(string filtroSap, DateTime inicio, DateTime fin)
        {
            _asuntosActuales.Clear();
            int total = 0, completados = 0, proceso = 0, vencidos = 0;
            int sumaAvance = 0;

            var tendenciaDiaria = new Dictionary<string, (int Nuevos, int Atendidos)>();

            for (DateTime d = inicio.Date; d <= fin.Date; d = d.AddDays(1))
            {
                tendenciaDiaria[d.ToString("yyyy-MM-dd")] = (0, 0);
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    var cmd1 = conn.CreateCommand();
                    string queryAsuntos = @"
                        SELECT a.Id_asunto, dc.tipo_descripcion, a.Porcentaje_avance, a.Fecha_Compromiso, 
                               cs.Descripcion_Sap, a.Fecha_recepcion
                        FROM Asuntos a
                        LEFT JOIN Cat_SAP cs ON a.id_sap = cs.Id_SAP
                        LEFT JOIN Descripcion_corta dc ON a.id_descripcionCorta = dc.id_descripcionCorta ";

                    if (filtroSap != "TODOS")
                    {
                        queryAsuntos += "WHERE cs.Descripcion_Sap = $sapFiltro";
                        cmd1.Parameters.AddWithValue("$sapFiltro", filtroSap);
                    }
                    cmd1.CommandText = queryAsuntos;

                    using (var reader = cmd1.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            total++;
                            int id = reader.GetInt32(0);

                            string descripcion = reader.IsDBNull(1) ? "Sin descripción" : reader.GetValue(1).ToString();
                            if (string.IsNullOrWhiteSpace(descripcion)) descripcion = "Sin descripción";

                            int avance = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                            string fechaComp = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            string sap = reader.IsDBNull(4) ? "SIN SAP" : reader.GetString(4);
                            string fechaRecStr = reader.IsDBNull(5) ? "" : reader.GetString(5);

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

                            if (DateTime.TryParse(fechaRecStr, out DateTime fechaRec))
                            {
                                if (fechaRec.Date >= inicio.Date && fechaRec.Date <= fin.Date)
                                {
                                    string diaKey = fechaRec.ToString("yyyy-MM-dd");
                                    if (tendenciaDiaria.ContainsKey(diaKey))
                                    {
                                        var datosDia = tendenciaDiaria[diaKey];
                                        tendenciaDiaria[diaKey] = (datosDia.Nuevos + 1, datosDia.Atendidos);
                                    }
                                }
                            }
                        }
                    }

                    var cmd2 = conn.CreateCommand();
                    string querySeguimiento = @"
                        SELECT s.fecha_Seguimiento 
                        FROM Seguimiento s
                        INNER JOIN Asuntos a ON s.num_Asunto = a.Id_asunto
                        LEFT JOIN Cat_SAP cs ON a.id_sap = cs.Id_SAP ";

                    if (filtroSap != "TODOS")
                    {
                        querySeguimiento += "WHERE cs.Descripcion_Sap = $sapFiltro";
                        cmd2.Parameters.AddWithValue("$sapFiltro", filtroSap);
                    }
                    cmd2.CommandText = querySeguimiento;

                    using (var reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fechaSeg = reader.IsDBNull(0) ? "" : reader.GetString(0);
                            if (fechaSeg.Length >= 10) fechaSeg = fechaSeg.Substring(0, 10);

                            if (DateTime.TryParse(fechaSeg, out DateTime fs))
                            {
                                if (fs.Date >= inicio.Date && fs.Date <= fin.Date)
                                {
                                    string diaKey = fs.ToString("yyyy-MM-dd");
                                    if (tendenciaDiaria.ContainsKey(diaKey))
                                    {
                                        var datosDia = tendenciaDiaria[diaKey];
                                        tendenciaDiaria[diaKey] = (datosDia.Nuevos, datosDia.Atendidos + 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error DB: " + ex.Message);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }

            TxtTotal.Text = total.ToString();
            TxtCompletados.Text = completados.ToString();
            TxtProceso.Text = proceso.ToString();
            TxtVencidos.Text = vencidos.ToString();

            double promedioAvance = total > 0 ? (double)sumaAvance / total : 0;
            PunteroAvance.Value = promedioAvance;
            TxtPromedioGauge.Text = Math.Round(promedioAvance, 1) + "%";

            var listaDona = new List<GraficaDonaModel>();
            var gruposSap = _asuntosActuales.GroupBy(a => a.SAP);

            foreach (var grupo in gruposSap)
            {
                int cantidadSap = grupo.Count();
                var asuntoUrgente = grupo.FirstOrDefault(a => a.Vencido == "SÍ") ?? grupo.FirstOrDefault();
                string descUrgente = asuntoUrgente != null ? $"Folio {asuntoUrgente.ID} - {asuntoUrgente.Descripcion}" : "Ninguno";

                int completadosSap = grupo.Count(a => a.Avance == "100%");
                double eficienciaSap = cantidadSap > 0 ? (completadosSap * 100.0) / cantidadSap : 0;

                listaDona.Add(new GraficaDonaModel
                {
                    Nombre = grupo.Key,
                    Cantidad = cantidadSap,
                    AsuntoMasUrgente = descUrgente,
                    EficienciaComparativa = $"Resolución: {Math.Round(eficienciaSap, 1)}%"
                });
            }
            DonaSap.ItemsSource = listaDona;

            var tendenciaNuevos = new List<TrendModel>();
            var tendenciaAtendidos = new List<TrendModel>();
            CultureInfo culturaEsp = new CultureInfo("es-ES");

            foreach (var dia in tendenciaDiaria.OrderBy(d => d.Key))
            {
                if (DateTime.TryParse(dia.Key, out DateTime fc))
                {
                    string etiqueta = fc.ToString("dd MMM", culturaEsp);
                    tendenciaNuevos.Add(new TrendModel(etiqueta, dia.Value.Nuevos));
                    tendenciaAtendidos.Add(new TrendModel(etiqueta, dia.Value.Atendidos));
                }
            }
            LineNuevos.ItemsSource = tendenciaNuevos;
            LineCerrados.ItemsSource = tendenciaAtendidos;
        }

        // ==========================================
        // LÓGICA DE INTERACTIVIDAD
        // ==========================================
        private void MostrarDetalles(string titulo, IEnumerable<DetalleAsuntoModel> datos)
        {
            TxtTituloDetalle.Text = titulo;
            GridDatosKpi.ItemsSource = datos.ToList();

            PanelGraficas.Visibility = Visibility.Collapsed;
            PanelDetalles.Visibility = Visibility.Visible;

            Storyboard sbMostrar = (Storyboard)this.FindResource("ShowPanelDetailsStoryboard");
            sbMostrar.Begin(this);
        }

        private void BtnCerrarDetalles_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sbOcultar = (Storyboard)this.FindResource("HidePanelDetailsStoryboard");
            EventHandler handler = null;
            handler = (s, ev) =>
            {
                PanelDetalles.Visibility = Visibility.Collapsed;
                PanelGraficas.Visibility = Visibility.Visible;
                sbOcultar.Completed -= handler;
            };
            sbOcultar.Completed += handler;
            sbOcultar.Begin(this);
        }

        private void CardTotal_Click(object sender, MouseButtonEventArgs e) =>
            MostrarDetalles("Detalle: Total de Asuntos", _asuntosActuales);

        private void CardCompletados_Click(object sender, MouseButtonEventArgs e) =>
            MostrarDetalles("Detalle: Asuntos Completados", _asuntosActuales.Where(a => a.Avance == "100%"));

        private void CardProceso_Click(object sender, MouseButtonEventArgs e) =>
            MostrarDetalles("Detalle: Asuntos en Proceso", _asuntosActuales.Where(a => a.Avance != "100%"));

        private void CardVencidos_Click(object sender, MouseButtonEventArgs e) =>
            MostrarDetalles("Detalle: Asuntos Vencidos (Requieren atención)", _asuntosActuales.Where(a => a.Vencido == "SÍ"));

        private void BtnRegresar_Click(object sender, RoutedEventArgs e) => this.Close();
    }

    // ==========================================
    // MODELOS DE DATOS 
    // ==========================================
    public class GraficaDonaModel
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public string AsuntoMasUrgente { get; set; }
        public string EficienciaComparativa { get; set; }
    }

    public class TrendModel
    {
        public string Mes { get; set; }
        public double Valor { get; set; }
        public TrendModel(string m, double v) { Mes = m; Valor = v; }
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