using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaBitacora : Wpf.Ui.Controls.FluentWindow
    {
        private int _idAsunto;

        public VistaBitacora(int idAsunto)
        {
            InitializeComponent();
            _idAsunto = idAsunto;

            TxtTitulo.Text = $"SEGUIMIENTO DEL ASUNTO N° {_idAsunto}";

            CargarHistorial();
        }

        private void CargarHistorial()
        {
            List<BitacoraModel> listaHistorial = new List<BitacoraModel>();

            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                using (var cmd = conn.CreateCommand())
                {
                    // Adaptado a la tabla Seguimiento y sus columnas reales
                    cmd.CommandText = "SELECT id_seguimiento, fecha_Seguimiento, Descripcion FROM Seguimiento WHERE num_Asunto = $id ORDER BY fecha_Seguimiento DESC";
                    cmd.Parameters.AddWithValue("$id", _idAsunto);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaHistorial.Add(new BitacoraModel
                            {
                                IdBitacora = reader.GetInt32(0).ToString(),
                                Fecha = reader.GetString(1),
                                Descripcion = reader.GetString(2)
                            });
                        }
                    }
                }

                dgBitacora.ItemsSource = listaHistorial;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNuevaBitacora.Text))
            {
                MessageBox.Show("Por favor, escribe un comentario o avance antes de guardar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                using (var cmd = conn.CreateCommand())
                {
                    // Hacemos 2 cosas: Insertamos la bitácora Y actualizamos la Fecha_atencion del asunto
                    cmd.CommandText = @"
                INSERT INTO Seguimiento (num_Asunto, fecha_Seguimiento, Descripcion) 
                VALUES ($id, $fechaHora, $descripcion);

                UPDATE Asuntos 
                SET Fecha_atencion = $fechaHoy 
                WHERE Id_asunto = $id;";

                    cmd.Parameters.AddWithValue("$id", _idAsunto);
                    cmd.Parameters.AddWithValue("$fechaHora", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); // Para la bitácora (con hora)
                    cmd.Parameters.AddWithValue("$fechaHoy", DateTime.Now.ToString("yyyy-MM-dd")); // Para el asunto (solo fecha)
                    cmd.Parameters.AddWithValue("$descripcion", TxtNuevaBitacora.Text.Trim());

                    cmd.ExecuteNonQuery();
                }

                TxtNuevaBitacora.Clear();
                CargarHistorial();

                MessageBox.Show("¡Avance guardado y fecha de atención actualizada!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el avance:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class BitacoraModel
    {
        public string IdBitacora { get; set; }
        public string Fecha { get; set; }
        public string Descripcion { get; set; }
    }
}