namespace DataCommunication
{
    public class DesignerUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public int Age { get; set; }
        public string? DesignSpeciality { get; set; }
        public string? Gender { get; set; }
        public string? PortfolioLink { get; set; }
    }

}
