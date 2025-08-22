using Microsoft.EntityFrameworkCore;
using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication.DataLibraries
{
    public class OfferDataLibrary
    {
        private readonly AppDbContext context;

        public OfferDataLibrary(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        public async Task<PromotionalOffer> Create(PromotionalOffer offer)
        {
            context.ChangeTracker.Clear();
            offer.CreatedAt = offer.UpdatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "UTC", "Central Standard Time");
            context.PromotionalOffers.Add(offer);
            await context.SaveChangesAsync();
            return offer;
        }

        public async Task<PromotionalOffer> UpdateOfferMediaId(PromotionalOffer offer, string mediaId)
        {
            context.ChangeTracker.Clear();
            offer.MediaId = mediaId;
            context.Entry(offer).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return offer;
        }

        public async Task<List<PromotionalOffer>> GetAllOffersForUser(string? guid)
        {
            return await context.PromotionalOffers.Include(x => x.City).Where(x => x.CreatedBy == guid && x.Status == Status.Active).AsSplitQuery().ToListAsync();
        }

        public async Task<List<OfferCategory>> GetAllCategories()
        {
            return await context.OfferCategories.AsNoTracking().ToListAsync();
        }
    }
}
