using Microsoft.EntityFrameworkCore;

namespace DataCommunication.DataLibraries
{
    public class EventDataLibrary
    {
        private readonly AppDbContext context;

        public EventDataLibrary(AppDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task<bool> UpdateEventMediaId(int id, string mediaId)
        {
            var Event = await context.Events.FirstOrDefaultAsync(e => e.Id == id);

            if (Event == null)
                return false;

            Event.MediaId = mediaId;
            context.Entry(Event).State = EntityState.Modified;
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<int> CreateEvent(Event Event)
        {
            context.ChangeTracker.Clear();
            Event.CreatedAt = Event.UpdatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "UTC", "Central Standard Time");
            Event.CreatedBy = Event.UpdatedBy = Event.EventOwnerId.ToString();
            context.Events.Add(Event);
            await context.SaveChangesAsync();
            return Event.Id;
        }

        public async Task<List<Event>> GetAllInvites()
        {
            return await context.Events.Include(x => x.EventOwner).Include(x => x.Congratulators).Include(x => x.Invitees).AsSplitQuery().ToListAsync();
        }
    }
}
