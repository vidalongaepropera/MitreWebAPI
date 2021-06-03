using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.SacProtocolos
{
    public class ResultSacProtocolosItem
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<InfoSacProtocolosItem> result { get; set; }

    }
}
