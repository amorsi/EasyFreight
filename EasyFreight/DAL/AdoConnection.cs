using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EasyFreight.DAL
{
    public class AdoConnection
    {
        string connection_string = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

        public DataTable GetDataTableSP(string StoredName, ArrayList ParameterName, ArrayList ParameterValue)
        {

            SqlConnection con = new SqlConnection(connection_string);
            SqlCommand cmd = new SqlCommand();

            

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand(StoredName, con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < ParameterName.Count; i++)
            {
                da.SelectCommand.Parameters.Add(new SqlParameter(ParameterName[i].ToString(), ParameterValue[i]));
            }

            DataSet ds = new DataSet();
            da.Fill(ds, "result_name");

            DataTable dt = ds.Tables["result_name"];

            return dt;
        }
    }
}