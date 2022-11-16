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

    public List<ShipmentOrder> Orders { get; } = new();
    public List<DeliveryMan> DeliveryMans { get; } = new();

    public ShipmentOrder? FindShipmentOrderById(int id)
    {
      foreach (var e in Repository.Current.Orders)
      {
        if (e.IdShipmentOrder == id)
        {
          return e;
        }
      }
      return null;
    }


    public DeliveryMan? FindDeliveryManById(int id)
    {
      foreach (var d in Repository.Current.DeliveryMans)
      {
        if (d.IdDeliveryMan == id)
        {
          return d;
        }
      }
      return null;
    }
  }
}