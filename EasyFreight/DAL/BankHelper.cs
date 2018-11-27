using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;
using AutoMapper;
using System.Data.Entity.Validation;

namespace EasyFreight.DAL
{
    public static class BankHelper
    {
        public static BankVm GetBankInfo(int id)
        {
            BankVm bankVm = new BankVm();
            if (id == 0)
            {
                BankAccountVm bankAccVm = new BankAccountVm();
                bankVm.BankAccounts.Add(bankAccVm);
            }
            else
            {
                AccountingEntities db = new AccountingEntities();
                Bank bankDbObj = db.Banks.Include("BankAccounts")
                   .Where(x => x.BankId == id).FirstOrDefault();

                Mapper.CreateMap<Bank, BankVm>()
                    .ForMember(x => x.BankAccounts, y => y.MapFrom(s => s.BankAccounts));
                Mapper.CreateMap<BankAccount, BankAccountVm>();
                
                Mapper.Map(bankDbObj, bankVm);

            }

            return bankVm;
        }

        public static string AddEditBank(BankVm bankVm)
        {
            int bankId = bankVm.BankId;
            string isSaved = "true";
            AccountingEntities db = new AccountingEntities();
            Bank bankDb;
            List<BankAccount> dbContactList = new List<BankAccount>();
            if (bankId == 0) //Add new case
                bankDb = new Bank();
            else
            {
                bankDb = db.Banks.Include("BankAccounts").Where(x => x.BankId == bankId).FirstOrDefault();
                //delete any removed contact on the screen
                dbContactList = bankDb.BankAccounts.ToList();

                //Get contact Ids sent from the screen
                List<int> contactVmIds = bankVm.BankAccounts.Select(x => x.BankAccId).ToList();
                var contactDel = dbContactList.Where(x => !contactVmIds.Contains(x.BankAccId)).ToList();

                foreach (var item in contactDel)
                {
                    db.BankAccounts.Remove(item);
                }

            }

            Mapper.CreateMap<BankVm, Bank>().IgnoreAllNonExisting();
            Mapper.CreateMap<BankAccountVm, BankAccount>().IgnoreAllNonExisting();
            Mapper.Map(bankVm, bankDb);



            if (bankId == 0)
            {
                db.Banks.Add(bankDb);
            }


            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.Message;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.Message;
            }

            return isSaved;
        }

        internal static List<BankVm> GetBankList()
        {
            List<BankVm> bankList = new List<BankVm>();
            AccountingEntities db = new AccountingEntities();
            var carrDbList = db.Banks.ToList();
            Mapper.CreateMap<Bank, BankVm>()
                .ForMember(x => x.BankAccounts, y => y.Ignore())
                .IgnoreAllNonExisting();
            Mapper.Map(carrDbList, bankList);
            return bankList;
        }

        internal static List<BankAccountVm> GetBankAccount(int bankId)
        {
            List<BankAccountVm> bankAccountVm = new List<BankAccountVm>();
            AccountingEntities db = new AccountingEntities();

            List<BankAccount> bankAccountDb = db.BankAccounts.Include("Currency").Where(x => x.BankId == bankId).ToList();

            Mapper.CreateMap<BankAccount, BankAccountVm>()
                .IgnoreAllNonExisting();
            Mapper.Map(bankAccountDb, bankAccountVm);

            foreach (var item in bankAccountVm)
            {
                item.CurrencySign = bankAccountDb.Where(x => x.BankAccId == item.BankAccId).FirstOrDefault().Currency.CurrencySign;
            }

            return bankAccountVm;
        }

        internal static BankDetailsVm GetBankDetByCurrency(int bankId, int currencyId)
        {
            BankDetailsVm bankDetails = new BankDetailsVm();

            AccountingEntities db = new AccountingEntities();
            Bank bankDbObj = db.Banks.Include("BankAccounts")
               .Where(x => x.BankId == bankId).FirstOrDefault();

            Mapper.CreateMap<Bank, BankDetailsVm>()
                .ForMember(x => x.BankAccounts, y => y.Ignore())
                .IgnoreAllNonExisting();
            Mapper.Map(bankDbObj, bankDetails);

            var bankAccountObj = bankDbObj.BankAccounts.Where(x => x.CurrencyId == currencyId).FirstOrDefault();

            if (bankAccountObj != null)
            {
                bankDetails.AccountName = bankAccountObj.AccountName;
                bankDetails.AccountNumber = bankAccountObj.AccountNumber;
                bankDetails.BankAccId = bankAccountObj.BankAccId;
            }
            else
                bankDetails.AccountName = "false";

            return bankDetails;
        }

        public static string DeleteBank(int id)
        {
            string isDeleted = "true";
            AccountingEntities db = new AccountingEntities();
            Bank bankDbObj = db.Banks.Include("BankAccounts")
               .Where(x => x.BankId == id).FirstOrDefault();
            db.Banks.Remove(bankDbObj);

            int bankFound = 0;

            AgentNote agNoteDb;
            agNoteDb = db.AgentNotes.Where(x => x.BankId == id && (x.InvStatusId != 1)).FirstOrDefault();
            bankFound = agNoteDb != null ? bankFound+1 : bankFound;

            CashInReceipt cashInReceiptDb;
            cashInReceiptDb = db.CashInReceipts.Where(x => x.BankId == id && (x.IsDeleted != true)).FirstOrDefault();
            bankFound = cashInReceiptDb != null ? bankFound+1 : bankFound;

            CashOutReceipt cashOutReceiptDb;
            cashOutReceiptDb = db.CashOutReceipts.Where(x => x.BankId == id && (x.IsDeleted != true)).FirstOrDefault();
            bankFound = cashOutReceiptDb != null ? bankFound+1 : bankFound;

            if (bankFound > 0)
            {
                isDeleted = "Cannot Delete this bank there are some related transactions ! "   ;
                return isDeleted;
            }
            

            try
            {
                 db.SaveChanges();
            }
            catch (Exception ex)
            {
                isDeleted = "false " + ex.Message;
            }

            return isDeleted;
        }

        internal static List<BankVm> GetAllBankAndAccountList()
        {
            List<BankVm> bankList = new List<BankVm>();
            AccountingEntities db = new AccountingEntities();
            var carrDbList = db.Banks.ToList();
            Mapper.CreateMap<Bank, BankVm>()
                .ForMember(x => x.BankAccounts, y => y.Ignore())
                .IgnoreAllNonExisting();
            Mapper.Map(carrDbList, bankList);

            //Accounts 
             List<BankAccountVm> bankAccountVm = new List<BankAccountVm>();
 
            List<BankAccount> bankAccountDb = db.BankAccounts.Include("Currency").ToList();

            Mapper.CreateMap<BankAccount, BankAccountVm>()
                .IgnoreAllNonExisting();
            Mapper.Map(bankAccountDb, bankAccountVm);

            foreach (var item in bankAccountVm)
            {
                item.CurrencySign = bankAccountDb.Where(x => x.BankAccId == item.BankAccId).FirstOrDefault().Currency.CurrencySign; 
            }



            foreach (var item in bankList)
            {
                item.BankAccounts = bankAccountVm.Where(x => x.BankId == item.BankId).ToList();
            }

            return bankList;
        }

        internal static BankAccountVm GetBankAccountInfo(int bankAccId)
        {
            BankAccountVm  bankAccountVm = new BankAccountVm ();
            AccountingEntities db = new AccountingEntities();

            BankAccount bankAccountDb = db.BankAccounts.Include("Currency").Where(x => x.BankAccId == bankAccId).FirstOrDefault();

            Mapper.CreateMap<BankAccount, BankAccountVm>()
                .IgnoreAllNonExisting();
            Mapper.Map(bankAccountDb, bankAccountVm);
             
            bankAccountVm.CurrencySign = bankAccountDb.Currency.CurrencySign; 

            return bankAccountVm;
        }
    }
}