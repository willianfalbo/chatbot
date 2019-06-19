using System.Collections.Generic;
using Chatbot.Common.Extensions;

namespace Chatbot.API.Extensions
{
    public static class CustomListExtension
    {
        /// <summary>Checks each item of the list for unaccented string, and add them to the list.</summary>
        /// <returns>The new list with unaccented items.</returns>
        public static List<string> GenerateUnaccented(this List<string> source)
        {
            var unaccentedItems = new List<string>();
            foreach (var item in source)
            {
                var unaccented = item.Unaccented();
                if (item != unaccented)
                    unaccentedItems.Add(unaccented);
            }
            source.AddRange(unaccentedItems);
            return source;
        }
    }
}