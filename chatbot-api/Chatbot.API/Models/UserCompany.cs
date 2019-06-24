using System.ComponentModel.DataAnnotations;

namespace Chatbot.API.Models
{
    public class UserCompany
    {
        [Required(AllowEmptyStrings = false)]
        public string CompanyName { get; set; } // razao social

        public string TradingName { get; set; } // nome fantasia
        
        [Required(AllowEmptyStrings = false)]
        public string TaxIdentificationNumber { get; set; } // cnpj
        
        [Required(AllowEmptyStrings = false)]
        public string Status { get; set; } // situacao
        
        [Required(AllowEmptyStrings = false)]
        public string CompanyAddress { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        public string CompanyPartners { get; set; }
    }
}