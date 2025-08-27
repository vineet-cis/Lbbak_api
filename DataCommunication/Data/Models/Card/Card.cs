using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication
{
    public class Card
    {
        public int Id { get; set; }
        public int UseCount { get; set; }
        public string? Name { get; set; }
        public EventType EventType { get; set; }
        public CardType CardType { get; set; }
        public int? EventTypeId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; } = "Active";
        public string? ProfileMediaId { get; set; }
        public string Guid { get; set; }
    }
}
