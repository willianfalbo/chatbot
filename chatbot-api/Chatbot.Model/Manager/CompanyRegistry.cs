using System.Collections.Generic;

namespace Chatbot.Model.Manager
{
    public class CompanyRegistry
    {
        public string CompanyName { get; set; } // razao social
        public string TradingName { get; set; } // nome fantasia
        public string TaxIdentificationNumber { get; set; } // cnpj
        public string Status { get; set; } // situacao
        public CompanyAddress CompanyAddress { get; set; } = new CompanyAddress();
        public List<CompanyPartner> CompanyPartners { get; set; } = new List<CompanyPartner>();
    }

    public class CompanyAddress
    {
        public string City { get; set; } // cidade / municipio
        public string Country { get; set; }
        public string PostalCode { get; set; } // cep
        public string Number { get; set; }
        public string Street { get; set; } // logradouro
        public string District { get; set; } // bairro
        public string State { get; set; } // uf / estado
    }

    public class CompanyPartner
    {
        public string Name { get; set; }
        public string LeadingPosition { get; set; }
    }
}