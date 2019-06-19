// AUTO GENERATED FROM
// https://app.quicktype.io/#l=cs&r=json2csharp

namespace Chatbot.Model.Service
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ReceitaServiceResponse
    {
        [JsonProperty("atividade_principal", NullValueHandling = NullValueHandling.Ignore)]
        public List<Atividade> AtividadePrincipal { get; set; }

        [JsonProperty("data_situacao", NullValueHandling = NullValueHandling.Ignore)]
        public string DataSituacao { get; set; }

        [JsonProperty("nome", NullValueHandling = NullValueHandling.Ignore)]
        public string Nome { get; set; }

        [JsonProperty("uf", NullValueHandling = NullValueHandling.Ignore)]
        public string Uf { get; set; }

        [JsonProperty("telefone", NullValueHandling = NullValueHandling.Ignore)]
        public string Telefone { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("atividades_secundarias", NullValueHandling = NullValueHandling.Ignore)]
        public List<Atividade> AtividadesSecundarias { get; set; }

        [JsonProperty("qsa", NullValueHandling = NullValueHandling.Ignore)]
        public List<Qsa> Qsa { get; set; }

        [JsonProperty("situacao", NullValueHandling = NullValueHandling.Ignore)]
        public string Situacao { get; set; }

        [JsonProperty("bairro", NullValueHandling = NullValueHandling.Ignore)]
        public string Bairro { get; set; }

        [JsonProperty("logradouro", NullValueHandling = NullValueHandling.Ignore)]
        public string Logradouro { get; set; }

        [JsonProperty("numero", NullValueHandling = NullValueHandling.Ignore)]
        public string Numero { get; set; }

        [JsonProperty("cep", NullValueHandling = NullValueHandling.Ignore)]
        public string Cep { get; set; }

        [JsonProperty("municipio", NullValueHandling = NullValueHandling.Ignore)]
        public string Municipio { get; set; }

        [JsonProperty("porte", NullValueHandling = NullValueHandling.Ignore)]
        public string Porte { get; set; }

        [JsonProperty("abertura", NullValueHandling = NullValueHandling.Ignore)]
        public string Abertura { get; set; }

        [JsonProperty("natureza_juridica", NullValueHandling = NullValueHandling.Ignore)]
        public string NaturezaJuridica { get; set; }

        [JsonProperty("cnpj", NullValueHandling = NullValueHandling.Ignore)]
        public string Cnpj { get; set; }

        [JsonProperty("ultima_atualizacao", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? UltimaAtualizacao { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("tipo", NullValueHandling = NullValueHandling.Ignore)]
        public string Tipo { get; set; }

        [JsonProperty("fantasia", NullValueHandling = NullValueHandling.Ignore)]
        public string Fantasia { get; set; }

        [JsonProperty("complemento", NullValueHandling = NullValueHandling.Ignore)]
        public string Complemento { get; set; }

        [JsonProperty("efr", NullValueHandling = NullValueHandling.Ignore)]
        public string Efr { get; set; }

        [JsonProperty("motivo_situacao", NullValueHandling = NullValueHandling.Ignore)]
        public string MotivoSituacao { get; set; }

        [JsonProperty("situacao_especial", NullValueHandling = NullValueHandling.Ignore)]
        public string SituacaoEspecial { get; set; }

        [JsonProperty("data_situacao_especial", NullValueHandling = NullValueHandling.Ignore)]
        public string DataSituacaoEspecial { get; set; }

        [JsonProperty("capital_social", NullValueHandling = NullValueHandling.Ignore)]
        public string CapitalSocial { get; set; }

        [JsonProperty("extra", NullValueHandling = NullValueHandling.Ignore)]
        public Extra Extra { get; set; }

        [JsonProperty("billing", NullValueHandling = NullValueHandling.Ignore)]
        public Billing Billing { get; set; }

        //THIS ATTIBUTE IS FILLED IN CASE OF ERROR
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }

    public partial class Atividade
    {
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
    }

    public partial class Billing
    {
        [JsonProperty("free", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Free { get; set; }

        [JsonProperty("database", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Database { get; set; }
    }

    public partial class Extra
    {
    }

    public partial class Qsa
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        public string Qual { get; set; }

        [JsonProperty("qual_rep_legal", NullValueHandling = NullValueHandling.Ignore)]
        public string QualRepLegal { get; set; }

        [JsonProperty("nome_rep_legal", NullValueHandling = NullValueHandling.Ignore)]
        public string NomeRepLegal { get; set; }

        [JsonProperty("pais_origem", NullValueHandling = NullValueHandling.Ignore)]
        public string PaisOrigem { get; set; }

        [JsonProperty("nome", NullValueHandling = NullValueHandling.Ignore)]
        public string Nome { get; set; }
    }
}
