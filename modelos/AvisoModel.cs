using System.Windows.Media;

namespace Proyecto_GRRLN_expediente.modelos
{
    public class AvisoModel
    {
        public int IdMensaje { get; set; }
        public string Emisor { get; set; }
        public string TextoMensaje { get; set; }
        public string Fecha { get; set; }
        public string TipoAlcance { get; set; }
        public int Archivado { get; set; }

        public string AvisoSymbol => TipoAlcance == "PERSONAL" ? "Person24" : (TipoAlcance == "DEPTO" ? "Building24" : "Megaphone24");

        public string TextoEtiqueta
        {
            get
            {
                if (TipoAlcance == "PERSONAL") return "AVISO DIRECTO";
                if (TipoAlcance == "DEPTO") return "AVISO DEPARTAMENTAL";
                return "COMUNICADO GENERAL";
            }
        }

        public string HexColor
        {
            get
            {
                if (TipoAlcance == "PERSONAL") return "#9B59B6"; // Morado
                if (TipoAlcance == "DEPTO") return "#3498DB";    // Azul
                return "#EF4444";                                // Rojo para global
            }
        }

        public Brush ColorEtiqueta
        {
            get
            {
                if (TipoAlcance == "PERSONAL") return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B59B6"));
                if (TipoAlcance == "DEPTO") return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
            }
        }
    }
}
