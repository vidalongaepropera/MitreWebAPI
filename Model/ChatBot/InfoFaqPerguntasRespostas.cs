using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Contem informações de Perguntas e Respostas de uma determinada categoria do Faq
    /// </summary>
    public class InfoFaqPerguntasRespostas
    {
        public string pergunta { get; set; }
        public string resposta { get; set; }
    }
}
