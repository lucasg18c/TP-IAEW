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
    var found = Repository.Current.FindShipmentOrderById(id);

    if (found is null) return NotFound($"No se encontró la orden con Id {id}");

    return Ok(found);
  }

  [HttpPost("{id:int}/repartidor")]
  public IActionResult PostDeliveryMan(int id, [FromBody] int IdDeliveryMan)
  {
    var orderFound = Repository.Current.FindShipmentOrderById(id);
    if (orderFound is null) return NotFound($"No se encontró la orden con Id {id}");

    var deliveryManFound = Repository.Current.FindDeliveryManById(IdDeliveryMan);
    if (deliveryManFound is null) return NotFound($"No se encontró el repartidor con id {IdDeliveryMan}");

    orderFound.DeliveryMan = deliveryManFound;

    return Ok("Repartidor asignado");
  }

  [HttpPost("{id:int}/entrega")]
  public IActionResult OrderDelivered(int id, [FromBody] int IdShipmentOrder)
  {
    return Ok();
  }
}
