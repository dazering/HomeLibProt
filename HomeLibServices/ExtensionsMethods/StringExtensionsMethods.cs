using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibServices.ExtensionsMethods
{
    internal static class StringExtensionsMethods
    {
        /// <summary>
        /// Processing authors name. If name is empty assign default value, else remove spaces and first liter is uppercase, remaining letters are lowercase
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string FormatName(this string name)
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
