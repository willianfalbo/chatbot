using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Chatbot.Api.DTO
{
    public class UserConversationDTO
    {
        public string UserId { get; set; }
        public UserPreferenceDTO UserPreference { get; set; } = new UserPreferenceDTO();
        public UserProfileDTO UserProfile { get; set; } = new UserProfileDTO();
        public UserCompanyDTO UserCompany { get; set; } = new UserCompanyDTO();
        public UserSocioEconomicDTO UserSocioEconomic { get; set; } = new UserSocioEconomicDTO();
    }

    public class UserPreferenceDTO
    {
        public string UserOption { get; set; }

        public static List<string> ChatbotOptions() =>
            new List<string> {
                UserHelpOption.MICROCREDITO,
                UserHelpOption.CAPITAL_GIRO,
                UserHelpOption.CAPITAL_GIRO_CARTORIO,
            };
    }

    public class UserProfileDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public bool AcceptedAgreement { get; set; }
        public bool WantsToProvideAge { get; set; }
    }

    public class UserCompanyDTO
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

    public class UserSocioEconomicDTO
    {
        public decimal MonthlyIncome { get; set; }
        public List<FamilyIncomeDTO> FamilyIncomes { get; set; } = new List<FamilyIncomeDTO>();
        public FamilyExpenseDTO FamilyExpense { get; set; }
        public decimal TotalFamilyIncome
        {
            get
            {
                return FamilyIncomes.Sum(i => i.Value);
            }
        }
        public decimal TotalMonthlyIncome
        {
            get
            {
                return MonthlyIncome + TotalFamilyIncome;
            }
        }
    }

    public class FamilyIncomeDTO
    {
        public FamilyIncomeDTO() { }
        public FamilyIncomeDTO(string personsName, string source, decimal value)
        {
            this.PersonsName = personsName;
            this.Source = source;
            this.Value = value;
        }

        public string PersonsName { get; set; }
        public string Source { get; set; }
        public decimal Value { get; set; }
    }

    public class FamilyExpenseDTO
    {
        [Required]
        public decimal? FoodAndBasicCare { get; set; }

        [Required]
        public decimal? Health { get; set; }

        [Required]
        public decimal? Education { get; set; }

        [Required]
        public decimal? RentalAndHousing { get; set; }

        [Required]
        public decimal? HousingRelatedBills { get; set; }

        public decimal? OtherExpenses { get; set; }
    }

    public static class UserHelpOption
    {
        public const string MICROCREDITO = "Microcrédito";
        public const string CAPITAL_GIRO = "Capital de Giro";
        public const string CAPITAL_GIRO_CARTORIO = "Capital de Giro Cartório";
    }
}