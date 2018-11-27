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
    
    public partial class CashInReceiptView
    {
        public int ReceiptId { get; set; }
        public string ReceiptCode { get; set; }
        public System.DateTime ReceiptDate { get; set; }
        public Nullable<int> ShipperId { get; set; }
        public string ShipperNameEn { get; set; }
        public string ShipperNameAr { get; set; }
        public Nullable<int> ConsigneeId { get; set; }
        public string ConsigneeNameEn { get; set; }
        public string ConsigneeNameAr { get; set; }
        public Nullable<int> AgentId { get; set; }
        public string AgentNameEn { get; set; }
        public string AgentNameAr { get; set; }
        public string Notes { get; set; }
        public decimal ReceiptAmount { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string DeleteReason { get; set; }
        public Nullable<int> TransId { get; set; }
        public string CurrencySign { get; set; }
        public Nullable<int> OperationId { get; set; }
        public string PaymentTermEn { get; set; }
        public string PaymentTermAr { get; set; }
        public string CheckNumber { get; set; }
        public Nullable<System.DateTime> CheckDueDate { get; set; }
        public Nullable<bool> IsCollected { get; set; }
        public Nullable<int> CurrencyId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UserName { get; set; }
        public byte PaymentTermId { get; set; }
    }
}