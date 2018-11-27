using EasyFreight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.DAL
{
    public static class CashDepositBalanceHelper
    {

        public static string AddEditArDepBalance(int? shipperId, int? consigneeId, 
            int currId,decimal amountToAdd, AccountingEntities db)
        {
            string isSaved = "true";

            //AccountingEntities db = new AccountingEntities();

            var arDepBalance = GetBalanceObj(shipperId, consigneeId, currId,db);

            //New balance case
            if(arDepBalance.BalanceAmount == 0)
            {
                arDepBalance.BalanceAmount = amountToAdd;
                arDepBalance.ConsigneeId = consigneeId;
                arDepBalance.ShipperId = shipperId;
                arDepBalance.CurrencyId = currId;
                db.ArCashDepositBalances.Add(arDepBalance);
            }
            else
            {
                db.ArCashDepositBalances.Attach(arDepBalance);
                arDepBalance.BalanceAmount = arDepBalance.BalanceAmount + amountToAdd;
              
            }

            db.SaveChanges();

            return isSaved;
        }

        /// <summary>
        /// Get current deposit balance for client and currency
        /// </summary>
        /// <param name="shipperId"></param>
        /// <param name="consigneeId"></param>
        /// <param name="currId"></param>
        /// <returns>decimal balance amount</returns>
        public static decimal GetDepositBalance(int? shipperId, int? consigneeId, int currId)
        {
            AccountingEntities db = new AccountingEntities();
            var arDepBalance = GetBalanceObj(shipperId, consigneeId, currId, db);

            return arDepBalance?.BalanceAmount ?? 0;
        }

        private static ArCashDepositBalance GetBalanceObj(int? shipperId, int? consigneeId,
            int currId, AccountingEntities db)
        {
           // AccountingEntities db = new AccountingEntities();

            //get balance record if any
            ArCashDepositBalance depBalance = null;

            if (shipperId != null)
                depBalance = db.ArCashDepositBalances.Where(x => x.ShipperId == shipperId && x.CurrencyId == currId).FirstOrDefault();
            else if (consigneeId != null)
                depBalance = db.ArCashDepositBalances.Where(x => x.ConsigneeId == consigneeId && x.CurrencyId == currId).FirstOrDefault();

            if(depBalance == null)
                depBalance = new ArCashDepositBalance();

            return depBalance;

        }


    }
}