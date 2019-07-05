using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Chatbot.API.DTO;
using Chatbot.API.Extensions;
using Chatbot.Common.Extensions;
using System.Collections.Generic;
using Chatbot.API.Helpers;

namespace Microsoft.BotBuilderSamples
{
    public class FamilyIncomesDialog : CustomComponentDialog
    {
        #region Attributes
        private const string CURRENT_INCOME_STEP = "CURRENT_INCOME_STEP";
        private const string LIST_OF_INCOMES_STEP = "LIST_OF_INCOMES_STEP";
        private const string NAME_VALIDATION = "NAME_VALIDATION";
        private const string SOURCE_VALIDATION = "SOURCE_VALIDATION";
        private const string MONTHLY_VALUE_VALIDATION = "MONTHLY_VALUE_VALIDATION";
        private readonly IDialogHelper _helper;
        #endregion

        public FamilyIncomesDialog(IDialogHelper helper)
            : base(nameof(FamilyIncomesDialog))
        {
            this._helper = helper ?? throw new System.ArgumentNullException(nameof(helper));

            AddDialog(new TextPrompt(NAME_VALIDATION, NamePromptValidatorAsync));
            AddDialog(new TextPrompt(SOURCE_VALIDATION, SourcePromptValidatorAsync));
            AddDialog(new NumberPrompt<decimal>(MONTHLY_VALUE_VALIDATION, MonthlyIncomePromptValidatorAsync));
            AddDialog(new CustomConfirmPrompt(nameof(CustomConfirmPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskForPersonsNameStepAsync,
                AskForPersonsSourceOfIncomeStepAsync,
                AskForPersonsMonthlyIncomeStepAsync,
                AskForAnotherIncomesStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        #region Waterfall's Dialog
        private async Task<DialogTurnResult> AskForPersonsNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            // Continue using the same selection list, if any, from the previous iteration of this dialog.
            var listOfIncomes = stepContext.Options as List<FamilyIncomeDTO> ?? new List<FamilyIncomeDTO>();
            stepContext.Values[LIST_OF_INCOMES_STEP] = listOfIncomes;
            stepContext.Values[CURRENT_INCOME_STEP] = new FamilyIncomeDTO();

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Qual é o nome do {string.Concat((listOfIncomes.Count + 1), "º")} membro da sua família?"),
                RetryPrompt = MessageFactory.Text("O nome da pessoa precisa ter apenas letras, contendo entre 3 e 100 caracteres.")
            };
            return await stepContext.PromptAsync(NAME_VALIDATION, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForPersonsSourceOfIncomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            var income = stepContext.Values[CURRENT_INCOME_STEP] as FamilyIncomeDTO;
            income.PersonsName = stepContext.Result.ToString().Trim().TitleCase();

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Qual é a fonte de renda de {income.PersonsName}?"),
                RetryPrompt = MessageFactory.Text("A fonte de renda precisa ter apenas letras, contendo entre 3 e 100 caracteres.")
            };
            return await stepContext.PromptAsync(NAME_VALIDATION, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForPersonsMonthlyIncomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            var income = stepContext.Values[CURRENT_INCOME_STEP] as FamilyIncomeDTO;
            income.Source = stepContext.Result.ToString().Trim();

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Qual é o valor mensal da renda de {income.PersonsName}?"),
                // RetryPrompt = MessageFactory.Text("Na verdade eu espero apenas números!")
            };
            return await stepContext.PromptAsync(MONTHLY_VALUE_VALIDATION, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForAnotherIncomesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            var income = stepContext.Values[CURRENT_INCOME_STEP] as FamilyIncomeDTO;
            income.Value = decimal.Parse(stepContext.Result.ToString());

            var listOfIncomes = stepContext.Values[LIST_OF_INCOMES_STEP] as List<FamilyIncomeDTO>;
            listOfIncomes.Add(income);

            return await stepContext.PromptAsync(nameof(CustomConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Você quer adicionar outra pessoa?"),
                RetryPrompt = MessageFactory.Text("Na verdade eu espero SIM ou NÃO como resposta!")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var listOfIncomes = stepContext.Values[LIST_OF_INCOMES_STEP] as List<FamilyIncomeDTO>;

            var conversation = await _helper.GetConversationState<UserConversationDTO>(stepContext.Context, cancellationToken);
            conversation.UserSocioEconomic.FamilyIncomes = listOfIncomes;

            // save conversation into the document db
            await this._helper.SaveConversationDB(stepContext.Context, cancellationToken);

            if ((bool)stepContext.Result)
                return await stepContext.ReplaceDialogAsync(nameof(FamilyIncomesDialog), listOfIncomes, cancellationToken);
            else
                return await stepContext.EndDialogAsync(listOfIncomes, cancellationToken);
        }
        #endregion

        #region Validators
        private async Task<bool> NamePromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                var name = promptContext.Recognized.Value?.Trim();
                //check if the string has a minimal and if it contains only characters
                if (name.Length >= 3 && name.Length <= 100)
                    if (name.IsNameValid())
                        valid = true;
            }
            return await Task.FromResult(valid);
        }
        private async Task<bool> SourcePromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                var name = promptContext.Recognized.Value?.Trim();
                //check if the string has a minimal and if it contains only characters
                if (name.Length >= 3 && name.Length <= 100)
                    valid = true;
            }
            return await Task.FromResult(valid);
        }
        private async Task<bool> MonthlyIncomePromptValidatorAsync(PromptValidatorContext<decimal> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                var value = promptContext.Recognized.Value;
                var maximumAcceptedValue = 4800000;
                var minimumAcceptedValue = 100;

                if (value < minimumAcceptedValue)
                    await promptContext.Context.SendActivityAsync(MessageFactory.Text($"Valores menores que {minimumAcceptedValue.ToString("C")} não são aceitos"), cancellationToken);
                else if (value <= maximumAcceptedValue)
                    valid = true;
                else
                    await promptContext.Context.SendActivityAsync(MessageFactory.Text($"O máximo que eu consigo aceitar é {maximumAcceptedValue.ToString("C")}"), cancellationToken);
            }
            else
                await promptContext.Context.SendActivityAsync(MessageFactory.Text("Na verdade eu espero apenas números!"), cancellationToken);

            return await Task.FromResult(valid);
        }
        #endregion
    }
}
