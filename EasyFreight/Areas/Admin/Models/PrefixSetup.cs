//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EasyFreight.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PrefixSetup
    {
        public byte PrefixForId { get; set; }
        public string PrefixFor { get; set; }
        public string PrefixChar { get; set; }
        public Nullable<bool> NumberAfterChar { get; set; }
        public Nullable<bool> IncludeMonth { get; set; }
        public Nullable<bool> IncludeYear { get; set; }
        public string Delimiter { get; set; }
        public string ResetNumberEvery { get; set; }
    
        public virtual PrefixLastId PrefixLastId { get; set; }
    }
}