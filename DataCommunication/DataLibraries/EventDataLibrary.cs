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

        public async Task AddEventMedia(string eventGuid, string mediaId, bool isVideo)
        {
            context.ChangeTracker.Clear();
            var Event = await context.Events.FirstOrDefaultAsync(x => x.Guid == eventGuid);

            if(Event == null) return;

            var eventMedia = new EventMedia
            {
                EventId = Event.Id,
                MediaId = mediaId,
                isVideo = isVideo
            };

            context.EventMedia.Add(eventMedia);
            await context.SaveChangesAsync();
        }


        public async Task<int> CreateEventType(EventType type)
        {
            context.ChangeTracker.Clear();
            type.CreatedAt = type.UpdatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "UTC", "Central Standard Time");
            context.EventTypes.Add(type);
            await context.SaveChangesAsync();
            return type.Id;
        }

        public async Task<List<Event>> GetAllInvites()
        {
            return await context.Events.Include(x => x.EventOwner).Include(x => x.Congratulators).Include(x => x.Invitees).AsSplitQuery().ToListAsync();
        }

        public async Task<List<Event>> GetAllUpcomingEvents(Guid UserId)
        {
            var today = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "UTC", "Central Standard Time");

            return await context.Events.Include(x => x.EventOwner).Include(x => x.Congratulators).Include(x => x.Invitees)
                .Where(x => x.StartDate > today && x.Congratulators != null && x.Congratulators.Any(c => c.UserId == UserId))
                .AsSplitQuery().ToListAsync();
        }

        public async Task<List<EventType>> GetEventTypes()
        {
            return await context.EventTypes.Where(x => x.Status == CommonComponents.Enums.Status.Active).ToListAsync();
        }
    }
}
