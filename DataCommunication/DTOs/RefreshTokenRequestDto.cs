namespace DataCommunication
{
    public class RefreshTokenRequestDto
    {
        public Guid AdminId { get; set; }
        public string RefreshToken { get; set; }
    }
}
