using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Util
{
    public static class StringUtils
    {
        public static string? GetFirstSecondLetter(string? input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 2)
                return null;

            return input.Substring(0, 2);
        }
    }
}
