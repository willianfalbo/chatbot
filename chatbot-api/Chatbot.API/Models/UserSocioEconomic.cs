using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chatbot.API.Models
{
    public class UserSocioEconomic
    {
        public decimal MonthlyIncome { get; set; }
        public List<FamilyIncome> FamilyIncomes { get; set; } = new List<FamilyIncome>();
        public FamilyExpense FamilyExpense { get; set; }
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

    public class FamilyIncome
    {
        public FamilyIncome() { }
        public FamilyIncome(string personsName, string source, decimal value)
        {
            this.PersonsName = personsName;
            this.Source = source;
            this.Value = value;
        }

        public string PersonsName { get; set; }
        public string Source { get; set; }
        public decimal Value { get; set; }
    }

    public class FamilyExpense
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
}