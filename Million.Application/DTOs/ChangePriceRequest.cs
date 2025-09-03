using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.DTOs
{
    public record ChangePriceRequest(int IdProperty, decimal NewPrice);
}
