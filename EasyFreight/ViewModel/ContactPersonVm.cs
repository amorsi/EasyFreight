using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class ContactPersonVm
    {
        public int ContactId { get; set; }
        public int FkValue { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}