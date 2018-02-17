using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace AdoDemo.Web.Repository
{
    public class CustomerDAO
    {
        private readonly IConfiguration _configuration;

        public CustomerDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private string GetConnectionString()
        {
            var conn = _configuration.GetValue<string>("ConnectionString:DefaultConnection");
            return conn;
        }

        private int GetCommandTimeOut()
        {
            int cto = 0;
            try
            {
                int.TryParse(_configuration.GetValue<string>("CommandTimeOut:DefaultCTO"), out cto);
                if (cto == 0)
                    cto = 30;
            }
            catch (Exception ex)
            {
                cto = 30;
            }
            return cto;
        }

        public DataSet ExecuteDataset(string sql)
        {
            var ds = new DataSet();
            using (var con = new NpgsqlConnection(GetConnectionString()))
            {
                var cmd = new NpgsqlCommand(sql, con);
                cmd.CommandTimeout = GetCommandTimeOut();
                NpgsqlDataAdapter da;
                try
                {
                    da = new NpgsqlDataAdapter(cmd);
                    da.Fill(ds);
                    da.Dispose();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    da = null;
                    cmd.Dispose();
                }
                return ds;
            }
        }

        public DataTable ExecuteDataTable(string sql)
        {
            using (var ds = ExecuteDataset(sql))
            {
                if (ds == null || ds.Tables.Count == 0)
                    return null;

                return ds.Tables[0];
            }
        }

        public DataRow ExecuteDataRow(string sql)
        {
            using (var ds = ExecuteDataset(sql))
            {
                if (ds == null || ds.Tables.Count == 0)
                    return null;

                if (ds.Tables[0].Rows.Count == 0)
                    return null;

                return ds.Tables[0].Rows[0];
            }
        }

        public String FilterString(string strVal)
        {
            var str = FilterQuote(strVal);

            if (str.ToLower() != "null")
                str = "'" + str + "'";

            return str;
        }

        public String FilterXmlString(string strVal)
        {
            return "'" + strVal + "'";
        }

        public String FilterXmlNodeString(string strVal)
        {
            var str = FilterQuote(strVal);

            return str;
        }

        public String FilterQuote(string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                strVal = "";
            }
            var str = strVal.Trim();

            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace(";", "");
                str = str.Replace("--", "");
                str = str.Replace("'", "");

                str = str.Replace("/*", "");
                str = str.Replace("*/", "");

                str = Regex.Replace(str, " select ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " insert ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " update ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " delete ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " drop ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " truncate ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " create ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " begin ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " end ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " char ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " exec ", string.Empty, RegexOptions.IgnoreCase);
                str = Regex.Replace(str, " xp_cmd ", string.Empty, RegexOptions.IgnoreCase);

                str = Regex.Replace(str, @"<.*?>", string.Empty);

            }
            else
            {
                str = "null";
            }
            return str;
        }

        public String FilterQuoteOnly(string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                strVal = "''";
            }
            else
            {
                strVal = string.Format("'{0}'", strVal);
            }
            return strVal;
        } 
    }
}
