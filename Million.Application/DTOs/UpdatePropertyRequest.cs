using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.DTOs
{
    public record UpdatePropertyRequest(
        string? Name,
        string? Address,
        int? Year,
        int? IdOwner
    );
}
