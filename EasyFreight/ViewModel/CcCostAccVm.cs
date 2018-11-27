using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.ViewModel
{
    public class CcCostAccVm
    {
        public int CcCostAccId { get; set; }
        public int OperationId { get; set; }
        public string OperationCode { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Notes { get; set; }
        public List<CcCostAccDetailVm> CcCostAccDetails { get; set; }

        public CcCostAccVm()
        {
            CcCostAccDetails = new List<CcCostAccDetailVm>();
            CreateDate = DateTime.Now;
            CreateBy = EasyFreight.DAL.AdminHelper.GetCurrentUserId();
        }
    }

    public class CcCostAccDetailVm
    {
        public int CcCostAccDetId { get; set; }
        public int CcCostAccId { get; set; }
        public int CostFkId { get; set; }
        public byte FkType { get; set; }
        public decimal MainAmount { get; set; }
        public int MainCurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public int CurrencyId { get; set; }
        public string MainCurrSign { get; set; }
    }

}