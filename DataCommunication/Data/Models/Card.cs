namespace DataCommunication
{
    public class Card
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? EventType { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; } = "Active";
        public string? ProfileMediaId { get; set; }
        public string? Guid { get; set; }
    }
}
