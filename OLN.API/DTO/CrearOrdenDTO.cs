namespace OLN.API.DTO
{
  public class CrearOrdenDTO
  {
    public List<string> direccionOrigen { get; set; }
    public List<string> direccionDestino { get; set; }
    public string contactoComprador { get; set; }
    public string detalleProducto { get; set; }
  }
}