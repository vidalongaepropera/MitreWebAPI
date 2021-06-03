using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Retorna as informações de agentes SAC que atende determinado produto
    /// </summary>
    public class ResultAgenteEmpreendimento
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Int32 agente_sac_1 { get; set; }
        public Int32 agente_sac_2 { get; set; }
        public Int32 agente_sac_3 { get; set; }
    }
}
