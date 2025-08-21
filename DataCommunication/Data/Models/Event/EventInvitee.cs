using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication
{
    public class EventInvitee
    {
        public int Id { get; set; }
        public Event Event { get; set; }
        public int EventId { get; set; }
        public User? User { get; set; }
        public Guid? UserId { get; set; }
        public string? InviteeName { get; set; }
        public string? MobileNumber { get; set; }
        public InvitationStatus Status { get; set; }
        public DateTime InvitedAt { get; set; }
    }
    
}
