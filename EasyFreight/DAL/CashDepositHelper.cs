using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using Newtonsoft.Json.Linq;

namespace EasyFreight.DAL
{
    public static class CashDepositHelper
    {
        public static string AddArCashDeposit(CashInVm cashInVm)
        {
            string isSaved = "true";
            AccountingEntities db = new AccountingEntities();
            ArCashDeposit arCashDeposit = new ArCashDeposit();

            Mapper.CreateMap<CashInVm, ArCashDeposit>()
              .IgnoreAllNonExisting();
            Mapper.Map(cashInVm, arCashDeposit);

            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    arCashDeposit.ReceiptCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.CashIn, true);
                    db.ArCashDeposits.Add(arCashDeposit);

                    db.SaveChanges();

                    

                    cashInVm.ReceiptId = arCashDeposit.ReceiptId;
                    cashInVm.ReceiptCode = arCashDeposit.ReceiptCode;

                    string creditAccountId = ((int)AccountingChartEnum.DepositRevenues).ToString();

                    //Add invoice to accounting transactions table
                    CashHelper.AddReceiptToTransTable(creditAccountId, cashInVm, "ArCashDeposit");

                    //Add or update deposit balance for this client
                    CashDepositBalanceHelper.AddEditArDepBalance(cashInVm.ShipperId, cashInVm.ConsigneeId,
                        cashInVm.CurrencyId, cashInVm.ReceiptAmount.Value, db); 

                    transaction.Complete();
                }
                catch (DbEntityValidationException e)
                {
                    isSaved = "false " + e.Message;
                }
                catch (Exception e)
                {
                    isSaved = "false " + e.Message;
                }
            }


            return isSaved;
        }

        internal static JObject GetCashReceiptsJson()
        {
            AccountingEntities db = new AccountingEntities();

            var receiptList = db.ArCashDepositViews.OrderByDescending(x => x.CreateDate ).ToList();


            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();

            foreach (var item in receiptList)
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
                pJTokenWriter.WriteValue(item.Notes ?? "");

                pJTokenWriter.WritePropertyName("Client");
                pJTokenWriter.WriteValue(string.IsNullOrEmpty(item.ShipperNameEn) ? item.ConsigneeNameEn : item.ShipperNameEn);

                pJTokenWriter.WritePropertyName("CreateDate");
                pJTokenWriter.WriteValue(item.CreateDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("UserName");
                pJTokenWriter.WriteValue(item.UserName);

                pJTokenWriter.WritePropertyName("PaymentTermEn");
                pJTokenWriter.WriteValue(item.PaymentTermEn);

                pJTokenWriter.WriteEndObject();
            }


            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static CashInVm GetCashReceiptInfo(int cashReceiptId = 0)
        {
            CashInVm cashVm = new CashInVm();
            AccountingEntities db = new AccountingEntities();

            var cashReceiptObj = db.ArCashDepositViews.Where(x => x.ReceiptId == cashReceiptId).FirstOrDefault();

            Mapper.CreateMap<ArCashDepositView, CashInVm>()
               .ForMember(x => x.CashInReceiptChecks, y => y.Ignore())
               .ForMember(x => x.CashInReceiptInvs, y => y.Ignore())
               .ForMember(x => x.PaymentTermName, y => y.MapFrom(u => u.PaymentTermEn));

            Mapper.Map(cashReceiptObj, cashVm);

            cashVm.CustomerName = string.IsNullOrEmpty(cashReceiptObj.ShipperNameEn) ? cashReceiptObj.ConsigneeNameEn : cashReceiptObj.ShipperNameEn;

            //for bank
            if (cashVm.PaymentTermId == (byte)PaymentTermEnum.BankCashDeposit)
            {
                int bankAccId = cashVm.BankAccId.Value;
                var bankInfo = db.BankAccounts.Include("Bank").Where(x => x.BankAccId == bankAccId).FirstOrDefault();
                cashVm.BankDetailsVm = new BankDetailsVm()
                {
                    AccountName = bankInfo.AccountName,
                    AccountNumber = bankInfo.AccountNumber,
                    BankNameEn = bankInfo.Bank.BankNameEn,
                    BankAddressEn = bankInfo.Bank.BankAddressEn
                };
            }

            return cashVm;
        }

        internal static string DeleteCashDeposit(int receiptId, string deleteReason)
        {
            return CashInOutReceiptHelper.DeleteCashDeposit(receiptId,deleteReason) ? "true" : "false";
        }
    
    }
}