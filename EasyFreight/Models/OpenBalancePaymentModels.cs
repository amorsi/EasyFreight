using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AutoMapper;
using EasyFreight.ViewModel;

namespace EasyFreight.Models
{
    public class OpenBalancePaymentModels
    {

        public static OpenBalancePaymentModels Instance
        {
            get
            {
                return new OpenBalancePaymentModels();
            }
        }
        protected SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        }


        private OpenBalancePaymentVm PopulateOpenBalancePaymentFromIDataReader(IDataReader reader)
        {
            OpenBalancePaymentVm hosCon = new OpenBalancePaymentVm();

            if (reader["AccountId"] != DBNull.Value)
                hosCon.AccountId = (string)reader["AccountId"];

            if (reader["CurrencyId"] != DBNull.Value)
                hosCon.CurrencyId = (int)reader["CurrencyId"];

            if (reader["DebitAmount"] != DBNull.Value)
                hosCon.DebitAmount = (decimal)reader["DebitAmount"];

            if (reader["CreditAmount"] != DBNull.Value)
                hosCon.CreditAmount = (decimal) reader["CreditAmount"];

            if (reader["Amount"] != DBNull.Value)
                hosCon.Amount = (decimal)reader["Amount"];

            if (reader["IsCredit"] != DBNull.Value)
                hosCon.IsCredit = (int) reader["IsCredit"] ;

            if (reader["AccountNameEn"] != DBNull.Value)
                hosCon.AccountNameEn = (string)reader["AccountNameEn"];

            if (reader["AccountNameAr"] != DBNull.Value)
                hosCon.AccountNameAr = (string)reader["AccountNameAr"];

            if (reader["CurrencySign"] != DBNull.Value)
                hosCon.CurrencySign = (string)reader["CurrencySign"];

            if (reader["CurrencyName"] != DBNull.Value)
                hosCon.CurrencyName = (string)reader["CurrencyName"]; 

            return hosCon;

        }
        
        protected virtual List<OpenBalancePaymentVm> GetOpenBalancePaymentVmCollectionFromReader(IDataReader reader)
        {
            List<OpenBalancePaymentVm> hosConList = new List<OpenBalancePaymentVm>();
            while (reader.Read())
                hosConList.Add(PopulateOpenBalancePaymentFromIDataReader(reader));
            return hosConList;
        }

        public List<OpenBalancePaymentVm> GetOpenBalancePaymentVm( )
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("GetOpenBalancePayment", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure; 
                myConnection.Open();
                return GetOpenBalancePaymentVmCollectionFromReader(myCommand.ExecuteReader());
            }
        }


        public OpenBalancePaymentVm Get_OpenBalanceObject(string accid, int cid)
        {

            OpenBalancePaymentVm openBalanceObj = null;
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("OpenBalance_GetObject", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@AccountId", accid);
                myCommand.Parameters.AddWithValue("@CurrencyId", cid);
                myConnection.Open();
                using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr.Read())
                    {
                        openBalanceObj = PopulateOpenBalancePaymentFromIDataReader(dr);
                    }
                    dr.Close();
                }
                myConnection.Close();
                return openBalanceObj;
            }
        }

        public void Get_AccountTypeID(string accid, out int Id, out int IdType)
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                Id = 0;
                IdType = 0;

                SqlCommand myCommand = new SqlCommand("Get_tblIdByAccountId", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@AccountId", accid);
                myCommand.Parameters.Add("@AccountType", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                myCommand.Parameters.Add("@Id", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myConnection.Close();
                IdType = (int)myCommand.Parameters["@AccountType"].Value;
                Id = (int)myCommand.Parameters["@Id"].Value;
            }
        }

        
     }
}