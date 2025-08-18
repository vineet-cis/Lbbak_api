using System.ComponentModel.DataAnnotations;

namespace DataCommunication
{
    public class UserRegisterDto
    {
        [Required]
        public string MobileNumber { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string? Country { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
    }
}
