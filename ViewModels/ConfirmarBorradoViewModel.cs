using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using Proyecto_GRRLN_expediente.ViewModels.Base;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class ConfirmarBorradoViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private string _mensajeAutorizacion = "Esta acción requiere autorización de seguridad. Por favor, ingrese su contraseña:";

        public string MensajeAutorizacion
        {
            get => _mensajeAutorizacion;
            set => SetProperty(ref _mensajeAutorizacion, value);
        }

        private string _motivo = "";
        public string Motivo
        {
            get => _motivo;
            set => SetProperty(ref _motivo, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public ICommand AutorizarCommand { get; }
        public ICommand CancelarCommand { get; }

        public Action CerrarVentanaAction { get; set; }

        public ConfirmarBorradoViewModel()
        {
            AutorizarCommand = new RelayCommand(ExecuteAutorizar);
            CancelarCommand = new RelayCommand(_ => 
            {
                DialogResult = false;
                CerrarVentanaAction?.Invoke();
            });
        }

        private void ExecuteAutorizar(object parameter)
        {
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            if (passwordBox == null) return;

            if (string.IsNullOrWhiteSpace(Motivo))
            {
                MessageBox.Show("Debe especificar el motivo de la baja.", "Campo Requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SesionSistema.UsuarioLogueado != null)
            {
                string passwordIngresada = passwordBox.Password;
                string passwordBD = SesionSistema.UsuarioLogueado.Contrasena;

                if (string.IsNullOrEmpty(passwordBD))
                {
                    MessageBox.Show("La contraseña del usuario activo está vacía en la base de datos.",
                                    "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                    CerrarVentanaAction?.Invoke();
                    return;
                }

                if (!passwordBD.Contains(":"))
                {
                    if (passwordIngresada == passwordBD)
                    {
                        DialogResult = true;
                        CerrarVentanaAction?.Invoke();
                        return;
                    }
                    else
                    {
                        MostrarErrorPass(passwordBox);
                        return;
                    }
                }

                try
                {
                    string[] partes = passwordBD.Split(':');
                    if (partes.Length != 2) throw new Exception("Formato Hash:Salt inválido.");

                    string hashBD = partes[0];
                    string saltBD = partes[1];

                    if (VerificarContrasena(passwordIngresada, hashBD, saltBD))
                    {
                        DialogResult = true;
                        CerrarVentanaAction?.Invoke();
                    }
                    else
                    {
                        MostrarErrorPass(passwordBox);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desencriptar la contraseña de la BD.\nDetalle: {ex.Message}", "Error Interno", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MostrarErrorPass(System.Windows.Controls.PasswordBox passwordBox)
        {
            MessageBox.Show("La contraseña ingresada es incorrecta. No se autorizó la acción.",
                            "Error de Seguridad",
                            MessageBoxButton.OK,
                            MessageBoxImage.Stop);
            passwordBox.Clear();
            passwordBox.Focus();
        }

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
