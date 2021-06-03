using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class InfoSacAlertaApp
    {
        public string cpf_cnpj { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string protocolId { get; set; }
        public string contractId { get; set; }
    }
}
