using Microsoft.Bot.Builder.Dialogs;

namespace Chatbot.API.Extensions
{
    // Up to this point, microsoft hasn't raised the "AdaptiveCardPrompt" yet
    // so I needed to create my own AdaptiveCardPrompt as the below links
    // TODO: Remove this entirely when microsoft releases the AdaptiveCardPrompt
    // https://github.com/microsoft/botframework-sdk/issues/5396
    // https://github.com/Microsoft/botbuilder-dotnet/issues/614
    public class CustomAdaptiveCardPrompt : TextPrompt
    {
        public CustomAdaptiveCardPrompt(string dialogId, PromptValidator<string> validator = null) : base(dialogId, validator)
        {
        }
    }
}