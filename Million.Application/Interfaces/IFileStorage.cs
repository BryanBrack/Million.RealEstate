using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(string fileBase64, string fileName, CancellationToken ct = default);
    }
}
