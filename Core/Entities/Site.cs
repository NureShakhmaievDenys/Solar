using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Site
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; } 

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}