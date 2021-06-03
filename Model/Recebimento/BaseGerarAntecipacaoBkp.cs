using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    public class BaseGerarAntecipacaoBkp
    {
		public string orgInCodigo { get; set; } //OK
		public string codContrato { get; set; } //OK
		public List<string> parcelas { get; set; } //OK
		public string dataBaixa { get; set; } //OK
		public string qtdParcelas { get; set; } = "0,00"; //OK
		public string antReCapitalizado { get; set; } = "0,00";
		public string taxaAntecipacao { get; set; } = "0,00";
		public string valorAtual { get; set; } = "0,00";
		public string antReSaldoAtual { get; set; } = "0,00";
		public string bonificacao { get; set; } = "0,00";
		public string vlrValorAntecipado { get; set; } = "0,00";
		public string antReDesconto { get; set; } = "0,00";
		public string desconto { get; set; } = "0,00"; //OK
		public string vlrAPagar { get; set; } = "0,00";
		public string vlrLiquido { get; set; } = "0,00";
		public string tipoParcela { get; set; }
		public string taxaBonificacao { get; set; } = "0,00";
		public string seqAntecipacao { get; set; }
		public string customBonificacao { get; set; } = "0,00";
		public string vlrAntecipado { get; set; } = "0,00";
		public string ctoReTaxaBonif { get; set; } = "0,00";
		public string cnditReTaxTabPrice { get; set; } = "0,00";
		public string origem { get; set; }
		public string vencimento { get; set; }
		public string vlrAtualizado { get; set; }
		public string filInCodigo { get; set; }
		public string vlrOriginal { get; set; }
		public string vlrVinculado { get; set; } = "0,00";
		public string vlrDiferencialTxs { get; set; } = "0,00";
		public string vlrCobrado { get; set; } = "0,00";
		public string msgRequest { get; set; } = "";
		public string statusRequest { get; set; } = "";
		public string observacao { get; set; } = "";
		public string vlrLiquidoHiddem { get; set; } = "0,00";
		public string parcelasSelecionadas { get; set; }
		public string fator { get; set; } = "";
		public string isBloqueado { get; set; } = "false";
		public string protocolo { get; set; } = "";
		public string codBloco { get; set; } //OK
		public string indLiberacao { get; set; } = "true";
		public string valorResiduo { get; set; } = "0,00";
		public string codEmpreendimento { get; set; } //OK
		public string codEstrutura { get; set; } = "0";
	}
}
