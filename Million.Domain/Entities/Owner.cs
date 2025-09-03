using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Domain.Entities
{
    public class Owner
    {
        public int IdOwner { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
        public DateOnly? Birthday { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
