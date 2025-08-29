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

        public async Task CreateOfferType(OfferCategory category)
        {
            context.ChangeTracker.Clear();
            context.OfferCategories.Add(category);
            await context.SaveChangesAsync();
        }

        public async Task<PromotionalOffer> UpdateOfferMediaId(PromotionalOffer offer, string mediaId)
        {
            context.ChangeTracker.Clear();
            offer.MediaId = mediaId;
            context.Entry(offer).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return offer;
        }

        public async Task<PromotionalOffer> UpdateOffer(PromotionalOffer offer)
        {
            context.ChangeTracker.Clear();
            context.Entry(offer).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return offer;
        }

        public async Task DeleteCategory(int Id)
        {
            context.ChangeTracker.Clear();
            var cat = await context.OfferCategories.FirstOrDefaultAsync(x => x.Id == Id);

            if (cat == null)
                throw new FileNotFoundException();

            cat.IsDeleted = true;
            context.Entry(cat).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task<PromotionalOffer> GetOffer(string guid)
        {
            return await context.PromotionalOffers.Include(x => x.City).AsSplitQuery().FirstOrDefaultAsync(x => x.Guid == guid);
        }

        public async Task<List<PromotionalOffer>> GetAllOffersForUser(string? guid)
        {
            return await context.PromotionalOffers.Include(x => x.City).Include(x => x.Category).Where(x => x.CreatedBy == guid).AsSplitQuery().ToListAsync();
        }

        public async Task<List<PromotionalOffer>> GetAllOffers()
        {
            return await context.PromotionalOffers.Include(x => x.City).Include(x => x.Category).AsSplitQuery().AsNoTracking().ToListAsync();
        }

        public async Task<List<PromotionalOffer>> GetAvailableOffers(int? cityId, string userId)
        {
            return await context.PromotionalOffers
                .AsNoTracking() // no change tracking needed for read
                .Include(x => x.City)
                .Include(x => x.Category)
                .Where(x =>
                    x.Status == Status.Active &&
                    x.CreatedBy != userId &&
                    (
                        x.Scope == PromotionScope.Public ||
                        (cityId.HasValue && x.CityId == cityId.Value)
                    )
                )
                .ToListAsync();
        }


        public async Task<List<OfferCategory>> GetAllCategories()
        {
            return await context.OfferCategories.Where(x => !x.IsDeleted).AsNoTracking().ToListAsync();
        }
    }
}
