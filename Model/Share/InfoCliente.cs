using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Model.Share
{
    /// <summary>
    /// Dados do Cliente
    /// tipo: F = Responsável financeiro e R = Residente
    /// principal: S = Sim e N = Não
    /// pct_participacao: de 0 a 100, para caso de mais de um responsável financeiro, para os residentes usar sempre 0
    /// tipo_pessoa: F = Física e J = Jurídica
    /// estado_civil: S = Solteiro(a), C = Casado(a), V = Viúvo(a), D = Divorciado(a), A = União Estável e J = Separado(a) Judicialmente
    /// regime_casamento: P = Comunhão Parcial de Bens, U = Comunhão Universal de Bens, S = Separação Total de Bens, D = Regime Dotal e A = Participação Final nos Aquestos    
    /// </summary>
    public class InfoCliente
    {
        //[StringLength(999999999)]
        public Int32 unidade_id { get; set; }
        //[MaxLength(1)]
        public string tipo { get; set; }
        //[MaxLength(1)]
        public string principal { get; set; }
        //[MaxLength(100)]
        public double pct_participacao { get; set; }
        //[MaxLength(100)]
        public string nome { get; set; }
        //[MaxLength(100)]
        public string nome_fantasia { get; set; }
        //[MaxLength(1)]
        public string tipo_pessoa { get; set; }
        //[MaxLength(20)]
        public string cpf_cnpj { get; set; }
        //[MaxLength(20)]
        public string rne { get; set; }
        //[MaxLength(10)]
        public string data_nascimento { get; set; }
        //[MaxLength(1)]
        public string estado_civil { get; set; }
        //[MaxLength(1)]
        public string regime_casamento { get; set; }
        //[MaxLength(100)]
        public string email { get; set; }
        public InfoClienteConjuge conjuge { get; set; }
        public List<InfoClienteTelefone> telefones { get; set; }
        public List<InfoClienteEndereco> enderecos { get; set; }

    }
}
