using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.ViewModels.Base;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VistaConsultasViewModel : ViewModelBase
    {
        public Action CerrarVentanaAction { get; set; }
        public Action<int> AbrirEdicionAction { get; set; }

        private string _textoBusqueda = "";
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set => SetProperty(ref _textoBusqueda, value);
        }

        private int _estadoSeleccionado = -1;
        public int EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set => SetProperty(ref _estadoSeleccionado, value);
        }

        private int _sedeSeleccionada = -1;
        public int SedeSeleccionada
        {
            get => _sedeSeleccionada;
            set => SetProperty(ref _sedeSeleccionada, value);
        }

        private ResultadoConsultaModel _asuntoSeleccionado;
        public ResultadoConsultaModel AsuntoSeleccionado
        {
            get => _asuntoSeleccionado;
            set => SetProperty(ref _asuntoSeleccionado, value);
        }

        public ObservableCollection<ItemFiltro> ListaEstatus { get; } = new ObservableCollection<ItemFiltro>();
        public ObservableCollection<ItemFiltro> ListaSedes { get; } = new ObservableCollection<ItemFiltro>();
        public ObservableCollection<ResultadoConsultaModel> ListaResultados { get; } = new ObservableCollection<ResultadoConsultaModel>();

        public ICommand RegresarCommand { get; }
        public ICommand BuscarCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand AbrirExpedienteCommand { get; }

        public VistaConsultasViewModel()
        {
            RegresarCommand = new RelayCommand(ExecuteRegresar);
            BuscarCommand = new RelayCommand(ExecuteBuscar);
            LimpiarCommand = new RelayCommand(ExecuteLimpiar);
            AbrirExpedienteCommand = new RelayCommand(ExecuteAbrirExpediente);

            CargarCatalogosFiltros();
        }

        private void CargarCatalogosFiltros()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    // 1. Cargar Estatus
                    ListaEstatus.Clear();
                    ListaEstatus.Add(new ItemFiltro { Id = -1, Descripcion = "TODOS LOS ESTADOS" });
                    
                    var cmdE = conn.CreateCommand();
                    cmdE.CommandText = "SELECT id_estatus, tipo_estatus FROM Estatus WHERE tipo_estatus != 'NO PROCEDE'";
                    using (var reader = cmdE.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListaEstatus.Add(new ItemFiltro { Id = reader.GetInt32(0), Descripcion = reader.GetString(1) });
                        }
                    }
                    EstadoSeleccionado = -1;

                    // 2. Cargar Sedes (SAP)
                    ListaSedes.Clear();
                    ListaSedes.Add(new ItemFiltro { Id = -1, Descripcion = "TODAS LAS SEDES" });
                    
                    var cmdS = conn.CreateCommand();
                    cmdS.CommandText = "SELECT Id_SAP, Descripcion_Sap FROM Cat_SAP";
                    using (var reader = cmdS.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListaSedes.Add(new ItemFiltro { Id = reader.GetInt32(0), Descripcion = reader.GetString(1) });
                        }
                    }
                    SedeSeleccionada = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar filtros: {ex.Message}");
            }
        }

        private void ExecuteBuscar(object parameter)
        {
            ListaResultados.Clear();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    var command = conn.CreateCommand();
                    string sql = @"
                        SELECT a.Id_asunto, a.Nombre_oficio, dc.tipo_descripcion, s.Descripcion_Sap, e.tipo_estatus, a.Fecha_Compromiso
                        FROM Asuntos a
                        LEFT JOIN Cat_SAP s ON a.id_sap = s.Id_SAP
                        LEFT JOIN Estatus e ON a.Id_estatus = e.id_estatus
                        LEFT JOIN Descripcion_corta dc ON a.id_descripcionCorta = dc.id_descripcionCorta
                        WHERE 1=1";

                    if (!string.IsNullOrWhiteSpace(TextoBusqueda))
                    {
                        sql += " AND (a.Nombre_oficio LIKE $texto OR dc.tipo_descripcion LIKE $texto OR CAST(a.Id_asunto AS TEXT) LIKE $texto)";
                        command.Parameters.AddWithValue("$texto", "%" + TextoBusqueda.Trim() + "%");
                    }

                    if (EstadoSeleccionado != -1)
                    {
                        sql += " AND a.id_estatus = $idEstado";
                        command.Parameters.AddWithValue("$idEstado", EstadoSeleccionado);
                    }

                    if (SedeSeleccionada != -1)
                    {
                        sql += " AND a.id_sap = $idSede";
                        command.Parameters.AddWithValue("$idSede", SedeSeleccionada);
                    }

                    command.CommandText = sql + " ORDER BY a.Id_asunto DESC";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListaResultados.Add(new ResultadoConsultaModel
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en la búsqueda: {ex.Message}");
            }
        }

        private void ExecuteLimpiar(object parameter)
        {
            TextoBusqueda = "";
            EstadoSeleccionado = -1;
            SedeSeleccionada = -1;
            ListaResultados.Clear();
        }

        private void ExecuteRegresar(object parameter)
        {
            CerrarVentanaAction?.Invoke();
        }

        private void ExecuteAbrirExpediente(object parameter)
        {
            var asunto = parameter as ResultadoConsultaModel ?? AsuntoSeleccionado;
            if (asunto != null)
            {
                int id = asunto.Id;
                AbrirEdicionAction?.Invoke(id);
                // Refrescar al volver
                ExecuteBuscar(null);
            }
        }
    }

    public class ItemFiltro
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class ResultadoConsultaModel
    {
        public int Id { get; set; }
        public string TituloAsunto { get; set; }
        public string CategoriaCorta { get; set; }
        public string SedeNombre { get; set; }
        public string EstatusNombre { get; set; }
        public string FechaLimite { get; set; }
    }
}
