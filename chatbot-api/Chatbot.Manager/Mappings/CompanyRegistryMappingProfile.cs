using AutoMapper;
using Chatbot.Model.Service;
using Chatbot.Common.Extensions;
using System.Linq;
using Chatbot.Model.Manager;

namespace Chatbot.Manager.Mappings
{
    public class CompanyRegistryMappingProfile : Profile
    {
        public CompanyRegistryMappingProfile()
        {
            CreateMap<ReceitaServiceResponse, CompanyRegistry>()
                .ForMember(dest => dest.CompanyName, opts => opts.MapFrom(src => src.Nome.TitleCase()))
                .ForMember(dest => dest.TradingName, opts => opts.MapFrom(src => src.Fantasia.TitleCase()))
                .ForMember(dest => dest.TaxIdentificationNumber, opts => opts.MapFrom(src => src.Cnpj))
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.TitleCase()))
                .ForMember(dest => dest.CompanyAddress, opts => opts.MapFrom(src =>
                    new CompanyAddress
                    {
                        City = src.Municipio.TitleCase(),
                        Country = "Brasil",
                        PostalCode = src.Cep.Digits('-'),
                        Number = src.Numero,
                        Street = src.Logradouro.TitleCase(),
                        District = src.Bairro.TitleCase(),
                        State = src.Uf,
                    }
                ))
                .ForMember(dest => dest.CompanyPartners, opts => opts.MapFrom(src =>
                    src.Qsa.Select(qsa => new CompanyPartner
                    {
                        Name = qsa.Nome.TitleCase(),
                        LeadingPosition = qsa.Qual.Letters(),
                    })
                    .ToList()
                ));
        }
    }
}