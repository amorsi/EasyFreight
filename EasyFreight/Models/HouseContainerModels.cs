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
    public class HouseContainerModels
    {
        public static HouseContainerModels Instance
        {
            get
            {
                return new HouseContainerModels();
            }
        }
        protected SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        }


        public bool Create(HouseContainerVm hosCon )
        {
            bool result = false;
           
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("HouseContainer_Actions", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                myCommand.Parameters.AddWithValue("@OperationId", hosCon.OperationId);
                myCommand.Parameters.AddWithValue("@HouseBillId", hosCon.HouseBillId);
                myCommand.Parameters.AddWithValue("@OperConId", hosCon.OperConId);
                myCommand.Parameters.AddWithValue("@Action", 1); 
                myConnection.Open();
                if (myCommand.ExecuteNonQuery() > 0)
                {
                    result = true; 
                }
                myConnection.Close();
                return result;
            }
        }

        public bool Delete(HouseContainerVm hosCon)
        {
            bool result = false;

            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("HouseContainer_Actions", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                myCommand.Parameters.AddWithValue("@OperationId", hosCon.OperationId);
                myCommand.Parameters.AddWithValue("@HouseBillId", hosCon.HouseBillId);
                myCommand.Parameters.AddWithValue("@OperConId",DBNull.Value);
                myCommand.Parameters.AddWithValue("@Action", 2);
                myConnection.Open();
                if (myCommand.ExecuteNonQuery() > 0)
                {
                    result = true;
                }
                myConnection.Close();
                return result;
            }
        }

        public List<HouseContainerVm> GetHousContainerByHouseID(int houseBillId)
        {
             using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("HouseContainer_Actions", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@OperationId", 0);
                myCommand.Parameters.AddWithValue("@HouseBillId", houseBillId);
                myCommand.Parameters.AddWithValue("@OperConId", DBNull.Value);
                myCommand.Parameters.AddWithValue("@Action", 3);
                 myConnection.Open();
                return GetHouseContainerCollectionFromReader(myCommand.ExecuteReader());
            }
        }

        public List<HouseContainerVm> GetHousContainerByOperationID(int operationId)
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("HouseContainer_Actions", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@OperationId", operationId);
                myCommand.Parameters.AddWithValue("@HouseBillId", 0);
                myCommand.Parameters.AddWithValue("@OperConId", DBNull.Value);
                myCommand.Parameters.AddWithValue("@Action", 4);
                myConnection.Open();
                return GetHouseContainerCollectionFromReader(myCommand.ExecuteReader());
            }
        }

        private HouseContainerVm PopulateHouseContainerFromIDataReader(IDataReader reader)
        {
            HouseContainerVm hosCon = new HouseContainerVm();
            if (reader["HouseBillId"] != DBNull.Value)
                hosCon.HouseBillId = (int)reader["HouseBillId"];
            if (reader["OperationId"] != DBNull.Value)
                hosCon.OperationId = (int)reader["OperationId"];
            if (reader["OperConId"] != DBNull.Value)
                hosCon.OperConId = (int)reader["OperConId"];
            return hosCon;

        }
        protected virtual List<HouseContainerVm> GetHouseContainerCollectionFromReader(IDataReader reader)
        {
            List<HouseContainerVm> hosConList = new List<HouseContainerVm>();
            while (reader.Read())
                hosConList.Add(PopulateHouseContainerFromIDataReader(reader));
            return hosConList;
        }
    
    }
}