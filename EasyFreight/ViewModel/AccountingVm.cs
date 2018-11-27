using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class OperationCostAccMainVm
    {
        public int OperationId { get; set; }
        public string OperationCode { get; set; }
        public string BookingNumber { get; set; }
        public string MBL { get; set; }
        public string ShipperName { get; set; }
        public string ConsigneeName { get; set; }
        public string ContainerSummary { get; set; }
        public string ClearanceByName { get; set; }
        public string TruckByName { get; set; }
        public string VesselName { get; set; }
        public string HouseBL { get; set; }
        public string CarrierName { get; set; }
        public string FromPort { get; set; }
        public string ToPort { get; set; }
        public string AgentName { get; set; }
        public string OrderFrom { get; set; }
        

        public IList<OperationCostAccVm> OperationCostAccVms { get; set; }
        public IList<OperationCostTotalAccVm> OperationCostTotalAccVms { get; set; }

        public OperationCostAccMainVm()
        {
            OperationCostAccVms = new List<OperationCostAccVm>();
            OperationCostTotalAccVms = new List<OperationCostTotalAccVm>();
        }
    }

    public class OperationCostAccVm
    {
        public int Id { get; set; }
        public string CostName { get; set; }
        public decimal NetRate { get; set; }
        public decimal SellingRate { get; set; }
        public string CurrencySign { get; set; }
        public bool IsAgent { get; set; }
        public int CurrencyId { get; set; }
        public byte FkType { get; set; }
        public int CostFkId { get; set; }
        public int CurrencyIdSelling { get; set; }
        public string CurrencySignSelling { get; set; }
        public int HouseBillId { get; set; }
    }

    public class OperationCostTotalAccVm
    {
        public decimal TotalNetRate { get; set; }
        public decimal TotalSellingRate { get; set; }
        public decimal TotalAgentRate { get; set; }
        public string CurrencySign { get; set; }
        public int CurrencyId { get; set; }
        public int CurrencyIdSelling { get; set; }
        public string CurrencySignSelling { get; set; }
    }

    public class AccTransactionVm
    {
        public int TransId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string TransactionName { get; set; }
        public string TransactionNameAr { get; set; }


        public List<AccTransactionDetailVm> AccTransactionDetails { get; set; }

        public AccTransactionVm()
        {
            AccTransactionDetails = new List<AccTransactionDetailVm>();
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
        }
    }

    public class AccTransactionDetailVm
    {
        public long TransDetailId { get; set; }
        public int TransId { get; set; }
        public string AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public int CurrencyId { get; set; }
    }

    public class OpenBalanceVm
    {
        public string TbName { get; set; }
        public int LibItemId { get; set; }
        public string AccountId { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsCreditAgent { get; set; }
        public string PkName { get; set; }
        public int TransId { get; set; }
        public List<OpenBalanceDetailVm> OpenBalanceDetails { get; set; }
        public OpenBalanceVm()
        {
            OpenBalanceDetails = new List<OpenBalanceDetailVm>();
        }
    }

    public class OpenBalanceDetailVm
    {
        public long TransDetailId { get; set; }
        public int CurrencyId { get; set; }
        public string AccountId { get; set; }
        public int TransId { get; set; }
        public string CurrencySign { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        //Will be used only in cash open balance
        public string CurrencyAccountId { get; set; }
        //Will be used only in cash open balance
        public int LibItemId { get; set; }
    }

    

}