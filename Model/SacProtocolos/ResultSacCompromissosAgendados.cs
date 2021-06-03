using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class ResultSacCompromissosAgendados
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Int32 cliente_id { get; set; }
        public Int32 empreendimento_id { get; set; }
        public Int32 unidade_id { get; set; }
        public List<InfoSacCompromissosAgendado> result { get; set; }
    }
}
