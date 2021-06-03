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

namespace MitreWebAPI.Controllers
{
    /// <summary>
    /// Controla as parcelas para antecipação
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RecebimentoController : ControllerBase
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
        public RecebimentoController(IConfiguration Configuration, InfoClienteConectado infoClienteConectado, IWebHostEnvironment env)
        {
            _configuration = Configuration;
            _environment = env;
            _infoClienteConectado = infoClienteConectado;
        }

        /// <summary>
        /// Lista as parcelas futuras com possibilidade de antecipar
        /// </summary>
        /// <param name="p_dt_vencimento"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com valores das parcelas</returns>
        [HttpGet("antecipacao/parcelas/{p_dt_vencimento},{p_cpf_cnpj},{p_contrato_id},{p_token_app}")]
        [HttpPost("antecipacao/parcelas/{p_dt_vencimento},{p_cpf_cnpj},{p_contrato_id},{p_token_app}")]
        public ResultAntecipacao ListaParcelasAntecipacao(string p_dt_vencimento, string p_cpf_cnpj, string p_contrato_id, string p_token_app)
        {

            _infoClienteConectado.token = p_token_app;
            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var view = new RecebimentoView(_configuration, _environment, p_token_app);

            ResultAntecipacao result = view.ListaParcelasAntecipacao(p_contrato_id, p_dt_vencimento);

            return result;

        }

        /// <summary>
        /// Atualiza valor de desconto sobre a antecipação
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <param name="resultAtuDesc"></param>
        /// <returns>O valor total do desconto</returns>
        [HttpGet("antecipacao/atualiza/desconto/{p_token_app}")]
        [HttpPost("antecipacao/atualiza/desconto/{p_token_app}")]
        public ResultAtulizaDesconto AtualizaDescontoAntecipacao(string p_token_app, [FromBody] InfoAtulizaDesconto resultAtuDesc)
        {

            double valorTotal = 0;

            _infoClienteConectado.token = p_token_app;

            var view = new RecebimentoView(_configuration, _environment, p_token_app);

            var result = new ResultAtulizaDesconto();

            try
            {

                _infoClienteConectado.cpf_cnpj = resultAtuDesc.cpf_cnpj;
                _infoClienteConectado.token = p_token_app;

                foreach (InfoParcelasGerarAntecipacao par in resultAtuDesc.Parcelas)
                {
                    valorTotal += float.Parse(par.valor_presente);
                }

                result.success = true;
                result.message = "OK";
                result.valor_desconto = 0; //Math.Round((valorTotal * 3 / 100), 2);

            }
            catch (Exception e)
            {

                result.success = false;
                result.message = e.Message + "\n" + e.StackTrace;
                result.valor_desconto = 0;

            }

            return result;

        }

        /// <summary>
        /// Gera a antecipação de uma ou mais parcelas
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <param name="resultAnt"></param>
        /// <returns></returns>
        [HttpGet("gerar/antecipacao/{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("gerar/antecipacao/{p_cpf_cnpj},{p_token_app}")]
        public ResultAntecipacaoGerar GerarAntecipacao(string p_cpf_cnpj, string p_token_app, [FromBody] InfoAntecipacaoGerar resultAnt)
        {

            _infoClienteConectado.token = p_token_app;

            var view = new RecebimentoView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = view.GerarAntecipacao(resultAnt); ;

            return fileContentResult;

        }

        /// <summary>
        /// Lista informações com detalhes e consolidado de valores pagos e valor quitação na data base;
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_contrato"></param>
        /// <param name="p_token_app"></param>
        /// <returns></returns>
        [HttpGet("lista/valores/pagoequitacao/{p_cpf_cnpj},{p_contrato},{p_token_app}")]
        [HttpPost("lista/valores/pagoequitacao/{p_cpf_cnpj},{p_contrato},{p_token_app}")]
        public ResultContratoPagoQuitacao ContratoDetalheValorPagoQuitacao(string p_cpf_cnpj, Int32 p_contrato, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var view = new RecebimentoView(_configuration, _environment, p_token_app);

            var result = view.ListaInfomacoesPagamentos(p_contrato);

            return result;

        }


    }
}
