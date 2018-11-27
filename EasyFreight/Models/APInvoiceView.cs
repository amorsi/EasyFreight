//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EasyFreight.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class APInvoiceView
    {
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string APInvoiceCode { get; set; }
        public int OperationId { get; set; }
        public Nullable<int> HouseBillId { get; set; }
        public Nullable<int> CarrierId { get; set; }
        public Nullable<int> ContractorId { get; set; }
        public string OperationCode { get; set; }
        public string BookingNumber { get; set; }
        public byte CarrierType { get; set; }
        public string MBL { get; set; }
        public string CarrierNameEn { get; set; }
        public string CarrierNameAr { get; set; }
        public int CurrencyId { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencySign { get; set; }
        public Nullable<byte> InvStatusId { get; set; }
        public string InvStatusNameEn { get; set; }
        public string InvStatusNameAr { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Notes { get; set; }
        public byte PaymentTermId { get; set; }
        public Nullable<int> TransId { get; set; }
        public int InvCurrencyId { get; set; }
        public string PaymentTermEn { get; set; }
        public string PaymentTermAr { get; set; }
        public byte OrderFrom { get; set; }
        public string ContractorNameEn { get; set; }
        public string ContractorNameAr { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string DeleteReason { get; set; }
        public byte InvoiceType { get; set; }
        public System.DateTime OperationDate { get; set; }
    }
}
