using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class InfoSacProtocolos
    {
        public Int32 contrato_id { get; set; }
        public Int32 protocolo_id { get; set; }
        public Int32 meio_comunicacao_id { get; set; }
        public Int32 departamento_id { get; set; }
        public Int32 assunto_id { get; set; }
        public string protocolo { get; set; }
        public string data_inclusao { get; set; }
        public string departamento { get; set; }
        public string assunto { get; set; }
        public string status { get; set; }
    }
}
