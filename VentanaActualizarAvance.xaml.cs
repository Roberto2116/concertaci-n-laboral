using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using Proyecto_GRRLN_expediente.db;

namespace Proyecto_GRRLN_expediente
{
    public partial class VentanaActualizarAvance : Wpf.Ui.Controls.FluentWindow
    {
        private int _idAsunto;
        private bool _isUpdating = false;

        public VentanaActualizarAvance(int idAsunto, int avanceActual)
        {
            InitializeComponent();
            _idAsunto = idAsunto;
            LblExpediente.Text = $"Expediente #{_idAsunto}";

            SldAvance.Value = avanceActual;
            TxtPorcentaje.Text = avanceActual.ToString();
        }

        private void SldAvance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdating) return;

            _isUpdating = true;
            int nuevoValor = (int)e.NewValue;

            if (TxtPorcentaje != null)
            {
                TxtPorcentaje.Text = nuevoValor.ToString();
            }

            _isUpdating = false;
        }

        private void TxtPorcentaje_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating || SldAvance == null) return;

            _isUpdating = true;

            if (int.TryParse(TxtPorcentaje.Text, out int nuevoValor))
            {
                if (nuevoValor > 100)
                {
                    nuevoValor = 100;
                    TxtPorcentaje.Text = "100";
                    TxtPorcentaje.CaretIndex = TxtPorcentaje.Text.Length;
                }

                SldAvance.Value = nuevoValor;
            }
            else if (string.IsNullOrEmpty(TxtPorcentaje.Text))
            {
                SldAvance.Value = 0;
            }

            _isUpdating = false;
        }

        private void TxtPorcentaje_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            int nuevoAvance = (int)SldAvance.Value;

            // --- LÓGICA DE ESTATUS ---
            int nuevoIdEstatus = (nuevoAvance == 0) ? 1 : (nuevoAvance == 100) ? 3 : 2;

            // --- LÓGICA DE FECHA DE ATENCIÓN (NUEVA REGLA) ---
            // El simple hecho de darle "Guardar" significa que se atendió el asunto, 
            // por lo tanto, la fecha siempre se actualiza a hoy.
            string fechaAtencionActual = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                SqliteConnection conn = DatabaseConnection.GetConnection();
                if (conn == null) return;

                using (var command = conn.CreateCommand())
                {
                    // Actualizamos avance, estatus y SIEMPRE la Fecha_atencion
                    command.CommandText = @"
                        UPDATE Asuntos 
                        SET Porcentaje_avance = $avance, 
                            Id_estatus = $idEst,
                            Fecha_atencion = $fechaAte
                        WHERE Id_asunto = $id";

                    command.Parameters.AddWithValue("$avance", nuevoAvance);
                    command.Parameters.AddWithValue("$idEst", nuevoIdEstatus);
                    command.Parameters.AddWithValue("$fechaAte", fechaAtencionActual);
                    command.Parameters.AddWithValue("$id", _idAsunto);

                    command.ExecuteNonQuery();
                }

                string nombreEstado = (nuevoIdEstatus == 1) ? "NUEVO" :
                                     (nuevoIdEstatus == 3) ? "ATENDIDO" : "EN PROCESO";

                MessageBox.Show($"Actualización exitosa.\nAvance: {nuevoAvance}%\nEstatus: {nombreEstado}",
                                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }
        }
    }
}