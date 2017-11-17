using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoDotNetBestPractices
{
    public class DataAccessLayer:IDisposable
    {
        private SqlConnection SqlConnection { get; }
        public static DataAccessLayer GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    return new DataAccessLayer();
                }
                return Instance;

            }
        }
        private static readonly DataAccessLayer Instance = null;
        private DataAccessLayer()
        {
            try
            {

                string connectionString = ConfigurationManager.ConnectionStrings["LocalDatabase"].ConnectionString;
                SqlConnection = new SqlConnection(connectionString);
                SqlConnection.Open();
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe);
            }
            catch (SqlException se)
            {
                Console.WriteLine(se);
            }
            catch (ConfigurationException ce)
            {
                Console.WriteLine(ce);
            }          
        }

        public int RunProcedure(string procedureName, string[] paramsName, string[] paramsVal)
        {
            try
            {
                int resultRows;
                using (SqlCommand sqlCommand =
                    new SqlCommand(procedureName, SqlConnection) {CommandType = CommandType.StoredProcedure})
                {
                    for (int i = 0; i < paramsName.Length; i++)
                    {
                        //adding input parameters
                        SqlParameter sqlInputParameter = new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            ParameterName = AppendAtSign(paramsName[i]),
                            Value = paramsVal[i]
                        };
                        sqlCommand.Parameters.Add(sqlInputParameter);
                    }
                    //adding output parameter
                    SqlParameter resultParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@Result",
                        Direction = ParameterDirection.ReturnValue
                    };

                    sqlCommand.Parameters.Add(resultParameter);
                    sqlCommand.ExecuteNonQuery();
                    resultRows = (int)sqlCommand.Parameters["@Result"].Value;
                    Dispose();
                }
                    return resultRows;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;

        }

        public DataSet ReturnData(string procName, string[] paramsName, string[] paramsVal)
        {
            try
            {
                using (SqlCommand sqlCommand =
                    new SqlCommand(procName, SqlConnection) { CommandType = CommandType.StoredProcedure })
                {
                    if (paramsName != null && paramsVal != null)
                    {
                        for (int i = 0; i < paramsName.Length; i++)
                        {
                            //adding input parameters
                            SqlParameter sqlInputParameter = new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                ParameterName = AppendAtSign(paramsName[i]),
                                Value = paramsVal[i]
                            };
                            sqlCommand.Parameters.Add(sqlInputParameter);
                        }

                    }
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        DataSet ds = new DataSet(procName);
                        sqlDataAdapter.Fill(ds);
                        Dispose();
                        return ds;
                    }
                }               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            return null;
        }
        private string AppendAtSign(string input)
        {
            return "@" + input;
        }

        public void Dispose()
        {
            SqlConnection.Close();
            SqlConnection.Dispose();
        }
    }
}
