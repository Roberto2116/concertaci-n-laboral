using System;
using System.Windows;
using Proyecto_GRRLN_expediente.ViewModels;
using Wpf.Ui.Controls;

// SOLUCIÓN A LA AMBIGÜEDAD DE LIBRERÍAS
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace Proyecto_GRRLN_expediente
{
    public partial class VentanaAdministrador : FluentWindow
    {
        public VentanaAdministrador()
        {
            InitializeComponent();
            
            var viewModel = new VentanaAdministradorViewModel();

            // Delegado para cerrar la ventana
            viewModel.CerrarVentanaAction = () => this.Close();

            // Delegado para mostrar mensajes (alertas/éxitos)
            viewModel.MostrarMensajeAction = (titulo, mensaje) =>
            {
                MessageBoxImage icono = titulo.ToLower().Contains("error") ? MessageBoxImage.Error :
                                        titulo.ToLower().Contains("éxito") ? MessageBoxImage.Information :
                                        MessageBoxImage.Warning;
                MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, icono);
            };

            // Delegado para la ventana de confirmación de cambio de contraseña
            viewModel.ConfirmarBorradoAction = () =>
            {
                ConfirmarBorrado confirmacion = new ConfirmarBorrado();
                return confirmacion.ShowDialog() == true;
            };

            this.DataContext = viewModel;
        }

        // Glue code para el PasswordBox (ya que no soporta Binding nativo)
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is VentanaAdministradorViewModel viewModel)
            {
                viewModel.PasswordUsuario = TxtPassword.Password;
            }
        }
    }
}