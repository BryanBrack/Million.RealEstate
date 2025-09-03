using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.DTOs
{
    public record AddImageRequest(int IdProperty, string FileBase64, string FileName, bool Enabled = true);
}
