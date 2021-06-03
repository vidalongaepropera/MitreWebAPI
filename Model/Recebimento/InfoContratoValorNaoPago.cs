using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    /// <summary>
    /// Lista detalhes das parcelas em aberto para quitação
    /// </summary>
    public class InfoContratoValorNaoPago
    {
        public Int32 numero_parcela { get; set; }
        public string data_vencimento { get; set; }
        public string valor_futuro { get; set; }
        public string valor_presente { get; set; }
    }
}
