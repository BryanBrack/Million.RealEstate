using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Domain.Entities
{
    public class PropertyTrace
    {
        public int IdPropertyTrace { get; set; }
        public int IdProperty { get; set; }
        public Property Property { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DateSale { get; set; }
        public string Name { get; set; } = "PRICE_CHANGE";
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
    }
}
