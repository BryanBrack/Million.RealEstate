using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.DTOs
{
    public record PropertyDto(int IdProperty, string Name, string Address, decimal Price, string CodeInternal, int Year, int IdOwner, string ImageUrls);
}
