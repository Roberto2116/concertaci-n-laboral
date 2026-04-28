using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Proyecto_GRRLN_expediente.ViewModels.Base;
using Proyecto_GRRLN_expediente.db;
using Proyecto_GRRLN_expediente.modelos;

namespace Proyecto_GRRLN_expediente.ViewModels
{
    public class VistaCrearAvisoViewModel : ViewModelBase
    {
        public Action CerrarVentanaAction { get; set; }
        public Action<string, string> MostrarMensajeAction { get; set; }

        // =======================================================
        // PROPIEDADES DE ENVÍO DE AVISO
        // =======================================================
        private bool _isAlcanceUsuario = true;
        public bool IsAlcanceUsuario
        {
            get => _isAlcanceUsuario;
            set
            {
                if (SetProperty(ref _isAlcanceUsuario, value) && value)
                {
                    CargarDestinatarios();
                }
            }
        }

        private bool _isAlcanceDepto;
        public bool IsAlcanceDepto
        {
            get => _isAlcanceDepto;
            set
            {
                if (SetProperty(ref _isAlcanceDepto, value) && value)
                {
                    CargarDestinatarios();
                }
            }
        }

        private bool _isAlcanceSAP;
        public bool IsAlcanceSAP
        {
            get => _isAlcanceSAP;
            set
            {
                if (SetProperty(ref _isAlcanceSAP, value) && value)
                {
                    CargarDestinatarios();
                }
            }
        }

        private bool _isBuscadorHabilitado = true;
        public bool IsBuscadorHabilitado { get => _isBuscadorHabilitado; set => SetProperty(ref _isBuscadorHabilitado, value); }

        public ObservableCollection<DestinatarioModel> ListaDestinatarios { get; } = new ObservableCollection<DestinatarioModel>();

        private DestinatarioModel _destinatarioSeleccionado;
        public DestinatarioModel DestinatarioSeleccionado { get => _destinatarioSeleccionado; set => SetProperty(ref _destinatarioSeleccionado, value); }

        private string _contenidoMensaje;
        public string ContenidoMensaje { get => _contenidoMensaje; set => SetProperty(ref _contenidoMensaje, value); }

        // =======================================================
        // PROPIEDADES DE HISTORIAL DE AVISOS ARCHIVADOS
        // =======================================================
        private List<AvisoModel> _listaHistorialCompleta = new List<AvisoModel>();
        public ObservableCollection<AvisoModel> ListaHistorialVisible { get; } = new ObservableCollection<AvisoModel>();

        private string _textoBusquedaHistorial;
        public string TextoBusquedaHistorial
        {
            get => _textoBusquedaHistorial;
            set
            {
                if (SetProperty(ref _textoBusquedaHistorial, value))
                {
                    FiltrarHistorial();
                }
            }
        }

        // =======================================================
        // COMANDOS
        // =======================================================
        public ICommand RegresarCommand { get; }
        public ICommand EnviarMensajeCommand { get; }
        public ICommand RestaurarMensajeCommand { get; }

        public VistaCrearAvisoViewModel()
        {
            RegresarCommand = new RelayCommand(ExecuteRegresar);
            EnviarMensajeCommand = new RelayCommand(ExecuteEnviarMensaje);
            RestaurarMensajeCommand = new RelayCommand(ExecuteRestaurarMensaje);

            CargarDestinatarios();
            CargarHistorial();
        }

        private void CargarDestinatarios()
        {
            ListaDestinatarios.Clear();

            if (IsAlcanceSAP)
            {
                IsBuscadorHabilitado = false;
                DestinatarioSeleccionado = null;
                return;
            }

            IsBuscadorHabilitado = true;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (IsAlcanceUsuario)
                        {
                            cmd.CommandText = "SELECT Ficha, nombre FROM usuario";
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ListaDestinatarios.Add(new DestinatarioModel
                                    {
                                        Id = reader[0].ToString(),
                                        Nombre = $"{reader[0].ToString()} - {reader[1].ToString()}"
                                    });
                                }
                            }
                        }
                        else if (IsAlcanceDepto)
                        {
                            cmd.CommandText = "SELECT DISTINCT clave_depto FROM usuario WHERE clave_depto IS NOT NULL";
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string depto = reader[0].ToString();
                                    ListaDestinatarios.Add(new DestinatarioModel
                                    {
                                        Id = depto,
                                        Nombre = $"DEPARTAMENTO {depto}"
                                    });
                                }
                            }
                        }
                    }
                }

                if (ListaDestinatarios.Any())
                {
                    DestinatarioSeleccionado = ListaDestinatarios.First();
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Error", $"Error al cargar destinatarios: {ex.Message}");
            }
        }

        private void ExecuteEnviarMensaje(object parameter)
        {
            if (string.IsNullOrWhiteSpace(ContenidoMensaje))
            {
                MostrarMensajeAction?.Invoke("Aviso", "Por favor, escribe el contenido del mensaje.");
                return;
            }

            try
            {
                string alcance = IsAlcanceUsuario ? "PERSONAL" : (IsAlcanceSAP ? "SAP" : "DEPTO");
                string destinoTexto = "";

                if (alcance != "SAP")
                {
                    if (DestinatarioSeleccionado != null)
                    {
                        destinoTexto = DestinatarioSeleccionado.Id.Trim();
                    }
                    else
                    {
                        MostrarMensajeAction?.Invoke("Aviso", "Por favor, selecciona un destinatario de la lista.");
                        return;
                    }
                }

                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO Mensaje (ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino) 
                            VALUES (@ficha, @msj, @fecha, @alcance, @fichaDest, @claveDest)";

                        string fichaActiva = SesionSistema.UsuarioLogueado != null ? SesionSistema.UsuarioLogueado.Ficha : "admin";
                        command.Parameters.AddWithValue("@ficha", fichaActiva);
                        command.Parameters.AddWithValue("@msj", ContenidoMensaje.Trim().ToUpper());
                        command.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@alcance", alcance);

                        if (alcance == "PERSONAL")
                        {
                            command.Parameters.AddWithValue("@fichaDest", destinoTexto);
                            command.Parameters.AddWithValue("@claveDest", DBNull.Value);
                        }
                        else if (alcance == "DEPTO")
                        {
                            command.Parameters.AddWithValue("@fichaDest", DBNull.Value);
                            if (int.TryParse(destinoTexto, out int deptoNum))
                            {
                                command.Parameters.AddWithValue("@claveDest", deptoNum);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@claveDest", DBNull.Value);
                            }
                        }
                        else // SAP / Global
                        {
                            command.Parameters.AddWithValue("@fichaDest", DBNull.Value);
                            command.Parameters.AddWithValue("@claveDest", DBNull.Value);
                        }

                        command.ExecuteNonQuery();
                    }
                }

                MostrarMensajeAction?.Invoke("Éxito", "Aviso enviado correctamente.");
                ContenidoMensaje = string.Empty;
                CargarHistorial();
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Error DB", "Error al publicar: " + ex.Message);
            }
        }

        private void CargarHistorial()
        {
            string miFicha = SesionGlobal.Ficha;
            string miDepto = SesionGlobal.ClaveDepto;

            _listaHistorialCompleta.Clear();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT Id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Archivado 
                            FROM Mensaje 
                            WHERE Archivado = 1 AND ( 
                                 (Tipo_alcance = 'PERSONAL' AND Ficha_destino = @ficha)
                              OR (Tipo_alcance = 'DEPTO' AND clave_depto_destino = @depto)
                              OR (Tipo_alcance = 'SAP')
                            )
                            ORDER BY fecha_posteo DESC;";

                        command.Parameters.AddWithValue("@ficha", miFicha ?? "");
                        if (int.TryParse(miDepto, out int deptoInt))
                        {
                            command.Parameters.AddWithValue("@depto", deptoInt);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@depto", DBNull.Value);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _listaHistorialCompleta.Add(new AvisoModel
                                {
                                    IdMensaje = Convert.ToInt32(reader[0]),
                                    Emisor = "De: " + reader[1].ToString(),
                                    TextoMensaje = reader[2].ToString(),
                                    Fecha = reader[3].ToString(),
                                    TipoAlcance = reader[4].ToString(),
                                    Archivado = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader[5])
                                });
                            }
                        }
                    }
                }

                TextoBusquedaHistorial = string.Empty;
                FiltrarHistorial();
            }
            catch (Exception ex)
            {
                MostrarMensajeAction?.Invoke("Error DB", $"Error al cargar el historial de archivados: {ex.Message}");
            }
        }

        private void FiltrarHistorial()
        {
            ListaHistorialVisible.Clear();

            string textoBusqueda = TextoBusquedaHistorial?.ToLower().Trim() ?? string.Empty;

            var filtrada = string.IsNullOrEmpty(textoBusqueda) 
                ? _listaHistorialCompleta 
                : _listaHistorialCompleta.Where(m => 
                    (m.Emisor != null && m.Emisor.ToLower().Contains(textoBusqueda)) ||
                    (m.TextoMensaje != null && m.TextoMensaje.ToLower().Contains(textoBusqueda)));

            foreach (var item in filtrada)
            {
                ListaHistorialVisible.Add(item);
            }
        }

        private void ExecuteRestaurarMensaje(object parameter)
        {
            if (parameter is int idMensaje)
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        if (conn == null) return;
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = "UPDATE Mensaje SET Archivado = 0 WHERE Id_mensaje = @id";
                            command.Parameters.AddWithValue("@id", idMensaje);
                            command.ExecuteNonQuery();
                        }
                    }

                    CargarHistorial();
                }
                catch (Exception ex)
                {
                    MostrarMensajeAction?.Invoke("Error DB", $"Error al restaurar el mensaje:\n{ex.Message}");
                }
            }
        }

        private void ExecuteRegresar(object parameter)
        {
            CerrarVentanaAction?.Invoke();
        }
    }

    public class DestinatarioModel
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }
}
