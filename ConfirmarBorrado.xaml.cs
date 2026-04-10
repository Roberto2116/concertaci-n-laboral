using System;
using System.Security.Cryptography;
using System.Windows;

namespace Proyecto_GRRLN_expediente
{
    /// <summary>
    /// Lógica de interacción para ConfirmarBorrado.xaml
    /// </summary>
    public partial class ConfirmarBorrado : Wpf.Ui.Controls.FluentWindow
    {
        public ConfirmarBorrado()
        {
            InitializeComponent();
            TxtPasswordConfirm.Focus();

            // ==========================================
            // DIAGNÓSTICO AL ABRIR LA VENTANA
            // ==========================================
            if (SesionSistema.UsuarioLogueado != null)
            {
                string infoPass = SesionSistema.UsuarioLogueado.Contrasena;
                string formatoInfo = "Desconocido";

                if (string.IsNullOrEmpty(infoPass)) formatoInfo = "VACÍA";
                else if (infoPass.Contains(":")) formatoInfo = "Encriptada (Hash:Salt)";
                else formatoInfo = "Texto Plano";

                MessageBox.Show($"--- DIAGNÓSTICO DE SESIÓN ---\n\n" +
                                $"Usuario Activo: {SesionSistema.UsuarioLogueado.Ficha}\n" +
                                $"Formato de Password en BD: {formatoInfo}\n\n" +
                                $"Por favor, ingresa la contraseña de la Ficha: {SesionSistema.UsuarioLogueado.Ficha}",
                                "Radar de Sesión", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("--- DIAGNÓSTICO DE SESIÓN ---\n\n¡ALERTA! El sistema detecta que NO HAY NINGÚN USUARIO ACTIVO en SesionSistema.UsuarioLogueado.",
                                "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
        }

        private void BtnAutorizar_Click(object sender, RoutedEventArgs e)
        {
            if (SesionSistema.UsuarioLogueado != null)
            {
                string passwordIngresada = TxtPasswordConfirm.Password;
                string passwordBD = SesionSistema.UsuarioLogueado.Contrasena;

                // 1. Validar que la contraseña no esté vacía
                if (string.IsNullOrEmpty(passwordBD))
                {
                    MessageBox.Show("La contraseña del usuario activo está vacía en la base de datos.",
                                    "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.DialogResult = false;
                    return;
                }

                // 2. Si la contraseña en BD NO tiene ":", significa que está en texto plano (Admin123)
                // (Esto puede pasar si metiste la contraseña directamente en DB Browser)
                if (!passwordBD.Contains(":"))
                {
                    if (passwordIngresada == passwordBD)
                    {
                        this.DialogResult = true; // Autorizado (Texto Plano)
                        return;
                    }
                    else
                    {
                        MostrarErrorPass();
                        return;
                    }
                }

                // 3. Si la contraseña sí tiene ":", está encriptada. Procedemos a desencriptar.
                try
                {
                    string[] partes = passwordBD.Split(':');
                    if (partes.Length != 2) throw new Exception("Formato Hash:Salt inválido.");

                    string hashBD = partes[0];
                    string saltBD = partes[1];

                    if (VerificarContrasena(passwordIngresada, hashBD, saltBD))
                    {
                        this.DialogResult = true; // Autorizado (Encriptada)
                    }
                    else
                    {
                        MostrarErrorPass();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desencriptar la contraseña de la BD.\nDetalle: {ex.Message}", "Error Interno", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MostrarErrorPass()
        {
            MessageBox.Show("La contraseña ingresada es incorrecta. No se autorizó la acción.",
                            "Error de Seguridad",
                            MessageBoxButton.OK,
                            MessageBoxImage.Stop);
            TxtPasswordConfirm.Clear();
            TxtPasswordConfirm.Focus();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        // ==========================================
        // MÉTODO DE ENCRIPTACIÓN
        // ==========================================
        private bool VerificarContrasena(string passwordIngresada, string hashBD, string saltBD)
        {
            try
            {
                byte[] saltBytes = Convert.FromBase64String(saltBD);
                using (var pbkdf2 = new Rfc2898DeriveBytes(passwordIngresada, saltBytes, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hashCalculado = pbkdf2.GetBytes(32);
                    string hashCalculadoTexto = Convert.ToBase64String(hashCalculado);
                    return hashCalculadoTexto == hashBD;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}