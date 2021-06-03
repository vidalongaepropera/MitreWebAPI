using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{
    public class InfoUnidades
    {
        public Int32 bloco_id { get; set; }
        public Int32 andar { get; set; }
        public Int32 unidade_id { get; set; }
        public string unidade_codigo { get; set; }
        public string unidade { get; set; }
        public string unidade_status { get; set; }
        public Int32 tipologia_id { get; set; }
        public string tipologia { get; set; }

    }
}
