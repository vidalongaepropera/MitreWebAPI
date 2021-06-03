using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Controllers.Util;
using MitreWebAPI.Model.Documentos;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace MitreWebAPI.Controllers.Documentos
{

    /// <summary>
    /// Controla Upload e Download e Arquivos
    /// </summary>
    public class ArquivosView
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private BoletoNetCore.IBanco _banco;
        private readonly string _token;
        private readonly string _filePathDocCliente;
        private bool _isTokenValido = false;

        /// <summary>
        /// Guarda as configuraçõpes da aplicação devalida o token da aplicação
        /// </summary>
        /// <param name="Configuration"></param>
        /// <param name="env"></param>
        /// <param name="p_token_app"></param>
        public ArquivosView(IConfiguration Configuration, IWebHostEnvironment env, string p_token_app)
        {
            _configuration = Configuration;
            _environment = env;
            _token = _configuration.GetSection("ConfigAPIChatBot").GetSection("Token").Value;
            _filePathDocCliente = _configuration.GetSection("ConfigAPIChatBot").GetSection("FilePathDocCliente").Value;

            if (p_token_app != null && !p_token_app.Equals(_token))
                throw new Exception("Token da aplicação inválida!");
            else
                _isTokenValido = true;

        }

        /// <summary>
        /// Fa o Upload do arquivo
        /// </summary>
        /// <param name="arq"></param>
        /// <returns>Infoermações do Upload</returns>
        public async Task<ResultInfoDocumentoUpload> UploadFile(InfoDocumentoUpload arq)
        {

            var pathBase = _filePathDocCliente;

            var result = new ResultInfoDocumentoUpload();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            long fileLenght = 0;

            try
            {
                arq.p_cpf_cnpj = Util.Formatacoes.SemFormatacao(arq.p_cpf_cnpj);

                pathBase += arq.p_cpf_cnpj + "\\" + arq.p_contrato_id + "\\";

                fileLenght = arq.p_arquivo_bin.Length;

                if (arq.p_arquivo_bin != null && fileLenght > 0)
                {
                    if (!Directory.Exists(pathBase))
                    {
                        Directory.CreateDirectory(pathBase);
                    }

                    FileStream filestream;

                    using (filestream = System.IO.File.Create(pathBase + arq.p_arquivo_bin.FileName))
                    {
                        await arq.p_arquivo_bin.CopyToAsync(filestream);

                        filestream.Flush();
                    }

                    if (filestream != null)
                        filestream.Close();

                }
                else
                {
                    throw new Exception("Ocorreu uma falha no envio do arquivo!");
                }

                var v_tipo_arquivo = Util.Formatacoes.GetContentType(arq.p_arquivo_bin.FileName);
                var v_documento_id = 0;
                var v_data_upload = "";

                conn = oraDb.AbreConexao();

                var comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_upload_file";

                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, arq.p_contrato_id, ParameterDirection.Input);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, arq.p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_nome_arquivo", OracleDbType.Varchar2, arq.p_arquivo_bin.FileName, ParameterDirection.Input);
                comm.Parameters.Add("p_titulo_arquivo", OracleDbType.Varchar2, arq.p_titulo_arquivo, ParameterDirection.Input);
                comm.Parameters.Add("p_desc_arquivo", OracleDbType.Varchar2, arq.p_desc_arquivo, ParameterDirection.Input);
                comm.Parameters.Add("p_tipo_conteudo", OracleDbType.Varchar2, v_tipo_arquivo, ParameterDirection.Input);
                comm.Parameters.Add("p_local_arquivo", OracleDbType.Varchar2, pathBase + arq.p_arquivo_bin.FileName, ParameterDirection.Input);
                comm.Parameters.Add("p_tamanho_arquivo", OracleDbType.Varchar2, fileLenght.ToString().Replace(".", ","), ParameterDirection.Input);

                OracleParameter docId = new OracleParameter();
                docId.OracleDbType = OracleDbType.Varchar2;
                docId.Direction = ParameterDirection.Output;
                docId.Size = Int32.MaxValue;
                comm.Parameters.Add(docId);


                OracleParameter docDataUpload = new OracleParameter();
                docDataUpload.OracleDbType = OracleDbType.Varchar2;
                docDataUpload.Direction = ParameterDirection.Output;
                docDataUpload.Size = 100;
                comm.Parameters.Add(docDataUpload);

                comm.ExecuteNonQuery();

                v_data_upload = docDataUpload.Value.ToString();
                v_documento_id = Int32.Parse(docId.Value.ToString());

                result.success = true;
                result.message = "OK";
                result.documento_id = v_documento_id;
                result.documento_nome = arq.p_arquivo_bin.FileName;
                result.documento_titulo = arq.p_titulo_arquivo;
                result.documento_size = fileLenght;
                result.documento_app_type = v_tipo_arquivo;
                result.documento_desc = arq.p_desc_arquivo;
                result.data_upload = v_data_upload;

            }
            catch(Exception e)
            {
                throw new Exception(e.Message + "\n" + e.StackTrace);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Lista dos documento do cliente
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns></returns>
        public ResultInfoDocumentoDownload ListaDocumentoCliente(Int32 p_contrato_id, string p_cpf_cnpj)
        {
            
            var result = new ResultInfoDocumentoDownload();
            var list = new List<InfoDocumentoDownload>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_documentos_cliente";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, p_contrato_id, ParameterDirection.Input);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {
                        var tipo_conteudo = dr["tipo_conteudo"].ToString();

                        var info = new InfoDocumentoDownload();

                        info.documento_id = Int32.Parse(dr["arquivo_id"].ToString());
                        info.documento_nome = dr["nome_arquivo"].ToString();
                        info.documento_desc = dr["desc_arquivo"].ToString();
                        info.documento_titulo = dr["titulo_arquivo"].ToString();

                        if (!String.IsNullOrEmpty(tipo_conteudo))
                        {
                            info.documento_app_type = tipo_conteudo;
                            info.documento_size = Int32.Parse(dr["tamanho_arquivo"].ToString()); ;
                        }
                        else
                        {
                            Byte[] buffer;

                            try
                            {
                                buffer = (Byte[])(dr.GetOracleBlob(7)).Value;
                            }
                            catch
                            {
                                buffer = new byte[0];
                            }

                            info.documento_app_type = Util.Formatacoes.GetContentType(info.documento_nome);
                            info.documento_size = buffer.Length;
                        }

                        info.data_upload = dr["data_inclusao"].ToString();

                        list.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.documentos = list;

                }
                else
                {

                    result.success = false;
                    result.message = "Sem documentos";
                }

            }
            catch(Exception e)
            {
                throw new Exception(e.Message + "\n" + e.StackTrace);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Apaga documento  que o cliente fez download
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_documento_id"></param>
        /// <returns>Status da eliminação do documento</returns>
        public ResultApagarDocumento ApagarDocumentoPessoal(Int32 p_contrato_id, string p_cpf_cnpj, Int32 p_documento_id)
        {

            var result = new ResultApagarDocumento();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_apaga_documento_cliente";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, p_contrato_id, ParameterDirection.Input);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_documento_id", OracleDbType.Varchar2, p_documento_id, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var local_arquivo = dr["local_arquivo"].ToString();

                        if (File.Exists(local_arquivo))
                            File.Delete(local_arquivo);

                    }

                    result.success = true;
                    result.message = "OK";

                }
                else
                {

                    result.success = false;
                    result.message = "Sem documento com o id " + p_documento_id + " para o contrato e cpf/cnpj informado";
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + e.StackTrace);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Donload do documento do cliente
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_documento_id"></param>
        /// <returns>Download do arquivo</returns>
        public FileContentResult DownloadDocumentoCliente(Int32 p_contrato_id, string p_cpf_cnpj, Int32 p_documento_id)
        {

            OracleConnection conn;
            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);
            var filePath = "";
            FileContentResult fileContentResult = null;

            var info = new InfoDocumentoDownload();

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_pega_documento_cliente";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, p_contrato_id, ParameterDirection.Input);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_documento_id", OracleDbType.Varchar2, p_documento_id, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        info.documento_id = Int32.Parse(dr["arquivo_id"].ToString());
                        info.documento_nome = dr["nome_arquivo"].ToString();
                        info.documento_desc = dr["desc_arquivo"].ToString();
                        info.documento_app_type = dr["tipo_conteudo"].ToString();
                        info.documento_size = Int32.Parse(dr["tamanho_arquivo"].ToString()); ;
                        info.data_upload = dr["data_inclusao"].ToString();
                        filePath = dr["local_arquivo"].ToString();

                        byte[] buffer = new byte[16 * 1024];

                        FileStream stream = File.OpenRead(filePath);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }

                            fileContentResult = new FileContentResult(ms.ToArray(), info.documento_app_type);
                            fileContentResult.FileDownloadName = info.documento_nome;
                        }

                    }
                }
                else
                {
                    throw new Exception("Arquivo não localizado com id: " + p_documento_id + ", para esse contrato e cpf");
                }

            }
            catch(Exception e)
            {
                throw new Exception("Ocorreu problema com o download do aqruivo: " + info.documento_nome + "\n" + e.Message + "\n" + e.StackTrace);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return fileContentResult;
        }

        /// <summary>
        /// Lista documentos contratuais do cliente
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns>Lista Json com dados dos documentos</returns>
        public ResultInfoDocumentoDownload ListaDocumentoContrato(Int32 p_contrato_id, string p_cpf_cnpj)
        {

            var result = new ResultInfoDocumentoDownload();
            var list = new List<InfoDocumentoDownload>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_documentos_contrato";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, p_contrato_id, ParameterDirection.Input);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        Byte[] buffer = new byte[0];

                        try
                        {
                            buffer = (Byte[])(dr.GetOracleBlob(4)).Value;
                        }
                        catch { };

                        var info = new InfoDocumentoDownload();

                        info.documento_id = Int32.Parse(dr["arquivo_id"].ToString());
                        info.documento_nome = dr["nome_arquivo"].ToString();
                        info.documento_desc = dr["desc_arquivo"].ToString();
                        info.documento_titulo = dr["desc_arquivo"].ToString();
                        info.documento_app_type = Formatacoes.GetContentType(info.documento_nome);
                        info.documento_size = buffer.Length;
                        info.data_upload = dr["data_inclusao"].ToString();

                        list.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.documentos = list;

                }
                else
                {

                    result.success = false;
                    result.message = "Sem documentos";
                }

            }
            catch(Exception e)
            {
                throw new Exception(e.Message + "\n" + e.StackTrace);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }


        /// <summary>
        /// Download de documento de contrato do cliente
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_documento_id"></param>
        /// <returns>PDF do documento</returns>
        public FileContentResult DownloadDocumentoContrato(Int32 p_contrato_id, string p_cpf_cnpj, Int32 p_documento_id)
        {

            OracleConnection conn;

            FileContentResult fileContentResult  = null;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_pega_documento_contrato";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, p_contrato_id, ParameterDirection.Input);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_documento_id", OracleDbType.Varchar2, p_documento_id, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var documento_nome = dr["nome_arquivo"].ToString();
                        var tipodocumento = Util.Formatacoes.GetContentType(documento_nome);

                        Byte[] buffer = (Byte[])(dr.GetOracleBlob(4)).Value;

                        fileContentResult = new FileContentResult(buffer, tipodocumento);
                        fileContentResult.FileDownloadName = documento_nome;

                    }

                }
                else
                {
                    throw new Exception("Arquivo não localizado com id: " + p_documento_id);
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return fileContentResult;
        }
    }
}
