using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Npgsql;
using Proyecto_GRRLN_expediente.ViewModels.Base;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VistaConsultaGeneralViewModel : ViewModelBase, IDisposable
    {
        private DispatcherTimer _timerRefresco;
        public Action CerrarVentanaAction { get; set; }
        public Action<int> AbrirEdicionAction { get; set; }
        public Action<int, int> AbrirAvanceAction { get; set; }

        private bool _isEdicionPermitida;
        public bool IsEdicionPermitida { get => _isEdicionPermitida; set => SetProperty(ref _isEdicionPermitida, value); }

        private string _textoBusqueda = "";
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                {
                    CargarDatos();
                }
            }
        }

        public ObservableCollection<AsuntoGridModel> ListaAsuntos { get; } = new ObservableCollection<AsuntoGridModel>();

        public ICommand RegresarCommand { get; }
        public ICommand EditarExpedienteCommand { get; }
        public ICommand ModificarAvanceCommand { get; }

        public VistaConsultaGeneralViewModel()
        {
            RegresarCommand = new RelayCommand(ExecuteRegresar);
            EditarExpedienteCommand = new RelayCommand(ExecuteEditarExpediente, _ => IsEdicionPermitida);
            ModificarAvanceCommand = new RelayCommand(ExecuteModificarAvance, _ => IsEdicionPermitida);

            string estrato = SesionGlobal.Estrato?.ToUpper() ?? "";
            IsEdicionPermitida = estrato != "GERENTE";

            CargarDatos();
            IniciarTemporizador();
        }

        private void IniciarTemporizador()
        {
            _timerRefresco = new DispatcherTimer();
            _timerRefresco.Interval = TimeSpan.FromMinutes(2);
            _timerRefresco.Tick += (s, e) => {
                if (string.IsNullOrWhiteSpace(TextoBusqueda)) 
                {
                    CargarDatos();
                }
            };
            _timerRefresco.Start();
        }

        public void Dispose()
        {
            _timerRefresco?.Stop();
        }

        public void CargarDatos()
        {
            ListaAsuntos.Clear();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

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
                            WHERE a.Eliminado = 1 ";

                        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
                        {
                            query += @" AND (
                                        a.Nombre_oficio LIKE @filtro OR 
                                        CAST(a.id_asunto AS TEXT) LIKE @filtro OR
                                        c.tipo_descripcion LIKE @filtro OR
                                        cs.Descripcion_Sap LIKE @filtro OR
                                        dp.descripcion LIKE @filtro
                                       )";
                            command.Parameters.AddWithValue("@filtro", "%" + TextoBusqueda.Trim() + "%");
                        }

                        query += " ORDER BY a.id_asunto DESC";
                        command.CommandText = query;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaAsuntos.Add(new AsuntoGridModel
                                {
                                    Id = Convert.ToInt32(reader[0]),
                                    Asunto = reader.IsDBNull(1) ? "SIN NOMBRE" : reader[1].ToString(),
                                    FechaLimite = reader.IsDBNull(2) ? "" : reader[2].ToString(),
                                    Avance = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader[3]),
                                    DescripcionCortaTexto = reader.IsDBNull(4) ? "Sin descripción" : reader[4].ToString(),
                                    GerenciaTexto = reader.IsDBNull(5) ? "Sin gerencia" : reader[5].ToString(),
                                    Sap = reader.IsDBNull(6) ? "Sin SAP" : reader[6].ToString(),
                                    Departamento = reader.IsDBNull(7) ? "Sin Depto" : reader[7].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer la base de datos: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteRegresar(object parameter)
        {
            CerrarVentanaAction?.Invoke();
        }

        private void ExecuteEditarExpediente(object parameter)
        {
            if (parameter is int id)
            {
                AbrirEdicionAction?.Invoke(id);
            }
        }

        private void ExecuteModificarAvance(object parameter)
        {
            if (parameter is AsuntoGridModel asunto)
            {
                AbrirAvanceAction?.Invoke(asunto.Id, asunto.Avance);
            }
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
