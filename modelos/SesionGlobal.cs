namespace Proyecto_GRRLN_expediente
{
    public static class SesionGlobal
    {
        public static string Ficha { get; set; }
        public static string Nombre { get; set; }
        public static string TipoRol { get; set; }
        public static string Estrato { get; set; }
        public static string ClaveDepto { get; set; }

        public static long IdSesionActual { get; set; }
    }
}