using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLN.API.Data;
using OLN.API.Models;

namespace OLN.API.Controllers
{
  [ApiController]
  [Route("/api/repartidores")]
  public class DeliveryManController : Controller
  {
    private readonly ILogger<DeliveryManController> _logger;

    public DeliveryManController(ILogger<DeliveryManController> logger)
    {
      _logger = logger;
    }

    [HttpGet]
    [Authorize("read:deliverymans")]
    public IActionResult GetDeliveryMans()
    {
      return Ok(Repository.Current.DeliveryMans);
    }

    [HttpPost]
    [Authorize("write:deliverymans")]
    public IActionResult PostDeliveryManBy([FromBody] Repartidor deliveryMan)
    {
      deliveryMan.IdRepartidor = Repository.Current.SiguienteId;
      Repository.Current.DeliveryMans.Add(deliveryMan);
      return Ok(deliveryMan);
    }

    [HttpDelete]
    [Authorize("delete:deliverymans")]
    public IActionResult DeleteDeliveryManById([FromBody] int IdRepartidor)
    {
      var found = Repository.Current.FindDeliveryManById(IdRepartidor);

      if (found is null) return NotFound($"No se encontró el repartidor con id {IdRepartidor}");

      foreach (var e in Repository.Current.Orders)
      {
        if (e.Repartidor != null && e.Repartidor.IdRepartidor == IdRepartidor)
        {
          return BadRequest("No se puede eliminar al repartidor ya que tiene ordenes vinculadas.");
        }
      }

      Repository.Current.DeliveryMans.Remove(found);
      return Ok(found);
    }
  }
}