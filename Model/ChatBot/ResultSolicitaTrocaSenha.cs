using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Solicitação de resete de senha do cliente
    /// </summary>
    public class ResultSolicitaTrocaSenha
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string expiracao { get; set; }
        public Int32 codigo { get; set; }
        public string erros { get; set; }

    }
}
