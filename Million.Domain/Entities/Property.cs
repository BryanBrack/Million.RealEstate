using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Domain.Entities
{
    public class Property
    {
        public int IdProperty { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = null!;
        public short Year { get; set; }
        public int IdOwner { get; set; }
        public Owner Owner { get; set; }
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<PropertyTrace> Traces { get; set; } = new List<PropertyTrace>();
    }
}
