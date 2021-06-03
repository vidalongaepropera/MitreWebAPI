//using BoletoNetCore;
//using BoletoNetCore.Pdf.BoletoImpressao;
//using BoletoNetCore;
using BoletoNetCore;
using BoletoNetCore.Pdf.BoletoImpressao;
using CrystalReportsServiceReference;
using HumanAPIClient.Model;
using HumanAPIClient.Service;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using MitreWebAPI.Model.ChatBot;
using MitreWebAPI.Util;
using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

namespace MitreWebAPI.Controllers.ChatBot
{
    /// <summary>
    /// Trata assuntos relacionando ao Chat Bot
    /// Contrele de Acesso por Login do usuário por cpf ou cnpj e senha
    /// Control de Acesso por Token temporário
    /// Download Extratos
    /// Download Boletos
    /// Download Demonstrativo de IR
    /// Listagem Extratos
    /// Listagem Boletos
    /// Listagen Demonstrativo de IR
    /// Listagem de Dados das Unidades
    /// Informações de Faq do SAC
    /// </summary>
    public class ChatBotView
    {
        /// <summary>
        /// Mantem a informação de que se a senha de acesso é a temporária.
        /// </summary>
        public bool ACESSO_COM_SENHA_TMP = false;

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly string _url_linha_digitavel_boleto;
        private BoletoNetCore.IBanco _banco;
        private readonly string _token;
        private bool _isTokenValido = false;
        private readonly string _url_download_boleto;
        private readonly string _url_wcf_crystalreports;
        
        /// <summary>
        /// Guarda as configuraçõpes da aplicação devalida o token da aplicação
        /// </summary>
        /// <param name="Configuration"></param>
        /// <param name="env"></param>
        /// <param name="p_token_app"></param>
        public ChatBotView(IConfiguration Configuration, IWebHostEnvironment env, string p_token_app)
        {
            _configuration = Configuration;
            _environment = env;
            _url_linha_digitavel_boleto = _configuration.GetSection("ConfigAPIChatBot").GetSection("url_linha_digitavel_boleto").Value;
            _url_download_boleto = _configuration.GetSection("ConfigAPIChatBot").GetSection("url_download_boleto").Value;
            _url_wcf_crystalreports = _configuration.GetSection("ConfigAPIChatBot").GetSection("url_wcf_crystalreports").Value;
            _token = _configuration.GetSection("ConfigAPIChatBot").GetSection("Token").Value;

            if (p_token_app != null && !p_token_app.Equals(_token))
                throw new Exception("Token da aplicação inválida!");
            else
                _isTokenValido = true;

        }

        /// <summary>
        /// Valida se o cpf ou cnpj enviado está na base de clientes.
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns>Retorna informações de validação do cliente</returns>
        public ResultValidaCliente ValidaClienteExistente(string p_cpf_cnpj)
        {

            var result = new ResultValidaCliente();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                var comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_valida_cliente";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                result.success = true;
                result.message = "OK";
                result.cliente_existe = (dr != null && dr.HasRows);

            }
            catch
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }
        /// <summary>
        /// Retorna as informações do contrto do cliente, como empreendimento, boco, unidade, status do contrato e etc
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_senha"></param>
        /// <param name="p_e_senha_tmp"></param>
        /// <returns>Retorna os dados do contrato do cliente</returns>
        public ResultContratos ContratosCliente(string p_cpf_cnpj, string p_senha, string p_e_senha_tmp)
        {

            var lstContratos = new List<InfoContratos>();
            var contratosRet = new ResultContratos();
            string v_mensagem; ;
            Int32 v_cliente_id = 0;
            string v_nome_cliente = "";
            string v_tipo = "";
            string v_email = "";
            string v_tel_cel = "";
            string v_tel_res = "";
            string v_tel_com = "";
            bool v_aceita_sms = true;
            bool v_aceita_email = true;

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                var comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_contratos";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_senha", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_senha, ParameterDirection.Input);
                comm.Parameters.Add("p_e_senha_tmp", OracleDbType.Varchar2, p_e_senha_tmp, ParameterDirection.Input);
                
                var p_mensagem = new OracleParameter();
                p_mensagem.OracleDbType = OracleDbType.Varchar2;
                p_mensagem.Direction = ParameterDirection.Output;
                p_mensagem.Size = 200;
                comm.Parameters.Add(p_mensagem);

                OracleParameter p_tipo = new OracleParameter();
                p_tipo.OracleDbType = OracleDbType.Varchar2;
                p_tipo.Direction = ParameterDirection.Output;
                p_tipo.Size = 2;
                comm.Parameters.Add(p_tipo);

                OracleDataReader dr = comm.ExecuteReader();

                v_mensagem = p_mensagem.Value.ToString();
                v_tipo = p_tipo.Value.ToString();

                if (dr != null && dr.HasRows && _isTokenValido)
                {


                    while (dr.Read())
                    {

                        InfoContratos infoContrato = new InfoContratos();
                        infoContrato.numero_organizacao = Int32.Parse(dr["numero_organizacao"].ToString());
                        infoContrato.numero_filial = Int32.Parse(dr["numero_filial"].ToString());
                        infoContrato.numero_contrato = Int32.Parse(dr["contrato"].ToString());
                        v_cliente_id = Int32.Parse(dr["cliente_id"].ToString());
                        v_nome_cliente = dr["cliente"].ToString();
                        v_email = dr["email"].ToString();
                        v_tel_cel = dr["tel_cel"].ToString();
                        v_tel_res = dr["tel_res"].ToString();
                        v_tel_com = dr["tel_com"].ToString();
                        v_aceita_email = (dr["aceita_email"].ToString().Equals("1")) ? true : false;
                        v_aceita_sms = (dr["aceita_sms"].ToString().Equals("1")) ? true : false;
                        infoContrato.numero_empreendimento = Int32.Parse(dr["numero_empreendimento"].ToString());
                        infoContrato.empreendimento = dr["empreendimento"].ToString();
                        infoContrato.numero_bloco = Int32.Parse(dr["numero_bloco"].ToString());
                        infoContrato.bloco = dr["bloco"].ToString();
                        infoContrato.numero_unidade = Int32.Parse(dr["numero_unidade"].ToString());
                        infoContrato.unidade = dr["unidade"].ToString();
                        infoContrato.numero_tipologia = Int32.Parse(dr["numero_tipologia"].ToString());
                        infoContrato.tipologia = dr["tipologia"].ToString();
                        infoContrato.status_contrato = dr["status_contrato"].ToString();
                        infoContrato.numero_empreendimento_sac = Int32.Parse(dr["numero_empreendimento_sac"].ToString());
                        infoContrato.numero_bloco_sac = Int32.Parse(dr["numero_bloco_sac"].ToString());
                        infoContrato.numero_unidade_sac = Int32.Parse(dr["numero_unidade_sac"].ToString());
                        infoContrato.agente_sac_1 = Int32.Parse(dr["numero_atendente_sac_1"].ToString());
                        infoContrato.agente_sac_2 = Int32.Parse(dr["numero_atendente_sac_2"].ToString());
                        infoContrato.agente_sac_3 = Int32.Parse(dr["numero_atendente_sac_3"].ToString());

                        lstContratos.Add(infoContrato);

                    }

                    contratosRet.success = true;
                    contratosRet.message = "OK";
                    contratosRet.tipo = "OK";
                    contratosRet.cliente_id = v_cliente_id;
                    contratosRet.nome_cliente = v_nome_cliente;
                    contratosRet.email = v_email;
                    contratosRet.tel_celular = v_tel_cel;
                    contratosRet.tel_residencial = v_tel_res;
                    contratosRet.tel_comercial = v_tel_com;
                    contratosRet.aceita_receber_email = v_aceita_email;
                    contratosRet.aceita_receber_sms = v_aceita_sms;
                    contratosRet.results = lstContratos;

                }
                else
                {

                    if (_isTokenValido != true)
                    {
                        contratosRet.success = false;
                        contratosRet.message = "Token invalido!";
                        contratosRet.tipo = "TI";
                        contratosRet.nome_cliente = v_nome_cliente;
                    }
                    else if (_isTokenValido == true && dr != null && !dr.HasRows)
                    {
                        contratosRet.success = false;
                        contratosRet.message = v_mensagem;
                        contratosRet.tipo = v_tipo;
                        contratosRet.nome_cliente = v_nome_cliente;
                    }

                }

            }
            catch
            {
                contratosRet.success = false;
                contratosRet.message = "Sistema indisponível, tente novamente mais tarde.";
                contratosRet.nome_cliente = v_nome_cliente;
                contratosRet.tipo = "DI";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return contratosRet;
        }

        /// <summary>
        /// Lista as tipologias utiiadas para cadastr unidades
        /// </summary>
        /// <returns>Lista com ID e nome da tipologia</returns>
        public ResultInfoTipologiaUnidade ListaTipologiaUnidades()
        {

            var result = new ResultInfoTipologiaUnidade();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_tipologia_unidades";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    var infos = new List<InfoTipologiaUnidade>();

                    while (dr.Read())
                    {

                        var info = new InfoTipologiaUnidade();

                        info.numero_tipologia = Int32.Parse(dr["tpu_in_codigo"].ToString());
                        info.tipologia = dr["tpu_st_descricao"].ToString();
                        
                        infos.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.results = infos;

                }
                else
                {

                    result.success = false;
                    result.message = "Sem dados de tipologia.";
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
        /// Solicita a troca de senha
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_reenvio"></param>
        /// <returns>Dados da efitavção da solicitação de troca de senha</returns>
        public ResultSolicitaTrocaSenha SolicitaTrocaSenha(string p_cpf_cnpj, string p_reenvio)
        {

            var result = new ResultSolicitaTrocaSenha();
            SenderEmail senderMail = null;

            var v_email = "";
            var v_telefone_cel = "";
            var v_cliente = "";
            var v_codigo = 0;

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_solicita_troca_senha";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_reenvio", OracleDbType.Varchar2, p_reenvio, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows && _isTokenValido)
                {

                    string pattern = @"(?<=[\w]{2})[\w-\._\+%]*(?=[\w]{2}@)";

                    while (dr.Read())
                    {
                        result.success = true;

                        v_cliente = dr["cliente"].ToString();
                        v_email = dr["email"].ToString();
                        v_telefone_cel = dr["telefone_cel"].ToString();
                        v_codigo = Int32.Parse(dr["codigo"].ToString());

                        var tel_cel_ret = "*****" + v_telefone_cel.Substring(Math.Max(0, v_telefone_cel.Length - 5)); ;
                        result.codigo = v_codigo;
                        result.message = "Foi enviado instrução para troca de senha para o e-mail cadastrado: " + Regex.Replace(v_email.Trim(), pattern, m => new string('*', m.Length))
                            + " e SMS para: " + tel_cel_ret;
                        result.expiracao = dr["expiracao"].ToString();


                    }

                    //Envia email para o cliente
                    List<string> emails = new List<string>();
                    emails.Add(v_email);

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("<div style='font - family: \''Trebuchet MS\'', Verdana, sans - serif;'>");
                    sb.AppendLine("<span style='font-size:30px;font-weight:bold;'>Olá&nbsp;" + v_cliente + ",</span><br><br>");
                    sb.AppendLine("<span>Por favor, use o seguinte código para concluir a verificação:</span><br><br>");
                    sb.AppendLine("<span style='font-size:30px;font-weight:bold;'>" + v_codigo + "</span><br><br>");
                    sb.AppendLine("<span>Este código irá expirar em 10 minutos.</b></span><br><br>");
                    sb.AppendLine("<span>Digite este código no local solicitado para continuar o atendimento.</span><br><br>");
                    sb.AppendLine("<span>Se você não solicitou, ou não lhe foi solicitado um código de verificação, altere sua senha imediatamente acessando as configurações da sua conta. Também recomendamos alterar sua senha em outros sites fora da Mitre, caso a senha for a mesma.</span><br><br>");
                    sb.AppendLine("<span>Obrigado por visitar a Mitre</span><br><br>");
                    sb.AppendLine("<span>Por favor não responder esta mensagem.</span>");
                    sb.AppendLine("</div>");

                    senderMail = new SenderEmail(_configuration, _environment);

                    var status_email = senderMail.EmailTo(emails, "Atendimento Mitre", "Solicitação de troca de senha", sb.ToString());
                    var status_sms = SenderSMS.Enviar("Mitre", v_telefone_cel, "Por favor, use o seguinte codigo para concluir a verificacao: " + v_codigo + "\nEsse codigo ira expirar em 10 minutos.");

                    if (status_email != true && status_sms != true)
                    {
                        result.success = false;
                        result.message = "Não foi possível enviar o e-mail e SMS";
                        if (senderMail.EXCEPTION != null)
                        {
                            result.erros = senderMail.EXCEPTION.StackTrace;
                            throw new Exception(senderMail.EXCEPTION.Message, senderMail.EXCEPTION);
                        }
                    }

                }
                else
                {

                    if (_isTokenValido != true)
                    {
                        result.success = false;
                        result.message = "Token invalido!";
                    }
                    else if (_isTokenValido == true && dr != null && !dr.HasRows)
                    {
                        result.success = false;
                        result.message = "Sem registros";
                    }

                }

            }
            catch (Exception e)
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
                
                if (senderMail != null && senderMail.EXCEPTION != null)
                    throw new Exception(e.Message, senderMail.EXCEPTION);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Efetua a troca da senha do cliente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_codigo"></param>
        /// <param name="p_nova_senha"></param>
        /// <param name="p_atual_senha"></param>
        /// <returns>Retorna dados da troca de senha do cliente</returns>
        public ResultTrocarSenha TrocarSenha(string p_cpf_cnpj, Int32 p_codigo, string p_nova_senha, string p_atual_senha)
        {

            var result = new ResultTrocarSenha();
            SenderEmail senderMail = null;
            var v_cliente = "";
            var v_email = "";
            var v_telefone_cel = "";
            var v_aviso = "";

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_efetua_troca_senha";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_codigo", OracleDbType.Varchar2, p_codigo.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_nova_senha", OracleDbType.Varchar2, p_nova_senha, ParameterDirection.Input);
                comm.Parameters.Add("p_atual_senha", OracleDbType.Varchar2, p_atual_senha, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows && _isTokenValido)
                {


                    while (dr.Read())
                    {

                        v_aviso = dr["aviso"].ToString();

                        if (String.IsNullOrEmpty(v_aviso))
                        {

                            result.success = true;
                            result.message = "OK";
                            
                            v_cliente = dr["cliente"].ToString();
                            v_email = dr["email"].ToString();
                            v_telefone_cel = dr["telefone_cel"].ToString();

                        }
                        else
                        {

                            result.success = false;
                            result.message = v_aviso;
                            v_cliente = dr["cliente"].ToString();
                            v_email = dr["email"].ToString();
                            v_telefone_cel = dr["telefone_cel"].ToString();

                        }

                    }

                    if (String.IsNullOrEmpty(v_aviso))
                    {

                        //Envia email para o cliente
                        List<string> emails = new List<string>();
                        emails.Add(v_email);
                        //emails.Add("irenilson.batista@mitrerealty.com.br");

                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine("<div style='font - family: \''Trebuchet MS\'', Verdana, sans - serif;'>");
                        sb.AppendLine("<span style='font-size:30px;font-weight:bold;'>Prezado(a)&nbsp;" + v_cliente + ",</span><br><br>");
                        sb.AppendLine("<span>Troca de senha efetuada com sucesso!</span><br><br>");
                        sb.AppendLine("<span>Por favor não responder esta mensagem.</span>");
                        sb.AppendLine("</div>");

                        senderMail = new SenderEmail(_configuration, _environment);

                        var status_email = senderMail.EmailTo(emails, "Atendimento Mitre", "Alteração de senha efetivada", sb.ToString());
                        var status_sms = SenderSMS.Enviar("Mitre", v_telefone_cel, "Senha alterada com sucesso!");

                        if (status_email != true && status_sms != true)
                        {
                            result.success = false;
                            result.message = "Não foi possível enviar o e-mail e SMS";
                            if (senderMail.EXCEPTION != null)
                            {
                                result.erros = senderMail.EXCEPTION.StackTrace;
                                throw new Exception(senderMail.EXCEPTION.Message, senderMail.EXCEPTION);
                            }
                        }

                    }

                }
                else
                {

                    if (_isTokenValido != true)
                    {
                        result.success = false;
                        result.message = "Token invalido!";
                    }
                    else if (_isTokenValido == true && dr != null && !dr.HasRows)
                    {
                        result.success = false;
                        result.message = "Sem registros";
                    }

                }

            }
            catch (Exception e)
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
                result.erros = e.Message;

                if (senderMail != null && senderMail.EXCEPTION != null)
                    throw new Exception(e.Message, senderMail.EXCEPTION);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Efetua a alteração de dados cadastrais do cliente
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Informações de sucesso ou erro na troca</returns>
        public ResultAlteraDadosCadastral AlterarDadoCadastral(InfoAlteraDadosCadastral info)
        {

            var result = new ResultAlteraDadosCadastral();
            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_efetua_alteracao_dados";

                comm.Parameters.Add("p_cliente_id", OracleDbType.Varchar2, info.cliente_id.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_email", OracleDbType.Varchar2, info.email, ParameterDirection.Input);
                comm.Parameters.Add("p_tel_celular", OracleDbType.Varchar2, info.tel_celular, ParameterDirection.Input);
                comm.Parameters.Add("p_tel_residencial", OracleDbType.Varchar2, info.tel_residencial, ParameterDirection.Input);
                comm.Parameters.Add("p_tel_comercial", OracleDbType.Varchar2, info.tel_comercial, ParameterDirection.Input);
                comm.Parameters.Add("p_aceita_receber_email", OracleDbType.Varchar2, info.aceita_receber_email.ToString(), ParameterDirection.Input);
                comm.Parameters.Add("p_aceita_receber_sms", OracleDbType.Varchar2, info.aceita_receber_sms.ToString(), ParameterDirection.Input);

                comm.ExecuteNonQuery();

                result.success = true;
                result.message = "OK";

            }
            catch (Exception e)
            {
                result.success = false;
                result.message = "ERRO";
                throw new Exception(e.Message + "\n" + e.StackTrace);

            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Informamações basica do cadastro do clienta
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns>Dados de cpf_cnpj, id, telefones e aceite de comunicação por sms e e-mail</returns>
        public ResultConsultaDadosCadastral ConsultarDadoCadastral(string p_cpf_cnpj)
        {

            var result = new ResultConsultaDadosCadastral();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_dados_cadastrais";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    var info = new InfoAlteraDadosCadastral();

                    while (dr.Read())
                    {
                        

                        info.cliente_id = Int32.Parse(dr["cliente_id"].ToString());
                        info.cpf_cnpj = dr["cpf_cnpj"].ToString();
                        info.email = dr["email"].ToString();
                        info.tel_celular = dr["fonecel"].ToString();
                        info.tel_residencial = dr["foneres"].ToString();
                        info.tel_comercial = dr["fonecom"].ToString();
                        info.aceita_receber_email = (dr["aceita_receber_email"].ToString().Equals("1")) ? true : false;
                        info.aceita_receber_sms = (dr["aceita_receber_sms"].ToString().Equals("1")) ? true : false;

                    }

                    result.success = true;
                    result.message = "OK";
                    result.results = info;

                }
                else
                {

                    result.success = false;
                    result.message = "Sem dados cadastrais para o CPF ou CNPJ: " + p_cpf_cnpj + ".";
                }

            }
            catch
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }
        /// <summary>
        /// Gera um código para o acesso temporário do cliente.
        /// p_reenvio: pode ser S ou N
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_reenvio"></param>
        /// <returns>Retorna o e-mail cadastrato com a informação do código gerado</returns>
        public ResultSolicitaTokenTemp GeraTokenTemporario(string p_cpf_cnpj, string p_reenvio)
        {

            var result = new ResultSolicitaTokenTemp();
            SenderEmail senderMail = null;

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_gera_token_temporario";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_reenvio", OracleDbType.Varchar2, p_reenvio, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows && _isTokenValido)
                {

                    string pattern = @"(?<=[\w]{2})[\w-\._\+%]*(?=[\w]{2}@)";

                    while (dr.Read())
                    {
                        result.success = true;

                        var tel_cel = dr["telefone_cel"].ToString();
                        var tel_cel_ret = "*****" + tel_cel.Substring(Math.Max(0, tel_cel.Length - 5)); ;

                        result.message = "Foi enviado a senha temporária de acesso para o e-mail cadastrado: " + Regex.Replace(dr["email"].ToString().Trim(), pattern, m => new string('*', m.Length)) 
                            + " e SMS para: " + tel_cel_ret;
                        result.tipo = dr["tipo"].ToString();
                        result.token = dr["token"].ToString();
                        result.expiracao = dr["expiracao"].ToString();
                        result.cliente = dr["cliente"].ToString();
                        result.email = dr["email"].ToString();
                        result.telefone_cel = tel_cel;
                    }
                    

                    //Envia email para o cliente
                    List<string> emails = new List<string>();
                    emails.Add(result.email);

                    //emails.Add("danilo.bonfim@mitrerealty.com.br");
                    //emails.Add("nanci.gazola@mitrerealty.com.br");

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("<div style='font - family: \''Trebuchet MS\'', Verdana, sans - serif;'>");
                    sb.AppendLine("<span style='font-size:30px;font-weight:bold;'>Olá&nbsp;" + result.cliente + ",</span><br><br>");
                    sb.AppendLine("<span>Por favor, use o seguinte código para concluir o acesso:</span><br><br>");
                    sb.AppendLine("<span style='font-size:30px;font-weight:bold;'>" + result.token + "</span><br><br>");
                    sb.AppendLine("<span>Este código irá expirar em 1 hora.</b></span><br><br>");
                    sb.AppendLine("<span>Digite este código no local solicitado para continuar o atendimento.</span><br><br>");
                    sb.AppendLine("<span>Se você não solicitou, ou não lhe foi solicitado um código de verificação, altere sua senha imediatamente acessando as configurações da sua conta. Também recomendamos alterar sua senha em outros sites fora da Mitre, caso a senha for a mesma.</span><br><br>");
                    sb.AppendLine("<span>Obrigado por visitar a Mitre</span><br><br>");
                    sb.AppendLine("<span>Por favor não responder esta mensagem.</span>");
                    sb.AppendLine("</div>");

                    senderMail = new SenderEmail(_configuration, _environment);

                    var status_email = senderMail.EmailTo(emails, "Atendimento Mitre", "Envio de código temporário para acesso", sb.ToString());
                    var status_sms = SenderSMS.Enviar("Mitre", result.telefone_cel, "Por favor, use o seguinte codigo para concluir o acesso: " + result.token + "\nEsse codigo ira expirar em 10 minutos.");

                    if (status_email != true && status_sms != true)
                    {
                        result.success = false;
                        result.message = "Não foi possível enviar o e-mail e SMS";
                        result.tipo = "EI";
                        if (senderMail.EXCEPTION != null)
                        {
                            result.erros = senderMail.EXCEPTION.StackTrace;
                            throw new Exception(senderMail.EXCEPTION.Message, senderMail.EXCEPTION);
                        }
                    }

                }
                else
                {

                    if (_isTokenValido != true)
                    {
                        result.success = false;
                        result.message = "Token invalido!";
                        result.tipo = "TI";
                    }
                    else if (_isTokenValido == true && dr != null && !dr.HasRows)
                    {
                        result.success = false;
                        result.message = "Sem registros";
                        result.tipo = "EI";
                    }

                }

            }
            catch (Exception e)
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
                result.tipo = "ER";

                if (senderMail != null && senderMail.EXCEPTION != null)
                    throw new Exception(e.Message, senderMail.EXCEPTION);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Lista todos os demonstrativos de IR, mais informações da unidade do cliente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns>Lista as unidades e ano que contem disponível o informe do IR</returns>
        public ResultDemoIR TodosDemoIR(string p_cpf_cnpj)
        {

            var resultDemoIR = new ResultDemoIR();
            var lstDemoIR = new List<InfoDemoIR>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_todos_informe_ir";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var infoDemoIR = new InfoDemoIR();

                        infoDemoIR.numero_organizacao = Int32.Parse(dr["numero_organizacao"].ToString());
                        infoDemoIR.numero_filial = Int32.Parse(dr["numero_filial"].ToString());
                        infoDemoIR.numero_contrato = Int32.Parse(dr["numero_contrato"].ToString());
                        infoDemoIR.empreendimento = dr["empreendimento"].ToString();
                        infoDemoIR.bloco = dr["bloco"].ToString();
                        infoDemoIR.unidade = dr["unidade"].ToString();
                        infoDemoIR.ano_do_informe = Int32.Parse(dr["ano_base"].ToString());

                        lstDemoIR.Add(infoDemoIR);

                    }

                    resultDemoIR.success = true;
                    resultDemoIR.message = "OK";
                    resultDemoIR.results = lstDemoIR;

                }
                else
                {

                    resultDemoIR.success = false;
                    resultDemoIR.message = "Sem Informe de Rendimentos para o CPF ou CNPJ: " + p_cpf_cnpj + ".";
                }

            }
            catch
            {
                resultDemoIR.success = false;
                resultDemoIR.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return resultDemoIR;
        }

        /// <summary>
        /// Lista todos os extratos de pagamentos, mais informações da unidade do cliente
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns>Lista as unidades com informações dos extratos de pagamentos disponíveis</returns>
        public ResultExtratos TodosExtratos(string p_cpf_cnpj)
        {

            var resultExtratos = new ResultExtratos();
            var lstExtratos = new List<InfoExtratos>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_todos_extratos";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var infoExtratos = new InfoExtratos();

                        infoExtratos.numero_organizacao = Int32.Parse(dr["numero_organizacao"].ToString());
                        infoExtratos.numero_filial = Int32.Parse(dr["numero_filial"].ToString());
                        infoExtratos.numero_contrato = Int32.Parse(dr["numero_contrato"].ToString());
                        infoExtratos.empreendimento = dr["empreendimento"].ToString();
                        infoExtratos.bloco = dr["bloco"].ToString();
                        infoExtratos.unidade = dr["unidade"].ToString();

                        lstExtratos.Add(infoExtratos);

                    }

                    resultExtratos.success = true;
                    resultExtratos.message = "OK";
                    resultExtratos.results = lstExtratos;

                }
                else
                {

                    resultExtratos.success = false;
                    resultExtratos.message = "Sem unidades para o CPF ou CNPJ: " + p_cpf_cnpj + ".";
                }

            }
            catch
            {
                resultExtratos.success = false;
                resultExtratos.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return resultExtratos;
        }

        /// <summary>
        /// Lista todos os boletos bancario, mais informações da unidade do cliente e linha digitável do boleto
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <returns>Lista as unidades com informações dos boletos bancários disponíveis</returns>
        public ResultBoletos TodosBoletos(string p_cpf_cnpj)
        {

            var resultBoletos = new ResultBoletos();
            var lstBoletos = new List<InfoBoletos>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_todos_boletos";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var infoBoleto = new InfoBoletos();

                        infoBoleto.numero_contrato = Int32.Parse(dr["numero_contrato"].ToString());
                        infoBoleto.nome_empreendimento = dr["nome_empreendimento"].ToString();
                        infoBoleto.nome_bloco = dr["nome_bloco"].ToString();
                        infoBoleto.nome_unidade = dr["nome_unidade"].ToString();
                        infoBoleto.cedente_cpf_cnpj = dr["cpf_cnpj_cedente"].ToString();
                        infoBoleto.cedente = dr["empresa_cedente"].ToString();
                        infoBoleto.banco_numero = Int32.Parse(dr["numero_banco"].ToString());
                        infoBoleto.banco = dr["nome_banco"].ToString();
                        infoBoleto.numero_parcela = Int32.Parse(dr["numero_parcela"].ToString());
                        infoBoleto.vencimento = dr["data_vencimento"].ToString();
                        infoBoleto.valor = dr["valor"].ToString();

                        lstBoletos.Add(infoBoleto);

                        var remoteFileUrl = _url_linha_digitavel_boleto + infoBoleto.numero_parcela.ToString();

                        using (System.Net.WebClient wc = new System.Net.WebClient())
                        {
                            try
                            {
                                infoBoleto.linha_digitavel = wc.DownloadString(remoteFileUrl).Replace(".", "").Replace(" ", "");
                            }
                            catch
                            {
                                infoBoleto.linha_digitavel = "Não foi possível criar a Linha digitáve";
                            }
                        }

                    }

                    resultBoletos.success = true;
                    resultBoletos.message = "OK";
                    resultBoletos.results = lstBoletos;

                }
                else
                {

                    resultBoletos.success = false;
                    resultBoletos.message = "Sem boletos para o CPF ou CNPJ: " + p_cpf_cnpj + ".";
                }

            }
            catch
            {
                resultBoletos.success = false;
                resultBoletos.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return resultBoletos;
        }

        /// <summary>
        /// Lista informações dos boletos dispoíveis de um determinado contrato
        /// </summary>
        /// <param name="p_numero_contrato"></param>
        /// <returns>Lista dados de boletos de um determinado contrato</returns>
        public ResultBoletos Boletos(string p_numero_contrato)
        {

            var lstBoletos = new List<InfoBoletos>();
            var boletosRet = new ResultBoletos();

            Int32 v_numero_contrato;

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_boletos";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_numero_contrato", OracleDbType.Varchar2, p_numero_contrato, ParameterDirection.Input);
                comm.Parameters.Add("p_numero_parcela", OracleDbType.Varchar2, "0", ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        InfoBoletos infoBoletos = new InfoBoletos();

                        v_numero_contrato = Int32.Parse(dr["numero_contrato"].ToString());
                        infoBoletos.numero_parcela = Int32.Parse(dr["numero_parcela"].ToString());
                        infoBoletos.vencimento = dr["data_vencimento"].ToString();
                        infoBoletos.valor = dr["valor"].ToString();

                        lstBoletos.Add(infoBoletos);

                    }

                    boletosRet.success = true;
                    boletosRet.message = "OK";

                }
                else
                {

                    boletosRet.success = false;
                    boletosRet.message = "Sem boletos para o contrato: " + p_numero_contrato + ".";

                }

            }
            catch
            {
                boletosRet.success = false;
                boletosRet.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return boletosRet;
        }

        /// <summary>
        /// Lista de todos os produtos
        /// Opcionamente, pode passar um id, para retornar o nome
        /// </summary>
        /// <param name="id_empreendimento"></param>
        /// <returns>Lista com co id_produto e nome do produto (empreendimento)</returns>
        public ResultProdutos ListaProdutos(Int32? id_empreendimento)
        {

            var result = new ResultProdutos();
            var lstProdutos = new List<InfoProduto>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_produtos";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);

                if (id_empreendimento != null)
                    comm.Parameters.Add("p_numero_empreendimento", OracleDbType.Varchar2, id_empreendimento.Value.ToString(), ParameterDirection.Input);
                else
                    comm.Parameters.Add("p_numero_empreendimento", OracleDbType.Varchar2, "", ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    result.success = true;
                    result.message = "OK";

                    while (dr.Read())
                    {

                        var info = new InfoProduto();

                        info.id_empreendimento = Int32.Parse(dr["numero_empreendimento"].ToString());
                        info.empreendimento = dr["empreendimento"].ToString();

                        lstProdutos.Add(info);

                    }

                    result.results = lstProdutos;

                }
                else
                {

                    result.success = false;
                    result.message = "Sem dados.";
                }

            }
            catch(Exception e)
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";

                throw new Exception(e.Message);
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Controla os logs de acesso à API, com informações de cpf ou cnpj, ip, browser e url
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_remote_ip_address"></param>
        /// <param name="p_user_agent"></param>
        /// <param name="p_path_request"></param>
        public void SetInfoLogApp(string p_cpf_cnpj, string p_remote_ip_address, string p_user_agent, string p_path_request)
        {

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_grava_log_app";

                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_remote_ip_address", OracleDbType.Varchar2, p_remote_ip_address, ParameterDirection.Input);
                comm.Parameters.Add("p_user_agent", OracleDbType.Varchar2, p_user_agent, ParameterDirection.Input);
                comm.Parameters.Add("p_path_request", OracleDbType.Varchar2, p_path_request, ParameterDirection.Input);
 
                comm.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

                oraDb.FechaConexao();

            }

        }

        /// <summary>
        /// Lista as categorias do Faq
        /// </summary>
        /// <returns>Lista com todas as categorias</returns>
        public ResultFaqCategorias ListaFaqCategorias()
        {

            var resultFaqCategoria = new ResultFaqCategorias();
            var lstFaqCategorias = new List<InfoFaqCategoria>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_faq_categorias";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var infoFaqCategoria = new InfoFaqCategoria();

                        infoFaqCategoria.nome = dr["categoria"].ToString();

                        lstFaqCategorias.Add(infoFaqCategoria);


                    }

                    resultFaqCategoria.success = true;
                    resultFaqCategoria.message = "OK";
                    resultFaqCategoria.results = lstFaqCategorias;

                }
                else
                {

                    resultFaqCategoria.success = false;
                    resultFaqCategoria.message = "Sem categorias";
                }

            }
            catch
            {
                resultFaqCategoria.success = false;
                resultFaqCategoria.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return resultFaqCategoria;
        }

        /// <summary>
        /// Lista dodas as perguntas e respostas, de acordo com a categoria solicitada
        /// </summary>
        /// <param name="p_categoria"></param>
        /// <returns>Lista com todas as perguntas e respostas</returns>
        public ResultFaqPerguntasRespostas ListaFaqPerguntasRespostas(string p_categoria)
        {

            var resultFaqPegRes = new ResultFaqPerguntasRespostas();
            var lstFaqPegRes = new List<InfoFaqPerguntasRespostas>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_faq_pergunta_resposta";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_categoria", OracleDbType.Varchar2, p_categoria, ParameterDirection.Input).Size = 4000;

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var infoFaqPegRes = new InfoFaqPerguntasRespostas();

                        infoFaqPegRes.pergunta = dr["pergunta"].ToString();
                        infoFaqPegRes.resposta = dr["resposta"].ToString();

                        lstFaqPegRes.Add(infoFaqPegRes);


                    }

                    resultFaqPegRes.success = true;
                    resultFaqPegRes.message = "OK";
                    resultFaqPegRes.categoria = p_categoria;
                    resultFaqPegRes.results = lstFaqPegRes;

                }
                else
                {

                    resultFaqPegRes.success = false;
                    resultFaqPegRes.message = "Sem categorias";
                    resultFaqPegRes.categoria = p_categoria;
                }

            }
            catch
            {
                resultFaqPegRes.success = false;
                resultFaqPegRes.message = "Serviço indisponível, tente novamente mais tarde.";
                resultFaqPegRes.categoria = p_categoria;
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return resultFaqPegRes;
        }

        /// <summary>
        /// Lista os atendente SAC que estão atribuidos ao produto
        /// </summary>
        /// <param name="p_cpf_cnpj"></param>
        /// <param name="p_emp_codigo"></param>
        /// <returns>Lista com os código do atendente SAC 1, 2 e 3</returns>
        public ResultAgenteEmpreendimento ListaAtendenteEmpreendimento(string p_cpf_cnpj, Int32 p_emp_codigo)
        {

            var result = new ResultAgenteEmpreendimento();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_atentedente_sac";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_cpf_cnpj", OracleDbType.Varchar2, p_cpf_cnpj, ParameterDirection.Input);
                comm.Parameters.Add("p_emp_codigo", OracleDbType.Varchar2, p_emp_codigo, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        result.success = true;
                        result.message = "OK";
                        result.agente_sac_1 = Int32.Parse(dr["agente_sac_1"].ToString());
                        result.agente_sac_2 = Int32.Parse(dr["agente_sac_2"].ToString());
                        result.agente_sac_3 = Int32.Parse(dr["agente_sac_3"].ToString());

                    }

                }
                else
                {

                    result.success = false;
                    result.message = "Sem atendente cadastrado para o empreendimento: " + p_emp_codigo;
                }


            }
            catch
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Efetua o doenloas do boleto bancário.
        /// </summary>
        /// <param name="p_numero_parcela"></param>
        /// <returns>PDF do boleto bancário</returns>
        public FileContentResult GetBoleto(string p_numero_parcela)
        {

            byte[] byteArray = null;

            var remoteFileUrl = _url_download_boleto + p_numero_parcela.ToString();

            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                try
                {
                    byteArray = wc.DownloadData(remoteFileUrl);
                }
                catch (System.Exception ex)
                {
                    throw new Exception("Boleto não dispoonivel!\n" + ex.Message);
                }
            }

            var fileContentResult = new FileContentResult(byteArray, "application/pdf");
            fileContentResult.FileDownloadName = "boleto" + p_numero_parcela.ToString() + ".pdf";

            return fileContentResult;
        }

        /// <summary>
        /// Efetua o download do extrato de pagamentos
        /// </summary>
        /// <param name="p_numero_organizacao"></param>
        /// <param name="p_numero_filial"></param>
        /// <param name="p_numero_contrato"></param>
        /// <param name="p_data_base"></param>
        /// <returns>PDF do extrato de pagamentos</returns>
        public FileContentResult GetExtrato(Int32 p_numero_organizacao, Int32 p_numero_filial, Int32 p_numero_contrato, string p_data_base)
        {
            var v_data_base = "";

            var crService = new ServiceClient(ServiceClient.EndpointConfiguration.BasicHttpsBinding_IService);
            
            //crService.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 1, 0);
            //crService.Endpoint.Binding.SendTimeout = new TimeSpan(0, 1, 0);
            //crService.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
            //crService.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 1, 0);
            crService.Endpoint.Address = new EndpointAddress(_url_wcf_crystalreports);

            try
            {
                v_data_base = DateTime.ParseExact(p_data_base, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy");
            }
            catch
            {
                v_data_base = DateTime.Now.ToString("dd-MM-yyyy");
            }

            var byteArray = crService.ExecutaRelatorioExtratoAsync(p_numero_organizacao, p_numero_filial, p_numero_contrato, v_data_base);

            var data = byteArray.Result;

            var fileContentResult = new FileContentResult(data, "application/pdf");
            fileContentResult.FileDownloadName = "ExtratoPagamentos_" + p_numero_contrato.ToString() + "_" + v_data_base + ".pdf";

            return fileContentResult;

        }

        /// <summary>
        /// Efetua o download do demonstrativo de IR
        /// </summary>
        /// <param name="p_numero_organizacao"></param>
        /// <param name="p_numero_filial"></param>
        /// <param name="p_numero_contrato"></param>
        /// <param name="p_ano_do_informe"></param>
        /// <returns>PDF com o demonstativo do IR</returns>
        public FileContentResult GetInformeIR(Int32 p_numero_organizacao, Int32 p_numero_filial, Int32 p_numero_contrato, Int32 p_ano_do_informe)
        {

            var crService = new ServiceClient(ServiceClient.EndpointConfiguration.BasicHttpsBinding_IService);
            crService.Endpoint.Address = new EndpointAddress(_url_wcf_crystalreports);

            var byteArray = crService.ExecutaRelatorioIRPFAsync(p_numero_organizacao, p_numero_filial, p_numero_contrato, p_ano_do_informe);

            var data = byteArray.Result;

            var fileContentResult = new FileContentResult(data, "application/pdf");
            fileContentResult.FileDownloadName = "DemonstrativoIRPF_" + p_numero_contrato.ToString() + "_" + (p_ano_do_informe) + ".pdf";

            return fileContentResult;
        }

        /// <summary>
        /// Testes
        /// </summary>
        public void GeraBoletoTeste()
        {


            var contaBancaria = new BoletoNetCore.ContaBancaria
            {

                Agencia = "2938",
                Conta = "44457",
                DigitoConta = "3",
                CarteiraPadrao = "1",
                TipoCarteiraPadrao = BoletoNetCore.TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = BoletoNetCore.TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = BoletoNetCore.TipoImpressaoBoleto.Empresa,
                OperacaoConta = "05"
                //MensagemFixaPagador = "MensagemFixaPagador",
                //MensagemFixaTopoBoleto = "MensagemFixaTopoBoleto"
            };

            _banco = BoletoNetCore.Banco.Instancia(BoletoNetCore.Bancos.Itau);
            _banco.Beneficiario = new BoletoNetCore.Beneficiario()
            {
                ContaBancaria = contaBancaria,
                CPFCNPJ = "07.882.930/0001-65",
                Nome = "MITRE REALTY EMPREENDIMENTOS E PARTICIPAÇÕES SA",
                Endereco = new BoletoNetCore.Endereco()
                {
                    LogradouroEndereco = "Al Santos",
                    LogradouroNumero = "700",
                    LogradouroComplemento = "5 andar",
                    Bairro = "Paulista",
                    Cidade = "São Paulo",
                    UF = "SP",
                    CEP = "04501002"
                },
                MostrarCNPJnoBoleto = true,
                Observacoes = "Não receber aoós a data de vencimento"

            };

            BoletoNetCore.Pagador pagador = new BoletoNetCore.Pagador()
            {
                Nome = "IRENILSON S BATISTA",
                CPFCNPJ = "117.743.578-01",
                Endereco = new BoletoNetCore.Endereco()
                {
                    LogradouroEndereco = "Rua Sabauadia",
                    LogradouroNumero = "100",
                    LogradouroComplemento = "Casa 1",
                    Bairro = "Artur Alvim",
                    Cidade = "São Paulo",
                    UF = "SP",
                    CEP = "03540200"
                }
            };

            BoletoNetCore.Boleto boleto = new BoletoNetCore.Boleto(_banco)
            {
                Banco = _banco,
                Carteira = "112",
                Pagador = pagador,
                DataVencimento = DateTime.Parse("15/11/2020"),
                DataProcessamento = DateTime.Parse("16/10/2020"),
                BancoCobradorRecebedor = "341",
                AgenciaCobradoraRecebedora = "1234",
                NossoNumero = "44457",
                NossoNumeroDV = "3",
                EspecieDocumento = BoletoNetCore.TipoEspecieDocumento.DM,
                NumeroDocumento = "193999",
                DataEmissao = DateTime.Parse("16/10/2020"),
                ComplementoInstrucao1 = "ComplementoInstrucao1",
                ComplementoInstrucao2 = "ComplementoInstrucao2",
                ComplementoInstrucao3 = "ComplementoInstrucao3",
                //MensagemInstrucoesCaixa = "MensagemInstrucoesCaixa",
                MensagemInstrucoesCaixaFormatado = "<p>MOSTRAR</p>",
                ValorTitulo = decimal.Parse("1261,67"),
                ValorPago = decimal.Parse("1261,67"),
                CodigoInstrucao1 = "CodigoInstrucao1",
                EspecieMoeda = "R$",
                CarteiraImpressaoBoleto = "112"
            };

            boleto.ValidarDados();

            //var boletoBancario = new BoletoBancarioPdf();
            var boletoBancario = new BoletoBancario();
            boletoBancario.OcultarInstrucoes = false;
            boletoBancario.Boleto = boleto;

            //var htmlInstrucoes = boletoBancario.GeraHtmlInstrucoes();

            //byte[] arqPdf = boletoBancario.MontaBytesPDF(true);
            var htmlBoleto = boletoBancario.MontaHtmlEmbedded();

            //File.WriteAllText("C:\\Boleto.html", htmlBoleto);


        }

        /// <summary>
        /// Lista de Empreendimentos Lançados ou Não
        /// </summary>
        /// <returns>Lista com ID e Nome</returns>
        public ResultProdutoEmpreendimentos ListaProdutosMitre(Int32 p_regiao_id)
        {

            var result = new ResultProdutoEmpreendimentos();
            var infos = new List<InfoProdutoEmpreendimentos>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_empreendimentos";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_empreendimento_id", OracleDbType.Varchar2, "", ParameterDirection.Input);
                comm.Parameters.Add("p_regiao_id", OracleDbType.Varchar2, p_regiao_id.ToString(), ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var info = new InfoProdutoEmpreendimentos();

                        info.empreendimento_id = Int32.Parse(dr["empreedimento_id"].ToString());
                        info.empreendimento_nome = dr["empreendimento"].ToString();
                        info.regiao_id = Int32.Parse(dr["regiao_id"].ToString());
                        info.regiao = dr["regiao"].ToString();
                        info.descricao = dr["descricao"].ToString();
                        info.situacao = dr["situacao"].ToString();
                        info.site = dr["site"].ToString();
                        info.ebook = dr["ebook"].ToString();

                        infos.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.results = infos;

                }
                else
                {

                    result.success = false;
                    result.message = "Sem categorias";
                }

            }
            catch(Exception e)
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Lista as Regisões de Produtos da Mitre
        /// </summary>
        /// <returns>Lista com as regiões</returns>
        public ResultRegiao ListaRegiaoProdutosMitre()
        {

            var result = new ResultRegiao();
            var infos = new List<InfoRegiao>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_empr_regiao";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_empreendimento_id", OracleDbType.Varchar2, "", ParameterDirection.Input);
                comm.Parameters.Add("regiao_id", OracleDbType.Varchar2, "", ParameterDirection.Input);
                
                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var info = new InfoRegiao();

                        info.regiao_id = Int32.Parse(dr["regiao_id"].ToString());
                        info.regiao = dr["regiao"].ToString();

                        infos.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.results = infos;

                }
                else
                {

                    result.success = false;
                    result.message = "Sem Região";
                }

            }
            catch
            {
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {

                oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary> 
        ///  Insere registro na tabela bot_interacao a cada interação do BOT com as informações de cada etapa relevante para métricas futuras 
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
        /// <returns>Status do Processamento</returns>
        public ResultGravaInteracaoBOT SetBotInteracaoApp(string p_idbot, string p_plataforma, string p_channel, string p_username, string p_decisaoinicial, string p_tipodocumento, string p_documento, string p_opcaomenu, string p_tipoduvida, string p_email, string p_objetivocompra, string p_tipoapartamento, string p_regiao, string p_tipoempreendimento, string p_baixarebook, string p_proximopasso)
        {
            var result = new ResultGravaInteracaoBOT();
            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {
                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_grava_bot_interacao";
                comm.Parameters.Add("p_idbot", OracleDbType.Varchar2, p_idbot, ParameterDirection.Input);
                comm.Parameters.Add("p_plataforma", OracleDbType.Varchar2, p_plataforma, ParameterDirection.Input);
                comm.Parameters.Add("p_channel", OracleDbType.Varchar2, p_channel, ParameterDirection.Input);
                comm.Parameters.Add("p_username", OracleDbType.Varchar2, p_username, ParameterDirection.Input);
                comm.Parameters.Add("p_decisaoinicial", OracleDbType.Varchar2, p_decisaoinicial, ParameterDirection.Input);
                comm.Parameters.Add("p_tipodocumento", OracleDbType.Varchar2, p_tipodocumento, ParameterDirection.Input);
                comm.Parameters.Add("p_documento", OracleDbType.Varchar2, p_documento, ParameterDirection.Input);
                comm.Parameters.Add("p_opcaomenu", OracleDbType.Varchar2, p_opcaomenu, ParameterDirection.Input);
                comm.Parameters.Add("p_tipoduvida", OracleDbType.Varchar2, p_tipoduvida, ParameterDirection.Input);
                comm.Parameters.Add("p_email", OracleDbType.Varchar2, p_email, ParameterDirection.Input);
                comm.Parameters.Add("p_objetivocompra", OracleDbType.Varchar2, p_objetivocompra, ParameterDirection.Input);
                comm.Parameters.Add("p_tipoapartamento", OracleDbType.Varchar2, p_tipoapartamento, ParameterDirection.Input);
                comm.Parameters.Add("p_regiao", OracleDbType.Varchar2, p_regiao, ParameterDirection.Input);
                comm.Parameters.Add("p_tipoempreendimento", OracleDbType.Varchar2, p_tipoempreendimento, ParameterDirection.Input);
                comm.Parameters.Add("p_baixarebook", OracleDbType.Varchar2, p_baixarebook, ParameterDirection.Input);
                comm.Parameters.Add("p_proximopasso", OracleDbType.Varchar2, p_proximopasso, ParameterDirection.Input);
                comm.ExecuteNonQuery();

                result.success = true;
                result.message = "OK";
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {
                oraDb.FechaConexao();
            }

            return result;
        }

        // <summary>  
        ///  Insere registro na tabela bot_interacao_pesquisa a cada interação do BOT com as informações de cada etapa relevante para métricas futuras  
        /// </summary>  
        /// <param name="p_idbot"></param>  
        /// <param name="p_nota"></param>  
        /// <param name="p_motivo"></param>  
        /// <returns>Status do Processamento</returns> 
        public ResultGravaInteracaoPesquisaBOT SetBotInteracaoPesquisaApp(string p_idbot, string p_nota, string p_motivo)
        {
            var result = new ResultGravaInteracaoPesquisaBOT();

            OracleConnection conn;
            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();
                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_grava_bot_interacao_pesquisa";
                comm.Parameters.Add("p_idbot", OracleDbType.Varchar2, p_idbot, ParameterDirection.Input);
                comm.Parameters.Add("p_nota", OracleDbType.Varchar2, p_nota, ParameterDirection.Input);
                comm.Parameters.Add("p_motivo", OracleDbType.Varchar2, p_motivo, ParameterDirection.Input);
                comm.ExecuteNonQuery();

                result.success = true;
                result.message = "OK";

            }
            catch (Exception e)
            {

                //throw new Exception(e.Message); 

                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {
                oraDb.FechaConexao();
            }

            return result;
        }

        /// <summary>  
        ///  Insere registro na tabela bot_interacao_atendimento a cada interação do BOT com as informações de cada etapa relevante para métricas futuras  
        /// </summary>  
        /// <param name="p_idbot"></param>  
        /// <param name="p_protocolo"></param>  
        /// <param name="p_depto"></param>  
        /// <param name="p_agente"></param>  
        /// <param name="p_status"></param>  
        /// <returns>Status do Processamento</returns> 
        public ResultGravaInteracaoAtendimentoBOT SetBotInteracaoAtendimentoApp(string p_idbot, string p_protocolo, string p_depto, string p_agente, string p_status)
        {
            var result = new ResultGravaInteracaoAtendimentoBOT();
            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();
                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_grava_bot_interacao_atendimento";
                comm.Parameters.Add("p_idbot", OracleDbType.Varchar2, p_idbot, ParameterDirection.Input);
                comm.Parameters.Add("p_protocolo", OracleDbType.Varchar2, p_protocolo, ParameterDirection.Input);
                comm.Parameters.Add("p_depto", OracleDbType.Varchar2, p_depto, ParameterDirection.Input);
                comm.Parameters.Add("p_agente", OracleDbType.Varchar2, p_agente, ParameterDirection.Input);
                comm.Parameters.Add("p_status", OracleDbType.Varchar2, p_status, ParameterDirection.Input);

                comm.ExecuteNonQuery();

                result.success = true;
                result.message = "OK";

            }
            catch (Exception e)
            {

                //throw new Exception(e.Message); 
                result.success = false;
                result.message = "Serviço indisponível, tente novamente mais tarde.";
            }
            finally
            {
                oraDb.FechaConexao();
            }

            return result;

        }

    }

}
