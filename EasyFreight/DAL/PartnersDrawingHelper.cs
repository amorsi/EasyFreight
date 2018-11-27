using AutoMapper;
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
    public static class PartnersDrawingHelper
    {
        /// <summary>
        /// Get all actions done by the patners drawing and payments from cash tables
        /// </summary>
        /// <param name="form">Advanced search</param>
        /// <returns>json to fill jquery datatable</returns>
        internal static JObject GetPartnersDrawingJson(System.Web.Mvc.FormCollection form)
        {
            AccountingEntities db = new AccountingEntities();
            //Get Only cash out receipts for expenses
            var cashoutReceiptIds = db.CashOutReceipts.Where(x => x.PartnerAccountId != string.Empty)
            .Select(x => x.ReceiptId).Distinct();

            var cashinReceiptIds = db.CashInReceipts.Where(x => x.PartnerAccountId != string.Empty)
           .Select(x => x.ReceiptId).Distinct();


            var cashtReceiptList = (from c in db.CashOutReceipts
                                    join u in db.AspNetUserAccs
                             on c.CreateBy equals u.Id
                                    join cu in db.CurrencyAccs
                                    on c.CurrencyId equals cu.CurrencyId
                                    join ch in db.AccountingCharts
                                    on c.PartnerAccountId equals ch.AccountId
                                    where cashoutReceiptIds.Contains(c.ReceiptId) && c.IsDeleted == false
                                    select new TempList
                                    {
                                        ReceiptId = c.ReceiptId,
                                        ReceiptDate = c.ReceiptDate,
                                        ReceiptCode = c.ReceiptCode,
                                        ReceiptAmount = c.ReceiptAmount,
                                        Notes = c.Notes,
                                        CreateDate = c.CreateDate,
                                        CreateBy = c.CreateBy,
                                        CurrencySign = cu.CurrencySign,
                                        UserName = u.UserName,
                                        AccountNameEn = ch.AccountNameEn,
                                        ReceiptType = "Cash Out"
                                    }).ToList();

            var cashinReceiptList = (from c in db.CashInReceipts
                                     join u in db.AspNetUserAccs
                                     on c.CreateBy equals u.Id
                                     join cu in db.CurrencyAccs
                                     on c.CurrencyId equals cu.CurrencyId
                                     join ch in db.AccountingCharts
                                     on c.PartnerAccountId equals ch.AccountId
                                     where cashinReceiptIds.Contains(c.ReceiptId) && c.IsDeleted == false
                                     select new TempList
                                     {
                                         ReceiptId = c.ReceiptId,
                                         ReceiptDate = c.ReceiptDate,
                                         ReceiptCode = c.ReceiptCode,
                                         ReceiptAmount = c.ReceiptAmount,
                                         Notes = c.Notes,
                                         CreateDate = c.CreateDate,
                                         CreateBy = c.CreateBy,
                                         CurrencySign = cu.CurrencySign,
                                         UserName = u.UserName,
                                         AccountNameEn = ch.AccountNameEn,
                                         ReceiptType = "Cash In"
                                     }).ToList();

            cashtReceiptList.AddRange(cashinReceiptList);

            if (!string.IsNullOrEmpty(form["ReceiptDateStart"]))
            {
                DateTime date = DateTime.Parse(form["ReceiptDateStart"]);
                cashtReceiptList = cashtReceiptList.Where(x => x.ReceiptDate >= date).ToList();
            }

            if (!string.IsNullOrEmpty(form["ReceiptDateEnd"]))
            {
                DateTime date = DateTime.Parse(form["ReceiptDateEnd"]);
                cashtReceiptList = cashtReceiptList.Where(x => x.ReceiptDate <= date).ToList();
            }

            if (!string.IsNullOrEmpty(form["CreateDateStart"]))
            {
                DateTime date = DateTime.Parse(form["CreateDateStart"]);
                cashtReceiptList = cashtReceiptList.Where(x => x.CreateDate >= date).ToList();
            }

            if (!string.IsNullOrEmpty(form["CreateDateEnd"]))
            {
                DateTime date = DateTime.Parse(form["CreateDateEnd"]);
                cashtReceiptList = cashtReceiptList.Where(x => x.CreateDate <= date).ToList();
            }

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();

            foreach (var item in cashtReceiptList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("ReceiptId");
                pJTokenWriter.WriteValue(item.ReceiptId);

                pJTokenWriter.WritePropertyName("ReceiptCode");
                pJTokenWriter.WriteValue(item.ReceiptCode);

                pJTokenWriter.WritePropertyName("ReceiptDate");
                pJTokenWriter.WriteValue(item.ReceiptDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("ReceiptAmount");
                pJTokenWriter.WriteValue(item.ReceiptAmount.ToString() + " " + item.CurrencySign);

                pJTokenWriter.WritePropertyName("Notes");
                pJTokenWriter.WriteValue(item.Notes);

                pJTokenWriter.WritePropertyName("CreateDate");
                pJTokenWriter.WriteValue(item.CreateDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("UserName");
                pJTokenWriter.WriteValue(item.UserName);

                pJTokenWriter.WritePropertyName("AccountNameEn");
                pJTokenWriter.WriteValue(item.AccountNameEn);

                pJTokenWriter.WritePropertyName("ReceiptType");
                pJTokenWriter.WriteValue(item.ReceiptType);

                pJTokenWriter.WritePropertyName("ReceiptTypeShort");
                if (item.ReceiptType == "Cash In")
                    pJTokenWriter.WriteValue("in");
                else
                    pJTokenWriter.WriteValue("out");


                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static CashInVm GetCashReceiptForPartners(string receiptType, int receiptId = 0)
        {
            CashInVm cashVm;
            if (receiptId != 0)
            {
                cashVm = CashOutHelper.FillCashVmForReceiptView(receiptId);
                return cashVm;
            }
            cashVm = new CashInVm();

            if (receiptType == "out")
                cashVm.CashType = "cashout";
            else
                cashVm.CashType = "cashin";

            cashVm.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashOut, false);
            //cashVm.PaymentTermId = 1;
            //cashVm.CurrencyId = 1;
            CashInCheckVm cashCheckVm = new CashInCheckVm();
            cashVm.CashInReceiptChecks.Add(cashCheckVm);

            return cashVm;
        }

    }

    class TempList
    {
        public int ReceiptId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string ReceiptCode { get; set; }
        public decimal ReceiptAmount { get; set; }
        public string Notes { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string CurrencySign { get; set; }
        public string UserName { get; set; }
        public string AccountNameEn { get; set; }
        public string ReceiptType { get; set; }
    }
}