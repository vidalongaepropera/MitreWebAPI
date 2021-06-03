using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Model.ChatBot;
using Microsoft.AspNetCore.Hosting;
using MitreWebAPI.Util;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using MitreWebAPI.Controllers.Util;
using MitreWebAPI.Controllers.Recebimentos;
using MitreWebAPI.Model.Recebimento;
using MitreWebAPI.Model.Share;
using MitreWebAPI.Controllers.Sahre;
using Newtonsoft.Json;

namespace MitreWebAPI.Controllers
{
    /// <summary>
    /// Controla as parcelas para antecipação
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ShareController : ControllerBase
    {

        /// <summary>
        /// Classe que mantem na sessão as informações do client conectado.
        /// </summary>
        public InfoClienteConectado _infoClienteConectado;

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Metodo princiap de inicialização docontrole de antecipação
        /// </summary>
        /// <param name="Configuration"></param>
        /// <param name="infoClienteConectado"></param>
        /// <param name="env"></param>
        public ShareController(IConfiguration Configuration, InfoClienteConectado infoClienteConectado, IWebHostEnvironment env)
        {
            _configuration = Configuration;
            _environment = env;
            _infoClienteConectado = infoClienteConectado;
        }

        /// <summary>
        /// Lista dados da unidades do emprendimento Share
        /// </summary>
        /// <param name="filial_id"></param>
        /// <param name="empreendiento_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com informações do bloco e unidades</returns>
        [HttpGet("lista/unidades/{filial_id},{empreendiento_id},{p_token_app}")]
        [HttpPost("lista/unidades/{filial_id},{empreendiento_id},{p_token_app}")]
        public ResultInfoUnidades ListaUnidades(Int32 filial_id, Int32 empreendiento_id, string p_token_app)
        {

            var viewer = new ShareView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = viewer.ListaUnidades(filial_id, empreendiento_id);


            return result;

        }


        /// <summary>
        /// Lista taxas do ERP
        /// </summary>
        /// <param name="filial_id"></param>
        /// <param name="empreendiento_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Dados dos tipos de taxas</returns>
        public ResultInfoUnidades ListaTaxas(Int32 filial_id, Int32 empreendiento_id, string p_token_app)
        {

            var viewer = new ShareView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = viewer.ListaUnidades(filial_id, empreendiento_id);


            return result;

        }

        /// <summary>
        /// Gera o contrato a partir da proposta origem
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <param name="proposta"></param>
        /// <returns></returns>
        [HttpGet("gerar/contrato/erp/{p_token_app}")]
        [HttpPost("gerar/contrato/erp/{p_token_app}")]
        public ResultInfoProposta GerarContrato(string p_token_app, [FromBody] InfoProposta proposta) 
        {

            //System.IO.File.WriteAllText("C:\\Temp\\Proposta.txt", proposta.data_assinatura);

            var viewer = new ShareView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = new ResultInfoProposta();

            result.success = true;
            result.message = "OK";

            var info = new InfoProposta();

            info.contrato_ori_id = 1010;
            info.data_assinatura = "20/05/2021";
            info.pct_juros = 2;
            info.pct_multa = 10;

            var cli_f = new InfoCliente();

            cli_f.unidade_id = 16911;
            cli_f.nome = "JOSE MARIA DA SILVA";
            cli_f.nome_fantasia = "JOSE MARIA DA SAILVA";
            cli_f.principal = "S";
            cli_f.pct_participacao = 100;
            cli_f.tipo = "F";
            cli_f.tipo_pessoa = "F";
            cli_f.cpf_cnpj = "117.743.554-02";
            cli_f.data_nascimento = "30/05/1977";
            cli_f.estado_civil = "C";
            cli_f.email = "teste@gmail.com";

            var cjg_f = new InfoClienteConjuge();

            cjg_f.nome = "MARIA APARECIDA DA SILVA";
            cjg_f.cpf = "222.888.484-10";
            cjg_f.email = "teste1@gmail.com";
            cjg_f.data_nascimento = "22/08/1979";
            
            cli_f.conjuge = cjg_f;

            var cli_r1 = new InfoCliente();

            cli_r1.unidade_id = 16911;
            cli_r1.nome = "PEDRO DA SILVA";
            cli_r1.nome_fantasia = "PEDRO DA SAILVA";
            cli_r1.principal = "N";
            cli_r1.pct_participacao = 0;
            cli_r1.tipo = "R";
            cli_r1.tipo_pessoa = "F";
            cli_r1.cpf_cnpj = "145.733.514-04";
            cli_r1.data_nascimento = "19/03/2009";
            cli_r1.estado_civil = "S";
            cli_r1.email = "residente1@gmail.com";

            var cli_r2 = new InfoCliente();

            cli_r2.unidade_id = 16912;
            cli_r2.nome = "ANA DA SILVA";
            cli_r2.nome_fantasia = "ANA DA SAILVA";
            cli_r2.principal = "N";
            cli_r2.pct_participacao = 0;
            cli_r2.tipo = "R";
            cli_r2.tipo_pessoa = "F";
            cli_r2.cpf_cnpj = "158.965.215-07";
            cli_r2.data_nascimento = "23/09/2011";
            cli_r2.estado_civil = "S";
            cli_r2.email = "residente2@gmail.com";

            var telc = new InfoClienteTelefone();
            telc.tipo = "CEL";
            telc.numero = "(11) 98977-4352";

            var telr = new InfoClienteTelefone();
            telr.tipo = "RES";
            telr.numero = "(11) 2241-73845";

            var tels = new List<InfoClienteTelefone>();
            tels.Add(telc);
            tels.Add(telr);

            cli_f.telefones = tels;

            var end = new InfoClienteEndereco();

            end.tipo = "RES";
            end.cep = "03540-200";
            end.numero = "222";
            end.complemento = "Casa 1";

            var ends = new List<InfoClienteEndereco>();
            ends.Add(end);

            cli_f.enderecos = ends;

            var clis = new List<InfoCliente>();
            clis.Add(cli_f);
            clis.Add(cli_r1);
            clis.Add(cli_r2);

            var par1 = new InfoPropostaPagto();

            par1.quantidade = 1;
            par1.tipo = "S";
            par1.data_vencimento = "30/05/2021";
            par1.valor = 4500.50;

            var par2 = new InfoPropostaPagto();

            par2.quantidade = 11;
            par2.tipo = "M";
            par2.data_vencimento = "30/06/2021";
            par2.valor = 1500.42;

            var pgtos = new List<InfoPropostaPagto>();
            pgtos.Add(par1);
            pgtos.Add(par2);

            var taxa1 = new InfoTaxasAdicionais();
            var taxa2 = new InfoTaxasAdicionais();

            taxa1.taxa_id = 32;
            taxa1.quantidade = 11;
            taxa1.data_vencimento = "30/05/2021";
            taxa1.valor = 200.50;

            taxa1.taxa_id = 44;
            taxa1.quantidade = 11;
            taxa1.data_vencimento = "30/06/2021";
            taxa1.valor = 139.19;

            var taxas = new List<InfoTaxasAdicionais>();
            taxas.Add(taxa1);
            taxas.Add(taxa2);

            info.cliente = clis;
            info.forma_pagamento = pgtos;
            info.taxas_adicionais = taxas;

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(info, Formatting.Indented);

            System.IO.File.WriteAllText("C:\\Temp\\Proposta.json", jsonString);

            result.contrato_id = 12345;
            result.proposta_id = 87654;

            return result;

        }


    }
}
