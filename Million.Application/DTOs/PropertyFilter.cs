using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.DTOs
{
    public record PropertyFilter(
    string? Text, int? OwnerId, decimal? MinPrice, decimal? MaxPrice, short? Year,
    int Page = 1, int PageSize = 20, string? Sort = "createdAt_desc")
    {
        public PropertyFilter ToFilter()
        {
            throw new NotImplementedException();
        }
    }
}
