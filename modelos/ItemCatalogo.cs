namespace Proyecto_GRRLN_expediente.modelos
{
    public class ItemCatalogo
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public long IdPadre { get; set; } 
        public long IdGerencia { get; set; }
    }
}
