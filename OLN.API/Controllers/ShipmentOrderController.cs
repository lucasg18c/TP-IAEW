using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLN.API.Data;
using OLN.API.DTO;
using OLN.API.Models;
using RestSharp;

namespace OLN.API.Controllers;

[ApiController]
[Route("api/ordenes_envio")]
public class ShipmentOrderController : ControllerBase
{
  private readonly ILogger<ShipmentOrderController> _logger;
  private readonly RestClient _client;

  public ShipmentOrderController(ILogger<ShipmentOrderController> logger, RestClient client)
  {
    _logger = logger;
    _client = client;
  }

  [HttpPost]
  [Authorize("write:shipmentorder")]
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
      State = ShipmentState.Created,
      Create = DateTime.Now
    };
    Repository.Current.Orders.Add(created);
    return Ok(created);
  }

  [HttpGet("{id:int}")]
  [Authorize("read:shipmentorder")]
  public IActionResult GetOrder(int id)
  {
    var found = Repository.Current.FindShipmentOrderById(id);

    if (found is null) return NotFound($"No se encontró la orden con Id {id}");

    return Ok(found);
  }

  [HttpPost("{id:int}/repartidor")]
  [Authorize("write:shipmentstate")]
  public IActionResult PostDeliveryMan(int id, [FromBody] int IdDeliveryMan)
  {
    var orderFound = Repository.Current.FindShipmentOrderById(id);
    if (orderFound is null) return NotFound($"No se encontró la orden con Id {id}");

    var deliveryManFound = Repository.Current.FindDeliveryManById(IdDeliveryMan);
    if (deliveryManFound is null) return NotFound($"No se encontró el repartidor con id {IdDeliveryMan}");

    orderFound.DeliveryMan = deliveryManFound;
    orderFound.State = ShipmentState.Transit;

    return Ok("Repartidor asignado");
  }

  [HttpPost("{id:int}/entrega")]
  [Authorize("write:shipmentstate")]
  public IActionResult OrderDelivered(int id)
  {
    var orderFound = Repository.Current.FindShipmentOrderById(id);
    if (orderFound is null) return NotFound($"No se encontró la orden con Id {id}.");

    if (orderFound.State != ShipmentState.Transit)
    {
      return BadRequest("La orden debe estar en Tránsito para poder ser entregada.");
    }

    orderFound.State = ShipmentState.Delivered;
    orderFound.Delivered = DateTime.Now;

    var req = new RestRequest($"/envios/{id}/novedades").AddBody(new
    {
      State = 2
    });
    _client.PostAsync(req);

    return Ok("Entrega registrada.");
  }
}
