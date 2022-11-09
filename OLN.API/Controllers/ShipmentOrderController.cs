using Microsoft.AspNetCore.Mvc;
using OLN.API.Data;
using OLN.API.DTO;

namespace OLN.API.Controllers;

[ApiController]
[Route("api/ordenes_envio")]
public class ShipmentOrderController : ControllerBase
{
  private readonly ILogger<ShipmentOrderController> _logger;

  public ShipmentOrderController(ILogger<ShipmentOrderController> logger)
  {
    _logger = logger;
  }

  [HttpPost]
  public IActionResult CreateShipmentOrder([FromBody] ShipmentOrderDTO order)
  {
    Repository.Current.Orders.Add(order);
    return Ok();
  }

  [HttpGet]
  public IActionResult GetOrders()
  {
    return Ok(Repository.Current.Orders);
  }

  [HttpGet("{id:int}")]
  public IActionResult GetOrder(int id)
  {
    foreach (var e in Repository.Current.Orders)
    {
      if (e.IdShipmentOrder == id)
      {
        return Ok(e);
      }
    }
    return NotFound();
  }

  [HttpPost("{id:int}/repartidor")]
  public IActionResult PostDeliveryMan(int id, [FromBody] DeliveryMan deliveryMan)
  {
    return Ok();
  }
}
