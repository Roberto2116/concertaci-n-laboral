using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Proyecto_GRRLN_expediente.ViewModels;

namespace Proyecto_GRRLN_expediente
{
    public class SemaforoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Intentamos obtener el modelo. Si no, usamos el color gris por defecto.
            dynamic modelo = value;
            try 
            {
                if (modelo == null) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));

                string estatus = modelo.EstatusNombre ?? "";
                if (!string.IsNullOrEmpty(estatus) && (estatus.ToUpper().Contains("COMPLETADO") || estatus.ToUpper().Contains("ATENDIDO")))
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00843D"));

                string fechaLimiteStr = modelo.FechaLimite;
                if (string.IsNullOrWhiteSpace(fechaLimiteStr)) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));

                DateTime f;
                if (DateTime.TryParse(fechaLimiteStr, out f))
                {
                    TimeSpan t = f.Date - DateTime.Now.Date;
                    if (t.TotalDays < 0) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8102E"));
                    if (t.TotalDays <= 3) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F"));
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
                }
            }
            catch { }
            
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5D8DC"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => throw new NotImplementedException();
    }
}
