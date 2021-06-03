using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.ChatBot
{
    public class InfoProdutoEmpreendimentos
    {
        public Int32 empreendimento_id { get; set; }
        public string empreendimento_nome { get; set; }
        public Int32 regiao_id { get; set; }
        public string regiao { get; set; }
        public string descricao { get; set; }
        public string situacao { get; set; }
        public string site { get; set; }
        public string ebook { get; set; }



    }
}
