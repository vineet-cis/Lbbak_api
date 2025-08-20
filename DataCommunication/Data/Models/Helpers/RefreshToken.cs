namespace DataCommunication
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public Guid AdminId { get; set; }
        public Admin Admin { get; set; }

        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
