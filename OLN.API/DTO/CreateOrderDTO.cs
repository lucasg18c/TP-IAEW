using OLN.API.Models;

namespace OLN.API.DTO
{
  public class CreateOrderDTO
  {
    public Address Origin { get; set; }
    public Address Destination { get; set; }
    public Contact Buyer { get; set; }
    public Product Product { get; set; }
  }
}