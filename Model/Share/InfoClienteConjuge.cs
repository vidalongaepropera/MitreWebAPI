using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{
    /// <summary>
    /// Dados do Conjuge do Cliente
    /// </summary>
    public class InfoClienteConjuge
    {
        //[MaxLength(100)]
        public string nome { get; set; }
        //[MaxLength(20)]
        public string cpf { get; set; }
        //[MaxLength(10)]
        public string data_nascimento { get; set; }
        //[MaxLength(100)]
        public string email { get; set; }

    }
}
