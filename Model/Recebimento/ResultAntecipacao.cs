using MitreWebAPI.Model.Recebimento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    /// <summary>
    /// Lista as parcelas para antecipação de um determinado contrato
    /// </summary>
    public class ResultAntecipacao
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string orgInCodigo { get; set; }
        public string codEmpreendimento { get; set; }
        public string codBloco { get; set; }
        public List<InfoParcelasAntecipacao> Parcelas { get; set; }
    }
}
