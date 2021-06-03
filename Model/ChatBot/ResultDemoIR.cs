using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Devolve uma lista de demosntartivos de IR informado pelo CPF ou CNPJ do cliente.
    /// </summary>
    public class ResultDemoIR
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoDemoIR> results { get; set; }

    }
}
