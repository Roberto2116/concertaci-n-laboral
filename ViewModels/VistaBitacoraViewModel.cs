using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Npgsql;
using Proyecto_GRRLN_expediente.ViewModels.Base;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VistaBitacoraViewModel : ViewModelBase
    {
        private int _idAsunto;

        public Action CerrarVentanaAction { get; set; }

        private string _tituloBitacora;
        public string TituloBitacora
        {
            get => _tituloBitacora;
            set => SetProperty(ref _tituloBitacora, value);
        }

        private string _nuevaBitacora;
        public string NuevaBitacora
        {
            get => _nuevaBitacora;
            set => SetProperty(ref _nuevaBitacora, value);
        }

        public ObservableCollection<BitacoraModel> ListaHistorial { get; } = new ObservableCollection<BitacoraModel>();

        public ICommand GuardarCommand { get; }
        public ICommand RegresarCommand { get; }

        public VistaBitacoraViewModel(int idAsunto)
        {
            _idAsunto = idAsunto;
            TituloBitacora = $"SEGUIMIENTO DEL ASUNTO N° {_idAsunto}";

            GuardarCommand = new RelayCommand(ExecuteGuardar);
            RegresarCommand = new RelayCommand(ExecuteRegresar);

            CargarHistorial();
        }

        private void CargarHistorial()
        {
            ListaHistorial.Clear();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT id_seguimiento, fecha_Seguimiento, Descripcion FROM Seguimiento WHERE num_Asunto = @id ORDER BY fecha_Seguimiento DESC";
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(_idAsunto));

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaHistorial.Add(new BitacoraModel
                                {
                                    IdBitacora = Convert.ToInt32(reader[0]).ToString(),
                                    Fecha = reader[1].ToString(),
                                    Descripcion = reader[2].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteGuardar(object parameter)
        {
            if (string.IsNullOrWhiteSpace(NuevaBitacora))
            {
                MessageBox.Show("Por favor, escribe un comentario o avance antes de guardar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO Seguimiento (num_Asunto, fecha_Seguimiento, Descripcion) 
                            VALUES (@id, @fechaHora, @descripcion);

                            UPDATE Asuntos 
                            SET Fecha_atencion = @fechaHoy 
                            WHERE Id_asunto = @id;";

                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(_idAsunto));
                        cmd.Parameters.AddWithValue("@fechaHora", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); 
                        cmd.Parameters.AddWithValue("@fechaHoy", DateTime.Now.ToString("yyyy-MM-dd")); 
                        cmd.Parameters.AddWithValue("@descripcion", NuevaBitacora.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                NuevaBitacora = string.Empty;
                CargarHistorial();

                MessageBox.Show("¡Avance guardado y fecha de atención actualizada!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el avance:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteRegresar(object parameter)
        {
            CerrarVentanaAction?.Invoke();
        }
    }

    public class BitacoraModel
    {
        public string IdBitacora { get; set; }
        public string Fecha { get; set; }
        public string Descripcion { get; set; }
    }
}
