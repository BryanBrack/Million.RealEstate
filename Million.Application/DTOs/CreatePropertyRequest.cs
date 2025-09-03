using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.DTOs
{
    public record CreatePropertyRequest(string Name, string Address, decimal Price, string CodeInternal, short Year, int IdOwner);
}
