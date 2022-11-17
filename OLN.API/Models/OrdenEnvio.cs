namespace OLN.API.Models
{
  public class OrdenEnvio
  {
    public int IdOrdenEnvio { get; set; }
    public int IdEnvio { get; set; }
    public List<string> DireccionOrigen { get; set; } = new();
    public List<string> DireccionDestino { get; set; } = new();
    public string ContactoComprador { get; set; }
    public string DetalleProducto { get; set; }
    public Repartidor? Repartidor { get; set; }
    public List<CambioEstadoOrden> Estados { get; set; } = new();

    public CambioEstadoOrden EstadoActual()
    {
      foreach (var estado in Estados)
      {
        if (estado.FechaBaja is null)
        {
          return estado;
        }
      }
      throw new ApplicationException($"No se encontró el estado actual de la orden con Id {IdOrdenEnvio}");
    }
  }
}