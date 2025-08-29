using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? PasswordHash { get; set; }
        public string[]? Permissions { get; set; }
        public string[]? Countries { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
