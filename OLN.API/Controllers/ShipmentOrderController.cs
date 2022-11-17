using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLN.API.Data;
using OLN.API.DTO;
using OLN.API.Models;
using RestSharp;
using RestSharp.Authenticators;

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
      IdEnvio = order.idEnvio,
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

    if (orderFound.EstadoActual().Estado != EstadoOrden.creado) return BadRequest("La orden debe estar en estado \"Creado\" para pasar a en Tránsito");

    orderFound.Repartidor = deliveryManFound;
    orderFound.EstadoActual().FechaBaja = DateTime.Now;
    orderFound.Estados.Add(new CambioEstadoOrden { Estado = EstadoOrden.enTransito, FechaAlta = DateTime.Now });

    return Ok("Repartidor asignado");
  }

  [HttpPost("{id:int}/entrega")]
  [Authorize("write:shipmentstate")]
  public async Task<IActionResult> OrderDelivered(int id)
  {
    var orderFound = Repository.Current.FindShipmentOrderById(id);
    if (orderFound is null) return NotFound($"No se encontró la orden con Id {id}.");

    if (orderFound.EstadoActual().Estado != EstadoOrden.enTransito)
    {
      return BadRequest("La orden debe estar en Tránsito para poder ser entregada.");
    }

    orderFound.EstadoActual().FechaBaja = DateTime.Now;
    orderFound.Estados.Add(new CambioEstadoOrden { Estado = EstadoOrden.entregado, FechaAlta = DateTime.Now });

    var reqToken = new RestRequest("/oauth/token")
    .AddJsonBody(new
    {
      client_id = "C0L2So2k7ZBdKwhMCWwRZCZQYHqThX7a",
      client_secret = "6pVQtQwSKefZ_4N6hTbo17NHfazaLcyp3gF1GqGLefl64eC_zn8RDrYuHvaYlWrH",
      audience = "https://api.procesadorenvios.com",
      grant_type = "client_credentials"
    }
  );

    bool entregaNotificada = false;
    try
    {
      var client = new RestClient("https://dev-pmt16h97.us.auth0.com");
      var tokenRes = await client.PostAsync(reqToken);

      if (tokenRes.IsSuccessful && tokenRes.Content != null)
      {
        var res = JsonSerializer.Deserialize<GetTokenDTO>(tokenRes.Content);
        if (res?.access_token != null)
        {
          var procesadorClient = new RestClient("http://ecs-services-1705455222.us-east-1.elb.amazonaws.com");
          procesadorClient.Authenticator = new JwtAuthenticator(res.access_token);
          var reqNovedades = new RestRequest($"/api/Envios/{orderFound.IdEnvio}/Novedades")
          .AddJsonBody(orderFound.EstadoActual().Estado.ToString());
          var novedadesRes = await procesadorClient.PostAsync(reqNovedades);
          if (novedadesRes.IsSuccessful) entregaNotificada = true;
        }
      }
    }
    catch (Exception) { }

    return Ok($"Entrega registrada{(entregaNotificada ? " y notificada" : "")}.");
  }
}
