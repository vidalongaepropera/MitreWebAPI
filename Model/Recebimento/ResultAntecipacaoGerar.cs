using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    public class ResultAntecipacaoGerar
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Int32 seq_antecipacao_id { get; set; }
        public string cedente_cpf_cnpj { get; set; }
        public string cedente { get; set; }
        public Int32 banco_numero { get; set; }
        public string banco { get; set; }
        public string linha_digitavel { get; set; }
        public string vencimento { get; set; }
        public string valor { get; set; }
    }
}
