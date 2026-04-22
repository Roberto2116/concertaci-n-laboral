namespace Proyecto_GRRLN_expediente.modelos
{
    public class UsuarioTabla
    {
        public string Ficha { get; set; }
        public string nombre { get; set; }
        public string Correo { get; set; }
        public string Estrato { get; set; }
        public string ClaveDepto { get; set; }
        public string tipo { get; set; }
        public int EstatusNum { get; set; }
        public string EstatusTexto { get; set; }
    }

    public class DepartamentoItem
    {
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public string DisplayTexto { get { return $"{Clave} - {Descripcion}"; } }
    }

    public class OrganismoItem
    {
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
    }
}
