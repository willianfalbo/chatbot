using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chatbot.API.Extensions;
using Chatbot.API.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Chatbot.Common.Extensions;
using Chatbot.Common.Interfaces;

namespace Microsoft.BotBuilderSamples
{
    public class UserProfileDialog : CustomComponentDialog
    {
        #region Attributes
        private const string USER_PROFILE_STEP = "USER_PROFILE_STEP";
        private const string NAME_VALIDATION = "NAME_VALIDATION";
        private const string EMAIL_VALIDATION = "EMAIL_VALIDATION";
        private const string AGE_VALIDATION = "AGE_VALIDATION";
        private readonly IAppSettings _appSettings;
        #endregion 

        public UserProfileDialog(IAppSettings appSettings, ICompanyRegistryManager companyRegistryManager, UserState userState, ConversationState conversationState)
            : base(nameof(UserProfileDialog), userState, conversationState)
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            if (userState is null)
                throw new System.ArgumentNullException(nameof(userState));
            if (conversationState is null)
                throw new System.ArgumentNullException(nameof(conversationState));
            if (companyRegistryManager is null)
                throw new System.ArgumentNullException(nameof(companyRegistryManager));

            AddDialog(new TextPrompt(NAME_VALIDATION, NamePromptValidatorAsync));
            AddDialog(new TextPrompt(EMAIL_VALIDATION, EmailPromptValidatorAsync));
            AddDialog(new NumberPrompt<int>(AGE_VALIDATION, AgePromptValidatorAsync));
            AddDialog(new CustomConfirmPrompt(nameof(CustomConfirmPrompt)));
            AddDialog(new UserCompanyDialog(_appSettings, userState, conversationState, companyRegistryManager));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskForAgreementStepAsync,
                AskForNameStepAsync,
                AskForEmailStepAsync,
                AskForAgeStepAsync,
                SkipOrAskForAgeStepAsync,
                StartCompanyDialogStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        #region Waterfall's Dialog        
        private async Task<DialogTurnResult> AskForAgreementStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await base.SendTypingActivity(stepContext.Context, cancellationToken);

            // Create an object in which to collect the user's information within the dialog.
            stepContext.Values[USER_PROFILE_STEP] = new UserProfile();

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync(nameof(CustomConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Para começarmos, eu preciso que você me autorize a salvar os seus dados em nossa base de dados. Tudo bem?"),
                RetryPrompt = MessageFactory.Text("Na verdade eu espero SIM ou NÃO como resposta!")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await base.SendTypingActivity(stepContext.Context, cancellationToken);

            // Set the user's chatting confirmation to what they entered in response to the prompt.
            var userProfile = (UserProfile)stepContext.Values[USER_PROFILE_STEP];
            userProfile.AcceptedAgreement = (bool)stepContext.Result;

            if (userProfile.AcceptedAgreement)
            {
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Qual é o seu primeiro nome?"),
                    RetryPrompt = MessageFactory.Text("O primeiro nome precisa ter apenas letras, contendo entre 3 e 20 caracteres.")
                };
                // Ask the user to enter their name.
                return await stepContext.PromptAsync(NAME_VALIDATION, promptOptions, cancellationToken);
            }
            else
            {
                //tells user that we can't continue chatting
                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(GetDisagreementCard().ToAttachment()), cancellationToken);

                // Exit the dialog, returning the collected user information.
                return await stepContext.EndDialogAsync(stepContext.Values[USER_PROFILE_STEP], cancellationToken);
            }
        }

        private async Task<DialogTurnResult> AskForEmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await base.SendTypingActivity(stepContext.Context, cancellationToken);

            // Set the user's chatting confirmation to what they entered in response to the prompt.
            var userProfile = (UserProfile)stepContext.Values[USER_PROFILE_STEP];
            userProfile.Name = stepContext.Result?.ToString()?.TitleCase();

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync(EMAIL_VALIDATION, new PromptOptions
            {
                Prompt = MessageFactory.Text($"{userProfile.Name}, qual é o seu e-mail?"),
                RetryPrompt = MessageFactory.Text("Este não parece ser um e-mail válido! Tente novamente.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForAgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await base.SendTypingActivity(stepContext.Context, cancellationToken);

            // Set the user's name to what they entered in response to the name prompt.
            var userProfile = (UserProfile)stepContext.Values[USER_PROFILE_STEP];
            userProfile.Email = stepContext.Result?.ToString()?.Trim();

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync(nameof(CustomConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text($"Poderia informar a sua idade?"),
                RetryPrompt = MessageFactory.Text("Na verdade eu espero SIM ou NÃO como resposta!")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> SkipOrAskForAgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Set the user's age providade to what they entered in response to the age confirmation prompt.
            var userProfile = (UserProfile)stepContext.Values[USER_PROFILE_STEP];
            userProfile.WantsToProvideAge = (bool)stepContext.Result;

            if (userProfile.WantsToProvideAge)
            {
                await base.SendTypingActivity(stepContext.Context, cancellationToken);

                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Quantos anos você tem?"),
                    RetryPrompt = MessageFactory.Text("Você precisa estar entre 18 até 120 anos")
                };
                // Ask the user to enter their age.
                return await stepContext.PromptAsync(AGE_VALIDATION, promptOptions, cancellationToken);
            }
            else
            {
                //go to the next step and send age as "-1"
                return await stepContext.NextAsync(-1, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> StartCompanyDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfile)stepContext.Values[USER_PROFILE_STEP];
            userProfile.Age = (int)stepContext.Result;

            // save the User Profile data into the User State Conversation
            await base.SetUserState(stepContext.Context, userProfile, cancellationToken);

            // begin the next dialog
            return await stepContext.BeginDialogAsync(nameof(UserCompanyDialog), userProfile, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Exit the dialog, returning the collected data information
            return await stepContext.EndDialogAsync(stepContext.Values[USER_PROFILE_STEP], cancellationToken);
        }
        #endregion

        private ThumbnailCard GetDisagreementCard()
        {
            var imageUrl = $"{_appSettings.WebUiAppUrl}/images/avatar/avatar-yes.png";

            var card = new ThumbnailCard
            {
                Title = "Se precisar de mim\n é só voltar!",
                Images = new List<CardImage>() { new CardImage(imageUrl) },
            };

            return card;
        }

        #region Validators
        private async Task<bool> AgePromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            return await Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value >= 18 && promptContext.Recognized.Value <= 120);
        }

        private async Task<bool> NamePromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                var name = promptContext.Recognized.Value?.Trim();
                //check if the string has a minimal and if it contains only characters
                if (name.Length >= 3 && name.Length <= 20)
                    if (name.IsEqual(name?.Letters()))
                        valid = true;
            }
            return await Task.FromResult(valid);
        }

        private async Task<bool> EmailPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            return await Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value.Trim().IsEmailAddressValid());
        }
        #endregion
    }
}
