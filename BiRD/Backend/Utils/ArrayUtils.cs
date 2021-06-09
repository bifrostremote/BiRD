using System;

namespace BifrostRemoteDesktop.Common.Utils
{
    public static class ArrayUtils
    {
        public static string[] ConvertToStringArray(object[] array)
        {
            return Array.ConvertAll(array, ConvertObjectToString);
        }

        private static string ConvertObjectToString(object obj)
        {
            return obj?.ToString() ?? string.Empty;
        }
    }
}
