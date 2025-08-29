using Microsoft.EntityFrameworkCore;
using static DataCommunication.CommonComponents.Enums;

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

        public async Task AddCardUseCount(string guid)
        {
            context.ChangeTracker.Clear();
            var card = await context.Cards.FirstOrDefaultAsync(x => x.Guid.ToString() == guid);
            if(card != null)
            {
                card.UseCount += 1;
                context.Entry(card).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Card>> GetAllCards()
        {
            return await context.Cards.Include(c => c.EventType).AsSplitQuery().ToListAsync();
        }

        public async Task<List<Card>> GetAllInvitationCards()
        {
            return await context.Cards.Include(c => c.EventType).Where(x => (x.CardType == CardType.Invitation || x.CardType == CardType.Both) && x.Status != "Deleted").AsSplitQuery().ToListAsync();
        }

        public async Task<List<Card>> GetAllGreetingCards()
        {
            return await context.Cards.Include(c => c.EventType).Where(x => (x.CardType == CardType.Greeting || x.CardType == CardType.Both) && x.Status != "Deleted").AsSplitQuery().ToListAsync();
        }

        public async Task UpdateCard(Card card)
        {
            context.ChangeTracker.Clear();
            context.Entry(card).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
