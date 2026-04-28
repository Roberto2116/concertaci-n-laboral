using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Npgsql;
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

            try
            {
                using (NpgsqlConnection conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"
                        SELECT Id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance 
                        FROM Mensaje 
                        WHERE COALESCE(Archivado, 0) = 0 AND ( 
                             (Tipo_alcance = 'PERSONAL' AND Ficha_destino = @ficha)
                          OR (Tipo_alcance = 'DEPTO' AND clave_depto_destino = @depto)
                          OR (Tipo_alcance = 'SAP')
                        )
                        ORDER BY fecha_posteo DESC;";

                        command.Parameters.AddWithValue("@ficha", miFicha ?? "");
                        if (int.TryParse(miDepto, out int deptoInt)) command.Parameters.AddWithValue("@depto", deptoInt);
                        else command.Parameters.AddWithValue("@depto", DBNull.Value);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaAvisos.Add(new AvisoModel
                                {
                                    IdMensaje = Convert.ToInt32(reader[0]),
                                    Emisor = "De: " + reader[1].ToString(),
                                    TextoMensaje = reader[2].ToString(),
                                    Fecha = reader[3].ToString(),
                                    TipoAlcance = reader[4].ToString()
                                });
                            }
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
        }

        private void ExecuteArchivar(object parameter)
        {
            if (parameter is AvisoModel mensajeSeleccionado)
            {
                try
                {
                    using (NpgsqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        if (conn == null) return;

                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = "UPDATE Mensaje SET Archivado = 1 WHERE Id_mensaje = @id";
                            command.Parameters.AddWithValue("@id", mensajeSeleccionado.IdMensaje);
                            command.ExecuteNonQuery();
                        }
                    }

                    CargarBandejaAvisos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al archivar el mensaje:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteArchivarTodo(object parameter)
        {
            try
            {
                using (NpgsqlConnection conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    string miFicha = SesionGlobal.Ficha;
                    string miDepto = SesionGlobal.ClaveDepto;

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"
                        UPDATE Mensaje 
                        SET Archivado = 1 
                        WHERE COALESCE(Archivado, 0) = 0 AND ( 
                             (Tipo_alcance = 'PERSONAL' AND Ficha_destino = @ficha)
                          OR (Tipo_alcance = 'DEPTO' AND clave_depto_destino = @depto)
                          OR (Tipo_alcance = 'SAP')
                        )";

                        command.Parameters.AddWithValue("@ficha", miFicha ?? "");
                        if (int.TryParse(miDepto, out int deptoInt2)) command.Parameters.AddWithValue("@depto", deptoInt2);
                        else command.Parameters.AddWithValue("@depto", DBNull.Value);
                        command.ExecuteNonQuery();
                    }
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
