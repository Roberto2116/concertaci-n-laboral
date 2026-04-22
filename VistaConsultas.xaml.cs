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

    public class SemaforoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var modelo = value as ResultadoConsultaModel;
            if (modelo == null) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));

            if (!string.IsNullOrEmpty(modelo.EstatusNombre) && (modelo.EstatusNombre.ToUpper().Contains("COMPLETADO") || modelo.EstatusNombre.ToUpper().Contains("ATENDIDO")))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00843D"));

            if (string.IsNullOrWhiteSpace(modelo.FechaLimite)) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));

            DateTime f;
            if (DateTime.TryParse(modelo.FechaLimite, out f))
            {
                TimeSpan t = f.Date - DateTime.Now.Date;
                if (t.TotalDays < 0) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8102E"));
                if (t.TotalDays <= 3) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F"));
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}