using DataCommunication.Data.Models;
using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication
{
    public class EventType : BaseModel
    {
        public string? Name { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public Status Status { get; set; }
        public City City { get; set; }
        public int? CityId { get; set; }
    }
}
