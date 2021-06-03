using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Documentos
{
    public class InfoDocumentoUpload
    {
        public string p_contrato_id { get; set; }
        public string p_cpf_cnpj { get; set; }
        public string p_titulo_arquivo { get; set; }
        public string p_desc_arquivo { get; set; }
        public IFormFile p_arquivo_bin { get; set; }
    }
}
