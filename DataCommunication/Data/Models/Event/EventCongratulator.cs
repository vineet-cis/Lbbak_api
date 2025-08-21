namespace DataCommunication
{
    public class EventCongratulator
    {
        public int Id { get; set; }
        public Event Event { get; set; }
        public int EventId { get; set; }
        public User? User { get; set; }
        public Guid? UserId { get; set; }

        public Guid? RecipientId { get; set; }
        public User? Recipient { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientMobile { get; set; }

        public string? Message { get; set; }
        public Card Card { get; set; }
        public int? CardId { get; set; }
        public double? GiftAmount { get; set; }
        public DateTime SentAt { get; set; }
    }
}
