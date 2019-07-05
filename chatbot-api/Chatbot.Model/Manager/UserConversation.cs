using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chatbot.Model.Manager
{
    public class UserConversation
    {
        public string UserId { get; set; }
        public UserPreference UserPreference { get; set; } = new UserPreference();
        public UserProfile UserProfile { get; set; } = new UserProfile();
        public UserCompany UserCompany { get; set; } = new UserCompany();
        public UserSocioEconomic UserSocioEconomic { get; set; } = new UserSocioEconomic();
    }

    public class UserPreference
    {
        public string UserOption { get; set; }
    }

    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public bool AcceptedAgreement { get; set; }
        public bool WantsToProvideAge { get; set; }
    }

    public class UserCompany
    {
        public string CompanyName { get; set; }
        public string TradingName { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string Status { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPartners { get; set; }
    }

    public class UserSocioEconomic
    {
        public decimal MonthlyIncome { get; set; }
        public List<FamilyIncome> FamilyIncomes { get; set; }
        public FamilyExpense FamilyExpense { get; set; }
        public decimal TotalFamilyIncome { get; set; }
        public decimal TotalMonthlyIncome { get; set; }
    }

    public class FamilyIncome
    {
        public string PersonsName { get; set; }
        public string Source { get; set; }
        public decimal Value { get; set; }
    }

    public class FamilyExpense
    {
        public decimal? FoodAndBasicCare { get; set; }
        public decimal? Health { get; set; }
        public decimal? Education { get; set; }
        public decimal? RentalAndHousing { get; set; }
        public decimal? HousingRelatedBills { get; set; }
        public decimal? OtherExpenses { get; set; }
    }
}