// DTOs/CompanyRegisterDto.cs
namespace DataCommunication
{
    public class CompanyRegisterDto : UserRegisterDto
    {
        public string? IBAN { get; set; }

        public string? LogoUrl { get; set; }
        public string? CompanyName { get; set; }
    }
}
