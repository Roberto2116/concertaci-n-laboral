using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaEditarExpediente : Wpf.Ui.Controls.FluentWindow
    {
        private int _idAsunto;

        public VistaEditarExpediente(int idAsunto)
        {
            try
            {
                InitializeComponent();
                _idAsunto = idAsunto;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error en InitializeComponent: " + ex.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = new VistaEditarExpedienteViewModel(_idAsunto);

                viewModel.CerrarVentanaAction = () =>
                {
                    this.Close();
                };

                viewModel.AbrirAvanceAction = (id, avanceActual) =>
                {
                    VentanaActualizarAvance win = new VentanaActualizarAvance(id, avanceActual);
                    if (win.ShowDialog() == true)
                    {
                        viewModel.CargarDatosExpediente(); // Recargar si hubo cambios
                    }
                };

                viewModel.AbrirBitacoraAction = (id) =>
                {
                    new VistaBitacora(id).ShowDialog();
                };

                this.DataContext = viewModel;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al cargar la vista de edición: " + ex.Message);
            }
        }
    }
}