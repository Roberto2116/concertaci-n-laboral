using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaBitacora : Wpf.Ui.Controls.FluentWindow
    {
        public VistaBitacora(int idAsunto)
        {
            InitializeComponent();
            
            var viewModel = new VistaBitacoraViewModel(idAsunto);
            
            viewModel.CerrarVentanaAction = () =>
            {
                this.Close();
            };

            this.DataContext = viewModel;
        }
    }
}