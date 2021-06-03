using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Devolve dados do Token da aplicação, se é valido ou não.
    /// </summary>
        public class ResultValidaToken
    {
        public bool success { get; set; }
        public string message { get; set; }
    }
}
