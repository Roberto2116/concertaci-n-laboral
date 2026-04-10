using System;
using System.Security.Cryptography; // Necesario para la encriptación
using System.Windows;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class VentanaLogin : Wpf.Ui.Controls.FluentWindow
    {
        public VentanaLogin()
        {
            InitializeComponent();
            TxtUsuario.Focus();
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

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            LblError.Visibility = Visibility.Collapsed;

            string usuario = TxtUsuario.Text.Trim();
            string password = TxtPassword.Password.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MostrarError("Por favor, llena ambos campos.");
                return;
            }

            bool loginExitoso = false;
            string nombreUsuarioLogueado = "";
            string tipoUsuarioLogueado = "";
            int contadorActual = 0;

            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                using (var command = conn.CreateCommand())
                {
                    // 1. Buscamos al usuario por su Ficha
                    command.CommandText = "SELECT nombre, tipo, clave_depto, contraseña, estatus, contador FROM usuario WHERE Ficha = $ficha";
                    command.Parameters.AddWithValue("$ficha", usuario);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string passwordBD = reader["contraseña"] != DBNull.Value ? reader["contraseña"].ToString() : "";
                            int estatusBD = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 1;
                            contadorActual = reader["contador"] != DBNull.Value ? Convert.ToInt32(reader["contador"]) : 0;

                            if (estatusBD == 0)
                            {
                                MostrarError("Tu cuenta ha sido desactivada. Contacta al administrador.");
                                return;
                            }

                            if (!VerificarContrasenaSegura(password, passwordBD))
                            {
                                MostrarError("Ficha o contraseña incorrectos.");
                                return;
                            }

                            loginExitoso = true;
                            nombreUsuarioLogueado = reader.GetString(0);
                            tipoUsuarioLogueado = reader.GetString(1);

                            // ====================================================
                            // LLENADO DE SESIONES (¡AQUÍ ESTÁ LA CORRECCIÓN!)
                            // ====================================================

                            // 1. Tu sesión original (SesionGlobal)
                            SesionGlobal.Ficha = usuario;
                            SesionGlobal.Nombre = nombreUsuarioLogueado;
                            SesionGlobal.TipoRol = tipoUsuarioLogueado;
                            SesionGlobal.ClaveDepto = reader.IsDBNull(2) ? "" : reader.GetString(2);

                            // 2. La sesión que usa ConfirmarBorrado (SesionSistema)
                            SesionSistema.UsuarioLogueado = new Usuario
                            {
                                Ficha = usuario,
                                Nombre = nombreUsuarioLogueado,
                                Contrasena = passwordBD, // ¡VITAL! Guardamos la contraseña encriptada en memoria
                                Tipo = tipoUsuarioLogueado,
                                ClaveDepto = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2))
                            };
                        }
                        else
                        {
                            MostrarError("Ficha o contraseña incorrectos.");
                            return;
                        }
                    }

                    // 4. ACTUALIZAR CONTADOR, FECHA Y BITÁCORA SI FUE EXITOSO
                    if (loginExitoso)
                    {
                        string fechaAhora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        // A. Actualizamos tabla de usuario (última entrada y contador)
                        command.CommandText = "UPDATE usuario SET fecha_ultimaEntrada = $fecha, contador = contador + 1 WHERE Ficha = $ficha";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("$fecha", fechaAhora);
                        command.Parameters.AddWithValue("$ficha", usuario);
                        command.ExecuteNonQuery();

                        // B. ALIMENTAR BITÁCORA: Insertar registro de entrada
                        command.CommandText = @"INSERT INTO Bitacora_Sesiones (Ficha_Usuario, Fecha_Entrada) 
                                                VALUES ($ficha, $fechaEntrada);
                                                SELECT last_insert_rowid();";

                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("$ficha", usuario);
                        command.Parameters.AddWithValue("$fechaEntrada", fechaAhora);

                        long idSesionGenerado = (long)command.ExecuteScalar();
                        SesionGlobal.IdSesionActual = idSesionGenerado;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error de base de datos:\n{ex.Message}");
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
                    LoginExitoso(nombreUsuarioLogueado, tipoUsuarioLogueado);
                }
                else
                {
                    LoginExitoso(nombreUsuarioLogueado, tipoUsuarioLogueado);
                }
            }
        }

        private void MostrarError(string mensaje)
        {
            LblError.Text = mensaje;
            LblError.Visibility = Visibility.Visible;
            TxtPassword.Clear();
            TxtPassword.Focus();
        }

        private void LoginExitoso(string nombre, string rol)
        {
            MainWindow ventanaPrincipal = new MainWindow();
            ventanaPrincipal.Show();
            this.Close();
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}