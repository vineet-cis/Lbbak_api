using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication.DTOs
{
    public class EventTypeAdminDTO
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public Status Status { get; set; }
        public City? City { get; set; }
    }
}
