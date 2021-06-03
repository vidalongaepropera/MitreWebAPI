using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class InfoSacCompromissosAgendado
    {
        public Int32 compromisso_id { get; set; }
        public Int32 protocolo_id { get; set; }
        public string status { get; set; }
        public string status_descricao { get; set; }
        public string data_ini { get; set; }
        public string data_fim { get; set; }
    }
}
