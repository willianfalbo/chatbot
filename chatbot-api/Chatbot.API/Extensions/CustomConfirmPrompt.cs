using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Chatbot.Api.Extensions
{
    public class CustomConfirmPrompt : ConfirmPrompt
    {
        public CustomConfirmPrompt(string dialogId, PromptValidator<bool> validator = null, string defaultLocale = null)
            : base(dialogId, validator, defaultLocale)
        {
            base.ConfirmChoices = CustomizedConfirmChoices();
            base.DefaultLocale = Thread.CurrentThread.CurrentUICulture.Name;
        }

        private Tuple<Choice, Choice> CustomizedConfirmChoices() =>
            new Tuple<Choice, Choice>(PositiveOption(), NegativeOption());

        private Choice PositiveOption() =>
            new Choice()
            {
                Value = "Sim",
                Synonyms =
                    PositiveOptions()
                    .Where(key => !string.IsNullOrWhiteSpace(key))
                    .ToList()
                    .GenerateUnaccented()
            };

        private Choice NegativeOption() =>
            new Choice()
            {
                Value = "Não",
                Synonyms =
                    NegativeOptions()
                    .Where(key => !string.IsNullOrWhiteSpace(key))
                    .ToList()
                    .GenerateUnaccented()
            };

        /// <summary>Positive options for users em prompted.</summary>
        /// <returns>List of positive options.</returns>
        private List<string> PositiveOptions() =>
            new List<string>()
            {
                "ok",
                "tá ok",
                "está ok",
                "okay",
                "tá okay",
                "está okay",
                "yes",
                "true",
                "sim",
                "boa",
                "tá bem",
                "está bem",
                "está tudo bem",
                "tudo bem",
                "certo",
                "tá certo",
                "tá tudo certo",
                "está certo",
                "está tudo certo",
                "combinado",
                "tá combinado",
                "está combinado",
                "correto",
                "tá correto",
                "está correto",
                "perfeito",
                "tá perfeito",
                "está perfeito",
                "claro",
                "é claro",
                "eh claro",
                "certamente",
                "quero",
                "eu quero",
                "legal",
                "que legal",
                "tá legal",
                "está legal",
                "aceito",
                "eu aceito",
                "verdade",
                "concordo",
                "positivo",
                "tá positivo",
                "com certeza",
                "válido",
                "tá válido",
                "está válido",
                "exato",
                "tá exato",
                "está exato",
                "justo",
                "tá justo",
                "está justo",
                "lógico",
            };

        /// <summary>Negative options for users em prompted.</summary>
        /// <returns>List of negative options.</returns>
        private List<string> NegativeOptions() =>
            new List<string>()
            {
                "no",
                "não",
                "false",
                "falso",
                "nada bom",
                "nem pensar",
                "não tá bom",
                "não tá certo",
                "não está certo",
                "nada combinado",
                "incorreto",
                "tá incorreto",
                "está incorreto",
                "claro que não",
                "certamente não",
                "não quero",
                "eu não quero",
                "não é legal",
                "não eh legal",
                "não aceito",
                "eu não aceito",
                "não é verdade",
                "não eh verdade",
                "não concordo",
                "eu não concordo",
                "negativo",
                "com certeza não",
                "inválido",
                "tá inválido",
                "está inválido",
                "não é justo",
                "não tá justo",
                "não está justo",
                "lógico que não",
                "de jeito nenhum",
            };

    }
}