using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Documentos
{
    public class ResultInfoDocumentoDownload
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoDocumentoDownload> documentos { get; set; }

    }
}
