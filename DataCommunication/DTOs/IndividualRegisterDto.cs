using System.ComponentModel.DataAnnotations;

namespace DataCommunication
{
    public class IndividualRegisterDto : UserRegisterDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int RegionId { get; set; }

        public string? IBAN { get; set; }

        public string? ProfileImageUrl { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }
    }
}
