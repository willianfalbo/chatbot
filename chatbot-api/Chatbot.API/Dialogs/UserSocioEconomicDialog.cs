using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Chatbot.API.Models;
using Chatbot.API.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Bot.Schema;
using Chatbot.API.Models.Util;
using Newtonsoft.Json;
using Chatbot.Common.Extensions;
using Chatbot.Common.Interfaces;

namespace Microsoft.BotBuilderSamples
{
    public class UserSocioEconomicDialog : CustomComponentDialog
    {
        #region Attributes
        private const string USER_SOCIOECONOMIC_STEP = "USER_SOCIOECONOMIC_STEP";
        private const string MONTHLY_INCOME_VALIDATION = "MONTHLY_INCOME_VALIDATION";
        private readonly IAppSettings _appSettings;
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        #endregion

        public UserSocioEconomicDialog(IAppSettings appSettings, UserState userState, ConversationState conversationState)
            : base(nameof(UserSocioEconomicDialog))
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            this._userState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            this._conversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));

            AddDialog(new NumberPrompt<decimal>(MONTHLY_INCOME_VALIDATION, MonthlyIncomePromptValidatorAsync));
            AddDialog(new CustomConfirmPrompt(nameof(CustomConfirmPrompt)));
            AddDialog(new FamilyIncomesDialog());
            AddDialog(new CustomAdaptiveCardPrompt(nameof(CustomAdaptiveCardPrompt), FormPromptValidatorAsync));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskForMonthlyIncomeStepAsync,
                AskForFamilyIncomesStepAsync,
                SkipOrStartFamilyIncomesDialogStepAsync,
                StartMonthlyExpensesDialogStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        #region Waterfall's Dialog
        private async Task<DialogTurnResult> AskForMonthlyIncomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[USER_SOCIOECONOMIC_STEP] = new UserSocioEconomic();

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Vamos começar a próxima etapa"), cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Qual é a renda mensal da sua empresa?")
                // RetryPrompt = MessageFactory.Text("Por favor, Verifique os campos obrigatórios!")
            };
            return await stepContext.PromptAsync(MONTHLY_INCOME_VALIDATION, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForFamilyIncomesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var socioEconomic = stepContext.Values[USER_SOCIOECONOMIC_STEP] as UserSocioEconomic;
            socioEconomic.MonthlyIncome = decimal.Parse(stepContext.Result.ToString());

            return await stepContext.PromptAsync(nameof(CustomConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Você teria outras Rendas Familiares?"),
                RetryPrompt = MessageFactory.Text("Na verdade eu espero SIM ou NÃO como resposta!")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> SkipOrStartFamilyIncomesDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
                return await stepContext.BeginDialogAsync(nameof(FamilyIncomesDialog), null, cancellationToken);
            else
                return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> StartMonthlyExpensesDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var socioEconomic = stepContext.Values[USER_SOCIOECONOMIC_STEP] as UserSocioEconomic;

            if (stepContext.Result is null)
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Vamos começar a próxima etapa"), cancellationToken);
            else
            {
                socioEconomic.FamilyIncomes = stepContext.Result as List<FamilyIncome>;

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Obrigado por me ajudar"), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Eu consegui registrar esses items abaixo:"), cancellationToken);

                // create monthly income adaptive card
                var monthlyIncomePath = Path.Combine(".", "Resources", "AdaptiveCard", "MonthlyIncomeDetails.json");
                var monthlyIncomeCardAttachment = base.CreateAdaptiveCardAttachment(monthlyIncomePath, MonthlyIncomeDetails(socioEconomic));
                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(monthlyIncomeCardAttachment), cancellationToken);
            }

            // save the User SocioEconomic data into the Conversation State
            var conversationStateAccessors = _conversationState.CreateProperty<UserSocioEconomic>(nameof(UserSocioEconomic));
            await conversationStateAccessors.SetAsync(stepContext.Context, socioEconomic, cancellationToken);

            // create family expense adaptive card
            var filePath = Path.Combine(".", "Resources", "AdaptiveCard", "FamilyExpensesForm.json");
            var familyExpenseFormCardAttachment = base.CreateAdaptiveCardAttachment(filePath, new { DefaultRootUrl = _appSettings.DefaultRootUrl });
            var promptOptions = new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(familyExpenseFormCardAttachment),
                RetryPrompt = MessageFactory.Text("Por favor, verifique os campos obrigatórios e clique em \"Enviar\"")
            };
            return await stepContext.PromptAsync(nameof(CustomAdaptiveCardPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var socioEconomic = (UserSocioEconomic)stepContext.Values[USER_SOCIOECONOMIC_STEP];
            socioEconomic.FamilyExpense = JsonConvert.DeserializeObject<FamilyExpense>(stepContext.Result?.ToString());

            // save the User SocioEconomic data into the Conversation State
            var conversationStateAccessors = _conversationState.CreateProperty<UserSocioEconomic>(nameof(UserSocioEconomic));
            await conversationStateAccessors.SetAsync(stepContext.Context, socioEconomic, cancellationToken);

            // Exit the dialog, returning the collected data information
            return await stepContext.EndDialogAsync(stepContext.Values[USER_SOCIOECONOMIC_STEP], cancellationToken);
        }
        #endregion

        private object MonthlyIncomeDetails(UserSocioEconomic socioEconomic) =>
            new
            {
                MonthlyIncome = socioEconomic.MonthlyIncome.ToString("C"),
                FamilyIncomes = socioEconomic.FamilyIncomes.Select(x => new
                {
                    PersonsName = x.PersonsName,
                    Source = x.Source,
                    Value = x.Value.ToString("C"),
                }),
                TotalFamilyIncome = socioEconomic.TotalFamilyIncome.ToString("C"),
                TotalMonthlyIncome = socioEconomic.TotalMonthlyIncome.ToString("C"),
                DefaultRootUrl = _appSettings.DefaultRootUrl,
            };

        #region Validators
        private async Task<bool> MonthlyIncomePromptValidatorAsync(PromptValidatorContext<decimal> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                var value = promptContext.Recognized.Value;
                var maximumAcceptedValue = 4800000;
                if (value <= maximumAcceptedValue)
                {
                    await promptContext.Context.SendActivityAsync(MessageFactory.Text("Legal"), cancellationToken);

                    var minimumAcceptedValue = 1000;
                    if (value < minimumAcceptedValue)
                        await promptContext.Context.SendActivityAsync(MessageFactory.Text($"Valores menores que {minimumAcceptedValue.ToString("C")} não são aceitos"), cancellationToken);
                    else
                    {
                        if (value <= 81000)
                            await promptContext.Context.SendActivityAsync(MessageFactory.Text("Provavelmente você é um Microempreendedor Individual"), cancellationToken);
                        else if (value <= 360000)
                            await promptContext.Context.SendActivityAsync(MessageFactory.Text("Provavelmente esta é uma Microempresa"), cancellationToken);
                        else
                            await promptContext.Context.SendActivityAsync(MessageFactory.Text("Provavelmente esta é uma Empresa de Pequeno Porte"), cancellationToken);

                        valid = true;
                    }
                }
                else
                {
                    await promptContext.Context.SendActivityAsync(MessageFactory.Text("Este é um valor alto e não se enquadra em nossa politica!"), cancellationToken);
                    await promptContext.Context.SendActivityAsync(MessageFactory.Text($"O máximo que eu consigo aceitar é {maximumAcceptedValue.ToString("C")}"), cancellationToken);
                }
            }
            else
            {
                await promptContext.Context.SendActivityAsync(MessageFactory.Text("Na verdade eu espero apenas números!"), cancellationToken);
            }

            return await Task.FromResult(valid);
        }

        private async Task<bool> FormPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                if (promptContext.Recognized.Value.IsJsonSchema())
                {
                    var form = JsonConvert.DeserializeObject<FamilyExpense>(promptContext.Recognized.Value);

                    var result = ModelValidator.IsValid(form);

                    if (!result.HasError)
                        valid = true;
                }
            }

            return await Task.FromResult(valid);
        }
        #endregion
    }
}
