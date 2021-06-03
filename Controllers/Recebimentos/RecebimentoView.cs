using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Model.ChatBot;
using MitreWebAPI.Model.Recebimento;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MitreWebAPI.Controllers.Recebimentos
{
    /// <summary>
    /// Controle de Pagamentos do cliente
    /// </summary>
    public class RecebimentoView
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly string url_antecipacao;
        private readonly string url_gerar_antecipacao;
        private readonly string _url_linha_digitavel_boleto;
        private readonly string _token;
        private bool _isTokenValido = false;

        /// <summary>
        /// Guarda as configuraçõpes da aplicação devalida o token da aplicação
        /// </summary>
        /// <param name="Configuration"></param>
        /// <param name="env"></param>
        /// <param name="p_token_app"></param>
        public RecebimentoView(IConfiguration Configuration, IWebHostEnvironment env, string p_token_app)
        {
            _configuration = Configuration;
            _environment = env;
            url_antecipacao = _configuration.GetSection("ConfigAPIChatBot").GetSection("url_antecipacao").Value;
            url_gerar_antecipacao = _configuration.GetSection("ConfigAPIChatBot").GetSection("url_gerar_antecipacao").Value;
            _url_linha_digitavel_boleto = _configuration.GetSection("ConfigAPIChatBot").GetSection("url_linha_digitavel_boleto").Value;
            _token = _configuration.GetSection("ConfigAPIChatBot").GetSection("Token").Value;

            if (p_token_app != null && !p_token_app.Equals(_token))
                throw new Exception("Token da aplicação inválida!");
            else
                _isTokenValido = true;

        }

        /// <summary>
        /// Lista Parcelas de um determinado contrato para antecipação
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <param name="p_dt_vencimento"></param>
        /// <returns>Lista com informações das parcelas antecipaveis</returns>
        public ResultAntecipacao ListaParcelasAntecipacao(string p_contrato_id, string p_dt_vencimento)
        {

            var lstParcelas = new List<InfoParcelasAntecipacao>();
            var result = new ResultAntecipacao();
            OracleConnection conn;
            string v_cliente_id = "";

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {
                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_pega_id_cliente";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, p_contrato_id, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                        v_cliente_id = dr["codprospect"].ToString();

                }
                else
                {

                    throw new Exception("Sem dados para o cliente do contrato:  " + p_contrato_id + ".");
                }

                string json = "";

                var remoteFileUrl = url_antecipacao.Replace("VAR_CLIENTE_ID", v_cliente_id).Replace("VAR_CONTRATO_ID", p_contrato_id.ToString()).Replace("VAR_DT_VENCIMENTO", p_dt_vencimento);

                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    try
                    {
                        json = wc.DownloadString(remoteFileUrl);
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception("Paracelas não disponíveis!\n" + ex.Message);
                    }
                }

                var parcelas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BaseParcelasAntecipacao>>(json);

                InfoParcelasAntecipacao x = new InfoParcelasAntecipacao();

                if (parcelas != null && _isTokenValido)
                {
                    foreach (BaseParcelasAntecipacao info in parcelas)
                    {
                        result.orgInCodigo = info.orgInCodigo;
                        result.codEmpreendimento = info.codEmpreendimento;
                        result.codBloco = info.codBloco;

                        var parInfo = new InfoParcelasAntecipacao()
                        {

                            id = info.id,
                            key = info.key,
                            dataVencimento = info.dataVencimento,
                            fator = info.fator,
                            origem = info.origem,
                            antReCapitalizado = double.Parse(info.antReCapitalizado.Replace(".", ",")),
                            antReDesconto = double.Parse(info.antReDesconto.Replace(".", ",")),
                            antReSaldoAtual = double.Parse(info.antReSaldoAtual.Replace(".", ",")),
                            bonificacao = Util.Formatacoes.FormataValorStringToDecimal(info.bonificacao),
                            cndiReTaxTabPrice = double.Parse(info.cndiReTaxTabPrice.Replace(".", ",")),
                            taxaAntecipacao = Util.Formatacoes.FormataValorStringToDecimal(info.taxaAntecipacao),
                            taxaBonificacao = Util.Formatacoes.FormataValorStringToDecimal(info.taxaBonificacao),
                            tipo = info.tipo,
                            valorAPagar = Util.Formatacoes.FormataValorStringToDecimal(info.valorAPagar),
                            valorAntecipado = double.Parse(info.valorAntecipado.Replace(".", ",")),
                            valorAtual = Util.Formatacoes.FormataValorStringToDecimal(info.valorAtual),
                            valorCobrado = Util.Formatacoes.FormataValorStringToDecimal(info.valorCobrado),
                            valorLiquido = Util.Formatacoes.FormataValorStringToDecimal(info.valorLiquido),
                            valorOriginal = Util.Formatacoes.FormataValorStringToDecimal(info.valorOriginal),
                            valorTotal = Util.Formatacoes.FormataValorStringToDecimal(info.valorTotal),
                            valorVinculado = Util.Formatacoes.FormataValorStringToDecimal(info.valorVinculado)
                        };

                        lstParcelas.Add(parInfo);
                    }

                    result.success = true;
                    result.message = "OK";
                    result.Parcelas = lstParcelas;

                }
                else
                {

                    if (_isTokenValido != true)
                    {
                        result.success = false;
                        result.message = "Token invalido!";
                    }
                    else if (_isTokenValido == true/* && dr != null && !dr.HasRows*/)
                    {
                        result.success = false;
                        result.message = "Sem registros";
                    }

                }

            }
            catch(Exception e)
            {
                result.success = false;
                result.message = e.Message + "\n "+ e.StackTrace;
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();

            }

            return result;
        }

        /// <summary>
        /// Gera a antecipação de parcelas em um determinado contrato do cliente
        /// </summary>
        /// <param name="infoAntecipacao"></param>
        /// <returns>Lista com status da geração</returns>
        public ResultAntecipacaoGerar GerarAntecipacao(InfoAntecipacaoGerar infoAntecipacao)
        {

            var result = new ResultAntecipacaoGerar();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {

                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_pega_info_antecipacao";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, infoAntecipacao.contrato_id, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                var baseInfoAntecipacao = new BaseGerarAntecipacao();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        baseInfoAntecipacao.orgInCodigo = dr["org_in_codigo"].ToString();
                        baseInfoAntecipacao.filInCodigo = dr["fil_in_codigo"].ToString();
                        baseInfoAntecipacao.codEmpreendimento = dr["emp_codigo"].ToString();
                        baseInfoAntecipacao.codBloco = dr["blo_codigo"].ToString();
                    }

                }
                else
                {

                    throw new Exception("Sem dados para o cliente com cpf ou cnpj:  " + infoAntecipacao.cpf_cnpj + ".");
                }

                baseInfoAntecipacao.bonificacao = infoAntecipacao.valor_total_desconto;
                baseInfoAntecipacao.codContrato = infoAntecipacao.contrato_id.ToString();
                baseInfoAntecipacao.dataBaixa = infoAntecipacao.dt_vcto_boleto;
                baseInfoAntecipacao.desconto = infoAntecipacao.valor_total_desconto;

                List<string> pars = new List<string>();

                foreach (InfoParcelasGerarAntecipacao par in infoAntecipacao.Parcelas)
                {
                    pars.Add(par.id);
                }

                baseInfoAntecipacao.parcelas = pars;

                baseInfoAntecipacao.qtdParcelas = infoAntecipacao.Parcelas.Count.ToString();
                baseInfoAntecipacao.soma = infoAntecipacao.valor_total_liquido;
                baseInfoAntecipacao.valorAtual = infoAntecipacao.valor_total_pago;
                baseInfoAntecipacao.vlrAntecipado = infoAntecipacao.valor_total_liquido;
                baseInfoAntecipacao.vlrAPagar = infoAntecipacao.valor_total_liquido;
                baseInfoAntecipacao.vlrLiquido = infoAntecipacao.valor_total_pago;
                baseInfoAntecipacao.vlrPagar = infoAntecipacao.valor_total_pago;

                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(baseInfoAntecipacao, Formatting.Indented);

                if (String.IsNullOrEmpty(jsonString))
                    throw new Exception("JSON vazio ou nulo");

                //File.WriteAllText("C:\\Temp\\GeraAntecipacao_" + baseInfoAntecipacao.codContrato + ".json", jsonString);

                byte[] buffer = new byte[16 * 1024];

                var http = (HttpWebRequest)WebRequest.Create(new Uri(url_gerar_antecipacao));

                //File.WriteAllText("C:\\Temp\\url_gerar_antecipacao" + baseInfoAntecipacao.codContrato + ".txt", url_gerar_antecipacao);


                http.Accept = "text/plain";
                http.ContentType = "application/json";
                http.Method = "POST";

                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(jsonString);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);

                result.success = true;
                result.message = "OK";
                result.seq_antecipacao_id = Int32.Parse(sr.ReadToEnd());

                var remoteFileUrl = _url_linha_digitavel_boleto + result.seq_antecipacao_id.ToString();

                //File.WriteAllText("C:\\Temp\\LinhaDigitavel" + baseInfoAntecipacao.codContrato + ".txt", remoteFileUrl);

                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    try
                    {
                        result.linha_digitavel = wc.DownloadString(remoteFileUrl).Replace(".", "").Replace(" ", "");
                    }
                    catch(Exception e)
                    {
                        //File.WriteAllText("C:\\Temp\\erro_antecipacao_linha_gigitavel" + infoAntecipacao.contrato_id + ".txt", e.Message + "\n" + e.StackTrace);
                        result.linha_digitavel = "Não foi possível criar a Linha digitáve";
                    }
                }


                //PEGA INFO DO CEDENTE
                var comm2 = new OracleCommand();

                comm2.Connection = conn;
                comm2.CommandType = CommandType.StoredProcedure;
                comm2.CommandText = "dwmitre.pck_app_charbot.sp_pega_info_cedente_ant_boleto";

                comm2.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm2.Parameters.Add("p_numero_boleto", OracleDbType.Varchar2, result.seq_antecipacao_id, ParameterDirection.Input);

                OracleDataReader info = comm2.ExecuteReader();

                if (info != null && info.HasRows)
                {

                    while (info.Read())
                    {

                        result.cedente_cpf_cnpj = info["cpf_cnpj_cedente"].ToString();
                        result.cedente = info["empresa_cedente"].ToString();
                        result.banco_numero = Int32.Parse(info["numero_banco"].ToString());
                        result.banco = info["nome_banco"].ToString();
                        result.vencimento = info["data_vencimento"].ToString();
                        result.valor = info["valor"].ToString();
                    }

                }
                else
                {

                    throw new Exception("Sem dados para a antecipação:  " + result.seq_antecipacao_id + ".");
                }


            }
            catch (Exception e)
            {
                //File.WriteAllText("C:\\Temp\\erro_antecipacao" + infoAntecipacao.contrato_id + ".txt", e.Message + "\n" + e.StackTrace);
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
        /// Lista Parcelas Pagas e Não pagas do Contrato do Cliente
        /// </summary>
        /// <param name="p_contrato_id"></param>
        /// <returns>Parcelas Pagas e Não pagas, Valor Total Pago e Total para Quitação </returns>
        public ResultContratoPagoQuitacao ListaInfomacoesPagamentos(Int32 p_contrato_id)
        {

            var result = new ResultContratoPagoQuitacao();
            var lstParPago = new List<InfoContratoValorPago>();
            var lstParNaoPago = new List<InfoContratoValorNaoPago>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {
                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                //VALORES PAGOS
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_lista_info_extrato";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("p_contrato_id", OracleDbType.Varchar2, p_contrato_id, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    var valor_total_pago   = "0,00";
                    var valor_total_apagar = "0,00";
                    var data_base_ref = "";
                    var status_pgto = "";

                    while (dr.Read())
                    {
                        valor_total_pago = Util.Formatacoes.FormataValorZeroDb(dr["valor_total_pago"].ToString());
                        valor_total_apagar = Util.Formatacoes.FormataValorZeroDb(dr["valor_total_apagar"].ToString());
                        data_base_ref = dr["data_base_ref"].ToString();
                        status_pgto = dr["status_pgto"].ToString();

                        if (status_pgto.Equals("P"))
                        {

                            var info = new InfoContratoValorPago();

                            info.numero_parcela = Int32.Parse(dr["numero_parcela"].ToString());
                            info.data_vencimento = dr["data_vencimento"].ToString();
                            info.data_pagamento = dr["data_pagamento"].ToString();
                            info.valor = Util.Formatacoes.FormataValorZeroDb(dr["valor"].ToString());

                            lstParPago.Add(info);
                        }
                        else
                        {
                            var info = new InfoContratoValorNaoPago();

                            info.numero_parcela = Int32.Parse(dr["numero_parcela"].ToString());
                            info.data_vencimento = dr["data_vencimento"].ToString();
                            info.valor_futuro = Util.Formatacoes.FormataValorZeroDb(dr["valor_futuro"].ToString());
                            info.valor_presente = Util.Formatacoes.FormataValorZeroDb(dr["valor_presente"].ToString());

                            lstParNaoPago.Add(info);

                        }
                    }

                    result.success = true;
                    result.message = "OK";
                    result.numero_contrato = p_contrato_id;
                    result.valor_total_pago = valor_total_pago;
                    result.valor_quitacao = valor_total_apagar;
                    result.data_base = data_base_ref;
                    result.results_pago = lstParPago;
                    result.results_quitacao = lstParNaoPago;

                }
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n " + e.StackTrace);
            }
            finally
            {
                if (oraDb != null)
                    oraDb.FechaConexao();

            }

            return result;
        }

    }
}
