using Newtonsoft.Json;

namespace Chatbot.Common.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>Compare two objects using Serialization Json. Importante: The field's order matter!</summary>
        /// <param name="item">The target object to compare.</param>
        /// <returns>true for matched object; otherwise, false.</returns>
        public static bool IsEqual<T>(this T source, object item)
        {
            if (JsonConvert.SerializeObject(source) == JsonConvert.SerializeObject(item))
                return true;
            else
                return false;
        }
    }
}