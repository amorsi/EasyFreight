using EasyFreight.Models;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;

namespace EasyFreight.DAL
{
    public static class CheckManageHelper
    {
        internal static JObject GetARChecksListJson(System.Web.Mvc.FormCollection form)
        {
            AccountingEntities db = new AccountingEntities();

            var checkDbList = db.CashInReceiptViews
                .Where(x => !string.IsNullOrEmpty(x.CheckNumber) && x.IsDeleted == false)
                .Select(x => new
                {
                    x.ReceiptId,
                    x.CheckNumber,
                    x.CheckDueDate,
                    x.ShipperId,
                    x.ShipperNameEn,
                    x.ConsigneeId,
                    x.ConsigneeNameEn,
                    x.IsCollected,
                    x.ReceiptCode,
                    x.ReceiptDate,
                    x.ReceiptAmount,
                    x.CurrencySign,
                    x.AgentId,
                    x.AgentNameEn
                })
                .ToList();
            if (form != null)
            {
                //filter data here
            }

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();

            foreach (var item in checkDbList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("ReceiptId");
                pJTokenWriter.WriteValue(item.ReceiptId);

                pJTokenWriter.WritePropertyName("CheckNumber");
                pJTokenWriter.WriteValue(item.CheckNumber);

                pJTokenWriter.WritePropertyName("CheckDueDate");
                pJTokenWriter.WriteValue(item.CheckDueDate.Value.ToString("dd/MM/yyyy"));


                if (item.IsCollected.Value)
                {
                    pJTokenWriter.WritePropertyName("IsCollected");
                    pJTokenWriter.WriteValue("Collected");

                    pJTokenWriter.WritePropertyName("Status");
                    pJTokenWriter.WriteValue("1");
                }

                else
                {
                    pJTokenWriter.WritePropertyName("IsCollected");
                    pJTokenWriter.WriteValue("Not Collected");

                    pJTokenWriter.WritePropertyName("Status");
                    pJTokenWriter.WriteValue("0");
                }


                pJTokenWriter.WritePropertyName("ReceiptCode");
                pJTokenWriter.WriteValue(item.ReceiptCode);

                pJTokenWriter.WritePropertyName("ReceiptDate");
                pJTokenWriter.WriteValue(item.ReceiptDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("CustomerName");

                if (item.ShipperId != null && item.ShipperId != 0)
                    pJTokenWriter.WriteValue(item.ShipperNameEn);
                else if (item.ConsigneeId != null && item.ConsigneeId != 0)
                    pJTokenWriter.WriteValue(item.ConsigneeNameEn);
                else if (item.AgentId != null && item.AgentId != 0)
                    pJTokenWriter.WriteValue(item.AgentNameEn);
                else
                    pJTokenWriter.WriteValue("");


                pJTokenWriter.WritePropertyName("ReceiptAmount");
                pJTokenWriter.WriteValue(item.ReceiptAmount + " (" + item.CurrencySign + ")");

                pJTokenWriter.WritePropertyName("CurrencySign");
                pJTokenWriter.WriteValue(item.CurrencySign);


                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        internal static string CollectCheck(int receiptId)
        {
            string isCollected = "true";
            AccountingEntities db = new AccountingEntities();
            var receiptData = db.CashInReceipts.Where(x => x.ReceiptId == receiptId)
                .Select(x => new { x.ReceiptAmount, x.CurrencyId, x.ReceiptCode, x.BankAccId })
                .FirstOrDefault();
            var receiptCheck = db.CashInReceiptChecks.Where(x => x.ReceiptId == receiptId).FirstOrDefault();

            receiptCheck.IsCollected = true;
            receiptCheck.CollectDate = DateTime.Now;
            receiptCheck.CollectBy = AdminHelper.GetCurrentUserId();

            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    db.SaveChanges();

                    //Add Transactions
                    string debitAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(receiptData.BankAccId.Value, "BankAccount", "BankAccId");
                    string creditAccId = ((int)AccountingChartEnum.NotesReceivable).ToString();
                    string comment = "Collect check number " + receiptCheck.CheckNumber + " received by cash receipt code " + receiptData.ReceiptCode;
                    string commentAr = "تحصيل الشيك رقم " + receiptCheck.CheckNumber + " المستلم بموجب ايصال استلام رقم " + receiptData.ReceiptCode;
                    AddCheckActionToTransTable(debitAccId, creditAccId, receiptData.ReceiptAmount,
                        receiptData.CurrencyId.Value, comment, commentAr, receiptId, "CashInReceiptCheck");

                    transaction.Complete();
                }
                catch (DbEntityValidationException e)
                {
                    isCollected = "false " + e.Message;
                }
                catch (Exception e)
                {
                    isCollected = "false " + e.Message;
                }


                return isCollected;

            }
        }

        internal static JObject GetAPChecksListJson(System.Web.Mvc.FormCollection form)
        {
            AccountingEntities db = new AccountingEntities();

            var checkDbList = db.CashOutReceiptViews
                .Where(x => !string.IsNullOrEmpty(x.CheckNumber) && x.IsDeleted == false)
                .Select(x => new
                {
                    x.ReceiptId,
                    x.CheckNumber,
                    x.CheckDueDate,
                    x.CarrierId,
                    x.CarrierNameEn,
                    x.ContractorId,
                    x.ContractorNameEn,
                    x.IsCollected,
                    x.ReceiptCode,
                    x.ReceiptDate,
                    x.ReceiptAmount,
                    x.CurrencySign,
                    x.AgentId,
                    x.AgentNameEn
                })
                .ToList();
            if (form != null)
            {
                //filter data here
            }

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();

            foreach (var item in checkDbList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("ReceiptId");
                pJTokenWriter.WriteValue(item.ReceiptId);

                pJTokenWriter.WritePropertyName("CheckNumber");
                pJTokenWriter.WriteValue(item.CheckNumber);

                pJTokenWriter.WritePropertyName("CheckDueDate");
                pJTokenWriter.WriteValue(item.CheckDueDate.ToString("dd/MM/yyyy"));


                if (item.IsCollected)
                {
                    pJTokenWriter.WritePropertyName("IsPaid");
                    pJTokenWriter.WriteValue("Paid");

                    pJTokenWriter.WritePropertyName("Status");
                    pJTokenWriter.WriteValue("1");
                }

                else
                {
                    pJTokenWriter.WritePropertyName("IsPaid");
                    pJTokenWriter.WriteValue("Not Paids");

                    pJTokenWriter.WritePropertyName("Status");
                    pJTokenWriter.WriteValue("0");
                }


                pJTokenWriter.WritePropertyName("ReceiptCode");
                pJTokenWriter.WriteValue(item.ReceiptCode);

                pJTokenWriter.WritePropertyName("ReceiptDate");
                pJTokenWriter.WriteValue(item.ReceiptDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("CustomerName");

                if (item.CarrierId != null && item.CarrierId != 0)
                    pJTokenWriter.WriteValue(item.CarrierNameEn);
                else if (item.ContractorId != null && item.ContractorId != 0)
                    pJTokenWriter.WriteValue(item.ContractorNameEn);
                else if (item.AgentId != null && item.AgentId != 0)
                    pJTokenWriter.WriteValue(item.AgentNameEn);
                else
                    pJTokenWriter.WriteValue("");


                pJTokenWriter.WritePropertyName("ReceiptAmount");
                pJTokenWriter.WriteValue(item.ReceiptAmount + " (" + item.CurrencySign + ")");

                pJTokenWriter.WritePropertyName("CurrencySign");
                pJTokenWriter.WriteValue(item.CurrencySign);


                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        internal static string PayCheck(int receiptId)
        {
            string isCollected = "true";
            AccountingEntities db = new AccountingEntities();
            var receiptData = db.CashOutReceipts.Where(x => x.ReceiptId == receiptId)
                .Select(x => new { x.ReceiptAmount, x.CurrencyId, x.ReceiptCode, x.BankAccId })
                .FirstOrDefault();
            var receiptCheck = db.CashOutReceiptChecks.Where(x => x.ReceiptId == receiptId).FirstOrDefault();

            receiptCheck.IsCollected = true;
            receiptCheck.CollectDate = DateTime.Now;
            receiptCheck.CollectBy = AdminHelper.GetCurrentUserId();

            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    db.SaveChanges();

                    //Add Transactions
                    string creditAccId = AccountingChartHelper.GetAccountIdByPkAndTbName(receiptData.BankAccId.Value, "BankAccount", "BankAccId");
                    string debitAccId = ((int)AccountingChartEnum.NotesPayable).ToString();
                    string comment = "Pay check number " + receiptCheck.CheckNumber + " received by cash receipt code " + receiptData.ReceiptCode;
                    string commentAr = "دفع الشيك رقم " + receiptCheck.CheckNumber + " المستلم بموجب ايصال استلام رقم " + receiptData.ReceiptCode;
                    AddCheckActionToTransTable(debitAccId, creditAccId, receiptData.ReceiptAmount,
                        receiptData.CurrencyId.Value, comment, commentAr, receiptId, "CashOutReceiptCheck");

                    transaction.Complete();
                }
                catch (DbEntityValidationException e)
                {
                    isCollected = "false " + e.Message;
                }
                catch (Exception e)
                {
                    isCollected = "false " + e.Message;
                }


                return isCollected;

            }
        }

        private static void AddCheckActionToTransTable(string debitAccId, string creditAccId, decimal amount, int currencyId,
            string comment, string commentAr, int receiptId, string tbName)
        {

            AccTransactionVm accTrans = new AccTransactionVm()
            {
                CreateDate = DateTime.Now,
                CreateBy = AdminHelper.GetCurrentUserId(),
                TransactionName = comment,
                TransactionNameAr = commentAr
            };

            AccTransactionDetailVm accTransDetDebit = new AccTransactionDetailVm()
            {
                AccountId = debitAccId,
                CreditAmount = 0,
                CurrencyId = currencyId,
                DebitAmount = amount
            };

            accTrans.AccTransactionDetails.Add(accTransDetDebit);

            AccTransactionDetailVm accTransDetCredit = new AccTransactionDetailVm()
            {
                AccountId = creditAccId,
                CreditAmount = amount,
                CurrencyId = currencyId,
                DebitAmount = 0
            };
            accTrans.AccTransactionDetails.Add(accTransDetCredit);


            int transId = AccountingHelper.AddTransaction(accTrans);

            //Update TransId in invoice table
            AccountingChartHelper.AddTransIdToObj(transId, tbName, receiptId, "ReceiptId");

        }
    }
}