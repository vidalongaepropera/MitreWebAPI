using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Devolte a informação de envio de código de acesso temporario para o cliente.
    /// </summary>
    public class ResultSolicitaTokenTemp
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string tipo { get; set; }
        public string token { get; set; }
        public string expiracao { get; set; }
        public string cliente { get; set; }
        public string email { get; set; }
        public string telefone_cel { get; set; }
        public string erros { get; set; }

    }
}
