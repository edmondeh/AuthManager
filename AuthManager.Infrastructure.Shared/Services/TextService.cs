using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthManager.Infrastructure.Shared.Services
{
    public static class TextService
    {
        /// <summary>
        /// FirstLetter To UpperCase
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UpperCase(string text)
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
