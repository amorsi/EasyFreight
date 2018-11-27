using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class AgentNoteVm : InvoiceMainVm
    {
        public int AgentNoteId { get; set; }
        public byte AgentNoteType { get; set; }
        public string AgentNoteCode { get; set; }
        public DateTime? AgentNoteDate { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string ContainerSummary { get; set; }
        public int BankAccId { get; set; }
        public int BankId { get; set; }
        public int CurrencyId { get; set; }
        public DateTime? DateOfDeparture { get; set; }
        public DateTime? ArriveDate { get; set; }
        public string VesselName { get; set; }
        public decimal TotalAmount { get; set; }

        public List<AgentNoteDetailVm> AgentNoteDetails { get; set; }

        public AgentNoteVm(byte noteType)
        {
            AgentNoteDate = DateTime.Now;
            CreateDate = DateTime.Now;
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
            OperationCostAccVms = new List<OperationCostAccVm>();
            OperationCostTotalAccVms = new List<OperationCostTotalAccVm>();
            InvStatusId = 2; // Approved .. no workfollow for now
            AgentNoteType = noteType;
            AgentNoteDetails = new List<AgentNoteDetailVm>();
        }

        public AgentNoteVm()
        {

        }
    }

    public class AgentNoteDetailVm
    {
        public int AgentNoteDetailId { get; set; }
        public int AgentNoteId { get; set; }
        public int OperCostId { get; set; }
        public int OperCostLibId { get; set; }
        public string CostName { get; set; }
        public decimal MainAmount { get; set; }
        public int MainCurrencyId { get; set; }
        public string MainCurrencySign { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal? AgentNoteAmount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }
        public bool IsSelected { get; set; }

    }


}