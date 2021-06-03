using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    public class BaseGerarAntecipacao
    {
        public string bonificacao { get; set; }
        public string codBloco { get; set; }
        public string codContrato { get; set; }
        public string codEmpreendimento { get; set; }
        public string dataBaixa { get; set; }
        public string desconto { get; set; }
        public string orgInCodigo { get; set; }
		public string filInCodigo { get; set; }
		public List<string> parcelas { get; set; }
        public string qtdParcelas { get; set; }
        public string soma { get; set; }
        public string valorAtual { get; set; }
        public string vlrAntecipado { get; set; }
        public string vlrAPagar { get; set; }
        public string vlrLiquido { get; set; }
        public string vlrPagar { get; set; }

		/*
		public string antReCapitalizado { get; set; } = "0.00";
		public string taxaAntecipacao { get; set; } = "0.00";
		public string antReSaldoAtual { get; set; } = "0.00";
		public string vlrValorAntecipado { get; set; } = "0.00";
		public string antReDesconto { get; set; } = "0.00";
		public string tipoParcela { get; set; } = "M";
		public string taxaBonificacao { get; set; } = "0.00";
		public string seqAntecipacao { get; set; } = "0";
		public string customBonificacao { get; set; } = "0.00";
		public string ctoReTaxaBonif { get; set; } = "0.00";
		public string cnditReTaxTabPrice { get; set; } = "0.00";
		public string origem { get; set; } = "Contrato";
		public string vencimento { get; set; } = "";
		public string vlrAtualizado { get; set; } = "0.00";
		public string vlrOriginal { get; set; } = "0.00";
		public string vlrVinculado { get; set; } = "0.00";
		public string vlrDiferencialTxs { get; set; } = "0.00";
		public string vlrCobrado { get; set; } = "0.00";
		public string msgRequest { get; set; } = "";
		public string statusRequest { get; set; } = "";
		public string observacao { get; set; } = "";
		public string vlrLiquidoHiddem { get; set; } = "0.00";
		public string parcelasSelecionadas { get; set; } = "0";
		public string fator { get; set; } = "";
		public string isBloqueado { get; set; } = "false";
		public string protocolo { get; set; } = "";
		public string indLiberacao { get; set; } = "true";
		public string valorResiduo { get; set; } = "0.00";
		public string codEstrutura { get; set; } = "0";*/

	}
}
