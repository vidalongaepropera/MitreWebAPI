using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Devolve a lista de perguntas e respostas do Faq de acordo com a categoria escolhida.
    /// </summary>
    public class ResultFaqPerguntasRespostas
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string categoria { get; set; }
        public List<InfoFaqPerguntasRespostas> results { get; set; }
    }
}
