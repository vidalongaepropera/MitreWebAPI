using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class InfoSacProtocoloIntemCriarNovo
    {
        public Int32 p_contrato_id { get; set; }
        public Int32 p_agente_id { get; set; }
        public Int32 p_meio_comunicacao_id { get; set; } = 86;
        public Int32 p_departamento_id { get; set; }
        public Int32 p_assunto_id { get; set; }
        public string p_tratativa { get; set; }
        public string p_data_agenda_ini { get; set; }
        public string p_data_agenda_fim { get; set; }

    }
}
