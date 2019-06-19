using Newtonsoft.Json;

namespace Chatbot.Common.Extensions
{
    public static class ObjectExtension
    {
        public static bool Equals<T>(this T source, object item, bool useSerializationComparison)
        {
            if (JsonConvert.SerializeObject(source) == JsonConvert.SerializeObject(item))
                return true;
            else
                return false;
        }
    }
}