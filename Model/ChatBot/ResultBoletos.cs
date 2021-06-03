using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Retorna uma lista de informações de boleto em aberto e disponível para o cliente.
    /// /// </summary>
    public class ResultBoletos
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoBoletos> results { get; set; }

    }
}
