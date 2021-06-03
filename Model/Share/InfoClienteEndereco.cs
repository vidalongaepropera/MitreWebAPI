using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{
    /// <summary>
    /// Informações de endereçamento do cliente
    /// tipo: RES = Residencial, COM = Comercial, COB = Cobrança, FAT = Faturamento
    /// </summary>
    public class InfoClienteEndereco
    {
        //[MaxLength(3)]
        public string tipo { get; set; }
        //[MaxLength(10)]
        public string cep { get; set; }
        //[MaxLength(10)]
        public string numero { get; set; }
        //[MaxLength(50)]
        public string complemento { get; set; }
    }
}
