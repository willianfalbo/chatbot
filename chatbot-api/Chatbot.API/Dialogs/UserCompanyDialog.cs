using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Chatbot.API.DTO;
using Chatbot.Common.Extensions;
using Chatbot.Common.Interfaces;
using System.IO;
using Chatbot.API.Extensions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Linq;
using Chatbot.API.Helpers;
using Chatbot.Model.Manager;

namespace Microsoft.BotBuilderSamples
{
    public class UserCompanyDialog : CustomComponentDialog
    {
        #region Attributes
        private const string USER_COMPANY_STEP = "USER_COMPANY_STEP";
        private const string CNPJ_VALIDATION = "CNPJ_VALIDATION";
        private readonly IDialogHelper _helper;
        private readonly ICompanyRegistryManager _companyRegistryManager;
        #endregion

        public UserCompanyDialog(
            IDialogHelper helper,
            ICompanyRegistryManager companyRegistryManager)
            : base(nameof(UserCompanyDialog))
        {
            this._helper = helper ?? throw new System.ArgumentNullException(nameof(helper));
            this._companyRegistryManager = companyRegistryManager ?? throw new System.ArgumentNullException(nameof(companyRegistryManager));

            AddDialog(new TextPrompt(CNPJ_VALIDATION, CnpjPromptValidatorAsync));
            AddDialog(new CustomAdaptiveCardPrompt(nameof(CustomAdaptiveCardPrompt), FormPromptValidatorAsync));
            AddDialog(new UserSocioEconomicDialog(helper));

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
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            // Create an object in which to collect the user's information within the dialog.
            stepContext.Values[USER_COMPANY_STEP] = new UserCompanyDTO();

            var conversation = await _helper.GetConversationState<UserConversationDTO>(stepContext.Context, cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"{conversation?.UserProfile?.Name}, qual é o CNPJ da sua empresa?"),
                RetryPrompt = MessageFactory.Text("Este não é um CNPJ válido! Tente novamente.")
            };
            // Ask the user to enter their name.
            return await stepContext.PromptAsync(CNPJ_VALIDATION, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForCheckingCompanyDetailsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _helper.SendTypingActivity(stepContext.Context, cancellationToken);

            // Set the user's chatting confirmation to what they entered in response to the prompt.
            var company = (UserCompanyDTO)stepContext.Values[USER_COMPANY_STEP];
            company.TaxIdentificationNumber = stepContext.Result?.ToString()?.Trim();

            var result = await _companyRegistryManager.SearchForCnpj(company.TaxIdentificationNumber);
            var message = string.Empty;

            if (!result.HasError)
            {
                if (result?.Value != null)
                    message += "Eu já identifiquei suas informações. \nPoderia confirmar se os dados abaixo estão corretos? ";
                else
                    message += "Não encontrei nenhuma informação. \nPor favor, preencha o formulário. ";
            }
            else
            {
                message += $"\nInfelizmente eu encontrei alguns problemas. Erros: {string.Join(", ", result.Errors)}";
                message += "\nPor favor, preencha o formulário.";
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(message), cancellationToken);

            var filePath = Path.Combine(".", "Resources", "AdaptiveCard", "UserCompanyForm.json");

            var data = UserCompanyFormData(result?.Value ?? new CompanyRegistry() { TaxIdentificationNumber = company.TaxIdentificationNumber });
            var cardAttachment = _helper.CreateAdaptiveCardAttachment(filePath, data);
            var promptOptions = new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(cardAttachment),
                RetryPrompt = MessageFactory.Text("Por favor, verifique os campos obrigatórios e clique em \"Confirmar\"")
            };
            return await stepContext.PromptAsync(nameof(CustomAdaptiveCardPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> StartUserSocioEconomicDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userCompany = (UserCompanyDTO)stepContext.Values[USER_COMPANY_STEP];
            userCompany = JsonConvert.DeserializeObject<UserCompanyDTO>(stepContext.Result?.ToString());

            // save the User Company data into the Conversation State
            var conversation = await _helper.GetConversationState<UserConversationDTO>(stepContext.Context, cancellationToken);
            conversation.UserCompany = userCompany;

            // save conversation into the document db
            await this._helper.SaveConversationDB(stepContext.Context, cancellationToken);

            // begin the next dialog
            return await stepContext.BeginDialogAsync(nameof(UserSocioEconomicDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Exit the dialog, returning the collected data information
            return await stepContext.EndDialogAsync(stepContext.Values[USER_COMPANY_STEP], cancellationToken);
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
                        (company.Status.IsEqual("OK") ? "Good" : "Warning"),
                Endereco =
                    company?.CompanyAddress == null ?
                        string.Empty : (
                        company.CompanyAddress.IsEqual(new CompanyRegistry().CompanyAddress) ?
                            string.Empty :
                            $"{company?.CompanyAddress?.Street}, {company?.CompanyAddress?.Number} - {company?.CompanyAddress?.District}, {company?.CompanyAddress?.City} - {company?.CompanyAddress?.State}, {company?.CompanyAddress?.PostalCode}"
                        ),
                SociosAdministradores =
                    string.Join(",\\n",
                        company?.CompanyPartners
                        ?.Select(i => new
                        {
                            PartnerAdmin = $"{i?.Name} ({i?.LeadingPosition})"
                        })
                        ?.Select(i => i?.PartnerAdmin)
                        ?.ToList()
                    ),
                WebUiAppUrl = _helper._appSettings.WebUiAppUrl,
            };
        }

        #region Validators
        private async Task<bool> CnpjPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;
            if (promptContext.Recognized.Succeeded)
            {
                var cnpj = promptContext.Recognized.Value?.Trim()?.Replace('–', '-');

                // if (cnpj.IsEqual(cnpj?.Digits('.', '/', '-')))
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
                    var form = JsonConvert.DeserializeObject<UserCompanyDTO>(promptContext.Recognized.Value);

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
