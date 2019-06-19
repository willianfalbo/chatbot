using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Chatbot.Model.Bot;
using Chatbot.Common.Extensions;
using Chatbot.Common.Interfaces;
using System.IO;
using Chatbot.API.Extensions;
using Microsoft.Bot.Schema;
using Chatbot.Model.Manager;
using Newtonsoft.Json;
using System.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class UserSocioEconomicDialog : CustomComponentDialog
    {
        #region Attributes
        private const string USER_SOCIOECONOMIC_STEP = "USER_SOCIOECONOMIC_STEP";
        private const string CNPJ_VALIDATION = "CNPJ_VALIDATION";
        private readonly ICompanyRegistryManager _companyRegistryManager;
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        #endregion

        public UserSocioEconomicDialog(ICompanyRegistryManager companyRegistryManager, UserState userState, ConversationState conversationState)
            : base(nameof(UserSocioEconomicDialog))
        {
            this._companyRegistryManager = companyRegistryManager ?? throw new System.ArgumentNullException(nameof(companyRegistryManager));
            this._userState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            this._conversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));

            AddDialog(new TextPrompt(nameof(TextPrompt), CnpjPromptValidatorAsync));
            AddDialog(new CustomAdaptiveCardPrompt(nameof(CustomAdaptiveCardPrompt), FormPromptValidatorAsync));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskForTaxIdentificationNumberStepAsync,
                AskForCheckingCompanyDetailsStepAsync,
                StartUserSocioEconomicDialogStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        #region Waterfall's Dialog
        private async Task<DialogTurnResult> AskForTaxIdentificationNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Create an object in which to collect the user's information within the dialog.
            stepContext.Values[USER_SOCIOECONOMIC_STEP] = new UserCompany();

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Diga lá..."), cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Qual é o CNPJ da sua empresa?"),
                RetryPrompt = MessageFactory.Text("Este não é um CNPJ válido! Tente novamente.")
            };
            // Ask the user to enter their name.
            return await stepContext.PromptAsync(CNPJ_VALIDATION, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForCheckingCompanyDetailsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Set the user's chatting confirmation to what they entered in response to the prompt.
            var company = (UserCompany)stepContext.Values[USER_SOCIOECONOMIC_STEP];
            company.TaxIdentificationNumber = stepContext.Result?.ToString()?.Trim();

            var result = await _companyRegistryManager.SearchForCnpj(company.TaxIdentificationNumber);

            if (!result.HasError)
            {
                if (result?.Value != null)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Obrigado"), cancellationToken);
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Já identifiquei suas informações"), cancellationToken);
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Poderia confirmar se os dados abaixo estão corretos?"), cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Não encontrei nenhuma informação"), cancellationToken);
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, preencha o formulário"), cancellationToken);
                }
            }
            else
            {
                foreach (var error in result.Errors)
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Erro: {error.Message}"), cancellationToken);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Infelizmente eu encontrei alguns problemas"), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, preencha o formulário"), cancellationToken);
            }

            var filePath = Path.Combine(".", "Resources", "AdaptiveCard", "UserCompanyForm.json");

            var data = UserCompanyFormData(result?.Value ?? new CompanyRegistry() { TaxIdentificationNumber = company.TaxIdentificationNumber });
            var cardAttachment = base.CreateAdaptiveCardAttachment(filePath, data);
            var promptOptions = new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(cardAttachment),
                RetryPrompt = MessageFactory.Text("Por favor, Verifique os campos obrigatórios!")
            };
            return await stepContext.PromptAsync(nameof(CustomAdaptiveCardPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> StartUserSocioEconomicDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userCompany = (UserCompany)stepContext.Values[USER_SOCIOECONOMIC_STEP];
            userCompany = JsonConvert.DeserializeObject<UserCompany>(stepContext.Result?.ToString()?.Trim());

            // save the User Company data into the Conversation State
            var conversationStateAccessors = _conversationState.CreateProperty<UserCompany>(nameof(UserCompany));
            await conversationStateAccessors.SetAsync(stepContext.Context, userCompany, cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Muito obrigado"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Vamos começar a próxima etapa"), cancellationToken);

            // begin the next dialog
            return await stepContext.BeginDialogAsync(nameof(UserSocioEconomicDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Exit the dialog, returning the collected data information
            return await stepContext.EndDialogAsync(stepContext.Values[USER_SOCIOECONOMIC_STEP], cancellationToken);
        }
        #endregion

        private object UserCompanyFormData(CompanyRegistry company)
        {
            if (company == null)
                company = new CompanyRegistry();

            return new
            {
                Cnpj = company?.TaxIdentificationNumber,
                RazaoSocial = company?.CompanyName,
                NomeFantasia = company?.TradingName,
                Situacao = company?.Status ?? "Analise Pendente",
                CorSituacao =
                    company?.Status == null ?
                        "Warning" :
                        (company.Status.Equals("OK", true) ? "Good" : "Warning"),
                Endereco =
                    company?.CompanyAddress == null ?
                        string.Empty : (
                        company.CompanyAddress.Equals(new CompanyRegistry().CompanyAddress, true) ?
                            string.Empty :
                            $"{company?.CompanyAddress?.Street}, {company?.CompanyAddress?.Number} - {company?.CompanyAddress?.District}, {company?.CompanyAddress?.City} - {company?.CompanyAddress?.State}, {company?.CompanyAddress?.PostalCode}"
                        ),
                SociosAdministradores =
                    string.Join(", ",
                        company?.CompanyPartners
                        ?.Select(i => new
                        {
                            PartnerAdmin = $"{i?.Name} ({i?.LeadingPosition})"
                        })
                        ?.Select(i => i?.PartnerAdmin)
                        ?.ToList()
                    ),
            };
        }

        #region Validators
        private async Task<bool> CnpjPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                var cnpj = promptContext.Recognized.Value?.Trim()?.Replace('–', '-');

                if (cnpj.Equals(cnpj?.Digits('.', '/', '-'), true))
                    if (cnpj.IsCnpjValid())
                        valid = true;
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
                    var form = JsonConvert.DeserializeObject<UserCompany>(promptContext.Recognized.Value);

                    if (!string.IsNullOrEmpty(form?.CompanyName?.Trim()))
                        if (!string.IsNullOrEmpty(form?.CompanyAddress?.Trim()))
                            if (!string.IsNullOrEmpty(form?.CompanyPartners?.Trim()))
                                valid = true;
                }
            }

            return await Task.FromResult(valid);
        }
        #endregion
    }
}
