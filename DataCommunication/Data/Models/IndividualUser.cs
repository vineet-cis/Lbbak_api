namespace DataCommunication
{
   
    public class IndividualUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } 
        public int RegionId { get; set; }
        public int Age { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? IBAN { get; set; }
        public string? BankName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } 

        public bool IsDeactivated { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }
    }
}
