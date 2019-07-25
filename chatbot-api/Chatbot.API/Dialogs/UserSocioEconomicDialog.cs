using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Chatbot.Api.DTO;
using Chatbot.Api.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Bot.Schema;
using Chatbot.Api.Helpers;
using Newtonsoft.Json;
using Chatbot.Common.Extensions;

namespace Chatbot.Api.Dialogs
{
    public class UserSocioEconomicDialog : CustomComponentDialog
    {
        #region Attributes
        private const string USER_SOCIOECONOMIC_STEP = "USER_SOCIOECONOMIC_STEP";
        private const string MONTHLY_INCOME_VALIDATION = "MONTHLY_INCOME_VALIDATION";
        private readonly IDialogHelper _helper;
        #endregion

        public UserSocioEconomicDialog(IDialogHelper helper)
            : base(nameof(UserSocioEconomicDialog))
        {
            this._helper = helper ?? throw new System.ArgumentNullException(nameof(helper));

            AddDialog(new NumberPrompt<decimal>(MONTHLY_INCOME_VALIDATION, MonthlyIncomePromptValidatorAsync));
            AddDialog(new CustomConfirmPrompt(nameof(CustomConfirmPrompt)));
            AddDialog(new FamilyIncomesDialog(helper));
            AddDialog(new CustomAdaptiveCardPrompt(nameof(CustomAdaptiveCardPrompt), FormPromptValidatorAsync));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskForMonthlyIncomeStepAsync,
                AskForFamilyIncomesStepAsync,
                SkipOrStartFamilyIncomesDialogStepAsync,
                AskForMonthlyExpensesStepAsync,
                StartMonthlyExpensesDialogStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        #region Waterfall's Dialog
        private async Task<DialogTurnResult> AskForMonthlyIncomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            stepContext.Values[USER_SOCIOECONOMIC_STEP] = new UserSocioEconomicDTO();

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Qual é a renda mensal da sua empresa?")
                // RetryPrompt = MessageFactory.Text("Por favor, Verifique os campos obrigatórios!")
            };
            return await stepContext.PromptAsync(MONTHLY_INCOME_VALIDATION, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForFamilyIncomesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            var socioEconomic = stepContext.Values[USER_SOCIOECONOMIC_STEP] as UserSocioEconomicDTO;
            socioEconomic.MonthlyIncome = decimal.Parse(stepContext.Result.ToString());

            return await stepContext.PromptAsync(nameof(CustomConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Você teria outras rendas familiares?"),
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

        private async Task<DialogTurnResult> AskForMonthlyExpensesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            var socioEconomic = stepContext.Values[USER_SOCIOECONOMIC_STEP] as UserSocioEconomicDTO;

            if (!(stepContext.Result is null))
            {
                socioEconomic.FamilyIncomes = stepContext.Result as List<FamilyIncomeDTO>;

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Obrigado por me ajudar. \nEu consegui registrar esses items abaixo:"), cancellationToken);

                // create monthly income adaptive card
                var monthlyIncomePath = Path.Combine(".", "Resources", "AdaptiveCard", "MonthlyIncomeDetails.json");
                var monthlyIncomeCardAttachment = _helper.CreateAdaptiveCardAttachment(monthlyIncomePath, MonthlyIncomeDetails(socioEconomic));
                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(monthlyIncomeCardAttachment), cancellationToken);
            }

            // save the User SocioEconomic data into the Conversation State
            var conversation = await _helper.UserAccessor.GetAsync(stepContext.Context, () => new UserConversationDTO());
            conversation.UserSocioEconomic = socioEconomic;

            return await stepContext.PromptAsync(nameof(CustomConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Agora eu preciso saber as suas despesas mensais ok?"),
                RetryPrompt = MessageFactory.Text("Na verdade eu espero SIM ou NÃO como resposta!")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> StartMonthlyExpensesDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            if ((bool)stepContext.Result) { }

            // create family expense adaptive card
            var filePath = Path.Combine(".", "Resources", "AdaptiveCard", "FamilyExpensesForm.json");
            var familyExpenseFormCardAttachment = _helper.CreateAdaptiveCardAttachment(filePath, new { WebUiAppUrl = _helper.AppSettings.WebUiAppUrl });
            var promptOptions = new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(familyExpenseFormCardAttachment),
                RetryPrompt = MessageFactory.Text("Por favor, verifique os campos obrigatórios e clique em \"Enviar\"")
            };
            return await stepContext.PromptAsync(nameof(CustomAdaptiveCardPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var socioEconomic = (UserSocioEconomicDTO)stepContext.Values[USER_SOCIOECONOMIC_STEP];
            socioEconomic.FamilyExpense = JsonConvert.DeserializeObject<FamilyExpenseDTO>(stepContext.Result?.ToString());

            // save the User SocioEconomic data into the Conversation State
            var conversation = await _helper.UserAccessor.GetAsync(stepContext.Context, () => new UserConversationDTO());
            conversation.UserSocioEconomic = socioEconomic;

            // Exit the dialog, returning the collected data information
            return await stepContext.EndDialogAsync(stepContext.Values[USER_SOCIOECONOMIC_STEP], cancellationToken);
        }
        #endregion

        private object MonthlyIncomeDetails(UserSocioEconomicDTO socioEconomic) =>
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
                WebUiAppUrl = _helper.AppSettings.WebUiAppUrl,
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
                    var minimumAcceptedValue = 1000;
                    if (value < minimumAcceptedValue)
                        await promptContext.Context.SendActivityAsync(MessageFactory.Text($"Valores menores que {minimumAcceptedValue.ToString("C")} não são aceitos"), cancellationToken);
                    else
                        valid = true;
                }
                else
                    await promptContext.Context.SendActivityAsync(MessageFactory.Text($"O máximo que eu consigo aceitar é {maximumAcceptedValue.ToString("C")}"), cancellationToken);
            }
            else
                await promptContext.Context.SendActivityAsync(MessageFactory.Text("Na verdade eu espero apenas números!"), cancellationToken);

            return await Task.FromResult(valid);
        }

        private async Task<bool> FormPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                if (promptContext.Recognized.Value.IsJsonSchema())
                {
                    var form = JsonConvert.DeserializeObject<FamilyExpenseDTO>(promptContext.Recognized.Value);

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
