using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using Npgsql;
using Proyecto_GRRLN_expediente.db;
using Proyecto_GRRLN_expediente.ViewModels.Base;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VentanaLoginViewModel : ViewModelBase
    {
        private string _usuario;
        private string _mensajeError;
        private bool _isErrorVisible;

        public string Usuario
        {
            get => _usuario;
            set => SetProperty(ref _usuario, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set
            {
                if (SetProperty(ref _mensajeError, value))
                {
                    IsErrorVisible = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set => SetProperty(ref _isErrorVisible, value);
        }

        public ICommand EntrarCommand { get; }
        public ICommand SalirCommand { get; }

        public Action AbrirMainWindowAction { get; set; }
        public Action CerrarVentanaAction { get; set; }

        public VentanaLoginViewModel()
        {
            EntrarCommand = new RelayCommand(ExecuteEntrar);
            SalirCommand = new RelayCommand(ExecuteSalir);
        }

        private void ExecuteEntrar(object parameter)
        {
            MensajeError = string.Empty;

            var passwordBox = parameter as Wpf.Ui.Controls.PasswordBox;
            if (passwordBox == null) return;

            string usuario = Usuario?.Trim();
            string password = passwordBox.Password?.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MensajeError = "Por favor, llena ambos campos.";
                return;
            }

            bool loginExitoso = false;
            string nombreUsuarioLogueado = "";
            string tipoUsuarioLogueado = "";
            int contadorActual = 0;

            try
            {
                using (NpgsqlConnection conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT nombre, tipo, clave_depto, contraseña, estatus, contador, Estrato FROM usuario WHERE Ficha = @ficha";
                        command.Parameters.AddWithValue("@ficha", usuario);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string passwordBD = reader["contraseña"] != DBNull.Value ? reader["contraseña"].ToString() : "";
                                int estatusBD = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 1;
                                contadorActual = reader["contador"] != DBNull.Value ? Convert.ToInt32(reader["contador"]) : 0;

                                if (estatusBD == 0)
                                {
                                    MensajeError = "Tu cuenta ha sido desactivada. Contacta al administrador.";
                                    return;
                                }

                                if (!VerificarContrasenaSegura(password, passwordBD))
                            {
                                MensajeError = "Ficha o contraseña incorrectos.";
                                return;
                            }

                            loginExitoso = true;
                            nombreUsuarioLogueado = reader[0].ToString();
                            tipoUsuarioLogueado = reader[1].ToString();

                            SesionGlobal.Ficha = usuario;
                            SesionGlobal.Nombre = nombreUsuarioLogueado;
                            SesionGlobal.TipoRol = tipoUsuarioLogueado;
                            SesionGlobal.Estrato = reader["Estrato"]?.ToString() ?? "";
                            SesionGlobal.ClaveDepto = reader.IsDBNull(2) ? "" : reader[2].ToString();

                            SesionSistema.UsuarioLogueado = new Usuario
                            {
                                Ficha = usuario,
                                Nombre = nombreUsuarioLogueado,
                                Contrasena = passwordBD,
                                Tipo = tipoUsuarioLogueado,
                                Estrato = SesionGlobal.Estrato,
                                ClaveDepto = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2))
                            };
                        }
                        else
                        {
                            MensajeError = "Ficha o contraseña incorrectos.";
                            return;
                        }
                    }

                    if (loginExitoso)
                    {
                        string fechaAhora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        command.CommandText = "UPDATE usuario SET fecha_ultimaEntrada = @fecha, contador = contador + 1 WHERE Ficha = @ficha";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@fecha", fechaAhora);
                        command.Parameters.AddWithValue("@ficha", usuario);
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO Bitacora_Sesiones (Ficha_Usuario, Fecha_Entrada) 
                                                VALUES (@ficha, @fechaEntrada)
                                                RETURNING Id_Sesion;";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@ficha", usuario);
                        command.Parameters.AddWithValue("@fechaEntrada", fechaAhora);

                        long idSesionGenerado = Convert.ToInt64(command.ExecuteScalar());
                        SesionGlobal.IdSesionActual = idSesionGenerado;
                    }
                }
            }
        }
        catch (Exception ex)
            {
                MensajeError = $"Error de base de datos:\n{ex.Message}";
                return;
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }

            if (loginExitoso)
            {
                if (contadorActual == 0)
                {
                    MessageBox.Show("Al ser tu primer inicio de sesión, por motivos de seguridad el sistema requiere que actualices tu contraseña provisional.", "Aviso de Seguridad", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
                AbrirMainWindowAction?.Invoke();
                CerrarVentanaAction?.Invoke();
            }
        }

        private bool VerificarContrasenaSegura(string passwordIngresada, string passwordEnBaseDeDatos)
        {
            try
            {
                if (!passwordEnBaseDeDatos.Contains(":"))
                {
                    return passwordIngresada == passwordEnBaseDeDatos;
                }

                string[] partes = passwordEnBaseDeDatos.Split(':');
                string hashBD = partes[0];
                string saltBD = partes[1];

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

        private void ExecuteSalir(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
