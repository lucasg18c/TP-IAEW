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
  public IActionResult CreateShipmentOrder([FromBody] CrearOrdenDTO order)
  {

    OrdenEnvio creada = new()
    {
      IdOrdenEnvio = Repository.Current.SiguienteId,
      ContactoComprador = order.contactoComprador,
      DetalleProducto = order.detalleProducto,
      DireccionDestino = order.direccionDestino,
      DireccionOrigen = order.direccionOrigen,
      Estados = new() {
        new CambioEstadoOrden {
          Estado = EstadoOrden.creado,
          FechaAlta = DateTime.Now
        }
      }
    };

    Repository.Current.Orders.Add(creada);
    return Ok(creada);
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
  public IActionResult PostDeliveryMan(int id, [FromBody] int IdRepartidor)
  {
    var orderFound = Repository.Current.FindShipmentOrderById(id);
    if (orderFound is null) return NotFound($"No se encontró la orden con Id {id}");

    var deliveryManFound = Repository.Current.FindDeliveryManById(IdRepartidor);
    if (deliveryManFound is null) return NotFound($"No se encontró el repartidor con id {IdRepartidor}");

    if (orderFound.EstadoActual().Estado != EstadoOrden.creado) return BadRequest("La orden debe estar en estado \"Creado \" para pasar a en Tránsito");

    orderFound.Repartidor = deliveryManFound;
    orderFound.EstadoActual().FechaBaja = DateTime.Now;
    orderFound.Estados.Add(new CambioEstadoOrden { Estado = EstadoOrden.enTransito, FechaAlta = DateTime.Now });

    return Ok("Repartidor asignado");
  }

  [HttpPost("{id:int}/entrega")]
  [Authorize("write:shipmentstate")]
  public IActionResult OrderDelivered(int id)
  {
    var orderFound = Repository.Current.FindShipmentOrderById(id);
    if (orderFound is null) return NotFound($"No se encontró la orden con Id {id}.");

    if (orderFound.EstadoActual().Estado != EstadoOrden.enTransito)
    {
      return BadRequest("La orden debe estar en Tránsito para poder ser entregada.");
    }

    orderFound.EstadoActual().FechaBaja = DateTime.Now;
    orderFound.Estados.Add(new CambioEstadoOrden { Estado = EstadoOrden.entregado, FechaAlta = DateTime.Now });

    var req = new RestRequest($"/envios/{id}/novedades").AddBody(new
    {
      State = 2
    });
    _client.PostAsync(req);

    return Ok("Entrega registrada.");
  }
}
