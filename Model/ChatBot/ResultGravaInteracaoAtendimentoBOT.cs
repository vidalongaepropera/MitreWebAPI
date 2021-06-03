using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Devolve a informação de se a interação do bloco atendimento do BOT ocorreu com sucesso.
    /// </summary>
    public class ResultGravaInteracaoAtendimentoBOT
    {
        public bool success { get; set; }
        public string message { get; set; }

    }
}
