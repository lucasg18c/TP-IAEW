namespace OLN.API.DTO
{
  public class State
  {
    public int? IdState { get; set; }
    public string Name { get; set; }
    public List<City> Cities { get; set; }
  }
}