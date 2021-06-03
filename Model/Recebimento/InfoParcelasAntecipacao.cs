using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    /// <summary>
    /// Lista detalhes das parcelas para antecipação
    /// </summary>
    public class InfoParcelasAntecipacao
    {
        public int id { get; set; }
        public string key { get; set; }
        public string dataVencimento { get; set; }
        public string fator { get; set; }
        public string origem { get; set; }
        public double antReCapitalizado { get; set; }
        public double antReDesconto { get; set; }
        public double antReSaldoAtual { get; set; }
        public double bonificacao { get; set; }
        public double cndiReTaxTabPrice { get; set; }
        public double taxaAntecipacao { get; set; }
        public double taxaBonificacao { get; set; }
        public string tipo { get; set; }
        public double valorAPagar { get; set; }
        public double valorAntecipado { get; set; }
        public double valorAtual { get; set; }
        public double valorCobrado { get; set; }
        public double valorLiquido { get; set; }
        public double valorOriginal { get; set; }
        public double valorTotal { get; set; }
        public double valorVinculado { get; set; }

    }
}
