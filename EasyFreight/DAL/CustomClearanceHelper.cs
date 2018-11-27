using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace EasyFreight.DAL
{
    public static class CustomClearanceHelper
    {

        internal static CustomClearVm GetCustomClearance(int operationId, int houseBillId)
        {
            CustomClearVm custClearVm = new CustomClearVm(operationId, houseBillId);
            OperationsEntities db = new OperationsEntities();
            //Get operation status
            byte oprStatus = db.HouseBills.Where(x => x.HouseBillId == houseBillId).FirstOrDefault().StatusId;
            CustomClearanceOrder custClearDb;

            if (oprStatus == 3 || oprStatus == 4)
                custClearDb = db.CustomClearanceOrders
                .Where(x => x.HouseBillId == houseBillId && (x.StatusId == 3 || x.StatusId == 4)).FirstOrDefault();

            else
                custClearDb = db.CustomClearanceOrders
                .Where(x => x.HouseBillId == houseBillId && (x.StatusId == 1 || x.StatusId == 2)).FirstOrDefault();

            if (custClearDb != null)
            {
                Mapper.CreateMap<CustomClearanceOrder, CustomClearVm>().IgnoreAllNonExisting();
                Mapper.Map(custClearDb, custClearVm);
            }

            return custClearVm;
        }

        internal static string AddEditCustClear(CustomClearVm custClearVm)
        {
            string isSaved = "true";
            int cCId = custClearVm.CCId;

            OperationsEntities db = new OperationsEntities();
            CustomClearanceOrder custClearDb;
            if (cCId == 0)
            {
                custClearDb = new CustomClearanceOrder();
            }
            else
            {
                custClearDb = db.CustomClearanceOrders.Where(x => x.CCId == cCId).FirstOrDefault();
            }

            Mapper.CreateMap<CustomClearVm, CustomClearanceOrder>().IgnoreAllNonExisting();
            Mapper.Map(custClearVm, custClearDb);

            if (cCId == 0)
                db.CustomClearanceOrders.Add(custClearDb);

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
        /// Get all custom clearance orders for grid
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        internal static JObject GetCCAllOrders(FormCollection form)
        {

            OperationsEntities db = new OperationsEntities();
            List<CustomClearanceView> operationList = new List<CustomClearanceView>();
            CustomClearanceView oprObj = new CustomClearanceView();
            if (form.Count > 0)
            {
                string where = CommonHelper.AdvancedSearch<CustomClearanceView>(form, oprObj);
                operationList = db.CustomClearanceViews.Where(where.ToString()).ToList();
            }
            else
            {
                operationList = db.CustomClearanceViews
                    .Where(x => x.StatusId == 1 || x.StatusId == 2)
                   .OrderByDescending(x => x.CreateDate).ToList();
            }

            operationList = operationList.OrderByDescending(x => x.CreateDate).ToList();


            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in operationList)
            {
                pJTokenWriter.WriteStartObject();

                pJTokenWriter.WritePropertyName("CCId");
                pJTokenWriter.WriteValue(item.CCId);

                pJTokenWriter.WritePropertyName("OperationId");
                pJTokenWriter.WriteValue(item.OperationId);

                pJTokenWriter.WritePropertyName("CarrierTypeImg");
                switch (item.CarrierType)
                {
                    case 1:
                        pJTokenWriter.WriteValue("<i class='fa fa-ship'></i>");
                        break;
                    case 2:
                        pJTokenWriter.WriteValue("<i class='fa fa-plane'></i>");
                        break;
                }


                switch (item.OrderFrom)
                {
                    case 1:
                        pJTokenWriter.WritePropertyName("OrderFromImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-level-up'></i>");

                        pJTokenWriter.WritePropertyName("Client");
                        pJTokenWriter.WriteValue(item.ShipperNameEn);

                        pJTokenWriter.WritePropertyName("Port");
                        pJTokenWriter.WriteValue(item.FromPort);

                        break;
                    case 2:
                        pJTokenWriter.WritePropertyName("OrderFromImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-level-down'></i>");

                        pJTokenWriter.WritePropertyName("Client");
                        pJTokenWriter.WriteValue(item.ConsigneeNameEn);

                        pJTokenWriter.WritePropertyName("Port");
                        pJTokenWriter.WriteValue(item.ToPort);
                        break;
                }

                pJTokenWriter.WritePropertyName("CarrierType");
                pJTokenWriter.WriteValue(item.CarrierType);

                pJTokenWriter.WritePropertyName("MBL");
                pJTokenWriter.WriteValue(item.MBL);

                pJTokenWriter.WritePropertyName("OperationCode");
                pJTokenWriter.WriteValue(item.OperationCode);

                pJTokenWriter.WritePropertyName("BookingNumber");
                pJTokenWriter.WriteValue(item.BookingNumber);

                pJTokenWriter.WritePropertyName("NeedArrive");
                pJTokenWriter.WriteValue(item.NeedArriveDate.ToString("dd/MM/yyyy") + " " + item.NeedArriveTime.ToString(@"h\:mm"));

                pJTokenWriter.WritePropertyName("StatusName");
                pJTokenWriter.WriteValue(item.StatusName);

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        /// <summary>
        /// Get one row from CustomClearanceView
        /// </summary>
        /// <param name="id">Custom Clearance Id</param>
        /// <returns>CustomClearanceView object</returns>
        internal static CustomClearanceView GetOneCustClearView(int ccId)
        {
            OperationsEntities db = new OperationsEntities();
            CustomClearanceView custClearObj = db.CustomClearanceViews.Where(x => x.CCId == ccId).FirstOrDefault();
            return custClearObj;
        }

        /// <summary>
        /// Get list of CustomClearanceDetailVm
        /// </summary>
        /// <param name="custClearId">Custom Clearance Id</param>
        /// <returns>CustomClearanceDetailVm List</returns>
        internal static CustomClearanceDetailMainVm GetCustClearDetailList(int custClearId)
        {
            List<CustomClearanceDetailVm> cusClearDetailList = new List<CustomClearanceDetailVm>();
            OperationsEntities db = new OperationsEntities();
            var custDbList = db.CustomClearanceDetails.Where(x => x.CCId == custClearId).ToList();
            if (custDbList.Count == 0)
            {
                CustomClearanceDetailVm custClearDetObj = new CustomClearanceDetailVm { CCId = custClearId };
                cusClearDetailList.Add(custClearDetObj);
            }
            else
            {
                Mapper.CreateMap<CustomClearanceDetail, CustomClearanceDetailVm>().IgnoreAllNonExisting();
                Mapper.Map(custDbList, cusClearDetailList);
            }

            CustomClearanceDetailMainVm custClearDetMain = new CustomClearanceDetailMainVm();
            custClearDetMain.CustomClearanceDetailVms.AddRange(cusClearDetailList);

            return custClearDetMain;

        }

        internal static string AddEditCustClearDet(CustomClearanceDetailMainVm custClearDetailMain,string notes="",string comment="")
        {
            OperationsEntities db = new OperationsEntities();
            var custClearDetaiList = custClearDetailMain.CustomClearanceDetailVms;
            int ccId = custClearDetaiList.FirstOrDefault().CCId;
            //Get costId sent from screen
            List<int> screenCostIds = custClearDetaiList.Select(x => x.CCDetailsId).ToList();
            //Get db saved list
            var custCostDbList = db.CustomClearanceDetails.Where(x => x.CCId == ccId).ToList();

            //delete removed items
            var costToDel = custCostDbList.Where(x => !screenCostIds.Contains(x.CCDetailsId)).ToList();

            foreach (var item in costToDel)
            {
                db.CustomClearanceDetails.Remove(item);
            }

            //Map the Vm to db list
            Mapper.CreateMap<CustomClearanceDetailVm, CustomClearanceDetail>().IgnoreAllNonExisting();
            Mapper.Map(custClearDetaiList, custCostDbList);

            string isSaved = "true";

            using (var dbCtx = new OperationsEntities())
            {
                foreach (var item in custCostDbList)
                {
                    if (item.CCDetailsId == 0)
                        dbCtx.CustomClearanceDetails.Add(item);
                    else
                        dbCtx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                try
                {
                    dbCtx.SaveChanges();

                    OperationsEntities db2 = new OperationsEntities();
                    CustomClearanceOrder custClearDb;
                    custClearDb = db2.CustomClearanceOrders.Where(x => x.CCId == ccId).FirstOrDefault();
                    custClearDb.Notes = notes;
                    custClearDb.Comment = comment;
                    try { 
                        db2.SaveChanges();
                    }
                    catch { }

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

            if (isSaved == "true")
            {
                CustomClearanceOrder custOrder = db.CustomClearanceOrders.Where(x => x.CCId == ccId).FirstOrDefault();
                custOrder.StatusId = 2;
                db.SaveChanges();
            }

            return isSaved;

        }

        internal static List<CustomClearanceListVm> GetCCOrdersList(int operationId, byte orderFrom)
        {
            OperationsEntities db = new OperationsEntities();
            EasyFreightEntities db1 = new EasyFreightEntities();
            List<CustomClearanceListVm> ccOrdersList = db.CustomClearanceOrders.Include("HouseBill")
                .Where(x => x.OperationId == operationId)
                .Select(x => new CustomClearanceListVm
                {
                    CCId = x.CCId,
                    ConsigneeId = x.HouseBill.ConsigneeId,
                    HouseBillId = x.HouseBillId,
                    HouseBL = x.HouseBill.HouseBL,
                    NeedArriveDate = x.NeedArriveDate,
                    NeedArriveTime = x.NeedArriveTime,
                    NumberOfPackages = x.HouseBill.NumberOfPackages,
                    OperationCode = x.HouseBill.OperationCode,
                    ShipperId = x.HouseBill.ShipperId,
                    OperationId = x.OperationId,
                    StatusId = x.StatusId
                }).ToList();

            if (orderFrom == 1) //Export .. client will be the shipper
            {
                List<int> shipperIds = ccOrdersList.Select(x => x.ShipperId).ToList();
                var shipperList = db1.Shippers
                    .Where(x => shipperIds.Contains(x.ShipperId))
                    .Select(x => new { x.ShipperId, x.ShipperNameEn, x.ShipperNameAr }).ToList();
                foreach (var item in ccOrdersList)
                {
                    item.ClientName = shipperList.Where(x => x.ShipperId == item.ShipperId).FirstOrDefault().ShipperNameEn;
                }
            }
            else //Export .. client will be consignee 
            {
                List<int> consigneeIds = ccOrdersList.Select(x => x.ConsigneeId).ToList();
                var consigneeList = db1.Consignees
                        .Where(x => consigneeIds.Contains(x.ConsigneeId))
                        .Select(x => new { x.ConsigneeId, x.ConsigneeNameEn, x.ConsigneeNameAr }).ToList();
                foreach (var item in ccOrdersList)
                {
                    item.ClientName = consigneeList.Where(x => x.ConsigneeId == item.ConsigneeId).FirstOrDefault().ConsigneeNameEn;
                }
            }

            return ccOrdersList;
        }

        internal static List<CustomClearanceDetailVm> GetCCDetailsList(int ccId)
        {
            List<CustomClearanceDetailVm> detailsListVm = new List<CustomClearanceDetailVm>();
            OperationsEntities db = new OperationsEntities();
            var detailsListDb = db.CustomClearanceDetails.Where(x => x.CCId == ccId).ToList();
            Mapper.CreateMap<CustomClearanceDetail, CustomClearanceDetailVm>().IgnoreAllNonExisting();
            Mapper.Map(detailsListDb, detailsListVm);

            Dictionary<int, string> ccCostLib = ListCommonHelper.GetCustClearCostList();
            Dictionary<int, string> currencyList = ListCommonHelper.GetCurrencyList();
            foreach (var item in detailsListVm)
            {
                item.CCCostName = ccCostLib[item.CCCostId];
                item.CurrencySign = currencyList[item.CurrencyId];
            }
            return detailsListVm;
        }

        /// <summary>
        /// Close CC order .. change statusId = 3
        /// </summary>
        /// <param name="ccId"></param>
        /// <returns>string "true" if success</returns>
        internal static string CloseCC(int ccId, byte statusId)
        {
            string isClosed = "true";
            OperationsEntities db = new OperationsEntities();
            var ccOrder = db.CustomClearanceOrders.Where(x => x.CCId == ccId).FirstOrDefault();

            if (statusId == 4) //cancel from HB will delete the record
                db.CustomClearanceOrders.Remove(ccOrder);
            else
                ccOrder.StatusId = statusId;

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

        //internal static CcCostAccVm AddCCCostForAccounting(int operId, int CcCostAccId = 0)
        //{
        //    CcCostAccVm costAccVm;
        //    //Get custom cleance cost for the full operation
        //    var operCostObj = AccountingHelper.GetOperationCost(operId);

        //    var operCostList = operCostObj.OperationCostAccVms;
        //    var operCostTotalList = operCostObj.OperationCostTotalAccVms;

        //    CcCostAccDetailVm costAccDetailVm;


        //}


        internal static List<CustomClearanceDetailVm> GetCCDetailsListByOperId(int operId)
        {
            List<CustomClearanceDetailVm> detailsListVm = new List<CustomClearanceDetailVm>();
            OperationsEntities db = new OperationsEntities();
            var detailsListDb = db.CustomClearanceDetails.Where(x => x.CustomClearanceOrder.OperationId == operId).ToList();
            Mapper.CreateMap<CustomClearanceDetail, CustomClearanceDetailVm>().IgnoreAllNonExisting();
            Mapper.Map(detailsListDb, detailsListVm);

            Dictionary<int, string> ccCostLib = ListCommonHelper.GetCustClearCostList();
            Dictionary<int, string> currencyList = ListCommonHelper.GetCurrencyList();
            foreach (var item in detailsListVm)
            {
                item.CCCostName = ccCostLib[item.CCCostId];
                item.CurrencySign = currencyList[item.CurrencyId];
            }
            return detailsListVm;
        }
    
    
    }

}