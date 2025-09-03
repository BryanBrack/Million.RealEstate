using Microsoft.AspNetCore.Mvc;
using Million.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<ActionResult<int>> CreateOwnerAsync(CreateOwnerRequest req, CancellationToken ct = default);
        Task<int> CreateAsync(CreatePropertyRequest req, CancellationToken ct = default);
        Task<int> AddImageAsync(int idProperty, string fileBase64, string fileName, CancellationToken ct = default);
        Task ChangePriceAsync(int idProperty, decimal newPrice, CancellationToken ct = default);
        Task UpdateAsync(int idProperty, UpdatePropertyRequest req, CancellationToken ct = default);
        Task<(IEnumerable<PropertyDto> Items, int Total)> ListAsync(PropertyFilter filter, CancellationToken ct = default);
    }
}
