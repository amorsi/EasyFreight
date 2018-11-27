using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;
using System.Data.Entity.Validation;
using EasyFreight.ViewModel;
using System.Text;
using System.Data.Entity.Infrastructure;

namespace EasyFreight.DAL
{
    public static class AccountingChartHelper
    {
        public static string AddAccountToChart(string accountNameEn, string accountNameAr, string parentAccountId)
        {
            AccountingEntities db = new AccountingEntities();
            // string accountRecCode = ((int) AccountingChartEnum.AccountsRecievable).ToString();
            // int addOne = db.AccountingCharts.Where(x => x.ParentAccountId == parentAccountId).Count() + 1;
            var addOne = db.AccountingCharts.Where(x => x.ParentAccountId == parentAccountId).ToList();
            string newAccountId = "";
            if (addOne.Count() > 0)
            {
                var accNumberAddOne = long.Parse(addOne.Max(x => long.Parse(x.AccountId)).ToString()
                .Replace(parentAccountId,"")) + 1;
                newAccountId = parentAccountId + accNumberAddOne;
            }

            else
                newAccountId = parentAccountId + "1";

            AccountingChart accChart = new AccountingChart()
            {
                AccountId = newAccountId,
                AccountNameAr = accountNameAr,
                AccountNameEn = accountNameEn,
                CreateDate = DateTime.Now,
                ParentAccountId = parentAccountId
            };

            db.AccountingCharts.Add(accChart);

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                newAccountId = "false " + e.Message;
                throw;
            }
            catch (Exception e)
            {
                newAccountId = "false " + e.InnerException;
                throw;
            }

            return newAccountId;
        }

        /// <summary>
        /// Will use it tho delete an account from the chart .. most of time when account is generated and the 
        /// main object insertion falied 
        /// </summary>
        /// <param name="accountId"></param>
        public static void DeleteAccount(string accountId)
        {
            AccountingEntities db = new AccountingEntities();
            var accountObj = db.AccountingCharts.Where(x => x.AccountId == accountId).FirstOrDefault();
            db.AccountingCharts.Remove(accountObj);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

        }



        /// <summary>
        /// Update AccountId column in any table
        /// </summary>
        /// <param name="accountId">AccountId</param>
        /// <param name="obj">Table Name which want to update</param>
        /// <param name="objId">record Id ..Table PK value</param>
        /// <param name="pkName">Table PK column name</param>
        public static void AddAccountIdToObj(string accountId, string obj, int objId, string pkName, string accountIdColumnName = "")
        {
            EasyFreightEntities db = new EasyFreightEntities();

            StringBuilder query = new StringBuilder();
            query.Append("Update ");
            query.Append(obj);
            if(string.IsNullOrEmpty(accountIdColumnName))
                query.Append(" Set AccountId = '");
            else
                query.Append(" Set " + accountIdColumnName + " = '");

            query.Append(accountId);
            query.Append("' Where ");
            query.Append(pkName);
            query.Append(" = ");
            query.Append(objId);

            db.Database.ExecuteSqlCommand(query.ToString());
        }

        /// <summary>
        /// Update TransactionID column in any table
        /// </summary>
        /// <param name="transId">TransactionID</param>
        /// <param name="obj">Table Name which want to update</param>
        /// <param name="objId">record Id ..Table PK value</param>
        /// <param name="pkName">Table PK column name</param>
        public static void AddTransIdToObj(int transId, string obj, int objId, string pkName)
        {
            AccountingEntities db = new AccountingEntities();
            StringBuilder query = new StringBuilder();
            query.Append("Update ");
            query.Append(obj);
            query.Append(" Set TransId = ");
            query.Append(transId);
            query.Append(" Where ");
            query.Append(pkName);
            query.Append(" = ");
            query.Append(objId);

            db.Database.ExecuteSqlCommand(query.ToString());
        }


        /// <summary>
        /// Get the AccountId for specific PK value and table name
        /// and update the accountId in shipper/ consignee table
        /// </summary>
        /// <param name="orderFrom"></param>
        /// <param name="shipperId"></param>
        /// <param name="consigneeId"></param>
        /// <returns>string Account Id</returns>
        public static string GetAccountIdByPkAndTbName(int pkValue, string tbName, string pkName)
        {
            string accountId = "";
            EasyFreightEntities db = new EasyFreightEntities();

            StringBuilder query = new StringBuilder();
            query.Append("SELECT AccountId FROM ");
            query.Append(tbName);
            query.Append(" Where ");
            query.Append(pkName);
            query.Append(" = ");
            query.Append(pkValue);

            DbRawSqlQuery<string> data = db.Database.SqlQuery<string>(query.ToString());
            accountId = data.FirstOrDefault();

            return accountId;
        }


        #region Add AccountId to specific tables
        /// <summary>
        /// Add Bank Account to The Accounting chart
        /// </summary>
        /// <param name="bankId">Bank Id .. optional .. if 0 .. will get it in code</param>
        /// <param name="bankAccountId">Bank Account Id</param>
        /// <returns>The Accounting chart Account Number</returns>
        public static string AddBankAccountToChart(int bankId, int bankAccountId)
        {
            string debitAccId;
            string parentAccountId = ((int)AccountingChartEnum.CashInBanks).ToString();
            if (bankId == 0)
            {
                AccountingEntities db = new AccountingEntities();
                bankId = db.BankAccounts.Where(x => x.BankAccId == bankAccountId).FirstOrDefault().BankId;
            }

            BankVm bankInfo = BankHelper.GetBankInfo(bankId);
            BankAccountVm bankAccount = bankInfo.BankAccounts.Where(x => x.BankAccId == bankAccountId).FirstOrDefault();
            string accNameEn = bankAccount.AccountName + " (" + bankAccount.AccountNumber + ")";
            //Add new accountId to the chart
            debitAccId = AccountingChartHelper
                .AddAccountToChart(accNameEn, accNameEn, parentAccountId);

            AccountingChartHelper.AddAccountIdToObj(debitAccId, "BankAccount", bankAccountId, "BankAccId");

            return debitAccId;
        }

        /// <summary>
        /// Add Agent To The Accounting chart .. 
        /// If note type = 1 "Debit" add under A/R .. else 2 "Credit" addd under A/P
        /// </summary>
        /// <param name="agentId">Agent Id</param>
        /// <param name="agentType">Agent Note Type</param>
        /// <returns></returns>
        public static string AddAgentToChart(int agentId, byte agentType)
        {
            string parentAccountId = "", accountId;
            if (agentType == 1) //Debit Note .. A/R
                parentAccountId = ((int)AccountingChartEnum.Agents).ToString();
            else
                parentAccountId = ((int)AccountingChartEnum.Agents).ToString();

            AgentVm agentObj = AgentHelper.GetAgentInfo(agentId);
            string accNameEn = agentObj.AgentNameEn;
            string accNameAr = string.IsNullOrEmpty(agentObj.AgentNameAr) ? agentObj.AgentNameEn : agentObj.AgentNameAr;

            //Add new accountId to the chart
            accountId = AccountingChartHelper
                .AddAccountToChart(accNameEn, accNameEn, parentAccountId);

            AccountingChartHelper.AddAccountIdToObj(accountId, "Agent", agentId, "AgentId");

            return accountId;
        }

        public static string AddCarrierToChart(int carrierId)
        {
            string parentAccountId = "", accountId;
            parentAccountId = ((int)AccountingChartEnum.APCarriers).ToString();
            var carrVm = CarrierHelper.GetCarrierInfo(carrierId);

            string accNameEn = carrVm.CarrierNameEn;
            string accNameAr = string.IsNullOrEmpty(carrVm.CarrierNameAr) ? carrVm.CarrierNameEn : carrVm.CarrierNameAr;

            //Add new accountId to the chart
            accountId = AccountingChartHelper
                .AddAccountToChart(accNameEn, accNameEn, parentAccountId);

            AccountingChartHelper.AddAccountIdToObj(accountId, "Carrier", carrierId, "CarrierId");

            return accountId;

        }

        public static string AddContractorToChart(int contrId)
        {
            string parentAccountId = "", accountId;
            parentAccountId = ((int)AccountingChartEnum.APContractors).ToString();
            var carrVm = ContractorHelper.GetContractorInfo(contrId);

            string accNameEn = carrVm.ContractorNameEn;
            string accNameAr = string.IsNullOrEmpty(carrVm.ContractorNameAr) ? carrVm.ContractorNameEn : carrVm.ContractorNameAr;

            //Add new accountId to the chart
            accountId = AccountingChartHelper
                .AddAccountToChart(accNameEn, accNameEn, parentAccountId);

            AccountingChartHelper.AddAccountIdToObj(accountId, "Contractor", contrId, "ContractorId");

            return accountId;

        }

        public static string AddShipperToChart(int shipeprId)
        {
            string parentAccountId = "", accountId;
            parentAccountId = ((int)AccountingChartEnum.AccountsRecievable).ToString();
            var carrVm = ShipperHelper.GetShipperInfo(shipeprId);

            string accNameEn = carrVm.ShipperNameEn;
            string accNameAr = string.IsNullOrEmpty(carrVm.ShipperNameAr) ? carrVm.ShipperNameAr : carrVm.ShipperNameEn;

            //Add new accountId to the chart
            accountId = AccountingChartHelper
                .AddAccountToChart(accNameEn, accNameEn, parentAccountId);

            AccountingChartHelper.AddAccountIdToObj(accountId, "Shipper", shipeprId, "ShipperId");

            return accountId;

        }

        public static string AddConsigneeToChart(int consigneeId)
        {
            string parentAccountId = "", accountId;
            parentAccountId = ((int)AccountingChartEnum.AccountsRecievable).ToString();
            var carrVm = ConsigneeHelper.GetConsigneeInfo(consigneeId);

            string accNameEn = carrVm.ConsigneeNameEn;
            string accNameAr = string.IsNullOrEmpty(carrVm.ConsigneeNameAr) ? carrVm.ConsigneeNameEn : carrVm.ConsigneeNameAr;

            //Add new accountId to the chart
            accountId = AccountingChartHelper
                .AddAccountToChart(accNameEn, accNameEn, parentAccountId);

            AccountingChartHelper.AddAccountIdToObj(accountId, "Consignee", consigneeId, "ConsigneeId");

            return accountId;

        }

        public static string AddCashToChart(int currencyId)
        {
            string parentAccountId = "", accountId;
            AccountingEntities db = new AccountingEntities();
            var currObj = db.CurrencyAccs.Where(x => x.CurrencyId == currencyId).FirstOrDefault();
            parentAccountId = ((int)AccountingChartEnum.Cash).ToString();
            //Add new accountId to the chart
            accountId = AccountingChartHelper
                .AddAccountToChart(currObj.CurrencySign, currObj.CurrencySign, parentAccountId);
            AccountingChartHelper.AddAccountIdToObj(accountId, "Currency", currObj.CurrencyId, "CurrencyId");

            return accountId;
        }

        public static string AddCCCashDepositToChart(int operationId)
        {
            string parentAccountId = "", accountId;
            OperationsEntities db = new OperationsEntities();
            var operationObj = db.Operations.Where(x => x.OperationId == operationId).FirstOrDefault();
            string operationCode = operationObj.OperationCode;
            string accNameEn = "CC Cash Deposit For Operation " + operationCode;
            string accNameAr = "عهدة تخليص جمركى للعملية " + operationCode;
            parentAccountId = ((int)AccountingChartEnum.CashDepositTemp).ToString();

            //Add new accountId to the chart
            accountId = AccountingChartHelper
                .AddAccountToChart(accNameEn, accNameAr, parentAccountId);
            operationObj.CCCashDepAccountId = accountId;
            db.SaveChanges();

            return accountId;
        }

        public static string GetCCCashDepAccountId(int operationId)
        {
            OperationsEntities db = new OperationsEntities();
            string accId = db.Operations.Where(x => x.OperationId == operationId).FirstOrDefault().CCCashDepAccountId;

            return accId;
        }


        #endregion
    }
}