using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Model.ChatBot;
using MitreWebAPI.Model.Documentos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MitreWebAPI.Controllers.Documentos
{
    /// <summary>
    /// Contre de Upload e Download de Arquivos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArquivosController : ControllerBase
    {
        /// <summary>
        /// Clase que mantem na sessão as informações do client conectado.
        /// </summary>
        public InfoClienteConectado _infoClienteConectado;

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Metodo princial de inicialização do controle de arquivos
        /// </summary>
        /// <param name="Configuration"></param>
        /// <param name="infoClienteConectado"></param>
        /// <param name="env"></param>
        public ArquivosController(IConfiguration Configuration, InfoClienteConectado infoClienteConectado, IWebHostEnvironment env)
        {
            _configuration = Configuration;
            _environment = env;
            _infoClienteConectado = infoClienteConectado;
        }

        /// <summary>
        /// Faz upload de arquivos que o cliente eenvia pelo APP
        /// </summary>
        /// <param name="p_token_app"></param>
        /// <param name="infoArqUpload"></param>
        /// <returns>Lista com informação do arquivo enviado</returns>
        [HttpPost("cliente/upload/{p_token_app}")]
        public async Task<ResultInfoDocumentoUpload> InfoDocumentoUpload(string p_token_app, [FromForm] InfoDocumentoUpload infoArqUpload)
        {
            var arqView = new ArquivosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = infoArqUpload.p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var result = await arqView.UploadFile(infoArqUpload);

            return result;

        }

        /// <summary>
        /// Lista documentos que o cliente fez upload
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_token_app"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns>Lista dados dos documentos</returns>
        [HttpGet("cliente/pessoal/lista/{p_contrato_id},{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("cliente/pessoal/lista/{p_contrato_id},{p_cpf_cnpj},{p_token_app}")]
        public ResultInfoDocumentoDownload InfoDocumentoDownload(Int32 p_contrato_id, string p_cpf_cnpj, string p_token_app)
        {
            var docView = new ArquivosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var list = docView.ListaDocumentoCliente(p_contrato_id, p_cpf_cnpj);

            return list;
        }

        /// <summary>
        /// Faz o download do documento do cliente
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_documento_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns></returns>
        [HttpGet("cliente/pessoal/download/{p_contrato_id},{p_cpf_cnpj},{p_documento_id},{p_token_app}")]
        [HttpPost("cliente/pessoal/download/{p_contrato_id},{p_cpf_cnpj},{p_documento_id},{p_token_app}")]
        public FileContentResult DownloadDocumentoCliente(Int32 p_contrato_id, string p_cpf_cnpj, Int32 p_documento_id, string p_token_app)
        {

            var docView = new ArquivosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;
            
            var fileContentResult = docView.DownloadDocumentoCliente(p_contrato_id, p_cpf_cnpj, p_documento_id);

            return fileContentResult;
        }

        /// <summary>
        /// Faz o download do documento do cliente
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_documento_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns></returns>
        [HttpGet("cliente/pessoal/download/json/{p_contrato_id},{p_cpf_cnpj},{p_documento_id},{p_token_app}")]
        [HttpPost("cliente/pessoal/download/json/{p_contrato_id},{p_cpf_cnpj},{p_documento_id},{p_token_app}")]
        public ResultDownloadInJson DownloadDocumentoClienteJson(Int32 p_contrato_id, string p_cpf_cnpj, Int32 p_documento_id, string p_token_app)
        {

            var docView = new ArquivosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = docView.DownloadDocumentoCliente(p_contrato_id, p_cpf_cnpj, p_documento_id);

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
        /// Apaga documento que o cliente fez upload
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_documento_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Staus da eliminação do documento</returns>
        [HttpGet("cliente/pessoal/apagar/{p_contrato_id},{p_cpf_cnpj},{p_documento_id},{p_token_app}")]
        [HttpPost("cliente/pessoal/apagar/{p_contrato_id},{p_cpf_cnpj},{p_documento_id},{p_token_app}")]
        public ResultApagarDocumento ApagarDocumentoPessoal(Int32 p_contrato_id, string p_cpf_cnpj, Int32 p_documento_id, string p_token_app)
        {
            var docView = new ArquivosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var list = docView.ApagarDocumentoPessoal(p_contrato_id, p_cpf_cnpj, p_documento_id);

            return list;
        }

        /// <summary>
        /// Lista documentos contratuais incluídos para o cliente
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_token_app"></param>
        /// <returns>Lista de documentos</returns>
        [HttpGet("cliente/contrato/lista/{p_contrato_id},{p_cpf_cnpj},{p_token_app}")]
        [HttpPost("cliente/contrato/lista/{p_contrato_id},{p_cpf_cnpj},{p_token_app}")]
        public ResultInfoDocumentoDownload InfoDocumentoContrato(Int32 p_contrato_id, string p_cpf_cnpj, string p_token_app)
        {
            var docView = new ArquivosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var list = docView.ListaDocumentoContrato(p_contrato_id, p_cpf_cnpj);

            return list;
        }

        /// <summary>
        /// Faz o download do documento do contrato em Json
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_documento_id"></param>
        /// <param name="p_token_app"></param>
        /// <returns></returns>
        [HttpGet("cliente/contrato/download/json/{p_contrato_id},{p_cpf_cnpj},{p_documento_id},{p_token_app}")]
        [HttpPost("cliente/contrato/download/json/{p_contrato_id},{p_cpf_cnpj},{p_documento_id},{p_token_app}")]
        public ResultDownloadInJson DownloadDocumentoContrato(Int32 p_contrato_id, string p_cpf_cnpj, Int32 p_documento_id, string p_token_app)
        {

            var docView = new ArquivosView(_configuration, _environment, p_token_app);

            _infoClienteConectado.cpf_cnpj = p_cpf_cnpj;
            _infoClienteConectado.token = p_token_app;

            var fileContentResult = docView.DownloadDocumentoContrato(p_contrato_id, p_cpf_cnpj, p_documento_id);

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

    }
}
