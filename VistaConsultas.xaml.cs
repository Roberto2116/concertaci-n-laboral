using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public partial class VistaConsultas : Wpf.Ui.Controls.FluentWindow
    {
        public VistaConsultas()
        {
            InitializeComponent();
            
            var viewModel = new VistaConsultasViewModel();
            
            viewModel.CerrarVentanaAction = () =>
            {
                this.Close();
            };

            viewModel.AbrirEdicionAction = (idAsunto) =>
            {
                try
                {
                    VistaEditarExpediente ventanaEdicion = new VistaEditarExpediente(idAsunto);
                    ventanaEdicion.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al abrir la ventana de edición: {ex.Message}");
                }
            };

            this.DataContext = viewModel;
        }
    }
}