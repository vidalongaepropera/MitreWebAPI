using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Contém informações da unidade e contrato do cliente
    /// </summary>
    public class InfoContratos
    {
        public Int32 numero_organizacao { get; set; }
        public Int32 numero_filial { get; set; }
        public Int32 numero_contrato { get; set; }
        public Int32 numero_empreendimento { get; set; }
        public string empreendimento { get; set; }
        public Int32 numero_bloco { get; set; }
        public string bloco { get; set; }
        public Int32 numero_unidade { get; set; }
        public string unidade { get; set; }
        public Int32 numero_tipologia { get; set; }
        public string tipologia { get; set; }
        public string status_contrato { get; set; }
        public Int32 numero_empreendimento_sac { get; set; }
        public Int32 numero_bloco_sac { get; set; }
        public Int32 numero_unidade_sac { get; set; }
        public Int32 agente_sac_1 { get; set; }
        public Int32 agente_sac_2 { get; set; }
        public Int32 agente_sac_3 { get; set; }
    }
}
