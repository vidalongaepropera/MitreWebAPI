using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{
    public class ResultInfoProposta
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Int32 contrato_id { get; set; }
        public Int32 proposta_id { get; set; }
    }
}
