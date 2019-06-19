using System.IO;
using HandlebarsDotNet;

namespace Chatbot.Common.Handlers
{
    public static class TemplateHandler
    {
        /// <summary>Handle custom templates like Html, Json file and whatever.</summary>
        /// <param name="source">The content file string to be render. It can contain "double curly braces", eg. '<strong>{{name}}</strong>'</param>
        /// <param name="data">The content object, eg. 'new {name = "Karen"}'</param>
        /// <returns>The full content handled.</returns>
        public static string RenderContent(string source, object data)
        {
            var template = Handlebars.Compile(source);
            var result = template(data);
            return result;
        }

        /// <summary>Handle custom templates like Html, Json file and whatever.</summary>
        /// <param name="filePath">The full file path to be render. It can contain "double curly braces" to be render, eg. '<strong>{{name}}</strong>'</param>
        /// <param name="data">The content object, eg. 'new {name = "Karen"}'</param>
        /// <returns>The full content handled.</returns>
        public static string RenderFile(string filePath, object data)
        {
            var contentFile = File.ReadAllText(filePath);
            return RenderContent(contentFile, data);
        }
    }
}