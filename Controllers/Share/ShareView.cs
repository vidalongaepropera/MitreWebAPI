using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Model.ChatBot;
using MitreWebAPI.Model.Recebimento;
using MitreWebAPI.Model.Share;
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

namespace MitreWebAPI.Controllers.Sahre
{
    /// <summary>
    /// Controle de Pagamentos do cliente
    /// </summary>
    public class ShareView
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
        public ShareView(IConfiguration Configuration, IWebHostEnvironment env, string p_token_app)
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
        /// Lista dados da unidades do emprendimento Share
        /// </summary>
        /// <param name="filial_id"></param>
        /// <param name="empreendiento_id"></param>
        /// <returns>Lista com informações do bloco e unidades</returns>
        public ResultInfoUnidades ListaUnidades(Int32 filial_id, Int32 empreendiento_id)
        {

            var result = new ResultInfoUnidades();
            var lstUnidades = new List<InfoUnidades>();

            OracleConnection conn;

            ControllersDatabase.OracleDb oraDb = new ControllersDatabase.OracleDb(_configuration);

            try
            {
                conn = oraDb.AbreConexao();

                OracleCommand comm = new OracleCommand();

                //VALORES PAGOS
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "dwmitre.pck_app_charbot.sp_share_lista_unidades";

                comm.Parameters.Add("c_dados", OracleDbType.RefCursor, ParameterDirection.Output);
                comm.Parameters.Add("filial_id", OracleDbType.Varchar2, filial_id, ParameterDirection.Input);
                comm.Parameters.Add("empreendiento_id", OracleDbType.Varchar2, empreendiento_id, ParameterDirection.Input);

                OracleDataReader dr = comm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {

                    while (dr.Read())
                    {

                        var info = new InfoUnidades();

                        info.bloco_id = Int32.Parse(dr["bloco_id"].ToString());
                        info.andar = Int32.Parse(dr["andar"].ToString()); ;
                        info.unidade_id = Int32.Parse(dr["unidade_id"].ToString()); ;
                        info.unidade_codigo = dr["unidade_codigo"].ToString();
                        info.unidade = dr["unidade"].ToString();
                        info.unidade_status = dr["unidade_status"].ToString();
                        info.tipologia_id = Int32.Parse(dr["tipologia_id"].ToString());
                        info.tipologia = dr["tipologia"].ToString();

                        lstUnidades.Add(info);

                    }

                    result.success = true;
                    result.message = "OK";
                    result.filial_id = filial_id;
                    result.empreendiento_id = empreendiento_id;
                    result.unidades = lstUnidades;

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
