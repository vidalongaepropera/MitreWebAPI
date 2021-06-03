using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{

    /// <summary>
    /// Informações da proposta para criação do contrato
    /// </summary>
    public class InfoProposta
    {
        //[MaxLength(999999999)]
        public Int32 contrato_ori_id { get; set; }
        //[MaxLength(10)]
        public string data_assinatura { get; set; }
        //[MaxLength(999999999)]
        public double pct_multa { get; set; }
        //[MaxLength(999999999)]
        public double pct_juros { get; set; }
        public List<InfoCliente> cliente { get; set; }
        public List<InfoPropostaPagto> forma_pagamento { get; set; }
        public List<InfoTaxasAdicionais> taxas_adicionais { get; set; }

        

    }
}
