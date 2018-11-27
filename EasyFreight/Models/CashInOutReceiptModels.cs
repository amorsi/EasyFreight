using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EasyFreight.Models
{
    internal class CashInOutReceiptModels
    {
        public static CashInOutReceiptModels Instance
        {
            get
            {
                return new CashInOutReceiptModels();
            }
        }
       
        protected SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        }


        public bool CashInReceipt_Delete(int receiptId, int invId, int agentNoteId, string deleteReason)
        {
            bool result = false;
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("CashInReceipt_Delete", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@ReceiptId", receiptId);
                myCommand.Parameters.AddWithValue("@DeletedBy", EasyFreight.DAL.AdminHelper.GetCurrentUserId());
                myCommand.Parameters.AddWithValue("@DeleteReason", deleteReason);
                myCommand.Parameters.AddWithValue("@invId", invId == 0 ? DBNull.Value : (object)invId);
                myCommand.Parameters.AddWithValue("@AgentNoteId", agentNoteId == 0 ? DBNull.Value : (object)agentNoteId);
                myConnection.Open();
                result = myCommand.ExecuteNonQuery() > 0;
                myConnection.Close();
                return result;
            }
        }

        public bool CashOutReceipt_Delete(int receiptId, int invId, int agentNoteId, string deleteReason)
        {
            bool result = false;
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("CashOutReceipt_Delete", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@ReceiptId", receiptId);
                myCommand.Parameters.AddWithValue("@DeletedBy", EasyFreight.DAL.AdminHelper.GetCurrentUserId());
                myCommand.Parameters.AddWithValue("@DeleteReason", deleteReason);
                myCommand.Parameters.AddWithValue("@invId", invId == 0 ? DBNull.Value : (object)invId);
                myCommand.Parameters.AddWithValue("@AgentNoteId", agentNoteId == 0 ? DBNull.Value : (object)agentNoteId);
                myConnection.Open();
                result = myCommand.ExecuteNonQuery() > 0;
                myConnection.Close();
                return result;
            }
        }


        public bool DeleteCashDeposit(int receiptId, string deleteReason)
        {
            bool result = false;
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand("ArCashDeposit_Delete", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddWithValue("@ReceiptId", receiptId);
                myCommand.Parameters.AddWithValue("@DeletedBy", EasyFreight.DAL.AdminHelper.GetCurrentUserId());
                myCommand.Parameters.AddWithValue("@DeleteReason", deleteReason);
                myConnection.Open();
                result = myCommand.ExecuteNonQuery() > 0;
                myConnection.Close();
                return result;
            }
        }
        
    
    
    }


    public class CashInOutReceiptHelper
    {

        public static bool CashInReceipt_Delete(int receiptId, int invId, int agentNoteId, string deleteReason)
        {
            return CashInOutReceiptModels.Instance.CashInReceipt_Delete(receiptId,   invId,   agentNoteId,   deleteReason);
        }

        public static bool CashOutReceipt_Delete(int receiptId, int invId, int agentNoteId, string deleteReason)
        {
            return CashInOutReceiptModels.Instance.CashOutReceipt_Delete(receiptId, invId, agentNoteId, deleteReason);
        }


        public static bool DeleteCashDeposit(int receiptId,  string deleteReason)
        {
            return CashInOutReceiptModels.Instance.DeleteCashDeposit(receiptId, deleteReason);
        }

       
    
    }
}