using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaConsultaGeneral : Wpf.Ui.Controls.FluentWindow
    {
        public VistaConsultaGeneral()
        {
            InitializeComponent();
            
            var viewModel = new VistaConsultaGeneralViewModel();
            
            viewModel.CerrarVentanaAction = () =>
            {
                this.Close();
            };

            viewModel.AbrirEdicionAction = (idAsunto) =>
            {
                try
                {
                    VistaEditarExpediente ventanaEdicion = new VistaEditarExpediente(idAsunto);
                    ventanaEdicion.ShowDialog();
                    viewModel.CargarDatos();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error al abrir la ventana de edición: {ex.Message}");
                }
            };

            viewModel.AbrirAvanceAction = (idAsunto, avanceActual) =>
            {
                VentanaActualizarAvance ventanaEmergente = new VentanaActualizarAvance(idAsunto, avanceActual);
                if (ventanaEmergente.ShowDialog() == true)
                {
                    viewModel.CargarDatos();
                }
            };

            this.DataContext = viewModel;
        }
    }
}