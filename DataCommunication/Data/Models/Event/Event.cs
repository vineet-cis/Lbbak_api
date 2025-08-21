using DataCommunication.Data.Models;
using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication
{
    public class Event : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Venue { get; set; }
        public string? MediaId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EventType Type { get; set; }
        public int? TypeId { get; set; }
        public User EventOwner { get; set; }
        public Guid EventOwnerId { get; set; }
        public Card Card { get; set; }
        public int? CardId { get; set; }
        public EventCategory Category { get; set; }
        public Privacy Privacy { get; set; }
        public string Status { get; set; } = "Active";
        public List<EventInvitee>? Invitees { get; set; }
        public List<EventCongratulator>? Congratulators { get; set; }
    }
}
