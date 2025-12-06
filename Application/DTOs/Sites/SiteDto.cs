using System;

namespace Application.DTOs.Sites
{
    public class SiteDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public Guid UserId { get; set; }
    }
}