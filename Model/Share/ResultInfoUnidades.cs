using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{
    public class ResultInfoUnidades
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Int32 filial_id { get; set; }
        public Int32 empreendiento_id { get; set; }
        public List<InfoUnidades> unidades { get; set; }
    }
}
