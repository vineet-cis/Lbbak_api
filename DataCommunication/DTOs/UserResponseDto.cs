namespace DataCommunication
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public int UserTypeId { get; set; }
        public int Age { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? Status { get; set; }
        public string? UserType { get; set; }
        public DateTime CreatedAt { get; set; }

        // Optional profiles
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Gender { get; set; }
        public string? CommercialRegistrationNumber { get; set; }
        public string? IBAN { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
