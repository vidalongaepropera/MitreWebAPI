using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    public class InfoAntecipacaoGerar
    {
        public Int32 contrato_id { get; set; }
        public string cpf_cnpj { get; set; }
        public string dt_vcto_boleto { get; set; }
        public string valor_total_pago { get; set; }
        public string valor_total_liquido { get; set; }
        public string valor_total_desconto { get; set; }
        public List<InfoParcelasGerarAntecipacao> Parcelas { get; set; }
    }
}
