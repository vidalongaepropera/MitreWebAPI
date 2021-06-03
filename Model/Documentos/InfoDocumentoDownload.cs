using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Documentos
{
    public class InfoDocumentoDownload
    {
        public Int32 documento_id { get; set; }
        public string documento_nome { get; set; }
        public string documento_titulo { get; set; }
        public string documento_desc { get; set; }
        public string documento_app_type { get; set; }
        public Double documento_size { get; set; }
        public string data_upload { get; set; }
    }
}
