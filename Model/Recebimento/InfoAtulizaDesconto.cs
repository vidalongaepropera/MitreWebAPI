using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    public class InfoAtulizaDesconto
    {
        public string contrato_id { get; set; }
        public string cpf_cnpj { get; set; }
        public string dt_vcto_boleto { get; set; }
        public List<InfoParcelasGerarAntecipacao> Parcelas { get; set; }
    }
}
