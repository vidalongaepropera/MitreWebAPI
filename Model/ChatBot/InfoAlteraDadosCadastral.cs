using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Dados para informações de alteração de dados cadastrais do cliente
    /// </summary>
    public class InfoAlteraDadosCadastral
    {
        public Int32 cliente_id { get; set; }
        public string cpf_cnpj { get; set; }
        public string email { get; set; }
        public string tel_celular { get; set; }
        public string tel_residencial { get; set; } = "";
        public string tel_comercial { get; set; } = "";
        public bool aceita_receber_email { get; set; } = true;
        public bool aceita_receber_sms { get; set; } = true;

    }
}
