namespace DataCommunication
{
    public class DesignerUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public int Age
        {
            get
            {
                if (!DateOfBirth.HasValue)
                    return 0;

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;

                // If the birthday hasn't happened yet this year, subtract 1
                if (DateOfBirth.Value.Date > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }
    }

}
