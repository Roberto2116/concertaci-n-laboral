using System.Windows;
using System.Windows.Controls;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaInicio : UserControl
    {
        public VistaInicio()
        {
            InitializeComponent();
            this.DataContext = new VistaInicioViewModel();
            this.Unloaded += VistaInicio_Unloaded;
        }

        private void VistaInicio_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is VistaInicioViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }
    }
}