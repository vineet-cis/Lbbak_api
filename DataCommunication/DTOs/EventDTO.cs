namespace DataCommunication.DTOs
{
    public class EventResponseDTO
    {
        public string? Guid { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Venue { get; set; }
        public EventType Type { get; set; }
        public int? TypeId { get; set; }
        public User EventOwner { get; set; }
        public Guid EventOwnerId { get; set; }
        public Card Card { get; set; }
        public int? CardId { get; set; }
        public string? Category { get; set; }
        public string? Privacy { get; set; }
        public string Status { get; set; } = "Active";
        public List<EventInvitee>? Invitees { get; set; }
        public List<EventCongratulator>? Congratulators { get; set; }
    }
}
