using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{
    /// <summary>
    /// Tipo do telefone: COM = Comercial, CEL = Celular, RES = Residencial.
    /// </summary>
    public class InfoClienteTelefone
    {

        //[MaxLength(3)]
        public string tipo { get; set; }
        //[MaxLength(16)]
        public string numero { get; set; }

    }
}
