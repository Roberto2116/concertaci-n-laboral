public class Asunto
{
    public int IdAsunto { get; set; } // ¡Nuestro folio autoincremental!
    public string FechaRecepcion { get; set; }
    public string Tipo { get; set; }
    public string NombreOficio { get; set; }
    public string FechaOficio { get; set; }
    public int IdSap { get; set; }
    public int ClaveDepto { get; set; }
    public string Agenda { get; set; }
    public int SecSindical { get; set; }
    public int IdOrganismo { get; set; }
    public int ClaveCentroTrabajo { get; set; }
    public int IdDescripcionCorta { get; set; }
    public int IdEstatus { get; set; }
    public string Ficha { get; set; } // El responsable

    // Estos tres son opcionales (pueden ser nulos), por eso llevan el "?"
    public string? Instruccion { get; set; }
    public string? Observaciones { get; set; }
    public string? FechaAtencion { get; set; }

    public string FechaCompromiso { get; set; }
    public int PorcentajeAvance { get; set; }
}