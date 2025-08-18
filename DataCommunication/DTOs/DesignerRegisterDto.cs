using System.ComponentModel.DataAnnotations;

namespace DataCommunication
{
    public class DesignerRegisterDto : UserRegisterDto
    {
        [Required]
        public string FullName { get; set; }

        public string? DesignSpeciality { get; set; }

        public string? PortfolioLink { get; set; }
    }
}
