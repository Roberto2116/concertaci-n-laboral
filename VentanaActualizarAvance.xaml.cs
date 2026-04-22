using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VentanaActualizarAvance : Wpf.Ui.Controls.FluentWindow
    {
        public VentanaActualizarAvance(int idAsunto, int avanceActual)
        {
            InitializeComponent();
            
            var viewModel = new VentanaActualizarAvanceViewModel(idAsunto, avanceActual);
            
            viewModel.CerrarVentanaAction = (dialogResult) =>
            {
                this.DialogResult = dialogResult;
                this.Close();
            };

            this.DataContext = viewModel;
        }

        private void TxtPorcentaje_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}