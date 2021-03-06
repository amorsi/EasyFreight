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
    
    public partial class OperationContainer
    {
        public long OperConId { get; set; }
        public int OperationId { get; set; }
        public int ContainerTypeId { get; set; }
        public string ContainerNumber { get; set; }
        public string SealNumber { get; set; }
        public Nullable<int> PackageTypeId { get; set; }
        public Nullable<int> NumberOfPackages { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public Nullable<decimal> CBM { get; set; }
    
        public virtual Operation Operation { get; set; }
    }
}
