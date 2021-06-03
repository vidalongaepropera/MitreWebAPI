using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{

    /// <summary>
    /// Informações da forma de pagamento
    /// tipo: Tipo de parcela S = Sinal, M = Mensal e I = Intermediária  
    /// </summary>
    public class InfoPropostaPagto
    {
        public Int32 quantidade { get; set; }
        //[MaxLength(1)]
        public string  tipo { get; set; }
        public double valor { get; set; }
        //[MaxLength(10)]
        public string data_vencimento { get; set; }



    }
}
