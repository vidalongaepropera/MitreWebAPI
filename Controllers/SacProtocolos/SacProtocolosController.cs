using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Model.ChatBot;
using MitreWebAPI.Model.Documentos;
using MitreWebAPI.Model.SacProtocolos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Controllers.SacProtocolos
{
    /// <summary>
    /// Controle protocolos do SAC
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SacProtocolosController : ControllerBase
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
        public SacProtocolosController(IConfiguration Configuration, InfoClienteConectado infoClienteConectado, IWebHostEnvironment env)
        {
            _configuration = Configuration;
            _environment = env;
            _infoClienteConectado = infoClienteConectado;
        }

        /// <summary>
        /// Lista departamentos do SAC
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <returns>ID e descrição</returns>
        [HttpGet("lista/departamentos/{p_token_app}")]
        [HttpPost("lista/departamentos/{p_token_app}")]
        public ResultSacDepartamentos ListaSacDepertamentos(string p_token_app)
        {

            _infoClienteConectado.token = p_token_app;

            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            var result = view.ListaSacDepertamentos();

            return result;

        }

        /// <summary>
        /// Lista assuntos vinculados ao departamento SAC
        /// </summary>
        /// <param name="p_dpto_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Id e descrição</returns>
        [HttpGet("lista/departamento/assuntos/{p_dpto_id},{p_token_app}")]
        [HttpPost("lista/departamento/assuntos/{p_dpto_id},{p_token_app}")]
        public ResultSacDptoAssuntos ListaSacDepertamentoAssunto(Int32 p_dpto_id, string p_token_app)
        {

            _infoClienteConectado.token = p_token_app;

            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            var result = view.ListaSacDepertamentoAssunto(p_dpto_id);

            return result;

        }

        /// <summary>
        /// Lista os protocolos do cliente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista com as informações</returns>
        [HttpGet("lista/{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("lista/{p_cpf_cnpj},{p_token_app}")]
        public ResultSacProtocolos ListaSacProtocolos(string p_cpf_cnpj, string p_token_app)
        {

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            var result = view.ListaSacProtocolos(p_cpf_cnpj);

            return result;

        }

        /// <summary>
        /// Lista detalhes das tratativas do protocolo 
        /// </summary>
        /// <param name="p_protocolo_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Tratativas e nomes de anexo</returns>
        [HttpGet("lista/tratativas/{p_protocolo_id},{p_token_app}")]
        [HttpPost("lista/tratativas/{p_protocolo_id},{p_token_app}")]
        public ResultSacProtocolosItem ListaSacProtocoloItem(Int32 p_protocolo_id, string p_token_app)
        {

            _infoClienteConectado.token = p_token_app;

            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            var result = view.ListaSacProtocoloItem(p_protocolo_id);

            return result;

        }

        /// <summary>
        /// Baixa o anexo do protocolo item
        /// </summary>
        /// <param name="p_protocolo_item_id"></param>
        /// <param name="p_anexo_item_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Arquivo binario anexo</returns>
        [HttpGet("download/anexo/{p_protocolo_item_id},{p_anexo_item_id},{p_token_app}")]
        [HttpPost("download/anexo/{p_protocolo_item_id},{p_anexo_item_id},{p_token_app}")]
        public FileContentResult DownloadSacProtocoloItemAnexo(Int32 p_protocolo_item_id, Int32 p_anexo_item_id, string p_token_app)
        {

            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var fileContentResult = view.DownloadSacProtocoloItemAnexo(p_protocolo_item_id, p_anexo_item_id);

            return fileContentResult;
        }

        /// <summary>
        /// Baixa o anexo do protocolo item em Json
        /// </summary>
        /// <param name="p_protocolo_item_id"></param>
        /// <param name="p_anexo_item_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Arquivo binario anexo</returns>
        [HttpGet("download/anexo/json/{p_protocolo_item_id},{p_anexo_item_id},{p_token_app}")]
        [HttpPost("download/anexo/json/{p_protocolo_item_id},{p_anexo_item_id},{p_token_app}")]
        public ResultDownloadInJson DownloadSacProtocoloItemAnexoJson(Int32 p_protocolo_item_id, Int32 p_anexo_item_id, string p_token_app)
        {

            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var fileContentResult = view.DownloadSacProtocoloItemAnexo(p_protocolo_item_id, p_anexo_item_id);

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
        /// Cria um novo protocolo pelo cliente
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <param name="infoTratativa"></param>
        /// <returns>Informações do protocolo criado</returns>
        [HttpPost("criar/novo/protocolo/{p_token_app}")]
        public ResultSacProtocolosItemCriarNovo SacProtocolosItemCriarNovo(string p_token_app, [FromForm] InfoSacProtocoloIntemCriarNovo infoTratativa)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = view.SacProtocolosItemCriarNovo(infoTratativa);

            return result;

        }

        /// <summary>
        /// Salva a tratativa do cliente no protocolo
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <param name="infoTratativa"></param>
        /// <returns>Status do salvamento</returns>
        [HttpPost("salvar/tratativa/{p_token_app}")]
        public ResultSacProtocolosItemSalvar SacProtocolosItemSalvar(string p_token_app, [FromForm] InfoSacProtocoloIntemSalvar infoTratativa)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = view.SacProtocolosItemSalvar(infoTratativa);

            return result;

        }

        /// <summary>
        /// Salva o arquivo anexo na tratativa do cliente
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <param name="anexoInfo"></param>
        /// <returns>Status do salvamento</returns>
        [HttpPost("upload/anexo/tratativa/{p_token_app}")]
        public ResultInfoDocumentoUpload UploadAnexoProtocoloItem(string p_token_app, [FromForm] InfoSacProtocoloItemAnexosSalvar anexoInfo)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = view.UploadAnexoProtocoloItem(anexoInfo);

            return result;

        }

        /// <summary>
        /// Conclui o protocolo pelo cliente
        /// </summary>
        /// <param name="p_agente_id"></param>
        /// <param name="p_protocolo_id"></param>
        /// <param name="p_protocolo_item_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Status da conclusão do protocolo</returns>
        [HttpGet("concluir/tratativa/{p_agente_id},{p_protocolo_id},{p_protocolo_item_id},{p_token_app}")]
        [HttpPost("concluir/tratativa/{p_agente_id},{p_protocolo_id},{p_protocolo_item_id},{p_token_app}")]
        public ResultSacProtocolosItemSalvar SacProtocolosItemConcluir(Int32 p_agente_id, Int32 p_protocolo_id, Int32 p_protocolo_item_id, string p_token_app)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = view.SacProtocolosItemConcluir(p_agente_id, p_protocolo_id, p_protocolo_item_id);

            return result;

        }

        /// <summary>
        /// Reagenda um nova data para visita técnica
        /// </summary>
        /// <param name="p_agente_id"></param>
        /// <param name="p_agendamento_id"></param>
        /// <param name="p_data_ini"></param>
        /// <param name="p_data_fim"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Informaçõs do agendamento</returns>
        [HttpGet("reagendar/compromisso/{p_agente_id},{p_agendamento_id},{p_data_ini},{p_data_fim},{p_token_app}")]
        [HttpPost("reagendar/compromisso/{p_agente_id},{p_agendamento_id},{p_data_ini},{p_data_fim},{p_token_app}")]
        public ResultSacReagendarCompromisso SacProtocolosReagendarComprmisso(Int32 p_agente_id, Int32 p_agendamento_id, string p_data_ini, string p_data_fim, string p_token_app)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = view.SacProtocolosReagendarComprmisso(p_agente_id, p_agendamento_id, p_data_ini, p_data_fim);

            return result;

        }

        /// <summary>
        /// Cancela um determinado agendamento de compromisso
        /// </summary>
        /// <param name="p_agendamento_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Status do cancelamento</returns>
        [HttpGet("cancelar/compromisso/{p_agendamento_id},{p_token_app}")]
        [HttpPost("cancelar/compromisso/{p_agendamento_id},{p_token_app}")]
        public ResultSacCancelarCompromisso SacProtocolosCancelarComprmisso(Int32 p_agendamento_id, string p_token_app)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = view.SacProtocolosCancelarComprmisso(p_agendamento_id);

            return result;

        }

        /// <summary>
        /// Lista datas disponíveis para o agendamento 
        /// </summary>
        /// <param name="p_cliente_id"></param>
        /// <param name="p_empreendimento_id"></param>
        /// <param name="p_unidade_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Datas disponiveis</returns>
        [HttpGet("agenda/lista/datas/disponiveis/{p_cliente_id},{p_empreendimento_id},{p_unidade_id},{p_token_app}")]
        [HttpPost("agenda/lista/datas/disponiveis/{p_cliente_id},{p_empreendimento_id},{p_unidade_id},{p_token_app}")]
        public ResultSacDatasDisponiveis SacAgendaDatasDisponivel(Int32 p_cliente_id, Int32 p_empreendimento_id, Int32 p_unidade_id, string p_token_app)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = view.SacAgendaDatasDisponivel(p_cliente_id, p_empreendimento_id, p_unidade_id);

            return result;

        }

        /// <summary>
        /// Lista os compromissos agendados
        /// </summary>
        /// <param name="p_cliente_id"></param>
        /// <param name="p_empreendimento_id"></param>
        /// <param name="p_unidade_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista de agendamentos</returns>
        [HttpGet("lista/compromissos/agendado/{p_cliente_id},{p_empreendimento_id},{p_unidade_id},{p_token_app}")]
        [HttpPost("lista/compromissos/agendado/{p_cliente_id},{p_empreendimento_id},{p_unidade_id},{p_token_app}")]
        public ResultSacCompromissosAgendados SacListaCompromissosAgendado(Int32 p_cliente_id, Int32 p_empreendimento_id, Int32 p_unidade_id, string p_token_app)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.token = p_token_app;

            var result = view.SacListaCompromissosAgendado(p_cliente_id, p_empreendimento_id, p_unidade_id);

            return result;

        }

        /// <summary>
        /// Alerta o APP sobre alterações no protocolo do cliente
        /// </summary>
        /// <param name="alertInfo"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Status da integração</returns>
        [HttpPost("alerta/app/{p_token_app}")]
        public ResultSacAletaApp SacInfoToApp([FromBody] InfoSacAlertaApp alertInfo, string p_token_app)
        {
            var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = alertInfo.cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var result = view.SacInfoToApp(alertInfo);

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <param name="p_alertInfo"></param>
        /// <returns></returns>
        [HttpGet("teste/plsql/{p_token_app}")]
        [HttpPost("teste/plsql/{p_token_app}")]
        public ResultSacAletaApp SacInfoToAppTeste([FromBody] InfoSacAlertaApp p_alertInfo, string p_token_app)
        {
            //var view = new SacProtocolosView(_configuration, _environment, p_token_app);

            //_infoClienteConectado.cpf_cnpj = alertInfo.cpf_cnpj;
            //_infoClienteConectado.token = p_token_app;

            //var result = view.SacInfoToApp(alertInfo);

            var result = new ResultSacAletaApp();

            result.success = true;
            result.message = "Ok";
            result.statusIntegracao = "OK";
            result.alerta = p_alertInfo;

            return result;

        }
    }
}
