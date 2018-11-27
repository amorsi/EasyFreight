using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;

namespace EasyFreight.DAL
{
    public static class HouseBillHelper
    {

        internal static HouseBillVm GetHbContent(int operationId, byte oprOrderFrom, int hbId = 0)
        {
            OperationsEntities db = new OperationsEntities();
            HouseBillVm houseBillVm = new HouseBillVm(oprOrderFrom);
            HouseBill houseBillDb = null;
            //if(hbId == 0) // add new house bill
            //    houseBillDb = db.HouseBills.Where(x => x.OperationId == operationId).FirstOrDefault();
            if (hbId != 0) // add new house bill
                houseBillDb = db.HouseBills.Where(x => x.HouseBillId == hbId).FirstOrDefault();


            if (houseBillDb == null)
            {
                Operation operationDb = db.Operations.Where(x => x.OperationId == operationId).FirstOrDefault();

                Mapper.CreateMap<Operation, HouseBillVm>()
                    .ForMember(x => x.OperationDate, y => y.Ignore())
                    .ForMember(x => x.CreateBy, y => y.Ignore())
                    .ForMember(x => x.CreateDate, y => y.Ignore())
                    .ForMember(x => x.StatusId, y => y.Ignore())
                    .ForMember(x => x.CollectedFreightCost, y => y.MapFrom(s => s.FreightCostAmount))
                    .ForMember(x => x.CollectedThcCost, y => y.MapFrom(s => s.ThcCostAmount))
                    .ForMember(x => x.OperationMinDate, y => y.MapFrom(s => s.OperationDate));

                Mapper.Map(operationDb, houseBillVm);

                //Check if has another HBLs get only the remaining amounts
                decimal nw = 0, gw = 0, cbm = 0, freightCost = 0, thcCost = 0;
                int packagesNum = 0;
                int hbCount = GetHBCount(operationId);
                if (hbCount > 0)
                {
                    var pervSum = db.HouseBills.Where(x => x.OperationId == operationId)
                        .Select(x => new
                        {
                            x.OperationId,
                            x.NetWeight,
                            x.GrossWeight,
                            x.CBM,
                            x.NumberOfPackages,
                            x.CollectedFreightCost,
                            x.CollectedThcCost
                        })
                        .GroupBy(s => s.OperationId)
                        .Select(g => new
                        {
                            nw = g.Sum(x => x.NetWeight),
                            gw = g.Sum(x => x.GrossWeight),
                            cbm = g.Sum(x => x.CBM),
                            packagesNum = g.Sum(x => x.NumberOfPackages),
                            freightCost = g.Sum(x => x.CollectedFreightCost),
                            thcCost = g.Sum(x => x.CollectedThcCost)
                        }).FirstOrDefault();

                    if (operationDb.NetWeight != null)
                        nw = operationDb.NetWeight.Value - pervSum.nw.Value;
                    if (operationDb.GrossWeight != null)
                        gw = operationDb.GrossWeight.Value - pervSum.gw.Value;
                    if (operationDb.CBM != null)
                        cbm = operationDb.CBM.Value - pervSum.cbm.Value;

                    if (operationDb.NumberOfPackages != null)
                        packagesNum = operationDb.NumberOfPackages.Value - pervSum.packagesNum.Value;

                    if (operationDb.FreightCostAmount != null)
                        freightCost = operationDb.FreightCostAmount.Value - pervSum.freightCost.Value;
                    if (operationDb.ThcCostAmount != null)
                        thcCost = operationDb.ThcCostAmount.Value - pervSum.thcCost.Value;

                    houseBillVm.NetWeight = nw;
                    houseBillVm.GrossWeight = gw;
                    houseBillVm.CBM = cbm;
                    houseBillVm.NumberOfPackages = packagesNum;
                    houseBillVm.CollectedFreightCost = freightCost;
                    houseBillVm.CollectedThcCost = thcCost;

                }

            }
            else
            {
                Mapper.CreateMap<HouseBill, HouseBillVm>().IgnoreAllNonExisting();
                Mapper.Map(houseBillDb, houseBillVm);
                //get departure and arrive date
                var operObj = db.Operations
                    .Where(x => x.OperationId == operationId)
                    .Select(x => new { x.DateOfDeparture, x.ArriveDate, x.OperationDate }).FirstOrDefault();

                houseBillVm.DateOfDeparture = operObj.DateOfDeparture;
                houseBillVm.ArriveDate = operObj.ArriveDate;
                houseBillVm.OperationMinDate = operObj.OperationDate;

            }

            return houseBillVm;

        }

        internal static string AddEditHouseBill(HouseBillVm houseBillVm)
        {
            string isSaved = "true";
            int houseBillId = houseBillVm.HouseBillId;
            OperationsEntities db = new OperationsEntities();
            HouseBill houseBillDb;
            if (houseBillId == 0)
            {
                houseBillDb = new HouseBill();
            }
            else
            {
                houseBillDb = db.HouseBills.Where(x => x.HouseBillId == houseBillId).FirstOrDefault();
            }

            Mapper.CreateMap<HouseBillVm, HouseBill>().IgnoreAllNonExisting();
            Mapper.Map(houseBillVm, houseBillDb);

            if (houseBillId == 0)
            {
                db.HouseBills.Add(houseBillDb);
                int operationId = houseBillDb.OperationId;
                Operation operationObj = db.Operations.Where(x => x.OperationId == operationId).FirstOrDefault();
                operationObj.StatusId = 2;
            }

            try
            {
                db.SaveChanges();
                isSaved = "true_" + houseBillDb.HouseBillId;
            }
            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.InnerException.InnerException;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.InnerException.InnerException;
            }

            return isSaved;
        }

        internal static List<HouseBillListVm> GetHBList(int operationId, byte orderFrom, bool includeInvList = false)
        {
            OperationsEntities db = new OperationsEntities();
            List<HouseBillListVm> hbList = db.HouseBills
                .Where(x => x.OperationId == operationId)
                .Select(x => new HouseBillListVm
                {
                    HouseBillId = x.HouseBillId,
                    OperationCode = x.OperationCode,
                    OperationDate = x.OperationDate,
                    ShipperId = x.ShipperId,
                    ConsigneeId = x.ConsigneeId,
                    HouseBL = x.HouseBL,
                    ClientName = "",
                    NumberOfPackages = x.NumberOfPackages,
                    OperationId = x.OperationId,
                    CarrierType = x.Operation.CarrierType,
                    StatusId = x.StatusId,
                    FreightCostAmount = x.CollectedFreightCost,
                    ThcCostAmount = x.CollectedThcCost,
                    FreighCurrencySign = x.Currency1.CurrencySign,
                    ThcCurrencySign = x.Currency.CurrencySign,
                    AgentId = x.AgentId
                })
                .ToList();



            EasyFreightEntities db1 = new EasyFreightEntities();
            if (orderFrom == 1) //Export .. client will be the shipper
            {
                List<int> shipperIds = hbList.Select(x => x.ShipperId).ToList();
                var shipperList = db1.Shippers
                    .Where(x => shipperIds.Contains(x.ShipperId))
                    .Select(x => new { x.ShipperId, x.ShipperNameEn, x.ShipperNameAr }).ToList();
                foreach (var item in hbList)
                {
                    item.ClientName = shipperList.Where(x => x.ShipperId == item.ShipperId).FirstOrDefault().ShipperNameEn;
                }
            }
            else //Export .. client will be consignee 
            {
                List<int> consigneeIds = hbList.Select(x => x.ConsigneeId).ToList();
                var consigneeList = db1.Consignees
                        .Where(x => consigneeIds.Contains(x.ConsigneeId))
                        .Select(x => new { x.ConsigneeId, x.ConsigneeNameEn, x.ConsigneeNameAr }).ToList();
                foreach (var item in hbList)
                {
                    item.ClientName = consigneeList.Where(x => x.ConsigneeId == item.ConsigneeId).FirstOrDefault().ConsigneeNameEn;
                }
            }

            if (includeInvList)
            {
                foreach (var item in hbList)
                {
                    var invList = InvoiceHelper.GetInvListForHb(item.HouseBillId);
                    item.InvoiceList = invList;
                }
            }

            return hbList;
        }

        internal static int GetHBCount(int operationId)
        {
            OperationsEntities db = new OperationsEntities();
            int count = db.HouseBills.Where(x => x.OperationId == operationId).Count();
            return count;
        }

        internal static HouseBillView GetHBView(int hbId)
        {
            OperationsEntities db = new OperationsEntities();
            HouseBillView hbView = db.HouseBillViews.Where(x => x.HouseBillId == hbId).FirstOrDefault();
            return hbView;
        }

        public static HouseBillView GetHBViewInfo(int hbId)
        {
            OperationsEntities db = new OperationsEntities();
            HouseBillView hbView = db.HouseBillViews.Where(x => x.HouseBillId == hbId).FirstOrDefault();
            return hbView;
        }

        /// <summary>
        /// Get cost list for House bill.. 
        /// </summary>
        /// <param name="houseBillId"></param>
        /// <param name="operationId"></param>
        /// <returns>OperationCostMainVm which contains list of OperationCostVm</returns>
        internal static OperationCostMainVm GetHBCost(int houseBillId, int operationId)
        {
            OperationsEntities db = new OperationsEntities();
            var operCostDb = db.OperationCosts.Where(x => x.HouseBillId == houseBillId).ToList();
            OperationCostMainVm operCostMainVm = new OperationCostMainVm();
            if (operCostDb.Count == 0)
            {
                OperationCostVm operCostVm = new OperationCostVm() { HouseBillId = houseBillId, OperationId = operationId };
                operCostMainVm.OperationCosts.Add(operCostVm);
            }
            else
            {
                OperationCostVm operCostVm;
                Mapper.CreateMap<OperationCost, OperationCostVm>().IgnoreAllNonExisting();
                foreach (var item in operCostDb)
                {
                    operCostVm = new OperationCostVm();
                    Mapper.Map(item, operCostVm);
                    //if operation id = 0 .. view mode .. load all data
                    if (operationId == 0)
                    {
                        var costLib = item.OperationCostLib;
                        operCostVm.OperCostNameEn = costLib.OperCostNameEn;
                        operCostVm.OperCostNameAr = costLib.OperCostNameAr;
                        operCostVm.CurrencySign = item.Currency.CurrencySign;
                    }
                    operCostMainVm.OperationCosts.Add(operCostVm);
                }
                OperationCostTotalsVm costTotals;
                var totalsObj = operCostMainVm.OperationCosts.GroupBy(g => g.CurrencySign)
                    .Select(x => new
                    {
                        CurrSign = x.Key,
                        TotalSelling = x.Sum(g => g.OperationCostSelling),
                        TotalNet = x.Sum(g => g.OperationCostNet)
                    }).ToList();

                foreach (var item in totalsObj)
                {
                    costTotals =
                        new OperationCostTotalsVm()
                        {
                            CurrencySign = item.CurrSign,
                            TotalCostNet = item.TotalNet,
                            TotalCostSelling = item.TotalSelling
                        };

                    operCostMainVm.Totals.Add(costTotals);
                }
            }
            return operCostMainVm;
        }

        internal static string AddEditHbCost(OperationCostMainVm operCostMainVm)
        {
            OperationsEntities db = new OperationsEntities();
            var operCostVmList = operCostMainVm.OperationCosts;
            int hbId = operCostVmList.FirstOrDefault().HouseBillId;
            //Get costId sent from screen
            List<int> screenCostIds = operCostVmList.Select(x => x.OperCostId).ToList();
            //Get db saved list
            var operCostDbList = db.OperationCosts.Where(x => x.HouseBillId == hbId).ToList();

            //delete removed items
            var costToDel = operCostDbList.Where(x => !screenCostIds.Contains(x.OperCostId)).ToList();

            foreach (var item in operCostDbList)
            {
                db.OperationCosts.Remove(item);
            }



            //Map the Vm to db list
            Mapper.CreateMap<OperationCostVm, OperationCost>().IgnoreAllNonExisting();
            Mapper.Map(operCostVmList, operCostDbList);

            string isSaved = "true";
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    db.SaveChanges();

                    using (var dbCtx = new OperationsEntities())
                    {
                        foreach (var item in operCostDbList)
                        {
                            if (item.HouseBillId != 0)
                                dbCtx.OperationCosts.Add(item);
                            //  else
                            //      dbCtx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }

                        dbCtx.SaveChanges();
                    }

                    transaction.Complete();

                }

                catch (DbEntityValidationException e)
                {
                    isSaved = "false " + e.Message;
                }
                catch (Exception e)
                {
                    isSaved = "false " + e.InnerException;
                }
            }



            return isSaved;
        }

        internal static string ChangeHBStatus(int hbId, byte statusId)
        {
            string isClosed = "true";
            OperationsEntities db = new OperationsEntities();

            int operCostCount = db.OperationCosts.Where(x => x.HouseBillId == hbId).Count();
            if (operCostCount == 0)
                return "Please add operation cost before close it.";

            HouseBill hbObj = db.HouseBills.Where(x => x.HouseBillId == hbId).FirstOrDefault();
            hbObj.StatusId = statusId; //closed
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                isClosed = "false " + e.Message;
            }
            catch (Exception e)
            {
                isClosed = "false " + e.InnerException;
            }
            return isClosed;
        }

        internal static ConcessionLetterVm GetDeliveryNoteInfo(int hbId, string langCode)
        {
            ConcessionLetterVm concLetterVm = new ConcessionLetterVm();
            OperationsEntities db = new OperationsEntities();
            var hbView = db.HouseBillViews.Where(x => x.HouseBillId == hbId).Select(x => new
                {
                    x.CarrierNameEn,
                    x.FromPort,
                    x.MBL,
                    x.GrossWeight,
                    x.CBM,
                    x.ConsigneeNameEn,
                    x.ToPort,
                    x.CarrierType,
                    x.NumberOfPackages,
                    x.OperationId,
                    x.VesselName,
                    x.ArriveDate,
                    x.HouseBL,
                    x.GoodsDescription

                }).FirstOrDefault();

            int operId = hbView.OperationId;
            concLetterVm.CarrierName = hbView.CarrierNameEn;
            concLetterVm.FromPort = hbView.FromPort;
            concLetterVm.MBL = hbView.MBL;
            concLetterVm.GrossWeight = hbView.GrossWeight.Value;
            concLetterVm.CBM = hbView.CBM.Value;
            concLetterVm.ConsigneeName = hbView.ConsigneeNameEn;
            concLetterVm.Containers = OperationHelper.GetContainersSummary(operId);
            concLetterVm.NumberOfPackages = hbView.NumberOfPackages.Value;
            concLetterVm.ToPort = hbView.ToPort;
            concLetterVm.VesselName = hbView.VesselName;
            concLetterVm.ArriveDate = hbView.ArriveDate;
            concLetterVm.HouseBL = hbView.HouseBL;
            concLetterVm.GoodsDescription = hbView.GoodsDescription;

            var containerList = db.OperationContainers.Where(x => x.OperationId == operId).Select(x => x.ContainerNumber).ToArray();
            concLetterVm.ContainerNumber = string.Join(" - ", containerList);

            concLetterVm.StaticLabels = CommonHelper.GetStaticLabels((int)StaticTextForScreenEnum.DeliveryNote, langCode);

            return concLetterVm;
        }


        #region House Container...
        internal static bool CreateHouseContainer(HouseContainerVm hosCon)
        {
            return HouseContainerModels.Instance.Create(hosCon);
        }
        internal static bool DeleteByHouseContainer(HouseContainerVm hosCon)
        {
            return HouseContainerModels.Instance.Delete(hosCon);
        }

        internal static List<HouseContainerVm> GetHousContainerByHouseID(int houseBillId)
        {
            return HouseContainerModels.Instance.GetHousContainerByHouseID(houseBillId);
        }
        internal static List<HouseContainerVm> GetHousContainerByOperationID(int operationId)
        {
            return HouseContainerModels.Instance.GetHousContainerByOperationID(operationId);
        }
        #endregion



        //kamal
        internal static HouseBillListVm GetHbContent(int hbId)
        {
            OperationsEntities db = new OperationsEntities();



            HouseBillListVm hbList = db.HouseBills
               .Where(x => x.HouseBillId == hbId)
               .Select(x => new HouseBillListVm
               {
                   HouseBillId = x.HouseBillId,
                   OperationCode = x.OperationCode,
                   OperationDate = x.OperationDate,
                   ShipperId = x.ShipperId,
                   ConsigneeId = x.ConsigneeId,
                   HouseBL = x.HouseBL,
                   ClientName = "",
                   NumberOfPackages = x.NumberOfPackages,
                   OperationId = x.OperationId,
                   CarrierType = x.Operation.CarrierType,
                   StatusId = x.StatusId,
                   FreightCostAmount = x.CollectedFreightCost,
                   ThcCostAmount = x.CollectedThcCost,
                   FreighCurrencySign = x.Currency1.CurrencySign,
                   ThcCurrencySign = x.Currency.CurrencySign,
                   AgentId = x.AgentId
               })
               .FirstOrDefault();


            Operation operationDb = db.Operations.Where(x => x.OperationId == hbList.OperationId).FirstOrDefault();

            EasyFreightEntities db1 = new EasyFreightEntities();
            if (operationDb.OrderFrom == 1) //Export .. client will be the shipper
            {

                var shipperList = db1.Shippers
                    .Where(x => x.ShipperId == hbList.ShipperId)
                    .Select(x => new { x.ShipperId, x.ShipperNameEn, x.ShipperNameAr }).ToList();

                hbList.ClientName = shipperList.Where(x => x.ShipperId == hbList.ShipperId).FirstOrDefault().ShipperNameEn;

            }
            else //Export .. client will be consignee 
            {
                //List<int> consigneeIds = hbList.Select(x => x.ConsigneeId).ToList();
                var consigneeList = db1.Consignees
                        .Where(x => x.ConsigneeId == hbList.ConsigneeId)
                        .Select(x => new { x.ConsigneeId, x.ConsigneeNameEn, x.ConsigneeNameAr }).ToList();

                hbList.ClientName = consigneeList.Where(x => x.ConsigneeId == hbList.ConsigneeId).FirstOrDefault().ConsigneeNameEn;
            }
            return hbList;
        }
        internal static OperationCostMainVm GetHBCost(int houseBillId)
        {
            OperationsEntities db = new OperationsEntities();
            var operCostDb = db.OperationCosts.Where(x => x.HouseBillId == houseBillId && x.IsAccounting == true).ToList();
            OperationCostMainVm operCostMainVm = new OperationCostMainVm();
            if (operCostDb.Count == 0)
            {
                OperationCostVm operCostVm = new OperationCostVm() { HouseBillId = houseBillId };
                operCostMainVm.OperationCosts.Add(operCostVm);
            }
            else
            {
                OperationCostVm operCostVm;
                Mapper.CreateMap<OperationCost, OperationCostVm>().IgnoreAllNonExisting();
                foreach (var item in operCostDb)
                {
                    operCostVm = new OperationCostVm();
                    Mapper.Map(item, operCostVm);
                    //if operation id = 0 .. view mode .. load all data
                   

                        var costLib = item.OperationCostLib;
                        operCostVm.OperCostNameEn = costLib.OperCostNameEn;
                        operCostVm.OperCostNameAr = costLib.OperCostNameAr;
                        operCostVm.CurrencySign = item.Currency.CurrencySign;
                   

                    operCostMainVm.OperationCosts.Add(operCostVm);
                }
                OperationCostTotalsVm costTotals;
                var totalsObj = operCostMainVm.OperationCosts.GroupBy(g => g.CurrencySign)
                    .Select(x => new
                    {
                        CurrSign = x.Key,
                        TotalSelling = x.Sum(g => g.OperationCostSelling),
                        TotalNet = x.Sum(g => g.OperationCostNet)
                    }).ToList();

                foreach (var item in totalsObj)
                {
                    costTotals =
                        new OperationCostTotalsVm()
                        {
                            CurrencySign = item.CurrSign,
                            TotalCostNet = item.TotalNet,
                            TotalCostSelling = item.TotalSelling
                        };

                    operCostMainVm.Totals.Add(costTotals);
                }
            }
            return operCostMainVm;
        }
        internal static string AddEditAccountHbCost(OperationCostMainVm operCostMainVm)
        {
            OperationsEntities db = new OperationsEntities();
            var operCostVmList = operCostMainVm.OperationCosts;
            int hbId = operCostVmList.FirstOrDefault().HouseBillId;
            //Get costId sent from screen
            List<int> screenCostIds = operCostVmList.Select(x => x.OperCostId).ToList();
            //Get db saved list
            var operCostDbList = db.OperationCosts.Where(x => x.HouseBillId == hbId && x.IsAccounting==true).ToList();

            //delete removed items
            var costToDel = operCostDbList.Where(x => !screenCostIds.Contains(x.OperCostId)).ToList();

            foreach (var item in operCostDbList)
            {
                db.OperationCosts.Remove(item);
            }



            //Map the Vm to db list
            Mapper.CreateMap<OperationCostVm, OperationCost>().IgnoreAllNonExisting();
            Mapper.Map(operCostVmList, operCostDbList);

            string isSaved = "true";
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    db.SaveChanges();

                    using (var dbCtx = new OperationsEntities())
                    {
                        foreach (var item in operCostDbList)
                        {
                            if (item.HouseBillId != 0)
                                dbCtx.OperationCosts.Add(item);
                            //  else
                            //      dbCtx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }

                        dbCtx.SaveChanges();
                    }

                    transaction.Complete();

                }

                catch (DbEntityValidationException e)
                {
                    isSaved = "false " + e.Message;
                }
                catch (Exception e)
                {
                    isSaved = "false " + e.InnerException;
                }
            }



            return isSaved;
        }
        
    }
}