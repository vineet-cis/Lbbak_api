namespace DataCommunication
{
    public class City
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Country County { get; set; }
        public int? CountryId { get; set; }
    }
}
