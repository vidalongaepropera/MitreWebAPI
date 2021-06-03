using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Contém informações do Cliente Conectado ao Serviço, como url, browser, ip e cpf ou cnpj.
    /// </summary>
    public class InfoClienteConectado
    {
        /// <summary>
        /// Contém o cpf ou cnpj do cliente
        /// </summary>
        public string cpf_cnpj { get; set; }
        /// <summary>
        /// Contém as informação do bowser client da aplicação
        /// </summary>
        public string browser { get; set; }
        /// <summary>
        /// Contem informação do IP do client da aplicação
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// Contém a URL do client da aplicação
        /// </summary>
        public string url_path { get; set; }
        /// <summary>
        /// Contém o Token da aplicação
        /// </summary>
        public string token { get; set; }

    }
}
