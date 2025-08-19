namespace DataCommunication
{
    public class User
    {
        public Guid Id { get; set; }

        public string? MobileNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? Email { get; set; } // Required only for Individual users

        public string? Name { get; set; }
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }

        public bool IsDeleted { get; set; } = false;
        public string Status { get; set; } = "Active"; // or from foreign table later
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
        public string? ProfileMediaId { get; set; }
        public bool TwoFactorEnabled { get; set; }

        // Navigation
        public IndividualUser? IndividualProfile { get; set; }
        public CompanyUser? CompanyProfile { get; set; }
        public DesignerUser? DesignerProfile { get; set; }
    }

}
