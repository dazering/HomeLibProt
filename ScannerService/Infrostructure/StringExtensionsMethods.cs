using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScannerService.Infrostructure
{
    public static class StringExtensionsMethods
    {
        public static string NameToUpperFirstLiteral(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Unknown";
            }
            name = name.Replace(" ","");
            if (char.IsLower(name[0]))
            {
                return char.ToUpper(name[0]) + name.Substring(1);
            }
            return name;
        }
    }
}
