using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


namespace EasyFreight.DAL
{
    public static class NotificationHelper
    {
        public static void Create_Notification(NotificationMsgVm notifi)
        {
            NotificationSql.Instance.Create_Notification(notifi);
        }

        public static List<NotificationMsgVm> GetTop10_Notification(int departmentID )
        {
            return NotificationSql.Instance.GetTop10_Notification(departmentID);
        }

        public static List<NotificationMsgVm> GetAll_Notification(int departmentID)
        {
            return NotificationSql.Instance.GetAll_Notification(departmentID);
        }
    }

    public  class NotificationSql
    {
        #region Connection...
        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServer"].ToString());
        }
        #endregion

        #region Class Instance..
        public static NotificationSql Instance
        {
            get
            {
                return new NotificationSql();
            }
        }
        #endregion


        public void Create_Notification(NotificationMsgVm notifi)
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                bool result = false;
                SqlCommand myCommand = new SqlCommand("Notifi_Actions", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                myCommand.Parameters.AddWithValue("@Action", 1);
                myCommand.Parameters.AddWithValue("@NotificationTypeID", notifi.NotificationTypeID);
                myCommand.Parameters.AddWithValue("@ObjectID", notifi.ObjectID);
                myCommand.Parameters.AddWithValue("@NotificationMsg", notifi.NotificationMsg); 
                // Execute the command
                myConnection.Open();
                result = myCommand.ExecuteNonQuery() > 0;
                myConnection.Close();
            }

        }
        public List<NotificationMsgVm> GetTop10_Notification(int departmentID)
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("Notifi_Actions", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Action",2);
                myCommand.Parameters.AddWithValue("@DepartmentID", departmentID);
                myConnection.Open();
                return GetNotifisFromReader(myCommand.ExecuteReader());
            }

        }
        public List<NotificationMsgVm> GetAll_Notification(int departmentID)
        {
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("Notifi_Actions", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@Action", 3);
                myCommand.Parameters.AddWithValue("@DepartmentID", departmentID);
                myConnection.Open();
                return GetNotifisFromReader(myCommand.ExecuteReader());
            }

        }
        private NotificationMsgVm PopulateNotificationMsgVmFromIDataReader(IDataReader reader)
        {
            NotificationMsgVm notifi = new NotificationMsgVm();
            if (reader["NotificationMsgID"] != DBNull.Value)
                notifi.NotificationMsgID = (int)reader["NotificationMsgID"];
            if (reader["NotificationTypeID"] != DBNull.Value)
                notifi.NotificationTypeID = (int)reader["NotificationTypeID"];
            if (reader["ObjectID"] != DBNull.Value)
                notifi.ObjectID = (int)reader["ObjectID"];

            if (reader["NotificationTitle"] != DBNull.Value)
                notifi.NotificationTitle = (string)reader["NotificationTitle"];
            if (reader["NotificationMsg"] != DBNull.Value)
                notifi.NotificationMsg = (string)reader["NotificationMsg"];

            if (reader["CreatedDate"] != DBNull.Value)
                notifi.CreatedDate = (DateTime)reader["CreatedDate"]; 

            return notifi;
        }
        protected virtual List<NotificationMsgVm> GetNotifisFromReader(IDataReader reader)
        {
            List<NotificationMsgVm> notifi = new List<NotificationMsgVm>();
            while (reader.Read())
                notifi.Add(PopulateNotificationMsgVmFromIDataReader(reader));
            return notifi;
        }
    }
}