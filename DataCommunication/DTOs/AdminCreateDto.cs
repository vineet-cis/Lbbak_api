using System.ComponentModel.DataAnnotations;

namespace DataCommunication
{
    public class AdminCreateDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public int AdminRoleId { get; set; }
        public string[]? Countries { get; set; }
        public string[]? Permissions { get; set; }
    }
}
