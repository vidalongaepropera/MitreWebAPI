using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    public class BaseParcelasAntecipacao
    {
        public string antReCapitalizado { get; set; }
        public string antReDesconto { get; set; }
        public string antReSaldoAtual { get; set; }
        public string bonificacao { get; set; }
        public string cndiReTaxTabPrice { get; set; }
        public string codBloco { get; set; }
        public string codEmpreendimento { get; set; }
        public string dataVencimento { get; set; }
        public string fator { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string orgInCodigo { get; set; }
        public string origem { get; set; }
        public string taxaAntecipacao { get; set; }
        public string taxaBonificacao { get; set; }
        public string tipo { get; set; }
        public string valorAPagar { get; set; }
        public string valorAntecipado { get; set; }
        public string valorAtual { get; set; }
        public string valorCobrado { get; set; }
        public string valorLiquido { get; set; }
        public string valorOriginal { get; set; }
        public string valorTotal { get; set; }
        public string valorVinculado { get; set; }

    }
}
