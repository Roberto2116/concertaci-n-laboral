using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VentanaLogin : Wpf.Ui.Controls.FluentWindow
    {
        public VentanaLogin()
        {
            InitializeComponent();
            
            var viewModel = new VentanaLoginViewModel();

            viewModel.AbrirMainWindowAction = () =>
            {
                MainWindow ventanaPrincipal = new MainWindow();
                ventanaPrincipal.Show();
            };

            viewModel.CerrarVentanaAction = () =>
            {
                this.Close();
            };

            this.DataContext = viewModel;
            TxtUsuario.Focus();
        }
    }
}