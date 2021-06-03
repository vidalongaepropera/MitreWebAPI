using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Model.ChatBot;
using MitreWebAPI.Model.Documentos;
using MitreWebAPI.Model.Recebimento;
using MitreWebAPI.Model.SacProtocolos;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MitreWebAPI.Controllers.SacProtocolos
{
    /// <summary>
    /// Controla protocolos SAC
    /// </summary>
    public class SacProtocolosView
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly string _url_base_sac_protocolos;
        private readonly string _token;

        /// <summary>
        /// Guarda as configuraçõpes da aplicação devalida o token da aplicação
        /// </summary>
        /// <param name="Configuration"></param>
        /// <param name="env"></param>
        /// <param name="p_token_app"></param>
        public SacProtocolosView(IConfiguration Configuration, IWebHostEnvironment env, string p_token_app)
        {
            _configuration = Configuration;
            _environment = env;
            _url_base_sac_protocolos = _configuration.GetSection("ConfigAPIChatBot").GetSection("url_base_sac_protocolos").Value;
            _token = _configuration.GetSection("ConfigAPIChatBot").GetSection("Token").Value;

            if (p_token_app != null && !p_token_app.Equals(_token))
                throw new Exception("Token da aplicação inválida!");

        }

        /// <summary>
        /// Lista Departamentos do SAC
        /// </summary>
        /// <returns>Lista com informações das parcelas antecipaveis</returns>
        public ResultSacDepartamentos ListaSacDepertamentos()
        {

            var result = new ResultSacDepartamentos();

            try
            {

                string json = "";

                var remoteFileUrl = _url_base_sac_protocolos + "/protocolos/departamentos";

                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    try
                    {
                        json = wc.DownloadString(remoteFileUrl);
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception("Departamentos não disponíveis!\n" + ex.Message + "\n" + ex.StackTrace);
                    }
                }

                var dptos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InfoSacDepartamentos>>(json);

                InfoParcelasAntecipacao x = new InfoParcelasAntecipacao();

                result.success = true;
                result.message = "OK";
                result.result = dptos;

            }
            catch (Exception e)
            {
                result.success = false;
                result.message = e.Message + "\n " + e.StackTrace;
            }
            finally
            {

            }

            return result;
        }

        /// <summary>
        /// Lista os assuntos de um determinado departamento
        /// </summary>
        /// <param name="p_depto_id"></param>
        /// <returns>ID e descrição</returns>
        public ResultSacDptoAssuntos ListaSacDepertamentoAssunto(Int32 p_depto_id)
        {

            var result = new ResultSacDptoAssuntos();

            try
            {

                string json = "";

                var remoteFileUrl = _url_base_sac_protocolos + "/protocolos/" + p_depto_id + "/departamentos/assuntos";

                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    try
                    {
                        json = wc.DownloadString(remoteFileUrl);
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception("Assuntos não disponíveis!\n" + ex.Message + "\n" + ex.StackTrace);
                    }
                }

                var assuntos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InfoSacDptoAssuntos>>(json);

                result.success = true;
                result.message = "OK";
                result.result = assuntos;

            }
            catch (Exception e)
            {
                result.success = false;
                result.message = e.Message + "\n " + e.StackTrace;
            }
            finally
            {

            }

            return result;
        }

        /// <summary>
        /// Lista Protocolos SAC do Cliente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns></returns>
        public ResultSacProtocolos ListaSacProtocolos(string p_cpf_cnpj)
        {

            var result = new ResultSacProtocolos();
            var list = new List<InfoSacProtocolos>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_protocolos_sac";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                Int32 cliente_id = 0;

                if (dr != null && dr.HasRows)
                {


                    while (dr.Read())
                    {

                        var info = new InfoSacProtocolos();

                        cliente_id = Int32.Parse(dr["cliente_id"].ToString());

                        info.contrato_id = Int32.Parse(dr["contrato_id"].ToString());
                        info.protocolo_id = Int32.Parse(dr["protocolo_id"].ToString());
                        info.meio_comunicacao_id = Int32.Parse(dr["meio_comunicacao_id"].ToString());
                        info.departamento_id = Int32.Parse(dr["departamento_id"].ToString());
                        info.assunto_id = Int32.Parse(dr["assunto_id"].ToString());
                        info.protocolo = dr["protocolo"].ToString();
                        info.data_inclusao = dr["data_inclusao"].ToString();
                        info.departamento = dr["departamento"].ToString();
                        info.assunto = dr["assunto"].ToString();
                        info.status = dr["status"].ToString();

                        list.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.cliente_id = cliente_id;
                    result.result = list;

                }
                else
                {

                    result.success = true;
                    result.message = "Sem protocolos";
                    result.cliente_id = 0;
                    result.result = list;

                    //throw new Exception("Sem dados para o cliente com cpf ou cnpj:  " + p_cpf_cnpj + ".");
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Lista detalhes das tratativas do protocolo 
        /// </summary>
        /// <param name="p_protocolo_id"></param>
        /// <returns>Tratativas e nomes de anexo</returns>
        public ResultSacProtocolosItem ListaSacProtocoloItem(Int32 p_protocolo_id)
        {

            var result = new ResultSacProtocolosItem();
            var list = new List<InfoSacProtocolosItem>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_protocolos_item_sac";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_protocolo_id", OracleDbType.Varchar2, p_protocolo_id.ToString(), ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {


                    while (dr.Read())
                    {

                        var info = new InfoSacProtocolosItem();

                        info.protocolo_item_id = Int32.Parse(dr["seq_protocoloitem"].ToString());
                        info.data_inclusao = dr["data"].ToString();
                        info.atendente_nome = dr["nomeagente"].ToString();
                        info.status = dr["status"].ToString();

                        OracleClob clob = dr.GetOracleClob(1);

                        try
                        {
                            info.tratativa = clob.Value;
                        }
                        catch
                        {
                            info.tratativa = "";
                        }

                        OracleCommand comm2 = new OracleCommand();

                        comm2.Connection = conn;
                        comm2.CommandType = CommandType.StoredProcedure;
                        comm2.CommandText = "dwmitre.pck_app_charbot.sp_lista_anexos_protocolos_item_sac";

                        comm2.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                        comm2.Parameters.Add("p_protocolo_item_id", OracleDbType.Varchar2, info.protocolo_item_id.ToString(), ParameterDirection.Input);

                        OracleDataReader drAnexo = comm2.ExecuteReader();

                        var anexos = new List<InfoSacProtocoloItemAnexos>();

                        if (drAnexo != null && drAnexo.HasRows)
                        {
                            while (drAnexo.Read())
                            {
                                InfoSacProtocoloItemAnexos anexo = new InfoSacProtocoloItemAnexos();

                                anexo.anexo_id = Int32.Parse(drAnexo["anexo_id"].ToString());
                                anexo.nome_arquivo = drAnexo["anexo_nome"].ToString();
                                anexo.titulo = drAnexo["titulo"].ToString();
                                anexo.descricao = drAnexo["descricao"].ToString();
                                anexo.documento_app_type = Util.Formatacoes.GetContentType(anexo.nome_arquivo);

                                anexos.Add(anexo);

                            }

                            info.anexos = anexos;

                        }
                        else
                        {
                            info.anexos = null;
                        }

                        list.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.result = list;

                }
                else
                {

                    throw new Exception("Sem tratativas para o protocolo:  " + p_protocolo_id.ToString() + ".");
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Baixa o anexo do protocolo item
        /// </summary>
        /// <param name="p_protocolo_item_id"></param>
        /// <param name="p_anexo_item_id"></param>
        /// <returns>Arquivo binario anexo</returns>
        public FileContentResult DownloadSacProtocoloItemAnexo(Int32 p_protocolo_item_id, Int32 p_anexo_item_id)
        {

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            FileContentResult fileContentResult = null;

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_pega_anexo_protocolo_item_sac";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_protocolo_item_id", OracleDbType.Varchar2, p_protocolo_item_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_anexo_item_id", OracleDbType.Varchar2, p_anexo_item_id.ToString(), ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var nome_anexo = dr["nome_anexo"].ToString();
                        var tipodocumento = Util.Formatacoes.GetContentType(nome_anexo);

                        Byte[] buffer = (Byte[])(dr.GetOracleBlob(1)).Value;

                        fileContentResult = new FileContentResult(buffer, tipodocumento);
                        fileContentResult.FileDownloadName = nome_anexo;
                    }
                }
                else
                {
                    throw new Exception("Arquivo não localizado com id: " + p_anexo_item_id + ", para esse contrato e cpf");
                }

            }
            catch (Exception e)
            {
                throw new Exception("Ocorreu problema com o download do arquivo: " + p_anexo_item_id + "\n" + e.Message + "\n" + e.StackTrace);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return fileContentResult;
        }

        /// <summary>
        /// Cria um novo protocolo pelo cliente
        /// </summary>
        /// <param name="infoTratativa"></param>
        /// <returns>Informações do protocolo criado</returns>
        public ResultSacProtocolosItemCriarNovo SacProtocolosItemCriarNovo(InfoSacProtocoloIntemCriarNovo infoTratativa)
        {

            var result = new ResultSacProtocolosItemCriarNovo();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_criar_novo_protocolos_item_sac";

                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, infoTratativa.p_contrato_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_agente_id", OracleDbType.Varchar2, infoTratativa.p_agente_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_meio_comunicacao_id", OracleDbType.Varchar2, infoTratativa.p_meio_comunicacao_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_departamento_id", OracleDbType.Varchar2, infoTratativa.p_departamento_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_assunto_id", OracleDbType.Varchar2, infoTratativa.p_assunto_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_tratativa", OracleDbType.Varchar2, infoTratativa.p_tratativa, ParameterDirection.Input);

                OracleParameter princId = new OracleParameter();
                princId.OracleDbType = OracleDbType.Varchar2;
                princId.Direction = ParameterDirection.Output;
                princId.Size = Int32.MaxValue;
                comm.Parameters.Add(princId);

                OracleParameter itemId = new OracleParameter();
                itemId.OracleDbType = OracleDbType.Varchar2;
                itemId.Direction = ParameterDirection.Output;
                itemId.Size = Int32.MaxValue;
                comm.Parameters.Add(itemId);

                comm.Parameters.Add("p_data_agenda_ini", OracleDbType.Varchar2, infoTratativa.p_data_agenda_ini, ParameterDirection.Input);
                comm.Parameters.Add("p_data_agenda_fim", OracleDbType.Varchar2, infoTratativa.p_data_agenda_fim, ParameterDirection.Input);

                OracleParameter compId = new OracleParameter();
                compId.OracleDbType = OracleDbType.Varchar2;
                compId.Direction = ParameterDirection.Output;
                compId.Size = Int32.MaxValue;
                comm.Parameters.Add(compId);

                Int32 v_protocolo_id = 0;
                Int32 v_protocolo_item_id = 0;
                Int32 v_compromisso_id = 0;

                comm.ExecuteNonQuery();

                v_protocolo_id = Int32.Parse(princId.Value.ToString());
                v_protocolo_item_id = Int32.Parse(itemId.Value.ToString());

                try
                {
                    v_compromisso_id = Int32.Parse(compId.Value.ToString());
                }
                catch { }

                result.success = true;
                result.message = "OK";
                result.protocolo_id = v_protocolo_id;
                result.protocolo_item_id = v_protocolo_item_id;
                result.agendamento_id = v_compromisso_id;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();
            }

            return result;
        }

        /// <summary>
        /// Salva a tratativa do cliente no protocolo
        /// </summary>
        /// <param name="infoTratativa"></param>
        /// <returns>Status do salvamento</returns>
        public ResultSacProtocolosItemSalvar SacProtocolosItemSalvar(InfoSacProtocoloIntemSalvar infoTratativa)
        {

            var result = new ResultSacProtocolosItemSalvar();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_salva_protocolos_item_sac";

                comm.Parameters.Add("p_agente_id", OracleDbType.Varchar2, infoTratativa.p_agente_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_protocolo_id", OracleDbType.Varchar2, infoTratativa.p_protocolo_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_meio_comunicacao_id", OracleDbType.Varchar2, infoTratativa.p_meio_comunicacao_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_departamento_id", OracleDbType.Varchar2, infoTratativa.p_departamento_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_assunto_id", OracleDbType.Varchar2, infoTratativa.p_assunto_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_comentarios", OracleDbType.Varchar2, infoTratativa.p_tratativa, ParameterDirection.Input);

                OracleParameter itemId = new OracleParameter();
                itemId.OracleDbType = OracleDbType.Varchar2;
                itemId.Direction = ParameterDirection.Output;
                itemId.Size = Int32.MaxValue;
                comm.Parameters.Add(itemId);

                Int32 v_protocolo_item_id = 0;

                comm.ExecuteNonQuery();

                v_protocolo_item_id = Int32.Parse(itemId.Value.ToString());

                result.success = true;
                result.message = "OK";
                result.protocolo_item_id = v_protocolo_item_id;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();
            }

            return result;
        }

        /// <summary>
        /// Conclui o protocolo pelo cliente
        /// </summary>
        /// <param name="p_agente_id"></param>
        /// <param name="p_protocolo_id"></param>
        /// <param name="p_protocolo_item_id"></param>
        /// <returns>Status da conclusão do protocolo</returns>
        public ResultSacProtocolosItemSalvar SacProtocolosItemConcluir(Int32 p_agente_id, Int32 p_protocolo_id, Int32 p_protocolo_item_id)
        {

            var result = new ResultSacProtocolosItemSalvar();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_concluir_protocolos_sac";
                comm.Parameters.Add("p_agente_id", OracleDbType.Varchar2, p_agente_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_protocolo_id", OracleDbType.Varchar2, p_protocolo_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_protocolo_item_id", OracleDbType.Varchar2, p_protocolo_item_id.ToString(), ParameterDirection.Input);

                comm.ExecuteNonQuery();

                result.success = true;
                result.message = "OK";
                result.protocolo_item_id = p_protocolo_item_id;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();
            }

            return result;
        }

        /// <summary>
        /// Salva o arquivo anexo na tratativa do cliente
        /// </summary>
        /// <param name="anexoInfo"></param>
        /// <returns>Status do salvamento</returns>
        public ResultInfoDocumentoUpload UploadAnexoProtocoloItem(InfoSacProtocoloItemAnexosSalvar anexoInfo)
        {


            var result = new ResultInfoDocumentoUpload();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                var v_tipo_arquivo = Util.Formatacoes.GetContentType(anexoInfo.p_arquivo_bin.FileName);
                var v_documento_id = 0;
                var v_data_upload = "";

                conn = oraDb.AbreConexao();

                var comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_salva_anexo_protocolos_item_sac";

                comm.Parameters.Add("p_protocolo_item_id", OracleDbType.Varchar2, anexoInfo.p_protocolo_item_id, ParameterDirection.Input);
                comm.Parameters.Add("p_nome_arquivo", OracleDbType.Varchar2, anexoInfo.p_arquivo_bin.FileName, ParameterDirection.Input);
                comm.Parameters.Add("p_titulo", OracleDbType.Varchar2, anexoInfo.p_titulo, ParameterDirection.Input);
                comm.Parameters.Add("p_descricao", OracleDbType.Varchar2, anexoInfo.p_descricao, ParameterDirection.Input);

                byte[] fileBytes = null; ;

                if (anexoInfo.p_arquivo_bin.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        anexoInfo.p_arquivo_bin.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                }

                OracleParameter docBin = new OracleParameter();
                docBin.ParameterName = "p_arquivo_bin";
                docBin.OracleDbType = OracleDbType.Blob;
                docBin.Direction = ParameterDirection.Input;
                docBin.Value = fileBytes;
                comm.Parameters.Add(docBin);

                OracleParameter docId = new OracleParameter();
                docId.ParameterName = "p_anexo_id";
                docId.OracleDbType = OracleDbType.Varchar2;
                docId.Direction = ParameterDirection.Output;
                docId.Size = Int32.MaxValue;
                comm.Parameters.Add(docId);

                OracleParameter docDataUpload = new OracleParameter();
                docDataUpload.ParameterName = "p_data_upload";
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
                result.documento_nome = anexoInfo.p_arquivo_bin.FileName;
                result.documento_titulo = anexoInfo.p_titulo;
                result.documento_size = fileBytes.Length;
                result.documento_app_type = v_tipo_arquivo;
                result.documento_desc = anexoInfo.p_descricao;
                result.data_upload = v_data_upload;

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
        /// Lista datas disponíveis para o agendamento 
        /// </summary>
        /// <param name="p_cliente_id"></param>
        /// <param name="p_empreendimento_id"></param>
        /// <param name="p_unidade_id"></param>
        /// <returns>Datas disponiveis</returns>
        public ResultSacDatasDisponiveis SacAgendaDatasDisponivel(Int32 p_cliente_id, Int32 p_empreendimento_id, Int32 p_unidade_id)
        {

            var result = new ResultSacDatasDisponiveis();
            var list = new List<InfoSacDatasDisponiveis>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_data_disponivel_sac";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_empreendimento_id", OracleDbType.Varchar2, p_empreendimento_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_cliente_id", OracleDbType.Varchar2, p_cliente_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_unidade_id", OracleDbType.Varchar2, p_unidade_id.ToString(), ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {


                    while (dr.Read())
                    {

                        var info = new InfoSacDatasDisponiveis();
                        
                        info.data_ini = dr["data_disp_ini"].ToString();
                        info.data_fim = dr["data_disp_fim"].ToString();
                        info.descricao = dr["data_descricao"].ToString();

                        list.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.result = list;

                }
                else
                {

                    result.success = true;
                    result.message = "Sem datas disponíveis";
                    result.result = list;
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Lista os compromissos agendados
        /// </summary>
        /// <param name="p_cliente_id"></param>
        /// <param name="p_empreendimento_id"></param>
        /// <param name="p_unidade_id"></param>
        /// <returns>Lista de agendamentos</returns>
        public ResultSacCompromissosAgendados SacListaCompromissosAgendado(Int32 p_cliente_id, Int32 p_empreendimento_id, Int32 p_unidade_id)
        {

            var result = new ResultSacCompromissosAgendados();
            var list = new List<InfoSacCompromissosAgendado>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_compromissos_agendado_sac";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_empreendimento_id", OracleDbType.Varchar2, p_empreendimento_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_cliente_id", OracleDbType.Varchar2, p_cliente_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_unidade_id", OracleDbType.Varchar2, p_unidade_id.ToString(), ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {


                    while (dr.Read())
                    {


                        var info = new InfoSacCompromissosAgendado();

                        info.compromisso_id = Int32.Parse(dr["cod_compromisso"].ToString());
                        info.protocolo_id = Int32.Parse(dr["seq_protocolo"].ToString());
                        info.data_ini = dr["data_ini"].ToString();
                        info.data_fim = dr["data_fim"].ToString();
                        info.status = dr["status"].ToString();
                        info.status_descricao = dr["status_desc"].ToString();

                        list.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.cliente_id = p_cliente_id;
                    result.empreendimento_id = p_empreendimento_id;
                    result.unidade_id = p_unidade_id;
                    result.result = list;

                }
                else
                {
                    result.success = true;
                    result.message = "Sem registros";
                    result.cliente_id = p_cliente_id;
                    result.empreendimento_id = p_empreendimento_id;
                    result.unidade_id = p_unidade_id;
                    result.result = list;
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Reagenda um nova data para visita técnica
        /// </summary>
        /// <param name="p_agente_id"></param>
        /// <param name="p_agendamento_id"></param>
        /// <param name="p_data_ini"></param>
        /// <param name="p_data_fim"></param>
        /// <returns>Informaçõs do agendamento</returns>
        public ResultSacReagendarCompromisso SacProtocolosReagendarComprmisso(Int32 p_agente_id, Int32 p_agendamento_id, string p_data_ini, string p_data_fim)
        {

            var result = new ResultSacReagendarCompromisso();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_reagendar_compromisso_sac";
                comm.Parameters.Add("p_agente_id", OracleDbType.Varchar2, p_agente_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_agendamento_id", OracleDbType.Varchar2, p_agendamento_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_data_ini", OracleDbType.Varchar2, p_data_ini, ParameterDirection.Input);
                comm.Parameters.Add("p_data_fim", OracleDbType.Varchar2, p_data_fim, ParameterDirection.Input);

                OracleParameter compId = new OracleParameter();
                compId.ParameterName = "p_agendamento_new_id";
                compId.OracleDbType = OracleDbType.Varchar2;
                compId.Direction = ParameterDirection.Output;
                compId.Size = Int32.MaxValue;
                comm.Parameters.Add(compId);

                var v_agendamento_id = 0;

                comm.ExecuteNonQuery();

                try
                {
                    v_agendamento_id = Int32.Parse(compId.Value.ToString());
                }
                catch { }

                result.success = true;
                result.message = "OK";
                result.agendamento_id = v_agendamento_id;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();
            }

            return result;
        }

        /// <summary>
        /// Cancela um determinado agendamento de compromisso
        /// </summary>
        /// <param name="p_agendamento_id"></param>
        /// <returns>Status do cancelamento</returns>
        public ResultSacCancelarCompromisso SacProtocolosCancelarComprmisso(Int32 p_agendamento_id)
        {

            var result = new ResultSacCancelarCompromisso();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_cancelar_compromisso_sac";
                comm.Parameters.Add("p_agendamento_id", OracleDbType.Varchar2, p_agendamento_id.ToString(), ParameterDirection.Input);
                
                comm.ExecuteNonQuery();

                result.success = true;
                result.message = "OK";

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();
            }

            return result;
        }

        /// <summary>
        /// Alerta o APP sobre alterações no protocolo do cliente
        /// </summary>
        /// <param name="alertInfo"></param>
        /// <returns>Status da integração</returns>
        public ResultSacAletaApp SacInfoToApp(InfoSacAlertaApp alertInfo)
        {

            var result = new ResultSacAletaApp();

            //OracleConnection conn;

            //ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {
                
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(alertInfo, Formatting.Indented);
                
                File.WriteAllText("C:\\Temp\\Alerta_" + alertInfo.protocolId + ".json", jsonString);

                if (String.IsNullOrEmpty(jsonString))
                    throw new Exception("JSON vazio ou nulo");

                var http = (HttpWebRequest)WebRequest.Create(new Uri("https://portal.mitreexperience.com.br/api/integration/user/sac/notify/user"));
                http.Accept = "*/*";
                http.ContentType = "application/json";
                http.Method = "POST";
                http.Headers.Add("Authorization", "FLsZw6y*50iARhUvrHlo");
                http.UseDefaultCredentials = true;
                http.Credentials = System.Net.CredentialCache.DefaultCredentials;
                http.ContentLength = jsonString.Length;

                //File.WriteAllText("C:\\Temp\\Alerta_" + alertInfo.protocolId + ".json", jsonString);

                //var culture = new CultureInfo("pt-BR");

                //var encoding = Encoding.GetEncoding("iso-8859-1");

                ASCIIEncoding encoding = new ASCIIEncoding();
                //UTF8Encoding encoding = new UTF8Encoding();
                //UnicodeEncoding encoding = new UnicodeEncoding();
                //UTF32Encoding encoding = new UTF32Encoding();

                Byte[] bytes = encoding.GetBytes(jsonString);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();

                File.WriteAllText("C:\\Temp\\Alerta2_" + alertInfo.protocolId + ".json", "Respondeu");

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);

                result.success = true;
                result.message = "OK";
                result.statusIntegracao = sr.ReadToEnd();

                var alerta = new InfoSacAlertaApp();

                alerta.cpf_cnpj = alertInfo.cpf_cnpj;
                alerta.title = alertInfo.title;
                alerta.body = alertInfo.body;
                alerta.protocolId = alertInfo.protocolId;
                alerta.contractId = alertInfo.contractId;

                result.alerta = alerta;

                System.IO.File.WriteAllText("C:\\Temp\\jsonInfoResp.txt", sr.ReadToEnd());

                //conn = oraDb.AbreConexao();

                //OracleCommand comm = new OracleCommand();

                //comm.Connection = conn;
                //comm.CommandType = CommandType.StoredProcedure;
                //comm.CommandText = "dwmitre.pck_app_charbot.sp_cancelar_compromisso_sac";
                //comm.Parameters.Add("p_agendamento_id", OracleDbType.Varchar2, p_agendamento_id.ToString(), ParameterDirection.Input);

                //comm.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                File.WriteAllText("C:\\Temp\\Alerta3_" + alertInfo.protocolId + ".json", e.Message + "\n" + e.StackTrace);
                throw new Exception(e.Message);
            }
            finally
            {
                //if (oraDb != null)
                //    oraDb.FechaConexao();
            }

            return result;
        }

    }
}
