using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaCrearAviso : Wpf.Ui.Controls.FluentWindow
    {
        private List<AvisoModel> listaHistorialCompleta = new List<AvisoModel>();

        public VistaCrearAviso()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDestinatarios();
            CargarHistorial(); // Carga el buzón de mensajes ARCHIVADOS
        }

        // ======================================================================
        // LÓGICA DEL FORMULARIO DE ENVÍO
        // ======================================================================
        private void RbAlcance_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                CargarDestinatarios();
            }
        }

        private void CargarDestinatarios()
        {
            if (RbSAP.IsChecked == true)
            {
                CmbBuscadorGlobal.ItemsSource = null;
                CmbBuscadorGlobal.IsEnabled = false;
                return;
            }

            CmbBuscadorGlobal.IsEnabled = true;
            List<DestinatarioModel> lista = new List<DestinatarioModel>();
            SqliteConnection conn = DatabaseConnection.GetConnection();

            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    if (RbUsuario.IsChecked == true)
                    {
                        cmd.CommandText = "SELECT Ficha, nombre FROM usuario";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new DestinatarioModel
                                {
                                    Id = reader.GetString(0),
                                    Nombre = $"{reader.GetString(0)} - {reader.GetString(1)}"
                                });
                            }
                        }
                    }
                    else if (RbDepto.IsChecked == true)
                    {
                        cmd.CommandText = "SELECT DISTINCT clave_depto FROM usuario WHERE clave_depto IS NOT NULL";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string depto = reader.GetString(0);
                                lista.Add(new DestinatarioModel
                                {
                                    Id = depto,
                                    Nombre = $"DEPARTAMENTO {depto}"
                                });
                            }
                        }
                    }
                }

                CmbBuscadorGlobal.ItemsSource = lista;
                CmbBuscadorGlobal.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar destinatarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void BtnEnviarGlobal_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtContenidoGlobal.Text))
            {
                MessageBox.Show("Por favor, escribe el contenido del mensaje.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string alcance = RbUsuario.IsChecked == true ? "PERSONAL" : (RbSAP.IsChecked == true ? "SAP" : "DEPTO");
                string destinoTexto = "";

                if (alcance != "SAP")
                {
                    if (CmbBuscadorGlobal.SelectedItem is DestinatarioModel seleccionado)
                    {
                        destinoTexto = seleccionado.Id.Trim();
                    }
                    else
                    {
                        MessageBox.Show("Por favor, selecciona un destinatario de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                SqliteConnection conn = DatabaseConnection.GetConnection();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Mensaje (ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Ficha_destino, clave_depto_destino) 
                        VALUES ($ficha, $msj, $fecha, $alcance, $fichaDest, $claveDest)";

                    string fichaActiva = SesionSistema.UsuarioLogueado != null ? SesionSistema.UsuarioLogueado.Ficha : "admin";
                    command.Parameters.AddWithValue("$ficha", fichaActiva);
                    command.Parameters.AddWithValue("$msj", TxtContenidoGlobal.Text.Trim().ToUpper());
                    command.Parameters.AddWithValue("$fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("$alcance", alcance);

                    if (alcance == "PERSONAL")
                    {
                        command.Parameters.AddWithValue("$fichaDest", destinoTexto);
                        command.Parameters.AddWithValue("$claveDest", DBNull.Value);
                    }
                    else if (alcance == "DEPTO")
                    {
                        command.Parameters.AddWithValue("$fichaDest", DBNull.Value);
                        if (long.TryParse(destinoTexto, out long deptoNum))
                        {
                            command.Parameters.AddWithValue("$claveDest", deptoNum);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("$claveDest", destinoTexto);
                        }
                    }
                    else // SAP / Global
                    {
                        command.Parameters.AddWithValue("$fichaDest", DBNull.Value);
                        command.Parameters.AddWithValue("$claveDest", DBNull.Value);
                    }

                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Aviso enviado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                TxtContenidoGlobal.Clear();
                CargarHistorial(); // Refrescamos por si queremos ver los mensajes (aunque los nuevos no estarán archivados aún)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al publicar: " + ex.Message, "Error DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }


        // ======================================================================
        // LÓGICA DEL HISTORIAL / BUZÓN DE ARCHIVADOS
        // ======================================================================

        private void CargarHistorial()
        {
            // Extraemos los datos de la sesión para mostrar solo lo que le corresponde al usuario
            string miFicha = SesionGlobal.Ficha;
            string miDepto = SesionGlobal.ClaveDepto;

            listaHistorialCompleta.Clear();
            SqliteConnection conn = DatabaseConnection.GetConnection();

            try
            {
                using (var command = conn.CreateCommand())
                {
                    // LÓGICA: Traer solo los mensajes con Archivado = 1 (Ocultos de la bandeja principal)
                    // que fueron enviados hacia el usuario activo (o a su departamento, o globales)
                    // Opcional: También podrías agregar una condición "OR ficha = $ficha" si quieres ver los que tú enviaste.

                    command.CommandText = @"
                        SELECT Id_mensaje, ficha, mensaje_enviado, fecha_posteo, Tipo_alcance, Archivado 
                        FROM Mensaje 
                        WHERE Archivado = 1 AND ( 
                             (Tipo_alcance = 'PERSONAL' AND Ficha_destino = $ficha)
                          OR (Tipo_alcance = 'DEPTO' AND clave_depto_destino = $depto)
                          OR (Tipo_alcance = 'SAP')
                        )
                        ORDER BY fecha_posteo DESC;";

                    command.Parameters.AddWithValue("$ficha", miFicha ?? "");
                    command.Parameters.AddWithValue("$depto", miDepto ?? "");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaHistorialCompleta.Add(new AvisoModel
                            {
                                IdMensaje = reader.GetInt32(0),
                                Emisor = "De: " + reader.GetString(1),
                                TextoMensaje = reader.GetString(2),
                                Fecha = reader.GetString(3),
                                TipoAlcance = reader.GetString(4),
                                Archivado = reader.IsDBNull(5) ? 0 : reader.GetInt32(5)
                            });
                        }
                    }
                }

                icHistorial.ItemsSource = listaHistorialCompleta;

                if (TxtBuscarHistorial != null) TxtBuscarHistorial.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial de archivados: {ex.Message}", "Error DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void TxtBuscarHistorial_TextChanged(object sender, TextChangedEventArgs e)
        {
            string textoBuscado = TxtBuscarHistorial.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(textoBuscado))
            {
                icHistorial.ItemsSource = listaHistorialCompleta;
            }
            else
            {
                var listaFiltrada = listaHistorialCompleta.Where(m =>
                    m.Emisor.ToLower().Contains(textoBuscado) ||
                    m.TextoMensaje.ToLower().Contains(textoBuscado)
                ).ToList();

                icHistorial.ItemsSource = listaFiltrada;
            }
        }

        private void BtnRestaurar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is AvisoModel mensajeSeleccionado)
            {
                try
                {
                    SqliteConnection conn = DatabaseConnection.GetConnection();
                    using (var command = conn.CreateCommand())
                    {
                        // Regresamos el campo Archivado a 0 para que vuelva a la bandeja de inicio
                        command.CommandText = "UPDATE Mensaje SET Archivado = 0 WHERE Id_mensaje = $id";
                        command.Parameters.AddWithValue("$id", mensajeSeleccionado.IdMensaje);
                        command.ExecuteNonQuery();
                    }

                    // Recargamos el historial (el mensaje desaparecerá de aquí porque ya no es "Archivado = 1")
                    CargarHistorial();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al restaurar el mensaje:\n{ex.Message}", "Error DB", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DatabaseConnection.CloseConnection();
                }
            }
        }

        // ======================================================================
        // CONTROL DE NAVEGACIÓN
        // ======================================================================
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            VolverAlInicio();
        }

        private void VolverAlInicio()
        {
            this.Close();
        }
    }

    // ======================================================================
    // MODELOS DE DATOS
    // ======================================================================
    public class DestinatarioModel
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }
}