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
    public class TruckingTopModels
    {

        public static TruckingTopModels Instance
        {
            get
            {
                return new TruckingTopModels();
            }
        }
        protected SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        }


        private TurckingTopSummary PopulateTurckingTopSummaryFromIDataReader(IDataReader reader)
        {
            TurckingTopSummary hosCon = new TurckingTopSummary();
            if (reader["ContractorId"] != DBNull.Value)
                hosCon.ContractorId = (int)reader["ContractorId"];
            if (reader["ContractorNameEn"] != DBNull.Value)
                hosCon.ContractorNameEn = (string)reader["ContractorNameEn"];
            if (reader["OpCount"] != DBNull.Value)
                hosCon.OpCount = (int)reader["OpCount"]; 
            if (reader["StatusId"] != DBNull.Value)
                hosCon.StatusId = (int)(byte)reader["StatusId"];
            if (reader["StatusName"] != DBNull.Value)
                hosCon.StatusName = (string)reader["StatusName"];
            if (reader["ContainersCount"] != DBNull.Value)
                hosCon.ContainersCount = (int)reader["ContainersCount"];
            if (reader["TotalNetCost"] != DBNull.Value)
                hosCon.TotalNetCost = (decimal)reader["TotalNetCost"];

            if (reader["TotalSellingCost"] != DBNull.Value)
                hosCon.TotalSellingCost = (decimal)reader["TotalSellingCost"];

            if (reader["CurrencyName"] != DBNull.Value)
                hosCon.CurrencyName = (string)reader["CurrencyName"];
            if (reader["CurrencySign"] != DBNull.Value)
                hosCon.CurrencySign = (string)reader["CurrencySign"];
            if (reader["CurrencyId"] != DBNull.Value)
                hosCon.CurrencyId = (int)reader["CurrencyId"];
            if (reader["OrdersCount"] != DBNull.Value)
                hosCon.OrdersCount = (int)reader["OrdersCount"];
            return hosCon;

        }
        protected virtual List<TurckingTopSummary> GetTurckingTopSummaryCollectionFromReader(IDataReader reader)
        {
            List<TurckingTopSummary> hosConList = new List<TurckingTopSummary>();
            while (reader.Read())
                hosConList.Add(PopulateTurckingTopSummaryFromIDataReader(reader));
            return hosConList;
        }

        public List<TurckingTopSummary> GetTruckingTopByDate(DateTime? fromdate ,DateTime? todate)
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("Trucking_GetTopByDate", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@FromDate", fromdate == null ? new DateTime(1900, 1, 1) : fromdate);
                myCommand.Parameters.AddWithValue("@ToDate", todate == null ? new DateTime(2900, 1, 1) : todate);
                myConnection.Open();
                return GetTurckingTopSummaryCollectionFromReader(myCommand.ExecuteReader());
            }
        }

        public bool HasActiveInovice(int truckingId)
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                bool hasinv = false;
                SqlCommand myCommand = new SqlCommand("Trucking_CheckInvoice", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@TruckingOrderId", truckingId);
                 myConnection.Open();
                 using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                 {
                     if (dr.Read())
                     {
                         hasinv = dr.GetInt32(0) > 0;
                     }
                     dr.Close();
                 }
                 myConnection.Close();
                 return hasinv;
            }


        }
    }
}