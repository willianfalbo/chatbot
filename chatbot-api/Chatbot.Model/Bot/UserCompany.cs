namespace Chatbot.Model.Bot
{
    public class UserCompany
    {
        public string CompanyName { get; set; } // razao social
        public string TradingName { get; set; } // nome fantasia
        public string TaxIdentificationNumber { get; set; } // cnpj
        public string Status { get; set; } // situacao
        public string CompanyAddress { get; set; }
        public string CompanyPartners { get; set; }
    }
}