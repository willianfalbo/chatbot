using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chatbot.Model.Service
{
    public class UserConversationReqResp
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }

        [JsonProperty("userPreferences", NullValueHandling = NullValueHandling.Ignore)]
        public UserPreferenceReqResp UserPreference { get; set; }

        [JsonProperty("userProfile", NullValueHandling = NullValueHandling.Ignore)]
        public UserProfileReqResp UserProfile { get; set; }

        [JsonProperty("userCompany", NullValueHandling = NullValueHandling.Ignore)]
        public UserCompanyReqResp UserCompany { get; set; }

        [JsonProperty("socioEconomic", NullValueHandling = NullValueHandling.Ignore)]
        public UserSocioEconomicReqResp UserSocioEconomic { get; set; }
    }

    public class UserPreferenceReqResp
    {
        [JsonProperty("option", NullValueHandling = NullValueHandling.Ignore)]
        public string UserOption { get; set; }
    }

    public class UserProfileReqResp
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("age", NullValueHandling = NullValueHandling.Ignore)]
        public int Age { get; set; }

        [JsonProperty("acceptedAgreement", NullValueHandling = NullValueHandling.Ignore)]
        public bool AcceptedAgreement { get; set; }

        [JsonProperty("wantsToProvideAge", NullValueHandling = NullValueHandling.Ignore)]
        public bool WantsToProvideAge { get; set; }
    }

    public class UserCompanyReqResp
    {
        [JsonProperty("companyName", NullValueHandling = NullValueHandling.Ignore)]
        public string CompanyName { get; set; }

        [JsonProperty("tradingName", NullValueHandling = NullValueHandling.Ignore)]
        public string TradingName { get; set; }

        [JsonProperty("taxIdentification", NullValueHandling = NullValueHandling.Ignore)]
        public string TaxIdentificationNumber { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("companyAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string CompanyAddress { get; set; }

        [JsonProperty("companyPartners", NullValueHandling = NullValueHandling.Ignore)]
        public string CompanyPartners { get; set; }
    }

    public class UserSocioEconomicReqResp
    {
        [JsonProperty("companyMonthlyIncome", NullValueHandling = NullValueHandling.Ignore)]
        public decimal MonthlyIncome { get; set; }

        [JsonProperty("familyIncomes", NullValueHandling = NullValueHandling.Ignore)]
        public List<FamilyIncomeReqResp> FamilyIncomes { get; set; }

        [JsonProperty("familyExpenses", NullValueHandling = NullValueHandling.Ignore)]
        public FamilyExpenseReqResp FamilyExpense { get; set; }

        [JsonProperty("totalFamilyIncomePerMonth", NullValueHandling = NullValueHandling.Ignore)]
        public decimal TotalFamilyIncome { get; set; }

        [JsonProperty("totalIncomePerMonth", NullValueHandling = NullValueHandling.Ignore)]
        public decimal TotalMonthlyIncome { get; set; }
    }

    public class FamilyIncomeReqResp
    {
        [JsonProperty("personsName", NullValueHandling = NullValueHandling.Ignore)]
        public string PersonsName { get; set; }

        [JsonProperty("sourceOfIncome", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Value { get; set; }
    }

    public class FamilyExpenseReqResp
    {
        [JsonProperty("foodAndBasicCare", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? FoodAndBasicCare { get; set; }

        [JsonProperty("health", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Health { get; set; }

        [JsonProperty("education", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Education { get; set; }

        [JsonProperty("rentalAndHousing", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? RentalAndHousing { get; set; }

        [JsonProperty("housingRelatedBills", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? HousingRelatedBills { get; set; }

        [JsonProperty("otherExpenses", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? OtherExpenses { get; set; }
    }
}