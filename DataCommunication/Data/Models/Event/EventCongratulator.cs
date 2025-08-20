namespace DataCommunication
{
    public class EventCongratulator
    {
        public int Id { get; set; }
        public Event Event { get; set; }
        public int EventId { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }

        public string Message { get; set; }
        public Card Card { get; set; }
        public int? CardId { get; set; }
        public decimal? GiftAmount { get; set; }
        public DateTime SentAt { get; set; }
    }
}
