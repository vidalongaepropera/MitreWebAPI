using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{

    /// <summary>
    /// Lista o total pago e quitação na última data base e detalhe das parcelas
    /// </summary>
    public class ResultContratoPagoQuitacao
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Int32 numero_contrato { get; set; }
        public string valor_total_pago { get; set; }
        public string valor_quitacao { get; set; }
        public string data_base { get; set; }
        public List<InfoContratoValorPago> results_pago { get; set; }
        public List<InfoContratoValorNaoPago> results_quitacao { get; set; }


    }
}
