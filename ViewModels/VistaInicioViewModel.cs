using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;
using Proyecto_GRRLN_expediente.modelos;
using Proyecto_GRRLN_expediente.ViewModels.Base;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VistaInicioViewModel : ViewModelBase, IDisposable
    {
        private ObservableCollection<AvisoModel> _avisos;
        private bool _tieneAvisos;
        private DispatcherTimer _timerMensajes;

        public ObservableCollection<AvisoModel> Avisos
        {
            get => _avisos;
            set => SetProperty(ref _avisos, value);
        }

        public bool TieneAvisos
        {
            get => _tieneAvisos;
            set 
            {
                if (SetProperty(ref _tieneAvisos, value))
                {
                    OnPropertyChanged(nameof(ListaAvisosVisibility));
                    OnPropertyChanged(nameof(PanelSinAvisosVisibility));
                }
            }
        }

        public Visibility ListaAvisosVisibility => TieneAvisos ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PanelSinAvisosVisibility => TieneAvisos ? Visibility.Collapsed : Visibility.Visible;

        public ICommand ArchivarCommand { get; }
        public ICommand ArchivarTodoCommand { get; }

        public VistaInicioViewModel()
        {
            Avisos = new ObservableCollection<AvisoModel>();
            
            ArchivarCommand = new RelayCommand(ExecuteArchivar);
            ArchivarTodoCommand = new RelayCommand(ExecuteArchivarTodo);

            CargarBandejaAvisos();
            IniciarTemporizador();
        }

        private void IniciarTemporizador()
        {
            _timerMensajes = new DispatcherTimer();
            _timerMensajes.Interval = TimeSpan.FromSeconds(10);
            _timerMensajes.Tick += TimerMensajes_Tick;
            _timerMensajes.Start();
        }

        private void TimerMensajes_Tick(object sender, EventArgs e)
        {
            CargarBandejaAvisos();
        }

        private void CargarBandejaAvisos()
        {
            string miFicha = SesionGlobal.Ficha;
            string miDepto = SesionGlobal.ClaveDepto;

            List<AvisoModel> listaAvisos = new List<AvisoModel>();
            SqliteConnection conn = DatabaseConnection.GetConnection();

            try
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance 
                        FROM Mensaje 
                        WHERE IFNULL(Archivado, 0) = 0 AND ( 
                             (Tipo_alcance = 'PERSONAL' AND Ficha_destino = $ficha)
                          OR (Tipo_alcance = 'DEPTO' AND clave_depto_destino = $depto)
                          OR (Tipo_alcance = 'SAP')
                        )
                        ORDER BY fecha_posteo DESC;";

                    command.Parameters.AddWithValue("$ficha", miFicha ?? "");
                    command.Parameters.AddWithValue("$depto", miDepto ?? "");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaAvisos.Add(new AvisoModel
                            {
                                IdMensaje = reader.GetInt32(0),
                                Emisor = "De: " + reader.GetString(1),
                                TextoMensaje = reader.GetString(2),
                                Fecha = reader.GetString(3),
                                TipoAlcance = reader.GetString(4)
                            });
                        }
                    }
                }

                // Update the UI collection safely
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Avisos.Clear();
                    foreach (var aviso in listaAvisos)
                    {
                        Avisos.Add(aviso);
                    }
                    TieneAvisos = Avisos.Count > 0;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error silencioso al cargar avisos: {ex.Message}");
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void ExecuteArchivar(object parameter)
        {
            if (parameter is AvisoModel mensajeSeleccionado)
            {
                try
                {
                    SqliteConnection conn = DatabaseConnection.GetConnection();
                    if (conn == null) return;

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "UPDATE Mensaje SET Archivado = 1 WHERE Id_mensaje = $id";
                        command.Parameters.AddWithValue("$id", mensajeSeleccionado.IdMensaje);
                        command.ExecuteNonQuery();
                    }

                    CargarBandejaAvisos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al archivar el mensaje:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DatabaseConnection.CloseConnection();
                }
            }
        }

        private void ExecuteArchivarTodo(object parameter)
        {
            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                string miFicha = SesionGlobal.Ficha;
                string miDepto = SesionGlobal.ClaveDepto;

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Mensaje 
                        SET Archivado = 1 
                        WHERE IFNULL(Archivado, 0) = 0 AND ( 
                             (Tipo_alcance = 'PERSONAL' AND Ficha_destino = $ficha)
                          OR (Tipo_alcance = 'DEPTO' AND clave_depto_destino = $depto)
                          OR (Tipo_alcance = 'SAP')
                        )";

                    command.Parameters.AddWithValue("$ficha", miFicha ?? "");
                    command.Parameters.AddWithValue("$depto", miDepto ?? "");
                    command.ExecuteNonQuery();
                }

                CargarBandejaAvisos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar la bandeja:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        public void Dispose()
        {
            if (_timerMensajes != null)
            {
                _timerMensajes.Stop();
                _timerMensajes.Tick -= TimerMensajes_Tick;
                _timerMensajes = null;
            }
        }
    }
}
