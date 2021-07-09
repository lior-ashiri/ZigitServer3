using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ZigitServer3.Services
{
    public class DBHandler
    {
        private string ConnectionString { get; set; }
        private SqlDataReader Reader { get; set; }
        private IConfiguration _config { get; set; }
        public DBHandler(IConfiguration configuration)
        {
            this._config = configuration;
            this.ConnectionString = _config.GetConnectionString("DBConnectionString");
        }
        public Dictionary<string, string> CheckIfUserExistsAndReturnValue(string Email, string Password)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        da.SelectCommand = new SqlCommand("sp_CheckIfUserExistsAndReturnValue", connection);
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        DataSet ds = new DataSet();
                        da.SelectCommand.Parameters.Add("@email", SqlDbType.VarChar).Value = Email;
                        da.SelectCommand.Parameters.Add("@password", SqlDbType.VarChar).Value = Password;

                        da.Fill(ds, "result_name");
                        foreach (DataRow row in ds.Tables["result_name"].Rows)
                        {
                            dictionary.Add("token", row["TokenID"].ToString());
                            dictionary.Add("name", row["sName"].ToString());
                            dictionary.Add("joinedAt", row["enrollDate"].ToString());
                            dictionary.Add("Team", row["teamName"].ToString());
                            dictionary.Add("Avatar", row["sAvatar"].ToString());
                        }
                        return dictionary;
                    }
                }
            }
            catch (Exception execption)
            {
                return null;
            }
        }

        public List<Dictionary<string, string>> GetUserProjectsByToken(string accessToken)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        da.SelectCommand = new SqlCommand("sp_GetUserProjectsByTokenID", connection);
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        DataSet ds = new DataSet();
                        da.SelectCommand.Parameters.Add("@TokenID", SqlDbType.UniqueIdentifier).Value = new Guid(accessToken);

                        da.Fill(ds, "result_name");
                        var result = new List<Dictionary<string, string>>();
                        foreach (DataRow row in ds.Tables["result_name"].Rows)
                        {
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            dictionary.Add("id", row["id"].ToString());
                            dictionary.Add("name", row["sname"].ToString());
                            dictionary.Add("score", row["score"].ToString());
                            dictionary.Add("durationInDays", row["durationInDays"].ToString());
                            dictionary.Add("bugsCount", row["bugsCount"].ToString());
                            dictionary.Add("madeDadeline", checkMadeDeadline(row["endDate"].ToString()).ToString());
                            result.Add(dictionary);
                        }
                        return result;
                    }
                }
            }
            catch (Exception execption)
            {
                return null;
            }
        }

        private bool checkMadeDeadline(string sEndDate)
        {
            
            var temp = sEndDate.Split(' ')[0];
            DateTime endDate = DateTime.ParseExact(temp, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return endDate.Date > DateTime.Now.Date;
        }
    }
}
