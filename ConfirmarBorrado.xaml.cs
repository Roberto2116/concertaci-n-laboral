using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class ConfirmarBorrado : Wpf.Ui.Controls.FluentWindow
    {
        public ConfirmarBorrado()
        {
            InitializeComponent();
            
            var viewModel = new ConfirmarBorradoViewModel();

            viewModel.CerrarVentanaAction = () =>
            {
                this.DialogResult = viewModel.DialogResult;
                this.Close();
            };

            this.DataContext = viewModel;
            TxtPasswordConfirm.Focus();
        }
    }
}
