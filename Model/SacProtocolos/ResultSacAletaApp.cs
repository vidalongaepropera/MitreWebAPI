using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class ResultSacAletaApp
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string statusIntegracao { get; set; }
        public InfoSacAlertaApp alerta { get; set; }
    }
}
