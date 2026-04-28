using System;
using System.Windows;
using System.Windows.Input;
using Npgsql;
using Proyecto_GRRLN_expediente.ViewModels.Base;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VentanaActualizarAvanceViewModel : ViewModelBase
    {
        private int _idAsunto;
        private int _avanceOriginal;

        public Action<bool> CerrarVentanaAction { get; set; }

        private string _tituloExpediente;
        public string TituloExpediente
        {
            get => _tituloExpediente;
            set => SetProperty(ref _tituloExpediente, value);
        }

        private int _avance;
        public int Avance
        {
            get => _avance;
            set
            {
                int newValue = value;
                if (newValue < 0) newValue = 0;
                if (newValue > 100) newValue = 100;
                
                if (SetProperty(ref _avance, newValue))
                {
                    AvanceTexto = newValue.ToString();
                }
            }
        }

        private string _avanceTexto;
        public string AvanceTexto
        {
            get => _avanceTexto;
            set
            {
                SetProperty(ref _avanceTexto, value);
                if (int.TryParse(value, out int num))
                {
                    if (num < 0) num = 0;
                    if (num > 100) num = 100;
                    
                    if (_avance != num)
                    {
                        Avance = num;
                    }
                }
            }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public VentanaActualizarAvanceViewModel(int idAsunto, int avanceActual)
        {
            _idAsunto = idAsunto;
            _avanceOriginal = avanceActual;
            TituloExpediente = $"Expediente #{_idAsunto}";
            
            // Al asignar Avance, se asignará automáticamente AvanceTexto
            Avance = avanceActual;

            GuardarCommand = new RelayCommand(ExecuteGuardar);
            CancelarCommand = new RelayCommand(ExecuteCancelar);
        }

        private void ExecuteCancelar(object parameter)
        {
            CerrarVentanaAction?.Invoke(false);
        }

        private void ExecuteGuardar(object parameter)
        {
            if (string.IsNullOrWhiteSpace(AvanceTexto))
            {
                MessageBox.Show("Por favor, ingresa un porcentaje de avance antes de guardar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int nuevoIdEstatus = (Avance == 0) ? 1 : (Avance == 100) ? 3 : 2;
            string fechaAtencionActual = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"
                            UPDATE Asuntos 
                            SET Porcentaje_avance = @avance, 
                                Id_estatus = @idEst,
                                Fecha_atencion = @fechaAte
                            WHERE Id_asunto = @id;

                            INSERT INTO Seguimiento (num_Asunto, Descripcion, fecha_Seguimiento)
                            VALUES (@id, @descSeguimiento, @fechaAte);";

                        command.Parameters.AddWithValue("@avance", Avance);
                        command.Parameters.AddWithValue("@idEst", nuevoIdEstatus);
                        command.Parameters.AddWithValue("@fechaAte", fechaAtencionActual);
                        command.Parameters.AddWithValue("@id", Convert.ToInt32(_idAsunto));
                        string nombreUsuario = SesionGlobal.Nombre ?? "Usuario";
                        command.Parameters.AddWithValue("@descSeguimiento", $"Avance actualizado del {_avanceOriginal}% al {Avance}% por {nombreUsuario}.");

                        command.ExecuteNonQuery();
                    }
                }

                string nombreEstado = (nuevoIdEstatus == 1) ? "NUEVO" :
                                     (nuevoIdEstatus == 3) ? "ATENDIDO" : "EN PROCESO";

                MessageBox.Show($"Actualización exitosa.\nAvance: {Avance}%\nEstatus: {nombreEstado}",
                                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                CerrarVentanaAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
