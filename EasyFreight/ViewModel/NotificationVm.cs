using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class NotificationTypeVm
    {
        public int NotificationTypeID { get; set; }
         public string NotificationTitle { get; set; } 
    }

    public class NotificationMsgVm
    {
        public int NotificationMsgID { get; set; }
        public int NotificationTypeID { get; set; }
        public int ObjectID { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationMsg { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}