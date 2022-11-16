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

    public List<ShipmentOrder> Orders { get; set; } = new();

  }
}