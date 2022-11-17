using System.ComponentModel.DataAnnotations;

namespace OLN.API.DTO
{
  public class CrearOrdenDTO
  {
    [Required]
    public int? idEnvio { get; set; } = null;
    public List<string> direccionOrigen { get; set; }
    public List<string> direccionDestino { get; set; }
    public string contactoComprador { get; set; }
    public string detalleProducto { get; set; }
  }
}