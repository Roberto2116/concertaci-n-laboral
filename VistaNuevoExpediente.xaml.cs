using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaNuevoExpediente : Wpf.Ui.Controls.FluentWindow
    {
        public VistaNuevoExpediente()
        {
            try
            {
                InitializeComponent();
                
                var viewModel = new VistaNuevoExpedienteViewModel();

                viewModel.AbrirConsultaAction = () =>
                {
                    VistaConsultaGeneral ventanaConsulta = new VistaConsultaGeneral();
                    ventanaConsulta.Show();
                };

                viewModel.CerrarVentanaAction = () =>
                {
                    this.Close();
                };

                this.DataContext = viewModel;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al inicializar la ventana: " + ex.ToString());
            }
        }
    }
}