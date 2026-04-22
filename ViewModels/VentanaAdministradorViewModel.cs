using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Windows.Input;
using Proyecto_GRRLN_expediente.ViewModels.Base;
using Proyecto_GRRLN_expediente.db;
using Proyecto_GRRLN_expediente.modelos;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VentanaAdministradorViewModel : ViewModelBase
    {
        // ==========================================
        // DELEGADOS / ACCIONES
        // ==========================================
        public Action CerrarVentanaAction { get; set; }
        public Action<string, string> MostrarMensajeAction { get; set; }
        public Func<bool> ConfirmarBorradoAction { get; set; }

        // ==========================================
        // ESTADO DE NAVEGACIÓN
        // ==========================================
        private bool _isPanelUsuariosVisible = true;
        public bool IsPanelUsuariosVisible { get => _isPanelUsuariosVisible; set => SetProperty(ref _isPanelUsuariosVisible, value); }

        private bool _isPanelSapVisible;
        public bool IsPanelSapVisible { get => _isPanelSapVisible; set => SetProperty(ref _isPanelSapVisible, value); }

        private bool _isPanelOrganismosVisible;
        public bool IsPanelOrganismosVisible { get => _isPanelOrganismosVisible; set => SetProperty(ref _isPanelOrganismosVisible, value); }

        public ICommand MostrarMenuCommand { get; }
        public ICommand SalirCommand { get; }

        // ==========================================
        // MÓDULO: USUARIOS
        // ==========================================
        public ObservableCollection<UsuarioTabla> ListaUsuarios { get; } = new ObservableCollection<UsuarioTabla>();
        public ObservableCollection<string> ListaEstratos { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> ListaRoles { get; } = new ObservableCollection<string>();
        public ObservableCollection<DepartamentoItem> ListaDepartamentosCmb { get; } = new ObservableCollection<DepartamentoItem>();

        private string _textoBuscarUsuario;
        public string TextoBuscarUsuario
        {
            get => _textoBuscarUsuario;
            set { if (SetProperty(ref _textoBuscarUsuario, value)) CargarUsuarios(); }
        }

        private UsuarioTabla _usuarioSeleccionado;
        public UsuarioTabla UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set
            {
                if (SetProperty(ref _usuarioSeleccionado, value) && value != null)
                {
                    CargarDetalleUsuario(value);
                }
            }
        }

        private string _fichaUsuario;
        public string FichaUsuario { get => _fichaUsuario; set => SetProperty(ref _fichaUsuario, value); }

        private string _nombreUsuario;
        public string NombreUsuario { get => _nombreUsuario; set => SetProperty(ref _nombreUsuario, value); }

        private string _correoUsuario;
        public string CorreoUsuario { get => _correoUsuario; set => SetProperty(ref _correoUsuario, value); }

        private string _passwordUsuario;
        public string PasswordUsuario { get => _passwordUsuario; set => SetProperty(ref _passwordUsuario, value); }

        private string _estratoSeleccionado;
        public string EstratoSeleccionado
        {
            get => _estratoSeleccionado;
            set
            {
                if (SetProperty(ref _estratoSeleccionado, value))
                {
                    ActualizarRolesPorEstrato();
                }
            }
        }

        private string _rolSeleccionado;
        public string RolSeleccionado { get => _rolSeleccionado; set => SetProperty(ref _rolSeleccionado, value); }

        private DepartamentoItem _deptoSeleccionado;
        public DepartamentoItem DeptoSeleccionado { get => _deptoSeleccionado; set => SetProperty(ref _deptoSeleccionado, value); }

        private bool _isEstatusUsuarioActivo = true;
        public bool IsEstatusUsuarioActivo { get => _isEstatusUsuarioActivo; set => SetProperty(ref _isEstatusUsuarioActivo, value); }

        private bool _isFichaUsuarioEnabled = true;
        public bool IsFichaUsuarioEnabled { get => _isFichaUsuarioEnabled; set => SetProperty(ref _isFichaUsuarioEnabled, value); }

        private string _textoBotonGuardarUsuario = "GUARDAR NUEVO";
        public string TextoBotonGuardarUsuario { get => _textoBotonGuardarUsuario; set => SetProperty(ref _textoBotonGuardarUsuario, value); }

        public ICommand GuardarUsuarioCommand { get; }
        public ICommand LimpiarUsuarioCommand { get; }

        // ==========================================
        // MÓDULO: DEPARTAMENTOS (SAP)
        // ==========================================
        public ObservableCollection<DepartamentoItem> ListaSaps { get; } = new ObservableCollection<DepartamentoItem>();

        private string _textoBuscarSap;
        public string TextoBuscarSap
        {
            get => _textoBuscarSap;
            set { if (SetProperty(ref _textoBuscarSap, value)) CargarTablaSaps(); }
        }

        private DepartamentoItem _sapSeleccionado;
        public DepartamentoItem SapSeleccionado
        {
            get => _sapSeleccionado;
            set
            {
                if (SetProperty(ref _sapSeleccionado, value) && value != null)
                {
                    CargarDetalleSap(value);
                }
            }
        }

        private string _claveSap;
        public string ClaveSap { get => _claveSap; set => SetProperty(ref _claveSap, value); }

        private string _descripcionSap;
        public string DescripcionSap { get => _descripcionSap; set => SetProperty(ref _descripcionSap, value); }

        private bool _isClaveSapEnabled = true;
        public bool IsClaveSapEnabled { get => _isClaveSapEnabled; set => SetProperty(ref _isClaveSapEnabled, value); }

        private string _textoBotonGuardarSap = "GUARDAR NUEVO";
        public string TextoBotonGuardarSap { get => _textoBotonGuardarSap; set => SetProperty(ref _textoBotonGuardarSap, value); }

        public ICommand GuardarSapCommand { get; }
        public ICommand LimpiarSapCommand { get; }

        // ==========================================
        // MÓDULO: ORGANISMOS
        // ==========================================
        public ObservableCollection<OrganismoItem> ListaOrganismos { get; } = new ObservableCollection<OrganismoItem>();

        private string _textoBuscarOrganismo;
        public string TextoBuscarOrganismo
        {
            get => _textoBuscarOrganismo;
            set { if (SetProperty(ref _textoBuscarOrganismo, value)) CargarTablaOrganismos(); }
        }

        private OrganismoItem _organismoSeleccionado;
        public OrganismoItem OrganismoSeleccionado
        {
            get => _organismoSeleccionado;
            set
            {
                if (SetProperty(ref _organismoSeleccionado, value) && value != null)
                {
                    CargarDetalleOrganismo(value);
                }
            }
        }

        private string _idOrganismo;
        public string IdOrganismo { get => _idOrganismo; set => SetProperty(ref _idOrganismo, value); }

        private string _claveOrganismo;
        public string ClaveOrganismo { get => _claveOrganismo; set => SetProperty(ref _claveOrganismo, value); }

        private string _descripcionOrganismo;
        public string DescripcionOrganismo { get => _descripcionOrganismo; set => SetProperty(ref _descripcionOrganismo, value); }

        private string _textoBotonGuardarOrganismo = "GUARDAR NUEVO";
        public string TextoBotonGuardarOrganismo { get => _textoBotonGuardarOrganismo; set => SetProperty(ref _textoBotonGuardarOrganismo, value); }

        public ICommand GuardarOrganismoCommand { get; }
        public ICommand LimpiarOrganismoCommand { get; }

        // ==========================================
        // CONSTRUCTOR
        // ==========================================
        public VentanaAdministradorViewModel()
        {
            MostrarMenuCommand = new RelayCommand(ExecuteMostrarMenu);
            SalirCommand = new RelayCommand(ExecuteSalir);

            GuardarUsuarioCommand = new RelayCommand(ExecuteGuardarUsuario);
            LimpiarUsuarioCommand = new RelayCommand(ExecuteLimpiarUsuario);

            GuardarSapCommand = new RelayCommand(ExecuteGuardarSap);
            LimpiarSapCommand = new RelayCommand(ExecuteLimpiarSap);

            GuardarOrganismoCommand = new RelayCommand(ExecuteGuardarOrganismo);
            LimpiarOrganismoCommand = new RelayCommand(ExecuteLimpiarOrganismo);

            InicializarDatos();
        }

        private void InicializarDatos()
        {
            CargarDepartamentosCmb();
            CargarEstratos();
            
            CargarTablaSaps();
            CargarUsuarios();
            CargarTablaOrganismos();
        }

        // ==========================================
        // NAVEGACIÓN
        // ==========================================
        private void ExecuteMostrarMenu(object parameter)
        {
            string menu = parameter as string;
            IsPanelUsuariosVisible = menu == "Usuarios";
            IsPanelSapVisible = menu == "Departamentos";
            IsPanelOrganismosVisible = menu == "Organismos";
        }

        private void ExecuteSalir(object parameter)
        {
            CerrarVentanaAction?.Invoke();
        }

        // ==========================================
        // MÉTODOS DE USUARIOS
        // ==========================================
        private void CargarEstratos()
        {
            ListaEstratos.Clear();
            ListaEstratos.Add("GERENTE");
            ListaEstratos.Add("SUPERINTENDENTE");
            ListaEstratos.Add("SUBGERENTE");
            ListaEstratos.Add("JEFE");
        }

        private void ActualizarRolesPorEstrato()
        {
            ListaRoles.Clear();
            if (string.IsNullOrEmpty(EstratoSeleccionado)) return;

            if (EstratoSeleccionado == "GERENTE" || EstratoSeleccionado == "SUPERINTENDENTE")
            {
                ListaRoles.Add("ADMINISTRADOR");
                ListaRoles.Add("OPERADOR");
                RolSeleccionado = "ADMINISTRADOR";
            }
            else
            {
                ListaRoles.Add("OPERADOR");
                ListaRoles.Add("ADMINISTRADOR");
                RolSeleccionado = "OPERADOR";
            }
        }

        private void CargarDepartamentosCmb()
        {
            ListaDepartamentosCmb.Clear();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT clave_depto, descripcion FROM Dep_personal";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaDepartamentosCmb.Add(new DepartamentoItem
                                {
                                    Clave = reader["clave_depto"]?.ToString() ?? "",
                                    Descripcion = reader["descripcion"]?.ToString() ?? ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { /* Manejado en llamadas */ }
        }

        private void CargarUsuarios()
        {
            ListaUsuarios.Clear();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        string query = "SELECT Ficha, nombre, Estrato, tipo, Correo, clave_depto, estatus FROM usuario";
                        if (!string.IsNullOrWhiteSpace(TextoBuscarUsuario))
                        {
                            query += " WHERE Ficha LIKE $filtro OR nombre LIKE $filtro";
                        }

                        cmd.CommandText = query;
                        if (!string.IsNullOrWhiteSpace(TextoBuscarUsuario))
                        {
                            cmd.Parameters.AddWithValue("$filtro", "%" + TextoBuscarUsuario + "%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int estatusNum = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 1;
                                ListaUsuarios.Add(new UsuarioTabla
                                {
                                    Ficha = reader["Ficha"]?.ToString() ?? "",
                                    nombre = reader["nombre"]?.ToString() ?? "",
                                    Estrato = reader["Estrato"]?.ToString() ?? "",
                                    tipo = reader["tipo"]?.ToString() ?? "",
                                    Correo = reader["Correo"]?.ToString() ?? "",
                                    ClaveDepto = reader["clave_depto"]?.ToString() ?? "",
                                    EstatusNum = estatusNum,
                                    EstatusTexto = estatusNum == 1 ? "✅ Activo" : "❌ Inactivo"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Aviso de Base de Datos", $"Asegúrate de ejecutar el comando ALTER TABLE en SQLite para la columna estatus.\n\nError: {ex.Message}");
            }
        }

        private void CargarDetalleUsuario(UsuarioTabla u)
        {
            FichaUsuario = u.Ficha;
            NombreUsuario = u.nombre;
            CorreoUsuario = u.Correo;
            DeptoSeleccionado = ListaDepartamentosCmb.FirstOrDefault(d => d.Clave == u.ClaveDepto);

            if (!ListaEstratos.Contains(u.Estrato) && !string.IsNullOrEmpty(u.Estrato))
            {
                ListaEstratos.Add(u.Estrato);
            }
            EstratoSeleccionado = u.Estrato;
            RolSeleccionado = u.tipo;

            PasswordUsuario = "";
            IsEstatusUsuarioActivo = u.EstatusNum == 1;

            IsFichaUsuarioEnabled = false;
            TextoBotonGuardarUsuario = "ACTUALIZAR DATOS";
        }

        private void ExecuteLimpiarUsuario(object parameter)
        {
            UsuarioSeleccionado = null;
            FichaUsuario = "";
            NombreUsuario = "";
            CorreoUsuario = "";
            PasswordUsuario = "";
            EstratoSeleccionado = null;
            RolSeleccionado = null;
            DeptoSeleccionado = null;
            IsEstatusUsuarioActivo = true;

            IsFichaUsuarioEnabled = true;
            TextoBotonGuardarUsuario = "GUARDAR NUEVO";
        }

        private void ExecuteGuardarUsuario(object parameter)
        {
            if (string.IsNullOrWhiteSpace(FichaUsuario) || string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrEmpty(RolSeleccionado) || string.IsNullOrEmpty(EstratoSeleccionado))
            {
                MostrarMensajeAction?.Invoke("Datos incompletos", "Por favor, llena al menos la Ficha, Nombre, Estrato y Rol.");
                return;
            }

            string ficha = FichaUsuario.Trim();
            string nombre = NombreUsuario.Trim();
            string correo = CorreoUsuario?.Trim() ?? "";
            string claveDepto = DeptoSeleccionado?.Clave ?? "";
            int estatus = IsEstatusUsuarioActivo ? 1 : 0;
            string passwordPlana = PasswordUsuario;

            bool enviarCorreo = false;
            bool esNuevo = false;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (IsFichaUsuarioEnabled) // Es nuevo
                        {
                            if (string.IsNullOrWhiteSpace(passwordPlana))
                            {
                                MostrarMensajeAction?.Invoke("Falta contraseña", "Para un usuario nuevo, la contraseña provisional es obligatoria.");
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
                        else // Actualizar
                        {
                            if (string.IsNullOrWhiteSpace(passwordPlana))
                            {
                                cmd.CommandText = @"UPDATE usuario SET nombre = $nombre, Correo = $correo, 
                                                    Estrato = $estrato, clave_depto = $claveDepto, tipo = $tipo, estatus = $estatus
                                                    WHERE Ficha = $ficha";
                            }
                            else
                            {
                                if (ConfirmarBorradoAction != null && !ConfirmarBorradoAction.Invoke()) return;

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
                        cmd.Parameters.AddWithValue("$estrato", EstratoSeleccionado);
                        cmd.Parameters.AddWithValue("$claveDepto", claveDepto);
                        cmd.Parameters.AddWithValue("$tipo", RolSeleccionado);
                        cmd.Parameters.AddWithValue("$estatus", estatus);

                        cmd.ExecuteNonQuery();
                    }
                }

                if (enviarCorreo && !string.IsNullOrWhiteSpace(correo))
                {
                    EnviarCorreoCredenciales(correo, nombre, ficha, passwordPlana, esNuevo);
                }

                MostrarMensajeAction?.Invoke("Éxito", "Datos guardados correctamente.");
                ExecuteLimpiarUsuario(null);
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Error de Base de Datos", $"Error al guardar usuario: {ex.Message}");
            }
        }

        // ==========================================
        // MÉTODOS DE DEPARTAMENTOS (SAP)
        // ==========================================
        private void CargarTablaSaps()
        {
            ListaSaps.Clear();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        string query = "SELECT clave_depto, descripcion FROM Dep_personal";
                        if (!string.IsNullOrWhiteSpace(TextoBuscarSap))
                        {
                            query += " WHERE clave_depto LIKE $filtro OR descripcion LIKE $filtro";
                        }

                        cmd.CommandText = query;
                        if (!string.IsNullOrWhiteSpace(TextoBuscarSap))
                        {
                            cmd.Parameters.AddWithValue("$filtro", "%" + TextoBuscarSap + "%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaSaps.Add(new DepartamentoItem
                                {
                                    Clave = reader["clave_depto"]?.ToString() ?? "",
                                    Descripcion = reader["descripcion"]?.ToString() ?? ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Error", $"Error al cargar departamentos: {ex.Message}");
            }
        }

        private void CargarDetalleSap(DepartamentoItem sap)
        {
            ClaveSap = sap.Clave;
            DescripcionSap = sap.Descripcion;
            IsClaveSapEnabled = false;
            TextoBotonGuardarSap = "ACTUALIZAR DATOS";
        }

        private void ExecuteLimpiarSap(object parameter)
        {
            SapSeleccionado = null;
            ClaveSap = "";
            DescripcionSap = "";
            IsClaveSapEnabled = true;
            TextoBotonGuardarSap = "GUARDAR NUEVO";
        }

        private void ExecuteGuardarSap(object parameter)
        {
            if (string.IsNullOrWhiteSpace(ClaveSap) || string.IsNullOrWhiteSpace(DescripcionSap))
            {
                MostrarMensajeAction?.Invoke("Datos incompletos", "Por favor, llena la Clave y la Descripción.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (IsClaveSapEnabled)
                        {
                            cmd.CommandText = "INSERT INTO Dep_personal (clave_depto, descripcion) VALUES ($clave, $descripcion)";
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE Dep_personal SET descripcion = $descripcion WHERE clave_depto = $clave";
                        }

                        cmd.Parameters.AddWithValue("$clave", ClaveSap.Trim());
                        cmd.Parameters.AddWithValue("$descripcion", DescripcionSap.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }

                MostrarMensajeAction?.Invoke("Éxito", "Departamento guardado correctamente.");
                ExecuteLimpiarSap(null);
                CargarTablaSaps();
                CargarDepartamentosCmb();
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Error de Base de Datos", $"Error al guardar departamento: {ex.Message}");
            }
        }

        // ==========================================
        // MÉTODOS DE ORGANISMOS
        // ==========================================
        private void CargarTablaOrganismos()
        {
            ListaOrganismos.Clear();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        string query = "SELECT id_organismo, Organismo, Descripcion FROM Organismos";
                        if (!string.IsNullOrWhiteSpace(TextoBuscarOrganismo))
                        {
                            query += " WHERE Organismo LIKE $filtro OR Descripcion LIKE $filtro";
                        }

                        cmd.CommandText = query;
                        if (!string.IsNullOrWhiteSpace(TextoBuscarOrganismo))
                        {
                            cmd.Parameters.AddWithValue("$filtro", "%" + TextoBuscarOrganismo + "%");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListaOrganismos.Add(new OrganismoItem
                                {
                                    Id = Convert.ToInt32(reader["id_organismo"]),
                                    Clave = reader["Organismo"]?.ToString() ?? "",
                                    Descripcion = reader["Descripcion"]?.ToString() ?? ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Error", $"Error al cargar Organismos: {ex.Message}");
            }
        }

        private void CargarDetalleOrganismo(OrganismoItem o)
        {
            IdOrganismo = o.Id.ToString();
            ClaveOrganismo = o.Clave;
            DescripcionOrganismo = o.Descripcion;
            TextoBotonGuardarOrganismo = "ACTUALIZAR DATOS";
        }

        private void ExecuteLimpiarOrganismo(object parameter)
        {
            OrganismoSeleccionado = null;
            IdOrganismo = "";
            ClaveOrganismo = "";
            DescripcionOrganismo = "";
            TextoBotonGuardarOrganismo = "GUARDAR NUEVO";
        }

        private void ExecuteGuardarOrganismo(object parameter)
        {
            if (string.IsNullOrWhiteSpace(ClaveOrganismo) || string.IsNullOrWhiteSpace(DescripcionOrganismo))
            {
                MostrarMensajeAction?.Invoke("Datos incompletos", "Por favor, llena la Clave (Siglas) y la Descripción.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (string.IsNullOrEmpty(IdOrganismo))
                        {
                            cmd.CommandText = "INSERT INTO Organismos (Organismo, Descripcion) VALUES ($clave, $descripcion)";
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE Organismos SET Organismo = $clave, Descripcion = $descripcion WHERE id_organismo = $id";
                            cmd.Parameters.AddWithValue("$id", Convert.ToInt32(IdOrganismo));
                        }

                        cmd.Parameters.AddWithValue("$clave", ClaveOrganismo.Trim());
                        cmd.Parameters.AddWithValue("$descripcion", DescripcionOrganismo.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }

                MostrarMensajeAction?.Invoke("Éxito", "Organismo guardado correctamente.");
                ExecuteLimpiarOrganismo(null);
                CargarTablaOrganismos();
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Error de Base de Datos", $"Error al guardar Organismo: {ex.Message}");
            }
        }

        // ==========================================
        // UTILIDADES Y SEGURIDAD
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
            catch (Exception)
            {
                // Ignorar si falla el SMTP local
            }
        }
    }
}
