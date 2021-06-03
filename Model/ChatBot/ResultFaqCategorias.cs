using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Devolve a lista de categorias do Faq.
    /// </summary>
    public class ResultFaqCategorias
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoFaqCategoria> results { get; set; }
    }
}
