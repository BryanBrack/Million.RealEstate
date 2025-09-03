using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Domain.Entities
{
    public class PropertyImage
    {
        public int IdPropertyImage { get; set; }
        public int IdProperty { get; set; }
        public Property Property { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
