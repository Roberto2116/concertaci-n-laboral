using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;
using Proyecto_GRRLN_expediente.ViewModels.Base;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand NavegarExpedientesCommand { get; }
        public ICommand NavegarNuevoCasoCommand { get; }
        public ICommand NavegarConsultasCommand { get; }
        public ICommand NavegarEstadisticasCommand { get; }
        public ICommand NavegarNuevoAvisoCommand { get; }
        public ICommand NavegarAdministracionCommand { get; }
        public ICommand SalirCommand { get; }

        public Action AbrirExpedientesAction { get; set; }
        public Action AbrirNuevoCasoAction { get; set; }
        public Action AbrirConsultasAction { get; set; }
        public Action AbrirEstadisticasAction { get; set; }
        public Action AbrirNuevoAvisoAction { get; set; }
        public Action AbrirAdministracionAction { get; set; }
        public Action CerrarSesionAction { get; set; }

        public MainWindowViewModel()
        {
            NavegarExpedientesCommand = new RelayCommand(_ => AbrirExpedientesAction?.Invoke());
            NavegarNuevoCasoCommand = new RelayCommand(_ => AbrirNuevoCasoAction?.Invoke());
            NavegarConsultasCommand = new RelayCommand(_ => AbrirConsultasAction?.Invoke());
            NavegarEstadisticasCommand = new RelayCommand(_ => AbrirEstadisticasAction?.Invoke());
            NavegarNuevoAvisoCommand = new RelayCommand(_ => AbrirNuevoAvisoAction?.Invoke());
            NavegarAdministracionCommand = new RelayCommand(_ => AbrirAdministracionAction?.Invoke());
            SalirCommand = new RelayCommand(ExecuteSalir);
        }

        private void ExecuteSalir(object parameter)
        {
            RegistrarSalidaBitacora();
            CerrarSesionAction?.Invoke();
        }

        public void RegistrarSalidaBitacora()
        {
            if (SesionGlobal.IdSesionActual <= 0) return;

            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE Bitacora_Sesiones SET Fecha_Salida = $fecha WHERE Id_Sesion = $id";
                    command.Parameters.AddWithValue("$fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("$id", SesionGlobal.IdSesionActual);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al registrar salida: " + ex.Message);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }
    }
}
