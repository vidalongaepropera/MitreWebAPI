using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Retorna as informações de contratos disponíveis para o cliente por meio do CPF ou CNPJ informado.
    /// tipo: 
    ///     UI - Usuário ou senha inválida
    ///     DI - Database ou serviço inválido
    ///     PI - Senha inválida
    /// </summary>
    public class ResultContratos
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string tipo { get; set; }
        public Int32 cliente_id { get; set; }
        public string nome_cliente { get; set; }
        public string email { get; set; }
        public string tel_celular { get; set; }
        public string tel_residencial { get; set; }
        public string tel_comercial { get; set; }
        public bool aceita_receber_sms { get; set; }
        public bool aceita_receber_email { get; set; }
        public List<InfoContratos> results { get; set; }

    }
}
