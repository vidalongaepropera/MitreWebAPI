using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class ResultSacProtocolosItemCriarNovo
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Int32 protocolo_id { get; set; }
        public Int32 protocolo_item_id { get; set; }
        public Int32 agendamento_id { get; set; }
    }
}
