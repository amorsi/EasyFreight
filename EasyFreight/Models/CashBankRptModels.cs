using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using EasyFreight.ViewModel;

namespace EasyFreight.Models
{
    public class CashBankRptModels
    {
        public static CashBankRptModels Instance
        {
            get
            {
                return new CashBankRptModels();
            }
        }
        protected SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        }

        public List<CashBankRptVm> GetCashBankRpt()
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("AccRpt_BankCashBalance", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure; 
                myConnection.Open();
                return GetCashBankRptCollectionFromReader(myCommand.ExecuteReader());
            }
        }
         

        private CashBankRptVm PopulateCashBankRptFromIDataReader(IDataReader reader)
        {
            CashBankRptVm hosCon = new CashBankRptVm();

            if (reader["AccountId"] != DBNull.Value)
                hosCon.AccountId = (string)reader["AccountId"];
            if (reader["AccountNameEn"] != DBNull.Value)
                hosCon.AccountNameEn = (string)reader["AccountNameEn"];

            if (reader["AccountNameAr"] != DBNull.Value)
                hosCon.AccountNameAr = (string)reader["AccountNameAr"];


            if (reader["EGP"] != DBNull.Value)
                hosCon.EGP = (decimal)reader["EGP"];

            if (reader["USD"] != DBNull.Value)
                hosCon.USD = (decimal)reader["USD"];


            if (reader["EUR"] != DBNull.Value)
                hosCon.EUR = (decimal)reader["EUR"];

            if (reader["GBP"] != DBNull.Value)
                hosCon.GBP = (decimal)reader["GBP"];

            return hosCon;

        }
        protected virtual List<CashBankRptVm> GetCashBankRptCollectionFromReader(IDataReader reader)
        {
            List<CashBankRptVm> hosConList = new List<CashBankRptVm>();
            while (reader.Read())
                hosConList.Add(PopulateCashBankRptFromIDataReader(reader));
            return hosConList;
        }
    }
}