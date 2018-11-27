using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class InvoiceMainVm
    {

        public int OperationId { get; set; }
        public int? HouseBillId { get; set; }
        public int? ShipperId { get; set; }
        public int? ConsigneeId { get; set; }
        public string CustomerName { get; set; }
        public int? CarrierId { get; set; }
        public int? ContractorId { get; set; }
        public string SupplierName { get; set; }
        public string OperationCode { get; set; }
        public string MBL { get; set; }
        public string FromPort { get; set; }
        public string ToPort { get; set; }
        public byte OrderFrom { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string CreateBy { get; set; }
        public string Notes { get; set; }
        public int InvStatusId { get; set; }
        public string InvStatusName { get; set; }
        public string CurrencySign { get; set; }
        

        public IList<OperationCostAccVm> OperationCostAccVms { get; set; }
        public IList<OperationCostTotalAccVm> OperationCostTotalAccVms { get; set; }
    }


    public class InvoiceVm : InvoiceMainVm
    {
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public byte PaymentTermId { get; set; }
        public string PaymentTermName { get; set; }
        public string HouseBL { get; set; }
        public byte InvoiceType { get; set; }
        public int InvCurrencyId { get; set; }
        public string APInvoiceCode { get; set; }
        
        public decimal AmountDue { get; set; }

        public List<InvoiceTotalVm> InvoiceTotals { get; set; }
        public List<InvoiceDetailVm> InvoiceDetails { get; set; }


        public InvoiceVm(byte invoiceType)
        {
            OperationCostAccVms = new List<OperationCostAccVm>();
            OperationCostTotalAccVms = new List<OperationCostTotalAccVm>();
            InvoiceDate = DateTime.Now;
            CreateDate = DateTime.Now;
            InvoiceType = invoiceType;
            InvStatusId = 2; // Approved .. no workfollow for now
            InvoiceTotals = new List<InvoiceTotalVm>();
            InvoiceDetails = new List<InvoiceDetailVm>();
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
            InvCurrencyId = 1; //EGP
            CurrencySign = "EGP";
        }

        public InvoiceVm()
        {

        }

    }

    public class InvoiceTotalVm
    {
        public int InvoiceId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxDepositAmount { get; set; }
        public decimal TotalBeforeTax { get; set; }
        public decimal VatTaxAmount { get; set; }
    }

    public class InvoiceLightVm
    {
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public byte InvoiceType { get; set; }
        public int HouseBillId { get; set; }
        public IList<OperationCostTotalAccVm> OperationCostTotalAccVms { get; set; }

        public InvoiceLightVm()
        {
            OperationCostTotalAccVms = new List<OperationCostTotalAccVm>();
        }
    }

    public class InvoiceDetailVm
    {
        public long InvDetailId { get; set; }
        public int InvoiceId { get; set; }
        public int CostFkId { get; set; }
        public string CostName { get; set; }
        public byte FkType { get; set; }
        public decimal MainAmount { get; set; }
        public decimal InvoiceAmount { get; set; }
        public int MainCurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public int CurrencyId { get; set; }
        public string MainCurrSign { get; set; }
        public bool IsSelected { get; set; }
        public int ItemOrder { get; set; }
        public string PrintedTitle { get; set; }

    }
}