namespace OLN.API.DTO
{
  public class Address
  {
    public int? IdAddress { get; set; }
    public string Street { get; set; }
    public int HouseNumber { get; set; }
    public int? Floor { get; set; }
    public int? ApartmentNumber { get; set; }
    public City City { get; set; }
  }
}