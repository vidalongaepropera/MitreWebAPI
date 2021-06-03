using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class InfoSacProtocoloIntemSalvar
    {
        public Int32 p_agente_id { get; set; }
        public Int32 p_protocolo_id { get; set; }
        public Int32 p_meio_comunicacao_id { get; set; }
        public Int32 p_departamento_id { get; set; }
        public Int32 p_assunto_id { get; set; }
        public string p_tratativa { get; set; }

    }
}
