using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    /// <summary>
    /// Detalha valores pagos
    /// </summary>
    public class InfoContratoValorPago
    {
        public Int32 numero_parcela { get; set; }
        public string data_vencimento { get; set; }
        public string data_pagamento { get; set; }
        public string valor { get; set; }

    }
}
