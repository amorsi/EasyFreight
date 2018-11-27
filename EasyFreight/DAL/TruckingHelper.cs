using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using AutoMapper;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace EasyFreight.DAL
{
    public static class TruckingHelper
    {

        public static TruckingOrderDetailVm GetTruckingOrderDetailInfo(int truckingOrderId)
        {
            TruckingOrderDetailVm trkoVmObj = new TruckingOrderDetailVm(truckingOrderId);
            EasyFreightEntities db = new EasyFreightEntities();

            int truckDetailCount = db.TruckingOrderDetails.Where(x => x.TruckingOrderId == truckingOrderId).Count();

            OperationsEntities db1 = new OperationsEntities();
            var truckViewObj = db1.TruckingOrdersViews.Where(x => x.TruckingOrderId == truckingOrderId).FirstOrDefault();

            Mapper.CreateMap<TruckingOrdersView, TruckingOrderDetailVm>().IgnoreAllNonExisting();

            Mapper.Map(truckViewObj, trkoVmObj);

            trkoVmObj.ClientName = truckViewObj.OrderFrom == 1 ? truckViewObj.ShipperNameEn : truckViewObj.ConsigneeNameEn;
            trkoVmObj.ContainersSummary = OperationHelper.GetContainersSummary(trkoVmObj.OperationId);

            List<TruckingOrderCost> truckCostList = db.TruckingOrderCosts.Where(x => x.TruckingOrderId == truckingOrderId).ToList();
            TruckingCostVm truckingCostObj;
            if (truckCostList.Count() == 0)
            {
                truckingCostObj = new TruckingCostVm(trkoVmObj.TruckingOrderId);
                trkoVmObj.TruckingOrderCosts.Add(truckingCostObj);
            }
            else
            {
                Mapper.CreateMap<TruckingOrderCost, TruckingCostVm>().IgnoreAllNonExisting();
                trkoVmObj.TotalCostNet = Math.Round(truckCostList.Sum(x => x.TruckingCostNet), 2);
                trkoVmObj.TotalCostSelling = Math.Round(truckCostList.Sum(x => x.TruckingCostSelling), 2);
                foreach (var cost in truckCostList)
                {
                    truckingCostObj = new TruckingCostVm();
                    Mapper.Map(cost, truckingCostObj);
                    truckingCostObj.TruckingCostSelling = Math.Round(truckingCostObj.TruckingCostSelling.Value, 2);
                    truckingCostObj.TruckingCostNet = Math.Round(truckingCostObj.TruckingCostNet.Value, 2);
                    truckingCostObj.TruckingCostName = cost.TruckingCostLib.TruckingCostName;
                    truckingCostObj.CurrencySign = cost.Currency.CurrencySign;

                    trkoVmObj.TruckingOrderCosts.Add(truckingCostObj);
                }
            }

            List<OperationContainerVm> operationContainerList = OperationHelper.GetOperationContainers(trkoVmObj.OperationId);
            if (operationContainerList.Count() > 0)
            {
                //kamal
                int truckHouseId = db.TruckingOrders.Where(x => x.TruckingOrderId == truckingOrderId).Select(a=>a.HouseBillId.Value).FirstOrDefault();
                List<HouseContainerVm> hcVmList = HouseBillHelper.GetHousContainerByHouseID(truckHouseId);
                if (hcVmList.Count > 0)
                {
                    foreach (var opcont in operationContainerList)
                    {
                        foreach (var item in hcVmList)
                        {
                            if(item.OperConId == opcont.OperConId)
                                trkoVmObj.OperationContainers.Add(opcont);
                        }
                        
                    }
                 }
                else
                {
                    foreach (var opcont in operationContainerList)
                    {
                        trkoVmObj.OperationContainers.Add(opcont);
                    }
                }
            }

            trkoVmObj.HbDate = db1.HouseBills
                .Where(x => x.HouseBillId == truckViewObj.HouseBillId).FirstOrDefault().OperationDate.Value;

            //Get code generated if first insert
            if (truckDetailCount == 0)
                trkoVmObj.TruckingOrderCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.TruckingOrder, false);

            return trkoVmObj;
        }

        public static string AddEditTruckingOrderDetails(TruckingOrderDetailVm truckingOrderVm)
        {
            int truckingOrderId = truckingOrderVm.TruckingOrderId;
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            TruckingOrderDetail orderDetailDb;
            //check if detauils is inserted before 
            int detailCount = db.TruckingOrderDetails.Where(x => x.TruckingOrderId == truckingOrderId).Count();

            TruckingOrder orderDb;
            orderDb = db.TruckingOrders.Where(x => x.TruckingOrderId == truckingOrderId).FirstOrDefault();
            // change the status
            if (truckingOrderVm.StatusId == 3)
            {
                orderDb.StatusId = 3; //closed
            }

            List<TruckingOrderCost> dbOrderCostList = new List<TruckingOrderCost>();
            if (detailCount == 0) //Add new case
                orderDetailDb = new TruckingOrderDetail();
            else
            {
                orderDetailDb = db.TruckingOrderDetails.Include("TruckingOrderCosts").Where(x => x.TruckingOrderId == truckingOrderId).FirstOrDefault();

                //delete any removed costs on the screen
                dbOrderCostList = orderDetailDb.TruckingOrderCosts.ToList();

                //Get cost Ids sent from the screen
                List<int> truckingOrderCostVmIds = truckingOrderVm.TruckingOrderCosts.Select(x => x.TruckingOrderCostId).ToList();
                var orderCostDel = dbOrderCostList.Where(x => !truckingOrderCostVmIds.Contains(x.TruckingOrderCostId)).ToList();

                foreach (var item in orderCostDel)
                {
                    db.TruckingOrderCosts.Remove(item);
                }


            }

            Mapper.CreateMap<TruckingOrderDetailVm, TruckingOrderDetail>().IgnoreAllNonExisting();
            Mapper.CreateMap<TruckingCostVm, TruckingOrderCost>().IgnoreAllNonExisting();
            Mapper.Map(truckingOrderVm, orderDetailDb);

            if (detailCount == 0)
            {
                orderDetailDb.TruckingOrderCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.TruckingOrder, true);
                db.TruckingOrderDetails.Add(orderDetailDb);

                if (truckingOrderVm.StatusId != 3)
                    orderDb.StatusId = 2; // open
            }

            List<OperationContainer> dbOperationContainerList = new List<OperationContainer>();
            OperationsEntities opdb = new OperationsEntities();

            dbOperationContainerList = opdb.OperationContainers.Where(x => x.OperationId == truckingOrderVm.OperationId).ToList();

            foreach (var item in truckingOrderVm.OperationContainers)
            {
                long operConIdvmId = item.OperConId;
                OperationContainer operationContainerDb = dbOperationContainerList
                    .Where(x => x.OperConId == operConIdvmId).FirstOrDefault();
                operationContainerDb.ContainerNumber = item.ContainerNumber;
                operationContainerDb.SealNumber = item.SealNumber;
                //Mapper.Map(item, operationContainerDb);
            }

            try
            {
                db.SaveChanges();

                opdb.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.Message;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.InnerException.InnerException.Message;
            }

            return isSaved;
        }

        public static JObject GetAllOrders(FormCollection form = null)
        {
            OperationsEntities db = new OperationsEntities();
            //List<TruckingOrdersView> truckingOrders = new List<TruckingOrdersView>();
            TruckingOrdersView truckObj = new TruckingOrdersView();
            //  if (form.Count > 0)
            //  {
            string where = CommonHelper.AdvancedSearch<TruckingOrdersView>(form, truckObj);
            var truckingOrders = db.TruckingOrdersViews.Where(where.ToString())
                  .Select(x => new
                  {
                      x.ArriveDate,
                      x.TruckingOrderId,
                      x.OperationId,
                      x.OrderFrom,
                      x.StatusId,
                      x.OperationCode,
                      x.CreateDate,
                      x.ShipperNameEn,
                      x.ConsigneeNameEn,
                      x.BookingNo,
                      x.ContractorNameEn,
                      x.NeedArriveDate,
                      x.NeedArriveTime,
                      x.ArriveTime,
                      x.StatusName,
                      x.HouseBL
                  }).ToList();
            //  }
            //else
            //{
            //    truckingOrders = db.TruckingOrdersViews.Where(x => x.StatusId == 1 || x.StatusId == 2)
            //        .OrderByDescending(x => x.CreateDate).ToList();

            //}

            //truckingOrders = truckingOrders.OrderByDescending(x => x.CreateDate).ToList();

            //if (orderFrom != 0)
            //    truckingOrders = truckingOrders.Where(x => x.OrderFrom == orderFrom).ToList();

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in truckingOrders)
            {
                pJTokenWriter.WriteStartObject();
                pJTokenWriter.WritePropertyName("TruckingOrderId");
                pJTokenWriter.WriteValue(item.TruckingOrderId);

                pJTokenWriter.WritePropertyName("OperationId");
                pJTokenWriter.WriteValue(item.OperationId);

                pJTokenWriter.WritePropertyName("OrderFromImg");
                switch (item.OrderFrom)
                {
                    case 1:
                        pJTokenWriter.WriteValue("<i class='fa fa-level-up'></i>");
                        break;
                    case 2:
                        pJTokenWriter.WriteValue("<i class='fa fa-level-down'></i>");
                        break;
                }

                pJTokenWriter.WritePropertyName("StatusId");
                pJTokenWriter.WriteValue(item.StatusId);

                pJTokenWriter.WritePropertyName("OperationCode");
                pJTokenWriter.WriteValue(item.OperationCode);

                pJTokenWriter.WritePropertyName("HouseBL");
                pJTokenWriter.WriteValue(item.HouseBL);

                pJTokenWriter.WritePropertyName("CreateDate");
                pJTokenWriter.WriteValue(item.CreateDate.Value.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("Client");
                if (!string.IsNullOrEmpty(item.ShipperNameEn))
                    pJTokenWriter.WriteValue(item.ShipperNameEn);
                else if (!string.IsNullOrEmpty(item.ConsigneeNameEn))
                    pJTokenWriter.WriteValue(item.ConsigneeNameEn);
                else
                    pJTokenWriter.WriteValue("");

                pJTokenWriter.WritePropertyName("BookingNo");
                pJTokenWriter.WriteValue(item.BookingNo);

                pJTokenWriter.WritePropertyName("ContractorName");
                pJTokenWriter.WriteValue(item.ContractorNameEn);

                pJTokenWriter.WritePropertyName("NeedArrive");
                pJTokenWriter.WriteValue(item.NeedArriveDate.ToString("dd/MM/yyyy") + " " + item.NeedArriveTime.ToString(@"h\:mm"));

                pJTokenWriter.WritePropertyName("ArriveTime");
                if (item.ArriveDate != null && item.ArriveTime != null)
                    pJTokenWriter.WriteValue(item.ArriveDate.Value.ToString("dd/MM/yyyy") + " " + item.ArriveTime.Value.ToString(@"h\:mm"));
                else
                    pJTokenWriter.WriteValue("");

                pJTokenWriter.WritePropertyName("StatusName");
                pJTokenWriter.WriteValue(item.StatusName);

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static List<TruckingCostVm> GetTruckingCostByTruckingOrderId(int truckingOrderId)
        {

            List<TruckingCostVm> truckingCostvmList = new List<TruckingCostVm>();
            EasyFreightEntities db = new EasyFreightEntities();
            //  Mapper.CreateMap<TruckingOrderCost, TruckingCostVm>().IgnoreAllNonExisting();
            TruckingCostVm truckingCostObj;

            //var  truckingCostList = db.TruckingOrderCosts
            //    .Include("TruckingCostLib").Where(x => x.TruckingOrderId == truckingOrderId).ToList();

            var truckingCostList2 = from c in db.TruckingOrderCosts
                                    join o in db.Currencies on c.CurrencyId equals o.CurrencyId
                                    where c.TruckingOrderId == truckingOrderId
                                    select new
                                    {
                                        ContractorId = c.ContractorId,
                                        CurrencyId = o.CurrencyId,
                                        CurrencySign = o.CurrencySign,
                                        TruckingCostId = c.TruckingCostId,
                                        TruckingCostName = c.TruckingCostLib.TruckingCostName,

                                        TruckingCostNet = c.TruckingCostNet,
                                        TruckingCostSelling = c.TruckingCostSelling,
                                        TruckingOrderCostId = c.TruckingOrderCostId,
                                        TruckingOrderId = c.TruckingOrderId,
                                    };


            foreach (var item in truckingCostList2)
            {
                truckingCostObj = new TruckingCostVm();
                truckingCostObj.ContractorId = item.ContractorId;
                truckingCostObj.CurrencyId = item.CurrencyId;
                truckingCostObj.CurrencySign = item.CurrencySign;
                truckingCostObj.TruckingCostId = item.TruckingCostId;
                truckingCostObj.TruckingCostName = item.TruckingCostName;
                truckingCostObj.TruckingCostNet = item.TruckingCostNet;
                truckingCostObj.TruckingCostSelling = item.TruckingCostSelling;
                truckingCostObj.TruckingOrderCostId = item.TruckingOrderCostId;
                truckingCostObj.TruckingOrderId = item.TruckingOrderId;

                truckingCostvmList.Add(truckingCostObj);

                //try
                //{
                //    truckingCostObj = new TruckingCostVm();
                //    Mapper.Map(item, truckingCostObj);
                //    truckingCostvmList.Add(truckingCostObj);
                //}
                //catch { }
            }
            //try
            //{
            //    Mapper.CreateMap<TruckingOrderCost, TruckingCostVm>().IgnoreAllNonExisting();
            //    Mapper.Map(truckingCostList2, truckingCostvmList);
            //}
            //catch
            //{ }
            return truckingCostvmList;


        }

        public static TruckingOrderVm NewTruckingOrderByOperation(int operationId, byte orderFrom = 1, int houseBillId = 0)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            OperationsEntities db1 = new OperationsEntities();
            TruckingOrderVm trkoVmObj = new TruckingOrderVm(orderFrom);
            //Get House Bill status
            var hbInfo = db1.HouseBills.Where(x => x.HouseBillId == houseBillId)
                .Select(x => new { x.StatusId, x.OperationDate }).FirstOrDefault();
            //Check if has trucking  order .. Get its details for edit 
            TruckingOrder truckingOrderDb;
            if (hbInfo.StatusId == 3 || hbInfo.StatusId == 4)
                truckingOrderDb = db.TruckingOrders
                   .Where(x => x.HouseBillId == houseBillId && (x.StatusId == 3 || x.StatusId == 4)).FirstOrDefault();
            else
                truckingOrderDb = db.TruckingOrders
                 .Where(x => x.HouseBillId == houseBillId && (x.StatusId == 1 || x.StatusId == 2)).FirstOrDefault();

            if (truckingOrderDb != null)
            {
                Mapper.CreateMap<TruckingOrder, TruckingOrderVm>().IgnoreAllNonExisting();
                Mapper.Map(truckingOrderDb, trkoVmObj);
                trkoVmObj.OperationDate = hbInfo.OperationDate.Value;
            }
            else
            {
                var houseBillVm = HouseBillHelper.GetHbContent(operationId, orderFrom, houseBillId);

                Mapper.CreateMap<HouseBillVm, TruckingOrderVm>()
                    .ForMember(x => x.StatusId, y => y.Ignore())
                    .IgnoreAllNonExisting();
                Mapper.Map(houseBillVm, trkoVmObj);

                trkoVmObj.BookingNo = db1.Operations.Where(x => x.OperationId == operationId)
                    .FirstOrDefault().BookingNumber;

            }

            return trkoVmObj;
        }

        public static string AddTruckingOrder(TruckingOrderVm truckingOrderVm)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();

            TruckingOrder truckingOrderDb = new TruckingOrder();

            Mapper.CreateMap<TruckingOrderVm, TruckingOrder>().IgnoreAllNonExisting();
            Mapper.Map(truckingOrderVm, truckingOrderDb);
            //Get carrier Id from operation
            OperationsEntities db1 = new OperationsEntities();
            int operationId = truckingOrderDb.OperationId;
            truckingOrderDb.CarrierId = db1.Operations.Where(x => x.OperationId == operationId).FirstOrDefault().CarrierId;

            var op_DeliveryNeeded = db1.Operations.Where(x => x.OperationId == operationId).FirstOrDefault();
            op_DeliveryNeeded.DeliveryNeeded = true;

            if (truckingOrderVm.TruckingOrderId == 0)
            {
                db.TruckingOrders.Add(truckingOrderDb);
                
            }

            try
            {
                db.SaveChanges();
                db1.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.Message;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.InnerException.InnerException;
            }

            return isSaved;
        }


        public static string ChangeStatus(int truckingOrderId, byte statusId)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            TruckingOrder orderDb;

            orderDb = db.TruckingOrders.Where(x => x.TruckingOrderId == truckingOrderId).FirstOrDefault();
            if (statusId == 4) //cancel from HB will delete the record
                db.TruckingOrders.Remove(orderDb);
            else
                orderDb.StatusId = statusId;

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.Message;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.Message;
            }

            return isSaved;
        }

        /// <summary>
        /// Get trucking orders list for operation
        /// </summary>
        /// <param name="operationId">operation Id</param>
        /// <param name="orderFrom">Order from 1 export 2 import</param>
        /// <returns>TruckingOrderListVm List</returns>
        public static List<TruckingOrderListVm> GetTruckingOrderList(int operationId, byte orderFrom)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<TruckingOrderListVm> truckListVm = new List<TruckingOrderListVm>();
            var truckListDb = db.TruckingOrders.Where(x => x.OperationId == operationId).ToList();
            Mapper.CreateMap<TruckingOrder, TruckingOrderListVm>().IgnoreAllNonExisting();
            Mapper.Map(truckListDb, truckListVm);

            OperationsEntities db1 = new OperationsEntities();

            foreach (var item in truckListVm)
            {
                int hbId = item.HouseBillId;
                item.NumberOfPackages = db1.HouseBills.Where(x => x.HouseBillId == hbId).FirstOrDefault().NumberOfPackages;
            }

            if (orderFrom == 1) //Export .. client will be the shipper
            {
                List<int> shipperIds = truckListVm.Select(x => x.ShipperId).ToList();
                var shipperList = db.Shippers
                    .Where(x => shipperIds.Contains(x.ShipperId))
                    .Select(x => new { x.ShipperId, x.ShipperNameEn, x.ShipperNameAr }).ToList();
                foreach (var item in truckListVm)
                {
                    item.ClientName = shipperList.Where(x => x.ShipperId == item.ShipperId).FirstOrDefault().ShipperNameEn;
                }
            }
            else //Export .. client will be consignee 
            {
                List<int> consigneeIds = truckListVm.Select(x => x.ConsigneeId).ToList();
                var consigneeList = db.Consignees
                        .Where(x => consigneeIds.Contains(x.ConsigneeId))
                        .Select(x => new { x.ConsigneeId, x.ConsigneeNameEn, x.ConsigneeNameAr }).ToList();
                foreach (var item in truckListVm)
                {
                    item.ClientName = consigneeList.Where(x => x.ConsigneeId == item.ConsigneeId).FirstOrDefault().ConsigneeNameEn;
                }
            }

            return truckListVm;
        }

        public static int GetTruckingOrdersCount(int operationId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            return db.TruckingOrders.Where(x => x.OperationId == operationId).Count();
        }

        public static List<TruckingNewSummary> GetTopNewFive()
        {
            List<TruckingNewSummary> topOrders = new List<TruckingNewSummary>();
            OperationsEntities db = new OperationsEntities();
            var topOrdersDb = db.TruckingOrdersViews.Where(x => x.StatusId == 1).OrderBy(x => x.NeedArriveDate)
                .Select(x => new
                {
                    x.TruckingOrderId,
                    x.ShipperNameEn,
                    x.ConsigneeNameEn,
                    x.NeedArriveDate,
                    x.NeedArriveTime,
                    x.FromAreaName,
                    x.ToAreaName,
                    x.OrderFrom
                }).Take(5);

            TruckingNewSummary topOrder;
            foreach (var item in topOrdersDb)
            {
                topOrder = new TruckingNewSummary();

                topOrder.TruckingOrderId = item.TruckingOrderId;

                if (!string.IsNullOrEmpty(item.ShipperNameEn))
                    topOrder.ClientName = item.ShipperNameEn;
                else if (!string.IsNullOrEmpty(item.ConsigneeNameEn))
                    topOrder.ClientName = item.ConsigneeNameEn;

                topOrder.FromArea = item.FromAreaName;
                topOrder.ToArea = item.ToAreaName;

                topOrder.NeedArrive = item.NeedArriveDate.ToString("dd/MM/yyyy") + " " + item.NeedArriveTime.ToString(@"h\:mm");
                topOrder.OrderFrom = item.OrderFrom;
                switch (item.OrderFrom)
                {
                    case 1:
                        topOrder.OrderFromImg = "<i class='fa fa-level-up'></i>";
                        break;
                    case 2:
                        topOrder.OrderFromImg = "<i class='fa fa-level-down'></i>";
                        break;
                }

                //Get how many days left to complete the order
                double daysDiff = (item.NeedArriveDate.Date - DateTime.Now.Date).TotalDays;

                if (daysDiff > 10)
                    topOrder.LabelClass = "label-success";
                else if(daysDiff > 4 && daysDiff < 10)
                    topOrder.LabelClass = "label-warning";
                else
                    topOrder.LabelClass = "label-danger";

                topOrders.Add(topOrder);
            }

            return topOrders;
        }

        public static string DeleteTruckingOrder(int truckingOrderId)
        {

            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            OperationsEntities db1 = new OperationsEntities();
            TruckingOrder orderDb;


            List<TruckingOrderDetail> orderDetailsDb;
            List<TruckingOrderCost> orderCostDb;

            orderDb = db.TruckingOrders.Where(x => x.TruckingOrderId == truckingOrderId).FirstOrDefault();
            int houseBillStatusID = db1.HouseBills.Where(c => c.HouseBillId == orderDb.HouseBillId).FirstOrDefault().StatusId;

            if (houseBillStatusID == 3 || houseBillStatusID == 6)
            {
                 return "false " + "Cannot delete this trucking. The house bill is closed";
            }
            //cancel from HB will delete the record
            db.TruckingOrders.Remove(orderDb);
            orderDetailsDb = db.TruckingOrderDetails.Where(x => x.TruckingOrderId == truckingOrderId).ToList();
            foreach (var item in orderDetailsDb)
            {
                db.TruckingOrderDetails.Remove(item);
            }
            orderCostDb = db.TruckingOrderCosts.Where(x => x.TruckingOrderId == truckingOrderId).ToList();
            foreach (var item in orderCostDb)
            {
                db.TruckingOrderCosts.Remove(item);
            }



            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                isSaved = "false " + e.Message;
            }
            catch (Exception e)
            {
                isSaved = "false " + e.Message;
            }

            return isSaved;

        }

        public static List<TurckingTopSummary> GetTruckingTopByDate(DateTime? fromdate, DateTime? todate)
        {
            return TruckingTopModels.Instance.GetTruckingTopByDate(fromdate, todate);
        }

        public static bool HasInv(int truckingId)
        {
            if (truckingId != 0)
                return TruckingTopModels.Instance.HasActiveInovice(truckingId);
            else
                return false;
        
        }
    
    }
}