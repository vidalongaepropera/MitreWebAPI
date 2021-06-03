using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Informamações basica do cadastro do clienta
    /// </summary>
    public class ResultConsultaDadosCadastral
    {
        public bool success { get; set; }
        public string message { get; set; }
        public InfoAlteraDadosCadastral results { get; set; }
    }
}
