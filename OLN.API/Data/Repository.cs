using OLN.API.Models;

namespace OLN.API.Data
{
  public class Repository
  {
    private static Repository? _current;
    public static Repository Current
    {
      get
      {
        if (_current is null) _current = new();
        return _current;
      }
    }

    private int siguienteId = 1;
    public int SiguienteId
    {
      get => siguienteId++;
    }

    public List<OrdenEnvio> Orders { get; } = new();
    public List<Repartidor> DeliveryMans { get; } = new();

    public OrdenEnvio? FindShipmentOrderById(int id)
    {
      foreach (var e in Repository.Current.Orders)
      {
        if (e.IdOrdenEnvio == id)
        {
          return e;
        }
      }
      return null;
    }


    public Repartidor? FindDeliveryManById(int id)
    {
      foreach (var d in Repository.Current.DeliveryMans)
      {
        if (d.IdRepartidor == id)
        {
          return d;
        }
      }
      return null;
    }
  }
}