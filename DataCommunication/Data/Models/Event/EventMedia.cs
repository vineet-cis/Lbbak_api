namespace DataCommunication
{
    public class EventMedia
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public string MediaId { get; set; }
        public bool isVideo { get; set; }
    }
}
