using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoDotNetBestPractices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] paramsName = {"UserName"};
            string[] paramsVal = {"s"};
            DataSet ds =DataAccessLayer.GetInstance.ReturnData("procReturnAllProfiles", paramsName, paramsVal);
            DataSet ds2 = DataAccessLayer.GetInstance.ReturnData("procReturnAllProfiles", null, null);

            if (ds?.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Console.WriteLine(dr[0]+" "+dr[1]+" "+dr[2]);
                }
                    
            }

            Console.ReadKey();
        }
        
    }
}
