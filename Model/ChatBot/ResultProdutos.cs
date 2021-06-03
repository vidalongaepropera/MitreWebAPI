using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Lista de todos os produtos disponíveis
    /// </summary>
    public class ResultProdutos
    {

        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoProduto> results { get; set; }

    }
}
