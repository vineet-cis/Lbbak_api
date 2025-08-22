using DataCommunication.Data.Models;
using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication
{
    public class PromotionalOffer : BaseModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public string? LocationLink { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public OfferCategory Category { get; set; }
        public int CategoryId { get; set; }
        public City City { get; set; }
        public int CityId { get; set; }
        public bool IsSingleUsePerUser { get; set; } = false;
        public int? MaxUsageCount { get; set; } 
        public int UsedCount { get; set; } = 0;
        public PromotionType Type { get; set; }
        public PromotionScope Scope { get; set; }
        public Status Status { get; set; }
        public string? MediaId { get; set; }
    }
}
