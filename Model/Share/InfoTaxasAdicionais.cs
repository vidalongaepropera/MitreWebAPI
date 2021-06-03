using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{
    public class InfoTaxasAdicionais
    {
        public Int32 taxa_id { get; set; }
        public Int32 quantidade { get; set; }
        //[MaxLength(1)]
        public double valor { get; set; }
        //[MaxLength(10)]
        public string data_vencimento { get; set; }
    }
}
