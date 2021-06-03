using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Recebimento
{
    public class ResultAtulizaDesconto
    {
        public bool success { get; set; }
        public string message { get; set; }
        public double valor_desconto { get; set; }
    }
}
