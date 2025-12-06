using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Sites
{
    public class CreateSiteRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Location { get; set; }
    }
}