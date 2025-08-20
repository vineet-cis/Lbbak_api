namespace DataCommunication
{
   
    public class IndividualUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } 
        public int RegionId { get; set; }
        public string? IBAN { get; set; }
        public string? BankName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        public int Age
        {
            get
            {
                if (!DateOfBirth.HasValue)
                    return 0;

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;

                if (DateOfBirth.Value.Date > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }
    }
}
