using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    public class ResultProdutoEmpreendimentos
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoProdutoEmpreendimentos> results { get; set; }
    }
}
