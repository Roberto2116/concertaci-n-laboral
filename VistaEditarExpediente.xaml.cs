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
                
                viewModel.ConfirmarBorradoAction = () =>
                {
                    ConfirmarBorrado confirmacion = new ConfirmarBorrado();
                    if (confirmacion.DataContext is ConfirmarBorradoViewModel cvm)
                    {
                        cvm.MensajeAutorizacion = "Para eliminar este expediente del sistema, por favor ingrese su contraseña de usuario:";
                        if (confirmacion.ShowDialog() == true)
                        {
                            return cvm.Motivo;
                        }
                    }
                    return null;
                };

                this.DataContext = viewModel;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al inicializar la ventana de edición: " + ex.Message);
            }
        }
    }
}