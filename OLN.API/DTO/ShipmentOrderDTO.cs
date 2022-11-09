namespace OLN.API.DTO
{
  public class ShipmentOrderDTO
  {
    public int? IdShipmentOrder { get; set; }
    public Address Origin { get; set; }
    public Address Destination { get; set; }
    public Contact Buyer { get; set; }
    public ShipmentState State { get; set; }
    public Product Product { get; set; }
    public string DeliveryMan { get; set; }
  }
}