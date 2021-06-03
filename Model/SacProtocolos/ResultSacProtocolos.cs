using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class ResultSacProtocolos
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Int32 cliente_id { get; set; }
        public List<InfoSacProtocolos> result { get; set; }
    }
}
