using Microsoft.AspNetCore.Mvc;
using OLN.API.Data;
using OLN.API.DTO;
using OLN.API.Models;

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
  public IActionResult CreateShipmentOrder([FromBody] CreateOrderDTO order)
  {
    ShipmentOrder created = new()
    {
      IdShipmentOrder = Repository.Current.Orders.Count,
      Origin = order.Origin,
      Destination = order.Destination,
      Buyer = order.Buyer,
      DeliveryMan = null,
      Product = order.Product,
      State = ShipmentState.Created
    };
    Repository.Current.Orders.Add(created);
    return Ok(created);
  }

  // NO ES NECESARIO
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
  public IActionResult PostDeliveryMan(int id, [FromBody] int IdDeliveryMan)
  {
    return Ok();
  }

  [HttpPost("{id:int}/entrega")]
  public IActionResult OrderDelivered(int id, [FromBody] int IdShipmentOrder)
  {
    return Ok();
  }
}
