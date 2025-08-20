namespace DataCommunication.DataLibraries
{
    public class EventDataLibrary
    {
        private readonly AppDbContext context;

        public EventDataLibrary(AppDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task<string> CreateEvent(Event Event)
        {
            context.ChangeTracker.Clear();
            Event.CreatedAt = Event.UpdatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "UTC", "Central Standard Time");
            Event.CreatedBy = Event.UpdatedBy = Event.EventOwnerId.ToString();
            context.Events.Add(Event);
            await context.SaveChangesAsync();
            return Event.Guid;
        }
    }
}
