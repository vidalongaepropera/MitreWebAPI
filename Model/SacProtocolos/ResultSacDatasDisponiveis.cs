using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class ResultSacDatasDisponiveis
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoSacDatasDisponiveis> result { get; set; }
    }
}
