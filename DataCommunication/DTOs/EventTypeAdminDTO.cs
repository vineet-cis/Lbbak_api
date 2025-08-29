using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication.DTOs
{
    public class EventTypeAdminDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public string? Status { get; set; }
        public string? City { get; set; }
        public int? CityId { get; set; }
    }
}
