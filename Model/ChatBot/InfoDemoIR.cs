using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    /// <summary>
    /// Contém informações do demonstrativo de IR do cliente
    /// </summary>
    public class InfoDemoIR
    {
        public Int32 numero_organizacao { get; set; }
        public Int32 numero_filial { get; set; }
        public Int32 numero_contrato { get; set; }
        public string empreendimento { get; set; }
        public string bloco { get; set; }
        public string unidade { get; set; }
        public Int32 ano_do_informe { get; set; }
    }
}
