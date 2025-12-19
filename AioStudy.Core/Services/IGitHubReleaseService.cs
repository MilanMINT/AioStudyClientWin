using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Services
{
    public interface IGitHubReleaseService
    {
       public Task<(string Tag, string Url)?> GetLatestReleaseTagAsync();
    }
}
