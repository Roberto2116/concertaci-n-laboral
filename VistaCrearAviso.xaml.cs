using System;
using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaCrearAviso : Wpf.Ui.Controls.FluentWindow
    {
        public VistaCrearAviso()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = new VistaCrearAvisoViewModel();

            viewModel.CerrarVentanaAction = () =>
            {
                this.Close();
            };

            viewModel.MostrarMensajeAction = (titulo, mensaje) =>
            {
                MessageBoxImage icono = titulo.ToLower().Contains("error") ? MessageBoxImage.Error :
                                        titulo.ToLower().Contains("éxito") ? MessageBoxImage.Information :
                                        MessageBoxImage.Warning;

                MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, icono);
            };

            this.DataContext = viewModel;
        }
    }
}