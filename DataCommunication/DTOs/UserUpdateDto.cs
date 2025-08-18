using System.ComponentModel.DataAnnotations;

namespace DataCommunication
{
    public class UserUpdateDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? Country { get; set; }

        public bool TwoFactorEnabled { get; set; }
    }
}
