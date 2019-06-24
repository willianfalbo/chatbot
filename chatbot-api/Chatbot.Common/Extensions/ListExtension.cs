using System.Collections.Generic;
using System.Linq;

namespace Chatbot.Common.Extensions
{
    public static class ListExtension
    {
        public static bool IsEmpty<T>(this List<T> source)
        {
            if (source is null || !source.Any())
                return true;
            else
                return false;
        }
    }
}