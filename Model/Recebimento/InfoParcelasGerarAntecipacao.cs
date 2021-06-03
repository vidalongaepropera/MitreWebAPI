using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    public class InfoParcelasGerarAntecipacao
    {
        public string id { get; set; }
        public string valor_futuro { get; set; }
        public string valor_presente { get; set; }

    }
}
