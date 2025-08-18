namespace DataCommunication
{
    public class CompanyUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string? IBAN { get; set; }
        public string? BankName { get; set; }
        public string? LogoUrl { get; set; }
        public string? CommercialRegistrationNumber { get; set; }
        public bool IsDeactivated { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }
    }
}
