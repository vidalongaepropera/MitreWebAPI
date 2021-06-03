using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class InfoSacProtocoloItemAnexos
    {
        public Int32 anexo_id { get; set; }
        public string nome_arquivo { get; set; }
        public string titulo { get; set; }
        public string descricao { get; set; }
        public string documento_app_type { get; set; }
    }
}
