using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class InfoSacProtocoloItemAnexosSalvar
    {
        public Int32 p_protocolo_id { get; set; }
        public Int32 p_protocolo_item_id { get; set; }
        public string p_titulo { get; set; }
        public string p_descricao { get; set; }
        public IFormFile p_arquivo_bin { get; set; }
    }
}
