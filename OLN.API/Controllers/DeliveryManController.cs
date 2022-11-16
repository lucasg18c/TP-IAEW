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
    public IActionResult GetDeliveryMans()
    {
      return Ok(Repository.Current.DeliveryMans);
    }

    [HttpPost]
    public IActionResult PostDeliveryManBy([FromBody] DeliveryMan deliveryMan)
    {
      deliveryMan.IdDeliveryMan = Repository.Current.DeliveryMans.Count;
      Repository.Current.DeliveryMans.Add(deliveryMan);
      return Ok(deliveryMan);
    }

    [HttpDelete]
    public IActionResult DeleteDeliveryManById([FromBody] int IdDeliveryMan)
    {
      var found = Repository.Current.FindDeliveryManById(IdDeliveryMan);

      if (found is null) return NotFound($"No se encontró el repartidor con id {IdDeliveryMan}");

      Repository.Current.DeliveryMans.Remove(found);
      return Ok(found);
    }
  }
}