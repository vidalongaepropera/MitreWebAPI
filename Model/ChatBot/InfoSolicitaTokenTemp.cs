using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Contém informações de geração de código temporário para acesso aos dados do cliente.
    /// Quando o cliente esquece a senha, é possível dar a ele um código de acesso temporário.
    /// </summary>
    public class InfoSolicitaTokenTemp
    {
        public string token { get; set; }
        public string expiracao { get; set; }

    }
}
