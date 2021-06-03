using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Contém informações de boletos bancários e linhas digitável
    /// </summary>
    public class InfoBoletos
    {

        public Int32 numero_contrato { get; set; }
        public string nome_empreendimento { get; set; }
        public string nome_bloco { get; set; }
        public string nome_unidade { get; set; }
        public string cedente_cpf_cnpj { get; set; }
        public string cedente { get; set; }
        public Int32 banco_numero { get; set; }
        public string banco { get; set; }
        public Int32 numero_parcela { get; set; }
        public string vencimento { get; set; }
        public string valor { get; set; }
        public string linha_digitavel { get; set; }
    }
}
