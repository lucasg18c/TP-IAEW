namespace OLN.API.Models
{
  public class CambioEstadoOrden
  {
    public EstadoOrden Estado { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
  }
}