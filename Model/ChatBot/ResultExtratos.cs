using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Devolve a lista de extratos disnoíveis do cliente.
    /// </summary>
    public class ResultExtratos
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoExtratos> results { get; set; }

    }
}
