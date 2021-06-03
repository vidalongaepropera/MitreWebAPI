using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Model.ChatBot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MitreWebAPI.Controllers.ChatBot
{
    /// <summary>
    /// Contém controles principais da API para uso do Chat Bot 
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [Route("api/")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {

        /// <summary>
        /// Clase que mantem na sessão as informações do client conectado.
        /// </summary>
        public InfoClienteConectado _infoClienteConectado;

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Metodo princiap de inicialização do Chat Bot
        /// </summary>
        /// <param name="Configuration"></param>
        /// <param name="infoClienteConectado"></param>
        /// <param name="env"></param>
        public ChatBotController(IConfiguration Configuration, InfoClienteConectado infoClienteConectado, IWebHostEnvironment env)
        {
            _configuration = Configuration;
            _environment = env;
            _infoClienteConectado = infoClienteConectado;

        }

        /// <summary>
        /// Valida o cliente e cliente passando o CPF ou CNPJ e o Token da aplicação.
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Retorna se é client ou não</returns>
        [HttpGet("login/valida/cliente/{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("login/valida/cliente/{p_cpf_cnpj},{p_token_app}")]
        public ResultValidaCliente ValidaClienteExistente(string p_cpf_cnpj, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);
            chatBotView.ACESSO_COM_SENHA_TMP = true;

            ResultValidaCliente result = chatBotView.ValidaClienteExistente(p_cpf_cnpj);

            return result;

        }

        /// <summary>
        /// Faz o Login do cliente e lista as unidades
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_senha"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista da unidades do cliente logado</returns>
        [AllowAnonymous]
        [HttpGet("login/lista/unidades/{p_cpf_cnpj},{p_senha},{p_token_app}")]
        [HttpPost("login/lista/unidades/{p_cpf_cnpj},{p_senha},{p_token_app}")]
        public ResultContratos LoginAndListaUnidade(string p_cpf_cnpj, string p_senha, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);
            chatBotView.ACESSO_COM_SENHA_TMP = true;

            ResultContratos contratos = chatBotView.ContratosCliente(p_cpf_cnpj, p_senha, "N");

            return contratos;

        }

        /// <summary>
        /// Lista as tipologias utiiadas para cadastr unidades
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com ID e nome da tipologia</returns>
        [AllowAnonymous]
        [HttpGet("lista/tipologias/unidades/{p_token_app}")]
        [HttpPost("lista/tipologias/unidades/{p_token_app}")]
        public ResultInfoTipologiaUnidade ListaTipologiaUnidades(string p_token_app)
        {

            _infoClienteConectado.token = p_token_app;

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);
            chatBotView.ACESSO_COM_SENHA_TMP = true;

            var result = chatBotView.ListaTipologiaUnidades();

            return result;

        }

        /// <summary>
        /// Efetua o acesso com um código (senha) temporária que o cliente solicita
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_senha"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista da unidades do cliente logado</returns>
        [AllowAnonymous]
        [HttpGet("login/comsenhatmp/lista/unidades/{p_cpf_cnpj},{p_senha},{p_token_app}")]
        [HttpPost("login/comsenhatmp/lista/unidades/{p_cpf_cnpj},{p_senha},{p_token_app}")]
        public ResultContratos LoginAndListaUnidadeTmp(string p_cpf_cnpj, string p_senha, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);
            chatBotView.ACESSO_COM_SENHA_TMP = true;

            ResultContratos contratos = chatBotView.ContratosCliente(p_cpf_cnpj, p_senha, "S");

            return contratos;
        }

        /// <summary>
        /// Cria um código temporário, válido por uma hora, par ao acesso do cliente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <param name="p_reenvia"></param>
        /// <returns>Informações de envio se código temporário npara o e-mail e sms para o e-mail e telefone cadastrado</returns>
        [AllowAnonymous]
        [HttpGet("gerar/token/{p_cpf_cnpj},{p_token_app},{p_reenvia}")]
        [HttpPost("gerar/token/{p_cpf_cnpj},{p_token_app},{p_reenvia}")]
        public ResultSolicitaTokenTemp GeraTokenTemporario(string p_cpf_cnpj, string p_token_app, string p_reenvia)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var result = new ChatBotView(_configuration, _environment, p_token_app).GeraTokenTemporario(p_cpf_cnpj, p_reenvia);

            return result;
        }

        /// <summary>
        /// Solicitação de troca de senha do cliente por esquecimento
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_reenvia"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Informações de pedido de troca de senha do cliente</returns>
        [AllowAnonymous]
        [HttpGet("solicita/troca/senha/{p_cpf_cnpj},{p_reenvia},{p_token_app}")]
        [HttpPost("solicita/troca/senha/{p_cpf_cnpj},{p_reenvia},{p_token_app}")]
        public ResultSolicitaTrocaSenha SolicitaTrocaSenha(string p_cpf_cnpj, string p_reenvia, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var result = new ChatBotView(_configuration, _environment, p_token_app).SolicitaTrocaSenha(p_cpf_cnpj, p_reenvia);

            return result;
        }

        /// <summary>
        /// Efetua a troca de senha do cliente com token
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_codigo_verif"></param>
        /// <param name="p_nova_senha"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Restutado a solicitação da troca de senha</returns>
        [AllowAnonymous]
        [HttpGet("efetua/alteracao/senha/{p_cpf_cnpj},{p_codigo_verif},{p_nova_senha},{p_token_app}")]
        [HttpPost("efetua/alteracao/senha/{p_cpf_cnpj},{p_codigo_verif},{p_nova_senha},{p_token_app}")]
        public ResultTrocarSenha TrocarSenha(string p_cpf_cnpj, Int32 p_codigo_verif, string p_nova_senha, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var result = new ChatBotView(_configuration, _environment, p_token_app).TrocarSenha(p_cpf_cnpj, p_codigo_verif, p_nova_senha, "");

            return result;
        }

        /// <summary>
        /// Efetua a troca de senha do cliente com senha anterior
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_atual_senha"></param>
        /// <param name="p_nova_senha"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Restutado a solicitação da troca de senha</returns>
        [AllowAnonymous]
        [HttpGet("efetua/alteracao/senha/anterior/{p_cpf_cnpj},{p_atual_senha},{p_nova_senha},{p_token_app}")]
        [HttpPost("efetua/alteracao/senha/anterior/{p_cpf_cnpj},{p_atual_senha},{p_nova_senha},{p_token_app}")]
        public ResultTrocarSenha TrocarSenhaAtual(string p_cpf_cnpj, string p_atual_senha, string p_nova_senha, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var result = new ChatBotView(_configuration, _environment, p_token_app).TrocarSenha(p_cpf_cnpj, 0, p_nova_senha, p_atual_senha);

            return result;
        }

        /// <summary>
        /// Efetua a alteração de dados cadastrais do cliente
        /// </summary>
        /// <param name="info"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Informações de sucesso ou erro na troca</returns>
        [AllowAnonymous]
        [HttpPost("cliente/alterar/dados/cadastrais/{p_token_app}")]
        public ResultAlteraDadosCadastral AlteradDadoCadastral(InfoAlteraDadosCadastral info, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = info.cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var result = new ChatBotView(_configuration, _environment, p_token_app).AlterarDadoCadastral(info);

            return result;
        }

        /// <summary>
        /// Informamações basica do cadastro do clienta
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Dados de cpf_cnpj, id, telefones e aceite de comunicação por sms e e-mail</returns>
        [AllowAnonymous]
        [HttpGet("cliente/consulta/dados/cadastrais/{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("cliente/consulta/dados/cadastrais/{p_cpf_cnpj},{p_token_app}")]
        public ResultConsultaDadosCadastral ConsultadDadoCadastral(string p_cpf_cnpj, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var result = new ChatBotView(_configuration, _environment, p_token_app).ConsultarDadoCadastral(p_cpf_cnpj);

            return result;
        }


        /// <summary>
        /// Lista todos os boletos diponíveis do cliente de um determinado contrato
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_numero_contrato"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com dados da unidade do contrato e seus respectivos boletos, quando disponíveis</returns>
        [AllowAnonymous]
        [HttpGet("lista/boletos/{p_cpf_cnpj},{p_numero_contrato},{p_token_app}")]
        [HttpPost("lista/boletos/{p_cpf_cnpj},{p_numero_contrato},{p_token_app}")]
        public ResultBoletos ListaBoletos(string p_cpf_cnpj, string p_numero_contrato, string p_token_app)
        {
            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            ResultBoletos boletos = new ChatBotView(_configuration, _environment, p_token_app).Boletos(p_numero_contrato);

            return boletos;

        }

        /// <summary>
        /// Lista todos os boletos de um determinado cliente
        /// Se o cliente tiver mais de uma unidade serão listados
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista dados das unidades com todos boletos disponíveis</returns>
        [AllowAnonymous]
        [HttpGet("lista/todosboletos/{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("lista/todosboletos/{p_cpf_cnpj},{p_token_app}")]
        public ResultBoletos ListaTodosBoletos(string p_cpf_cnpj, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            ResultBoletos boletos = new ChatBotView(_configuration, _environment, p_token_app).TodosBoletos(p_cpf_cnpj);

            return boletos;

        }

        /// <summary>
        /// Lista todos os extratos de um determinado cliente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com dados das unidades e extratos disponíveis</returns>
        [AllowAnonymous]
        [HttpGet("lista/extratos/{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("lista/extratos/{p_cpf_cnpj},{p_token_app}")]
        public ResultExtratos TodosExtratos(string p_cpf_cnpj, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            ResultExtratos extratos = new ChatBotView(_configuration, _environment, p_token_app).TodosExtratos(p_cpf_cnpj);

            return extratos;

        }

        /// <summary>
        /// Lista os informe de IR do cliente 
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista dados das unidades e IR disponíveis dos últimos 2 anos</returns>
        [AllowAnonymous]
        [HttpGet("lista/informes-ir/{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("lista/informes-ir/{p_cpf_cnpj},{p_token_app}")]
        public ResultDemoIR ListaTodosDemoIR(string p_cpf_cnpj, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            ResultDemoIR ir = new ChatBotView(_configuration, _environment, p_token_app).TodosDemoIR(p_cpf_cnpj);

            return ir;

        }

        /// <summary>
        /// Faz o download boleto bancário, baseando no que foi listado anteriormente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_numero_parcela"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Baixa o boleto bancário em PDF</returns>
        [AllowAnonymous]
        [HttpGet("download/boleto/{p_cpf_cnpj},{p_numero_parcela},{p_token_app}")]
        [HttpPost("download/boleto/{p_cpf_cnpj},{p_numero_parcela},{p_token_app}")]
        public FileContentResult DownloadBoleto(string p_cpf_cnpj, string p_numero_parcela, string p_token_app)
        {

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = chatBotView.GetBoleto(p_numero_parcela);

            return fileContentResult;
        }

        /// <summary>
        /// Baixa o boleto em PDF via JSON
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_numero_parcela"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Dados em JSON para conversão do documento</returns>
        [AllowAnonymous]
        [HttpGet("download/boleto/json/{p_cpf_cnpj},{p_numero_parcela},{p_token_app}")]
        [HttpPost("download/boleto/json/{p_cpf_cnpj},{p_numero_parcela},{p_token_app}")]
        public ResultDownloadInJson DownloadBoletoJson(string p_cpf_cnpj, string p_numero_parcela, string p_token_app)
        {

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = chatBotView.GetBoleto(p_numero_parcela);

            var result = new ResultDownloadInJson();
            var info = new InfoDownloadInJson();

            string strBase64 = System.Convert.ToBase64String(fileContentResult.FileContents);

            info.nome = fileContentResult.FileDownloadName;
            info.tipo = Util.Formatacoes.GetContentType(info.nome);
            info.conteudo = strBase64;

            result.success = true;
            result.message = "OK";
            result.arquivo = info;

            return result;
        }


        /// <summary>
        /// Baixa o extrato do cliente em PDF via JSON
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_numero_organizacao"></param>
        /// <param name="p_numero_filial"></param>
        /// <param name="p_numero_contrato"></param>
        /// <param name="p_data_base"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Baixa o PDF do extarto do contrato selecionado</returns>
        [AllowAnonymous]
        [HttpGet("download/extrato/{p_cpf_cnpj},{p_numero_organizacao},{p_numero_filial},{p_numero_contrato},{p_data_base},{p_token_app}")]
        [HttpPost("download/extrato/{p_cpf_cnpj},{p_numero_organizacao},{p_numero_filial},{p_numero_contrato},{p_data_base},{p_token_app}")]
        public FileContentResult DownloadExtrato(string p_cpf_cnpj, Int32 p_numero_organizacao, Int32 p_numero_filial, Int32 p_numero_contrato, string p_data_base, string p_token_app)
        {

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = chatBotView.GetExtrato(p_numero_organizacao, p_numero_filial, p_numero_contrato, p_data_base);

            return fileContentResult;

        }

        /// <summary>
        /// Baixa o extrato do cliente em PDF
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_numero_organizacao"></param>
        /// <param name="p_numero_filial"></param>
        /// <param name="p_numero_contrato"></param>
        /// <param name="p_data_base"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Baixa o PDF do extarto do contrato selecionado via JSON</returns>
        [AllowAnonymous]
        [HttpGet("download/extrato/json/{p_cpf_cnpj},{p_numero_organizacao},{p_numero_filial},{p_numero_contrato},{p_data_base},{p_token_app}")]
        [HttpPost("download/extrato/json/{p_cpf_cnpj},{p_numero_organizacao},{p_numero_filial},{p_numero_contrato},{p_data_base},{p_token_app}")]
        public ResultDownloadInJson DownloadExtratoJson(string p_cpf_cnpj, Int32 p_numero_organizacao, Int32 p_numero_filial, Int32 p_numero_contrato, string p_data_base, string p_token_app)
        {

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = chatBotView.GetExtrato(p_numero_organizacao, p_numero_filial, p_numero_contrato, p_data_base);

            var result = new ResultDownloadInJson();
            var info = new InfoDownloadInJson();

            string strBase64 = System.Convert.ToBase64String(fileContentResult.FileContents);

            info.nome = fileContentResult.FileDownloadName;
            info.tipo = Util.Formatacoes.GetContentType(info.nome);
            info.conteudo = strBase64;

            result.success = true;
            result.message = "OK";
            result.arquivo = info;

            return result;

        }


        /// <summary>
        /// Baixa o informe do IR do cliente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_numero_organizacao"></param>
        /// <param name="p_numero_filial"></param>
        /// <param name="p_numero_contrato"></param>
        /// <param name="p_ano_do_informe"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Baixa o PDF do informe de IR para o contrato e ano selecionado</returns>
        [AllowAnonymous]
        [HttpGet("download/informe-ir/{p_cpf_cnpj},{p_numero_organizacao},{p_numero_filial},{p_numero_contrato},{p_ano_do_informe},{p_token_app}")]
        [HttpPost("download/informe-ir/{p_cpf_cnpj},{p_numero_organizacao},{p_numero_filial},{p_numero_contrato},{p_ano_do_informe},{p_token_app}")]
        public FileContentResult GetInformeIR(string p_cpf_cnpj, Int32 p_numero_organizacao, Int32 p_numero_filial, Int32 p_numero_contrato, Int32 p_ano_do_informe, string p_token_app)
        {

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = chatBotView.GetInformeIR(p_numero_organizacao, p_numero_filial, p_numero_contrato, p_ano_do_informe);

            return fileContentResult;

        }

        /// <summary>
        /// Baixa o informe do IR do cliente em PDF via JSON
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_numero_organizacao"></param>
        /// <param name="p_numero_filial"></param>
        /// <param name="p_numero_contrato"></param>
        /// <param name="p_ano_do_informe"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Baixa o PDF do informe de IR para o contrato e ano selecionado</returns>
        [AllowAnonymous]
        [HttpGet("download/informe-ir/json/{p_cpf_cnpj},{p_numero_organizacao},{p_numero_filial},{p_numero_contrato},{p_ano_do_informe},{p_token_app}")]
        [HttpPost("download/informe-ir/json/{p_cpf_cnpj},{p_numero_organizacao},{p_numero_filial},{p_numero_contrato},{p_ano_do_informe},{p_token_app}")]
        public ResultDownloadInJson GetInformeIRJson(string p_cpf_cnpj, Int32 p_numero_organizacao, Int32 p_numero_filial, Int32 p_numero_contrato, Int32 p_ano_do_informe, string p_token_app)
        {

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = chatBotView.GetInformeIR(p_numero_organizacao, p_numero_filial, p_numero_contrato, p_ano_do_informe);

            var result = new ResultDownloadInJson();
            var info = new InfoDownloadInJson();

            string strBase64 = System.Convert.ToBase64String(fileContentResult.FileContents);

            info.nome = fileContentResult.FileDownloadName;
            info.tipo = Util.Formatacoes.GetContentType(info.nome);
            info.conteudo = strBase64;

            result.success = true;
            result.message = "OK";
            result.arquivo = info;

            return result;

        }

        /// <summary>
        /// Lista todos os produtos (empreedimentos).
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com ID e Nome doproduto</returns>
        [AllowAnonymous]
        [HttpGet("lista/produtos/{p_token_app}")]
        [HttpPost("lista/produtos/{p_token_app}")]
        public ResultProdutos ListaProdutos(string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = "";
            _infoClienteConectado.token = p_token_app;

            ResultProdutos boletos = new ChatBotView(_configuration, _environment, p_token_app).ListaProdutos(null);

            return boletos;

        }

        /// <summary>
        /// Lista o agente de atendimento SAC para o emprendimento selecionado
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_emp_codigo"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Retorna o código do agente qSAC que atende esse empreendimento. Esse código é o mesmo cadastrado em nosso sistema SAC</returns>
        [AllowAnonymous]
        [HttpGet("lista/atendente/empreendimento/{p_cpf_cnpj},{p_emp_codigo},{p_token_app}")]
        [HttpPost("lista/atendente/empreendimento/{p_cpf_cnpj},{p_emp_codigo},{p_token_app}")]
        public ResultAgenteEmpreendimento ListaAtendenteEmpreendimento(string p_cpf_cnpj, Int32 p_emp_codigo, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var chatBotView = new ChatBotView(_configuration, _environment, p_token_app);
            chatBotView.ACESSO_COM_SENHA_TMP = true;

            ResultAgenteEmpreendimento result = chatBotView.ListaAtendenteEmpreendimento(p_cpf_cnpj, p_emp_codigo);

            return result;

        }

        /// <summary>
        /// Lista as categorias do FAQ
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com o nomes das categorias</returns>
        [AllowAnonymous]
        [HttpGet("lista/faq/categorias/{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("lista/faq/categorias/{p_cpf_cnpj},{p_token_app}")]
        public ResultFaqCategorias ListaFaqCategorias(string p_cpf_cnpj, string p_token_app)
        {
            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            ResultFaqCategorias result = new ChatBotView(_configuration, _environment, p_token_app).ListaFaqCategorias();

            return result;

        }

        /// <summary>
        /// Lista todas as perguntas e respostas baseado na categoria solicitada
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_categoria"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com as perguntas e respostas</returns>
        [AllowAnonymous]
        [HttpGet("lista/faq/categorias/perguntas_respostas/{p_cpf_cnpj},{p_categoria},{p_token_app}")]
        [HttpPost("lista/faq/categorias/perguntas_respostas/{p_cpf_cnpj},{p_categoria},{p_token_app}")]
        public ResultFaqPerguntasRespostas ListaFaqPerguntasRespostas(string p_cpf_cnpj, string p_categoria, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;
            
            ResultFaqPerguntasRespostas result = new ChatBotView(_configuration, _environment, p_token_app).ListaFaqPerguntasRespostas(p_categoria);

            return result;

        }

        /// <summary>
        /// Lista os produtos da Mitre em determinada região
        /// </summary>
        /// <param name="p_regiao_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com os dados do produto</returns>
        [AllowAnonymous]
        [HttpGet("lista/produtos/mitre/{p_regiao_id},{p_token_app}")]
        [HttpPost("lista/produtos/mitre/{p_regiao_id},{p_token_app}")]
        public ResultProdutoEmpreendimentos ListaProdutosMitre(Int32 p_regiao_id, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = "";
            _infoClienteConectado.token = p_token_app;

            ResultProdutoEmpreendimentos result = new ChatBotView(_configuration, _environment, p_token_app).ListaProdutosMitre(p_regiao_id);

            return result;

        }

        /// <summary>
        /// Lista as regiões onde existem produtos da Mitre
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com as regiões</returns>
        [AllowAnonymous]
        [HttpGet("lista/regiao/produtos/mitre/{p_token_app}")]
        [HttpPost("lista/regiao/produtos/mitre/{p_token_app}")]
        public ResultRegiao ListaRegiaoProdutosMitre(string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = "";
            _infoClienteConectado.token = p_token_app;

            ResultRegiao result = new ChatBotView(_configuration, _environment, p_token_app).ListaRegiaoProdutosMitre();

            return result;

        }

        /// <summary>
        /// Grava os passos que o BOT fez a interação 
        /// </summary>
        /// <param name="p_idbot"></param> 
        /// <param name="p_plataforma"></param> 
        /// <param name="p_channel"></param> 
        /// <param name="p_username"></param> 
        /// <param name="p_decisaoinicial"></param> 
        /// <param name="p_tipodocumento"></param> 
        /// <param name="p_documento"></param> 
        /// <param name="p_opcaomenu"></param> 
        /// <param name="p_tipoduvida"></param> 
        /// <param name="p_email"></param> 
        /// <param name="p_objetivocompra"></param> 
        /// <param name="p_tipoapartamento"></param> 
        /// <param name="p_regiao"></param> 
        /// <param name="p_tipoempreendimento"></param> 
        /// <param name="p_baixarebook"></param> 
        /// <param name="p_proximopasso"></param> 
        /// <param name="p_token_app"></param>
        /// <returns>Status do Processamento</returns>
        [AllowAnonymous]
        [HttpGet("gravainteracao/{p_idbot},{p_plataforma},{p_channel},{p_username},{p_decisaoinicial},{p_tipodocumento},{p_documento},{p_opcaomenu},{p_tipoduvida},{p_email},{p_objetivocompra},{p_tipoapartamento},{p_regiao},{p_tipoempreendimento},{p_baixarebook},{p_proximopasso},{p_token_app}")]
        [HttpPost("gravainteracao/{p_idbot},{p_plataforma},{p_channel},{p_username},{p_decisaoinicial},{p_tipodocumento},{p_documento},{p_opcaomenu},{p_tipoduvida},{p_email},{p_objetivocompra},{p_tipoapartamento},{p_regiao},{p_tipoempreendimento},{p_baixarebook},{p_proximopasso},{p_token_app}")]
        public ResultGravaInteracaoBOT GravaInteracaoBOT(string p_idbot, string p_plataforma, string p_channel, string p_username, string p_decisaoinicial, string p_tipodocumento, string p_documento, string p_opcaomenu, string p_tipoduvida, string p_email, string p_objetivocompra, string p_tipoapartamento, string p_regiao, string p_tipoempreendimento, string p_baixarebook, string p_proximopasso, string p_token_app)
        {

            _infoClienteConectado.token = p_token_app;
            _infoClienteConectado.cpf_cnpj = "";

            ResultGravaInteracaoBOT result = new ChatBotView(_configuration, _environment, p_token_app).SetBotInteracaoApp(p_idbot, p_plataforma, p_channel, p_username, p_decisaoinicial, p_tipodocumento, p_documento, p_opcaomenu, p_tipoduvida, p_email, p_objetivocompra, p_tipoapartamento, p_regiao, p_tipoempreendimento, p_baixarebook, p_proximopasso);
            result.success = true;
            return result;

        }


        /// <summary> 
        /// Grava os passos que o BOT fez a interação pesquisa 
        /// </summary> 
        /// <param name="p_idbot"></param>  
        /// <param name="p_nota"></param>  
        /// <param name="p_motivo"></param>  
        /// <param name="p_token_app"></param> 
        /// <returns>Status do Processamento</returns> 
        [AllowAnonymous]
        [HttpGet("gravainteracaopesquisa/{p_idbot},{p_nota},{p_motivo},{p_token_app}")]
        [HttpPost("gravainteracaopesquisa/{p_idbot},{p_nota},{p_motivo},{p_token_app}")]
        public ResultGravaInteracaoPesquisaBOT GravaInteracaoPesquisaBOT(string p_idbot, string p_nota, string p_motivo, string p_token_app)
        {
            _infoClienteConectado.token = p_token_app;
            _infoClienteConectado.cpf_cnpj = "";

            ResultGravaInteracaoPesquisaBOT result = new ChatBotView(_configuration, _environment, p_token_app).SetBotInteracaoPesquisaApp(p_idbot, p_nota, p_motivo);

            return result;
        }

        /// <summary> 
        /// Grava os passos que o BOT fez a interação atendimento 
        /// </summary> 
        /// <param name="p_idbot"></param>  
        /// <param name="p_protocolo"></param>  
        /// <param name="p_depto"></param>  
        /// <param name="p_agente"></param> 
        /// <param name="p_status"></param>  
        /// <param name="p_token_app"></param> 
        /// <returns>Status do Processamento</returns> 
        [AllowAnonymous]
        [HttpGet("gravainteracaoatendimento/{p_idbot},{p_protocolo},{p_depto},{p_agente},{p_status},{p_token_app}")]
        [HttpPost("gravainteracaoatendimento/{p_idbot},{p_protocolo},{p_depto},{p_agente},{p_status},{p_token_app}")]
        public ResultGravaInteracaoAtendimentoBOT GravaInteracaoAtendimentoBOT(string p_idbot, string p_protocolo, string p_depto, string p_agente, string p_status, string p_token_app)
        {
            _infoClienteConectado.token = p_token_app;
            _infoClienteConectado.cpf_cnpj = "";

            ResultGravaInteracaoAtendimentoBOT result = new ChatBotView(_configuration, _environment, p_token_app).SetBotInteracaoAtendimentoApp(p_idbot, p_protocolo, p_depto, p_agente, p_status);

            return result;
        }
    }
}
