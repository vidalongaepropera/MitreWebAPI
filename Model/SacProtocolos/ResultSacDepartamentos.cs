using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class ResultSacDepartamentos
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoSacDepartamentos> result{ get; set; }
    }
}
