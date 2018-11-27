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
    
    public partial class Notifier
    {
        public Notifier()
        {
            this.NotifierContacts = new HashSet<NotifierContact>();
        }
    
        public int NotifierId { get; set; }
        public string NotifierCode { get; set; }
        public string NotifierNameEn { get; set; }
        public string NotifierNameAr { get; set; }
        public string NotifierAddressEn { get; set; }
        public string NotifierAddressAr { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> CityId { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public Nullable<int> ConsigneeId { get; set; }
    
        public virtual City City { get; set; }
        public virtual Consignee Consignee { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<NotifierContact> NotifierContacts { get; set; }
    }
}
