using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class InfoSacProtocolosItem
    {
        public Int32 protocolo_item_id { get; set; }
        public string data_inclusao { get; set; }
        public string atendente_nome { get; set; }
        public string tratativa { get; set; }
        public string status { get; set; }
        public List<InfoSacProtocoloItemAnexos> anexos { get; set; }

    }
}
