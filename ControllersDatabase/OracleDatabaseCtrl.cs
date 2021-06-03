using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using MitreWebAPI.Model.ChatBot;
using System.Security.Cryptography.Xml;

namespace MitreWebAPI.ControllersDatabase
{

    public class OracleDb
    {
        
        IConfiguration _configuration;
        OracleConnection conn;
        string _token = "";

        public OracleDb(IConfiguration Configuration)
        {
            _configuration = Configuration;
            _token = _configuration.GetSection("ConfigAPIChatBot").GetSection("Token").Value;
        }

        public OracleConnection AbreConexao()
        {
            string connectionString = "";

            try
            {
                connectionString = _configuration.GetSection("ConnectionStrings").GetSection("MEGAConnection").Value;
                conn = new OracleConnection(connectionString);

                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

            }
            catch (OracleException e){

                throw new Exception(e.Message);
            }

            return conn;
        }

        public void FechaConexao()
        {

            try
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                    conn.Close();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
     
    }

}
