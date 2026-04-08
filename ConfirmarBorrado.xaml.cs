using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Windows;
// using Wpf.Ui.Controls; // Descomenta esta línea si usas FluentWindow

namespace Proyecto_GRRLN_expediente
{
    /// <summary>
    /// Lógica de interacción para ConfirmarBorrado.xaml
    /// </summary>
    public partial class ConfirmarBorrado : Wpf.Ui.Controls.FluentWindow // Cambia a FluentWindow si tu XAML lo requiere
    {
        public ConfirmarBorrado()
        {
            InitializeComponent();

          
            TxtPasswordConfirm.Focus();
        }

        private void BtnAutorizar_Click(object sender, RoutedEventArgs e)
        {
            if (SesionSistema.UsuarioLogueado != null)
            {
                if (TxtPasswordConfirm.Password == SesionSistema.UsuarioLogueado.Contrasena)
                {
                    
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("La contraseña ingresada es incorrecta. No se autorizó la eliminación.",
                                    "Error de Seguridad",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Stop);

                    TxtPasswordConfirm.Clear();
                    TxtPasswordConfirm.Focus();
                }
            }
            else
            {
                MessageBox.Show("Error crítico: No se detectó un usuario activo en el sistema.",
                                "Error de Sesión",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                this.DialogResult = false;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            
            this.DialogResult = false;
        }
    }
}
