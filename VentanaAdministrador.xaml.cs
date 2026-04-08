using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;
using Wpf.Ui.Controls;
using System.Net;
using System.Net.Mail;

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

            PanelCatalogoSap.Visibility = Visibility.Collapsed;
            PanelUsuarios.Visibility = Visibility.Visible;
            PanelOrganismos.Visibility = Visibility.Collapsed; // Reemplaza a Descripciones

            CargarTablaSaps();
            CargarDepartamentos();
            CargarEstratos();
            CargarUsuarios();
            CargarTablaOrganismos(); // Cargamos organismos en segundo plano

            BtnGuardarUsuario.Content = "GUARDAR NUEVO";
            BtnGuardarSap.Content = "GUARDAR NUEVO";
            BtnGuardarOrganismo.Content = "GUARDAR NUEVO";
        }

        // ==========================================
        // SEGURIDAD Y ENCRIPTACIÓN
        // ==========================================
        private (string Hash, string Salt) HashearContrasenaConSalt(string passwordPlana)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordPlana, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32);
                return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
            }
        }

        private bool VerificarContrasena(string passwordIngresada, string hashBD, string saltBD)
        {
            byte[] saltBytes = Convert.FromBase64String(saltBD);
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordIngresada, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hashCalculado = pbkdf2.GetBytes(32);
                string hashCalculadoTexto = Convert.ToBase64String(hashCalculado);
                return hashCalculadoTexto == hashBD;
            }
        }

        // ==========================================
        // NAVEGACIÓN DEL MENÚ LATERAL
        // ==========================================
        private void BtnMenuSap_Click(object sender, RoutedEventArgs e)
        {
            PanelUsuarios.Visibility = Visibility.Collapsed;
            PanelOrganismos.Visibility = Visibility.Collapsed;
            PanelCatalogoSap.Visibility = Visibility.Visible;
        }

        private void BtnMenuUsuarios_Click(object sender, RoutedEventArgs e)
        {
            PanelCatalogoSap.Visibility = Visibility.Collapsed;
            PanelOrganismos.Visibility = Visibility.Collapsed;
            PanelUsuarios.Visibility = Visibility.Visible;
        }

        // NUEVO: Llama al Panel de Organismos
        private void BtnMenuOrganismos_Click(object sender, RoutedEventArgs e)
        {
            PanelCatalogoSap.Visibility = Visibility.Collapsed;
            PanelUsuarios.Visibility = Visibility.Collapsed;
            PanelOrganismos.Visibility = Visibility.Visible;
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ==========================================
        // LÓGICA DEL CATÁLOGO DE DEPARTAMENTOS (SAP)
        // ==========================================
        private void CargarTablaSaps(string filtro = "")
        {
            List<DepartamentoItem> lista = new List<DepartamentoItem>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        string query = "SELECT clave_depto, descripcion FROM Dep_personal";
                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            query += " WHERE clave_depto LIKE $filtro OR descripcion LIKE $filtro";
                        }

                        cmd.CommandText = query;
                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            cmd.Parameters.AddWithValue("$filtro", "%" + filtro + "%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new DepartamentoItem
                                {
                                    Clave = reader["clave_depto"] != DBNull.Value ? reader["clave_depto"].ToString() : "",
                                    Descripcion = reader["descripcion"] != DBNull.Value ? reader["descripcion"].ToString() : ""
                                });
                            }
                        }
                    }
                }
                GridSaps.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el catálogo de departamentos: {ex.Message}");
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void TxtBuscarSap_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarTablaSaps(TxtBuscarSap.Text);
        }

        private void GridSaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridSaps.SelectedItem is DepartamentoItem sapSeleccionado)
            {
                TxtClaveSap.Text = sapSeleccionado.Clave;
                TxtDescripcionSap.Text = sapSeleccionado.Descripcion;

                TxtClaveSap.IsEnabled = false;
                BtnGuardarSap.Content = "ACTUALIZAR DATOS";
            }
        }

        private void BtnLimpiarSap_Click(object sender, RoutedEventArgs e)
        {
            GridSaps.SelectedItem = null;
            TxtClaveSap.Text = "";
            TxtDescripcionSap.Text = "";

            TxtClaveSap.IsEnabled = true;
            BtnGuardarSap.Content = "GUARDAR NUEVO";
        }

        private void BtnGuardarSap_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtClaveSap.Text) || string.IsNullOrWhiteSpace(TxtDescripcionSap.Text))
            {
                MessageBox.Show("Por favor, llena la Clave y la Descripción.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string clave = TxtClaveSap.Text.Trim();
            string descripcion = TxtDescripcionSap.Text.Trim();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        if (TxtClaveSap.IsEnabled)
                        {
                            cmd.CommandText = "INSERT INTO Dep_personal (clave_depto, descripcion) VALUES ($clave, $descripcion)";
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE Dep_personal SET descripcion = $descripcion WHERE clave_depto = $clave";
                        }

                        cmd.Parameters.AddWithValue("$clave", clave);
                        cmd.Parameters.AddWithValue("$descripcion", descripcion);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Departamento guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                BtnLimpiarSap_Click(null, null);
                CargarTablaSaps();
                CargarDepartamentos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar departamento: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        // ==========================================
        // LÓGICA DE USUARIOS
        // ==========================================
        private void CargarDepartamentos()
        {
            List<DepartamentoItem> listaDeptos = new List<DepartamentoItem>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT clave_depto, descripcion FROM Dep_personal";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaDeptos.Add(new DepartamentoItem
                                {
                                    Clave = reader["clave_depto"] != DBNull.Value ? reader["clave_depto"].ToString() : "",
                                    Descripcion = reader["descripcion"] != DBNull.Value ? reader["descripcion"].ToString() : ""
                                });
                            }
                        }
                    }
                }
                CmbClaveDepto.ItemsSource = listaDeptos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar lista de departamentos: {ex.Message}");
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void CargarEstratos()
        {
            CmbEstrato.Items.Clear();
            CmbEstrato.Items.Add("GERENTE");
            CmbEstrato.Items.Add("SUPERINTENDENTE");
            CmbEstrato.Items.Add("SUBGERENTE");
            CmbEstrato.Items.Add("JEFE");
        }

        private void CmbEstrato_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CmbRolUsuario.Items.Clear();

            if (CmbEstrato.SelectedItem == null) return;

            string estratoSel = CmbEstrato.SelectedItem.ToString();

            if (estratoSel == "GERENTE" || estratoSel == "SUPERINTENDENTE")
            {
                CmbRolUsuario.Items.Add("ADMINISTRADOR");
                CmbRolUsuario.Items.Add("OPERADOR");
                CmbRolUsuario.SelectedItem = "ADMINISTRADOR";
            }
            else if (estratoSel == "SUBGERENTE" || estratoSel == "JEFE")
            {
                CmbRolUsuario.Items.Add("OPERADOR");
                CmbRolUsuario.Items.Add("ADMINISTRADOR");
                CmbRolUsuario.SelectedItem = "OPERADOR";
            }
            else
            {
                CmbRolUsuario.Items.Add("OPERADOR");
                CmbRolUsuario.Items.Add("ADMINISTRADOR");
                CmbRolUsuario.SelectedIndex = 0;
            }
        }

        private void CargarUsuarios(string filtro = "")
        {
            List<UsuarioTabla> listaUsuarios = new List<UsuarioTabla>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        string query = "SELECT Ficha, nombre, Estrato, tipo, Correo, clave_depto, estatus FROM usuario";

                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            query += " WHERE Ficha LIKE $filtro OR nombre LIKE $filtro";
                        }

                        cmd.CommandText = query;

                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            cmd.Parameters.AddWithValue("$filtro", "%" + filtro + "%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int estatusNum = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 1;

                                listaUsuarios.Add(new UsuarioTabla
                                {
                                    Ficha = reader["Ficha"] != DBNull.Value ? reader["Ficha"].ToString() : "",
                                    nombre = reader["nombre"] != DBNull.Value ? reader["nombre"].ToString() : "",
                                    Estrato = reader["Estrato"] != DBNull.Value ? reader["Estrato"].ToString() : "",
                                    tipo = reader["tipo"] != DBNull.Value ? reader["tipo"].ToString() : "",
                                    Correo = reader["Correo"] != DBNull.Value ? reader["Correo"].ToString() : "",
                                    ClaveDepto = reader["clave_depto"] != DBNull.Value ? reader["clave_depto"].ToString() : "",
                                    EstatusNum = estatusNum,
                                    EstatusTexto = estatusNum == 1 ? "✅ Activo" : "❌ Inactivo"
                                });
                            }
                        }
                    }
                }

                GridUsuarios.ItemsSource = listaUsuarios;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Asegúrate de ejecutar el comando ALTER TABLE en SQLite para la columna estatus.\n\nError: {ex.Message}", "Aviso de Base de Datos");
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void TxtBuscarUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarUsuarios(TxtBuscarUsuario.Text);
        }

        private void GridUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridUsuarios.SelectedItem is UsuarioTabla usuarioSeleccionado)
            {
                TxtFicha.Text = usuarioSeleccionado.Ficha;
                TxtNombreUsuario.Text = usuarioSeleccionado.nombre;
                TxtCorreo.Text = usuarioSeleccionado.Correo;
                CmbClaveDepto.SelectedValue = usuarioSeleccionado.ClaveDepto;

                if (!CmbEstrato.Items.Contains(usuarioSeleccionado.Estrato))
                {
                    if (!string.IsNullOrEmpty(usuarioSeleccionado.Estrato))
                    {
                        CmbEstrato.Items.Add(usuarioSeleccionado.Estrato);
                    }
                }
                CmbEstrato.SelectedItem = usuarioSeleccionado.Estrato;

                CmbRolUsuario.SelectedItem = usuarioSeleccionado.tipo;

                TxtPassword.Password = "";

                if (LblPassword != null) LblPassword.Text = "Nueva Contraseña (Opcional)";
                TxtPassword.PlaceholderText = "Escribe solo si deseas cambiarla";

                ToggleEstatusUsuario.IsChecked = usuarioSeleccionado.EstatusNum == 1;

                TxtFicha.IsEnabled = false;
                BtnGuardarUsuario.Content = "ACTUALIZAR DATOS";
            }
        }

        private void BtnLimpiarUsuario_Click(object sender, RoutedEventArgs e)
        {
            GridUsuarios.SelectedItem = null;
            TxtFicha.Text = "";
            TxtNombreUsuario.Text = "";
            TxtCorreo.Text = "";
            TxtPassword.Password = "";

            if (LblPassword != null) LblPassword.Text = "Contraseña Provisional";
            TxtPassword.PlaceholderText = "Ingresa contraseña";

            CmbEstrato.SelectedIndex = -1;
            CmbRolUsuario.Items.Clear();
            CmbClaveDepto.SelectedIndex = -1;
            CmbClaveDepto.Text = "";

            ToggleEstatusUsuario.IsChecked = true;
            TxtFicha.IsEnabled = true;
            BtnGuardarUsuario.Content = "GUARDAR NUEVO";
        }

        private void BtnGuardarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFicha.Text) || string.IsNullOrWhiteSpace(TxtNombreUsuario.Text) || CmbRolUsuario.SelectedItem == null || CmbEstrato.SelectedItem == null)
            {
                MessageBox.Show("Por favor, llena al menos la Ficha, Nombre, Estrato y Rol.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string ficha = TxtFicha.Text.Trim();
            string nombre = TxtNombreUsuario.Text.Trim();
            string correo = TxtCorreo.Text.Trim();
            string estrato = CmbEstrato.SelectedItem.ToString();
            string claveDepto = CmbClaveDepto.SelectedValue?.ToString() ?? CmbClaveDepto.Text.Trim();
            string tipo = CmbRolUsuario.SelectedItem.ToString();
            string passwordPlana = TxtPassword.Password;

            int estatus = ToggleEstatusUsuario.IsChecked == true ? 1 : 0;

            bool enviarCorreo = false;
            bool esNuevo = false;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        if (TxtFicha.IsEnabled)
                        {
                            if (string.IsNullOrWhiteSpace(passwordPlana))
                            {
                                MessageBox.Show("Para un usuario nuevo, la contraseña provisional es obligatoria.", "Falta contraseña", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            var (hash, salt) = HashearContrasenaConSalt(passwordPlana);
                            string passwordEncriptada = $"{hash}:{salt}";

                            cmd.CommandText = @"INSERT INTO usuario (Ficha, nombre, Correo, Estrato, clave_depto, tipo, contraseña, contador, fecha_ultimaEntrada, estatus) 
                                                VALUES ($ficha, $nombre, $correo, $estrato, $claveDepto, $tipo, $password, 0, $fechaActual, $estatus)";

                            cmd.Parameters.AddWithValue("$password", passwordEncriptada);
                            cmd.Parameters.AddWithValue("$fechaActual", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            enviarCorreo = true;
                            esNuevo = true;
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(passwordPlana))
                            {
                                cmd.CommandText = @"UPDATE usuario SET nombre = $nombre, Correo = $correo, 
                                                    Estrato = $estrato, clave_depto = $claveDepto, tipo = $tipo, estatus = $estatus
                                                    WHERE Ficha = $ficha";
                            }
                            else
                            {
                                ConfirmarBorrado confirmacion = new ConfirmarBorrado();
                                if (confirmacion.ShowDialog() != true) return;

                                var (hash, salt) = HashearContrasenaConSalt(passwordPlana);
                                string passwordEncriptada = $"{hash}:{salt}";

                                cmd.CommandText = @"UPDATE usuario SET nombre = $nombre, Correo = $correo, 
                                                    Estrato = $estrato, clave_depto = $claveDepto, tipo = $tipo,
                                                    contraseña = $password, estatus = $estatus 
                                                    WHERE Ficha = $ficha";
                                cmd.Parameters.AddWithValue("$password", passwordEncriptada);

                                enviarCorreo = true;
                                esNuevo = false;
                            }
                        }

                        cmd.Parameters.AddWithValue("$ficha", ficha);
                        cmd.Parameters.AddWithValue("$nombre", nombre);
                        cmd.Parameters.AddWithValue("$correo", correo);
                        cmd.Parameters.AddWithValue("$estrato", estrato);
                        cmd.Parameters.AddWithValue("$claveDepto", claveDepto);
                        cmd.Parameters.AddWithValue("$tipo", tipo);
                        cmd.Parameters.AddWithValue("$estatus", estatus);

                        cmd.ExecuteNonQuery();
                    }
                }

                if (enviarCorreo && !string.IsNullOrWhiteSpace(correo))
                {
                    EnviarCorreoCredenciales(correo, nombre, ficha, passwordPlana, esNuevo);
                }

                MessageBox.Show("Datos guardados correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnLimpiarUsuario_Click(null, null);
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar usuario: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void EnviarCorreoCredenciales(string correoDestino, string nombreEmpleado, string ficha, string password, bool esNuevoUsuario)
        {
            try
            {
                string correoRemitente = "test@sistema.com";

                MailMessage mensaje = new MailMessage();
                mensaje.From = new MailAddress(correoRemitente, "Sistema DIF - Pruebas");
                mensaje.To.Add(correoDestino);
                mensaje.Subject = esNuevoUsuario ? "NUEVA CUENTA - Prueba" : "CAMBIO PASS - Prueba";
                mensaje.Body = $"Hola {nombreEmpleado}, tus datos son:\nFicha: {ficha}\nPass: {password}";
                mensaje.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("localhost");
                smtp.Send(mensaje);
            }
            catch (Exception ex)
            {
            }
        }

        // ==========================================
        // NUEVA LÓGICA: CATÁLOGO DE ORGANISMOS PEMEX
        // ==========================================
        private void CargarTablaOrganismos(string filtro = "")
        {
            List<OrganismoItem> lista = new List<OrganismoItem>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        string query = "SELECT id_organismo, Organismo, Descripcion FROM Organismos";
                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            query += " WHERE Organismo LIKE $filtro OR Descripcion LIKE $filtro";
                        }

                        cmd.CommandText = query;
                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            cmd.Parameters.AddWithValue("$filtro", "%" + filtro + "%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new OrganismoItem
                                {
                                    Id = Convert.ToInt32(reader["id_organismo"]),
                                    Clave = reader["Organismo"] != DBNull.Value ? reader["Organismo"].ToString() : "",
                                    Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : ""
                                });
                            }
                        }
                    }
                }
                GridOrganismos.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el catálogo de Organismos: {ex.Message}");
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void TxtBuscarOrganismo_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarTablaOrganismos(TxtBuscarOrganismo.Text);
        }

        private void GridOrganismos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridOrganismos.SelectedItem is OrganismoItem orgSeleccionado)
            {
                TxtIdOrganismo.Text = orgSeleccionado.Id.ToString();
                TxtClaveOrganismo.Text = orgSeleccionado.Clave;
                TxtDescripcionOrganismo.Text = orgSeleccionado.Descripcion;

                BtnGuardarOrganismo.Content = "ACTUALIZAR DATOS";
            }
        }

        private void BtnLimpiarOrganismo_Click(object sender, RoutedEventArgs e)
        {
            GridOrganismos.SelectedItem = null;
            TxtIdOrganismo.Text = "";
            TxtClaveOrganismo.Text = "";
            TxtDescripcionOrganismo.Text = "";

            BtnGuardarOrganismo.Content = "GUARDAR NUEVO";
        }

        private void BtnGuardarOrganismo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtClaveOrganismo.Text) || string.IsNullOrWhiteSpace(TxtDescripcionOrganismo.Text))
            {
                MessageBox.Show("Por favor, llena la Clave (Siglas) y la Descripción.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string clave = TxtClaveOrganismo.Text.Trim();
            string descripcion = TxtDescripcionOrganismo.Text.Trim();
            string idTexto = TxtIdOrganismo.Text.Trim();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        if (string.IsNullOrEmpty(idTexto))
                        {
                            // Hacer INSERT (El ID lo pone SQLite solo)
                            cmd.CommandText = "INSERT INTO Organismos (Organismo, Descripcion) VALUES ($clave, $descripcion)";
                        }
                        else
                        {
                            // Hacer UPDATE basado en el ID que leímos de la tabla
                            cmd.CommandText = "UPDATE Organismos SET Organismo = $clave, Descripcion = $descripcion WHERE id_organismo = $id";
                            cmd.Parameters.AddWithValue("$id", Convert.ToInt32(idTexto));
                        }

                        cmd.Parameters.AddWithValue("$clave", clave);
                        cmd.Parameters.AddWithValue("$descripcion", descripcion);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Organismo guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                BtnLimpiarOrganismo_Click(null, null);
                CargarTablaOrganismos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar Organismo: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }
    }

    // ==========================================
    // CLASES MODELO
    // ==========================================
    public class UsuarioTabla
    {
        public string Ficha { get; set; }
        public string nombre { get; set; }
        public string Correo { get; set; }
        public string Estrato { get; set; }
        public string ClaveDepto { get; set; }
        public string tipo { get; set; }
        public int EstatusNum { get; set; }
        public string EstatusTexto { get; set; }
    }

    public class DepartamentoItem
    {
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public string DisplayTexto { get { return $"{Clave} - {Descripcion}"; } }
    }

    // NUEVO MODELO PARA ORGANISMOS
    public class OrganismoItem
    {
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
    }
}