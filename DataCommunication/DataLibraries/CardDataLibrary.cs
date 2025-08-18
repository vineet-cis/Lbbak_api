using Microsoft.EntityFrameworkCore;

namespace DataCommunication.DataLibraries
{
    public class CardDataLibrary
    {
        private readonly AppDbContext context;

        public CardDataLibrary(AppDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task<int> CreateCard(Card card)
        {
            context.ChangeTracker.Clear();
            context.Cards.Add(card);
            await context.SaveChangesAsync();
            return card.Id;
        }

        public async Task<Card> GetCardByGuid(string? guid)
        {
            return await context.Cards.FirstOrDefaultAsync(x => x.Guid == guid);
        }

        public async Task<List<Card>> GetAllCards()
        {
            return await context.Cards.Where(x => x.Status != "Deleted").ToListAsync();
        }

        public async Task UpdateCard(Card card)
        {
            context.ChangeTracker.Clear();
            context.Entry(card).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
