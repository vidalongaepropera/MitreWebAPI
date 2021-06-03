using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Devolve a onformação de se o cliente existe com o CPF ou CNPJ solicitante.
    /// </summary>
    public class ResultValidaCliente
    {
        public bool success { get; set; }
        public string message { get; set; }

        public bool cliente_existe { get; set; }
    }
}
