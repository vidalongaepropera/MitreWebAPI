using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Dados de Resete de Senaha do Cliente
    /// </summary>
    public class ResultTrocarSenha
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string erros { get; set; }

    }
}
