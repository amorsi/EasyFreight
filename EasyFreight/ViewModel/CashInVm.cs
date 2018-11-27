using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class CashInVm
    {
        public int ReceiptId { get; set; }
        public string ReceiptCode { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public int? ShipperId { get; set; }
        public int? ConsigneeId { get; set; }
        public int? CarrierId { get; set; }
        public int? ContractorId { get; set; }
        public int? AgentId { get; set; }
        public string CustomerName { get; set; }
        public byte PaymentTermId { get; set; }
        public string PaymentTermName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Notes { get; set; }
        public decimal? ReceiptAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? TransId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }
        public int? BankId { get; set; }
        public int? BankAccId { get; set; }
        public byte OrderFrom { get; set; }
        public bool IsDeleted { get; set; }
        public string InvCodeListString { get; set; }
        public byte InvoiceType { get; set; }
        public string CashType { get; set; }
        public int? OperationId { get; set; }
        public string OperationCode { get; set; }
        public string CreatedByFullName { get; set; }
        public string PartnerAccountId { get; set; }
        public string PartnerName { get; set; }
        public string CreditAccountId { get; set; }
        public InvoiceVm InvsVm { get; set; }
        public string ReceivedByName { get; set; }
        public string BankNumber { get; set; }

        public List<CashInCheckVm> CashInReceiptChecks { get; set; }

        public List<CashInInvoiceVm> CashInReceiptInvs { get; set; }

        public List<CCCashDepositVm> CCCashDepositVmList { get; set; }

        public BankDetailsVm BankDetailsVm { get; set; }

        public List<CashOutExpense> CashOutReceiptExpenses { get; set; }


        public CashInVm()
        {

            ReceiptDate = DateTime.Now;
            CreateDate = DateTime.Now;
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
            CashInReceiptInvs = new List<CashInInvoiceVm>();
            CashInReceiptChecks = new List<CashInCheckVm>();
            CCCashDepositVmList = new List<CCCashDepositVm>();
            CashOutReceiptExpenses = new List<CashOutExpense>();
            BankDetailsVm = new BankDetailsVm();
        }

    }

    public class CashInCheckVm
    {
        public int CashInCheckId { get; set; }
        public int ReceiptId { get; set; }
        public string CheckNumber { get; set; }
        public DateTime? CheckDueDate { get; set; }
        public bool IsCollected { get; set; }
        public int BankId { get; set; }
        public string BankNameEn { get; set; }
    }

    public class CashInInvoiceVm
    {
        public int ReceiptId { get; set; }
        public int InvoiceId { get; set; }
        public int AgentNoteId { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string InvoiceCode { get; set; }
        public string AgentNoteCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencySign { get; set; }
        public string CurrencyId { get; set; }
        public decimal CollectedAmount { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime? DueDate { get; set; }
        public string InvStatusNameEn { get; set; }
        public bool IsSelected { get; set; }
        public string OperationCode { get; set; }
        public string HouseBL { get; set; }
        public CashInVm CashInReceipt { get; set; }

        public CashInInvoiceVm()
        {
            CashInReceipt = new CashInVm();
        }

    }

    public class CCCashDepositVm
    {
        public int ReceiptId { get; set; }
        public int OperationId { get; set; }
        public decimal? ReceiptAmount { get; set; }
        public string CurrencySign { get; set; }
        public DateTime CreateDate { get; set; }
        public string ReceiptCode { get; set; }

    }

    public class  CashOutExpense
    {
        public int ReceiptId { get; set; }
        public int ExpenseId { get; set; }
        public decimal? PaidAmount { get; set; }
        public string ExpenseName { get; set; }
        public string CurrencySign { get; set; }
    }



    
}