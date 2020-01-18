using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeLib.Infrostructure
{
    public static class StringExtensionsMethods
    {
        public static string FormatName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Unknown";
            }
            name = name.Replace(" ", "");
            return char.ToUpper(name[0]) + name.Substring(1).IsPartNameInLowerCase();
        }
        private static string IsPartNameInLowerCase(this string partName)
        {
            if (partName.Length == 0)
            {
                return "";
            }
            partName = partName.ToLower();
            return partName;
        }
    }
}
