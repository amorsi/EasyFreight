using System;
using System.Collections.Generic;
using System.Linq;
using EasyFreight.ViewModel;
using EasyFreight.Models;
using AutoMapper;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Text;

namespace EasyFreight.DAL
{
    public static class OperationHelper
    {
        public static OperationVm GetOperationInfo(int id = 0, byte orderFrom = 1, int quoteId = 0)
        {
            OperationVm operationVm = new OperationVm(orderFrom, quoteId);
            OperationsEntities db = new OperationsEntities();

            if (quoteId == 0 && id == 0) //add new with no quotation
            {
                OperationContainerVm operContVm = new OperationContainerVm();
                operationVm.OperationContainers.Add(operContVm);
                return operationVm;
            }

            if (quoteId != 0 && id == 0)
            {
                //check if has record in operation table 
                Operation operationDb = db.Operations.Where(x => x.QuoteId == quoteId).FirstOrDefault();
                if (operationDb != null)
                    id = operationDb.OperationId;
            }

            if (id != 0) // has record in operation table 
            {
                var operationDb = db.Operations.Include("OperationContainers")
                    .Where(x => x.OperationId == id).FirstOrDefault();

                Mapper.CreateMap<Operation, OperationVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<OperationContainer, OperationContainerVm>().IgnoreAllNonExisting();
                Mapper.Map(operationDb, operationVm);

                if (operationVm.OperationContainers.Count == 0)
                {
                    OperationContainerVm operContVm = new OperationContainerVm();
                    operationVm.OperationContainers.Add(operContVm);
                }
                return operationVm;
            }
            else // No has record in operation table .. and has quote id
            {
                //Will fill operationVm from quotation data
                var quotationDb = db.Quotations.Include("QuotationContainers")
                 .Where(x => x.QuoteId == quoteId).FirstOrDefault();
                var quotationContListDb = quotationDb.QuotationContainers.ToList();
                Mapper.CreateMap<Quotation, OperationVm>().IgnoreAllNonExisting()
                    .ForMember(x => x.OperationContainers, v => v.Ignore())
                    .ForMember(x => x.CreateDate, y => y.Ignore())
                    .ForMember(x => x.CreateBy, y => y.Ignore());
                Mapper.Map(quotationDb, operationVm);
                OperationContainerVm operContVm;
                if (quotationContListDb.Count == 0)
                {
                    operContVm = new OperationContainerVm();
                    operationVm.OperationContainers.Add(operContVm);
                }
                else
                {
                    foreach (var item in quotationContListDb)
                    {
                        for (int i = 0; i < item.NumberOfContainers; i++)
                        {
                            operContVm = new OperationContainerVm();
                            operContVm.ContainerTypeId = item.ContainerTypeId;
                            operationVm.OperationContainers.Add(operContVm);
                        }
                    }
                }


            }
            return operationVm;
        }

        public static JObject GetOperationOrders(FormCollection form)
        {
            OperationsEntities db = new OperationsEntities();

            OperationView oprObj = new OperationView();

            string where = CommonHelper.AdvancedSearch<OperationView>(form, oprObj);
            var operationList = db.OperationViews.Where(where.ToString())
                .Select(x => new
                {
                    x.OperationId,
                    x.CarrierType,
                    x.QuoteId,
                    x.CreateDate,
                    x.OperationCode,
                    x.ShipperNameEn,
                    x.ConsigneeNameEn,
                    x.CarrierNameEn,
                    x.FromPort,
                    x.ToPort,
                    x.DateOfDeparture,
                    x.StatusName,
                    x.StatusId,
                    x.IsConsolidation,
                    x.OrderFrom,
                    x.AgentNameEn
                })
                .ToList();

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in operationList)
            {
                pJTokenWriter.WriteStartObject();

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

                pJTokenWriter.WritePropertyName("ConsolidationImg");
                switch (item.IsConsolidation)
                {
                    case true:
                        pJTokenWriter.WriteValue("<i class='fa fa-users'></i>");
                        break;
                    case false:
                        pJTokenWriter.WriteValue("<i class='fa fa-user'></i>");
                        break;
                }


                switch (item.OrderFrom)
                {
                    case 1:
                        pJTokenWriter.WritePropertyName("OrderFromImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-level-up'></i>");

                        pJTokenWriter.WritePropertyName("ClientName");
                        pJTokenWriter.WriteValue(item.ShipperNameEn);
                        break;
                    case 2:
                        pJTokenWriter.WritePropertyName("OrderFromImg");
                        pJTokenWriter.WriteValue("<i class='fa fa-level-down'></i>");

                        pJTokenWriter.WritePropertyName("ClientName");
                        pJTokenWriter.WriteValue(item.ConsigneeNameEn);
                        break;
                }

                pJTokenWriter.WritePropertyName("QuoteId");
                if (item.QuoteId != null)
                    pJTokenWriter.WriteValue(item.QuoteId);
                else
                    pJTokenWriter.WriteValue("");

                pJTokenWriter.WritePropertyName("CarrierType");
                pJTokenWriter.WriteValue(item.CarrierType);

                pJTokenWriter.WritePropertyName("CreateDate");
                pJTokenWriter.WriteValue(item.CreateDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("OperationCode");
                pJTokenWriter.WriteValue(item.OperationCode);

                pJTokenWriter.WritePropertyName("ShipperName");
                pJTokenWriter.WriteValue(item.ShipperNameEn);

                pJTokenWriter.WritePropertyName("ConsigneeName");
                pJTokenWriter.WriteValue(item.ConsigneeNameEn);

                pJTokenWriter.WritePropertyName("CarrierName");
                pJTokenWriter.WriteValue(item.CarrierNameEn);

                pJTokenWriter.WritePropertyName("FromPort");
                pJTokenWriter.WriteValue(item.FromPort);

                pJTokenWriter.WritePropertyName("ToPort");
                pJTokenWriter.WriteValue(item.ToPort);

                pJTokenWriter.WritePropertyName("DateOfDeparture");
                pJTokenWriter.WriteValue(item.DateOfDeparture != null ? item.DateOfDeparture.Value.ToString("dd/MM/yyyy") : "");

                pJTokenWriter.WritePropertyName("StatusName");
                pJTokenWriter.WriteValue(item.StatusName);

                pJTokenWriter.WritePropertyName("StatusId");
                pJTokenWriter.WriteValue(item.StatusId);

                pJTokenWriter.WritePropertyName("OrderFrom");
                pJTokenWriter.WriteValue(item.OrderFrom);

                pJTokenWriter.WritePropertyName("AgentName");
                pJTokenWriter.WriteValue(item.AgentNameEn);

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static string AddEditOperation(OperationVm operationVm, out int operId)
        {

            string isSaved = "true";
            int operationId = operationVm.OperationId;
            OperationsEntities db = new OperationsEntities();
            Operation operationDb;
            List<OperationContainer> operationContListDb;
            // notifications 
            NotificationMsgVm notifi = new NotificationMsgVm();

            if (operationId == 0)
            {
                operationDb = new Operation();
                operationContListDb = new List<OperationContainer>(); 
            }
            else
            {
                operationDb = db.Operations.Include("OperationContainers")
                    .Where(x => x.OperationId == operationId).FirstOrDefault();
                if (operationDb.StatusId > 2)
                {
                    operId = operationDb.OperationId;
                    isSaved = "false .. Cann't update operation is not open ";
                    return isSaved;
                }
                operationContListDb = operationDb.OperationContainers.ToList();

                //Get quotContainers Ids sent from the screen
                List<long> containerVmIds = operationVm.OperationContainers.Select(x => x.OperConId).ToList();
                var containerDel = operationContListDb.Where(x => !containerVmIds.Contains(x.OperConId)).ToList();

                foreach (var item in containerDel)
                {
                    db.OperationContainers.Remove(item);
                }
            }

            Mapper.CreateMap<OperationVm, Operation>().IgnoreAllNonExisting();
            // .ForMember(x => x.OperationContainers, v=> v.Ignore());
            Mapper.CreateMap<OperationContainerVm, OperationContainer>().IgnoreAllNonExisting();
            Mapper.Map(operationVm, operationDb);

            bool updateHB = false;
            if (operationId == 0)
            {
                //Generate code at save event
                if (operationDb.OrderFrom == 1)
                    operationDb.OperationCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.Export, true);
                else
                    operationDb.OperationCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.Import, true);

                db.Operations.Add(operationDb);
                //update quotation status if any
                int? quoteId = operationVm.QuoteId;
                if (quoteId != null)
                {
                    //status = 2 -- opened
                    Quotation quoteToUpdate = db.Quotations.Where(x => x.QuoteId == quoteId).FirstOrDefault();
                    quoteToUpdate.StatusId = 2;
                }

                notifi.NotificationTypeID = (operationDb.OrderFrom == 1) ? 1 : 3;
                notifi.ObjectID = -1;
            }
            else
            {
                List<HouseBillListVm> hbList = HouseBillHelper.GetHBList(operationId, operationDb.OrderFrom);
                if (hbList.Count == 1)
                {
                    if (!operationDb.IsConsolidation) { 
                     HouseBill  hb  = db.HouseBills.Where(x => x.OperationId == operationDb.OperationId).FirstOrDefault();
                     hb.CBM = operationDb.CBM;
                     hb.GoodsDescription = operationDb.GoodsDescription;
                     hb.GrossWeight = operationDb.GrossWeight;
                     hb.NetWeight = operationDb.NetWeight;
                     hb.NumberOfPackages = operationDb.NumberOfPackages;
                     hb.CollectedFreightCost = operationDb.FreightCostAmount;
                     hb.CollectedThcCost = operationDb.ThcCostAmount;
                        


                    // hb.ShipperId = operationDb.ShipperId;
                   //  hb.ConsigneeId = operationDb.ConsigneeId;
                    // hb.NotifierId = operationDb.NotifierId;
                   //  hb.NotifierAsConsignee = operationDb.NotifierAsConsignee;
                   //  hb.AgentId = operationDb.AgentId;

                     hb.FromPortId = operationDb.FromPortId;
                     hb.ToPortId = operationDb.ToPortId; 
                    }
                } 
            }
            operId = 0;
            try
            {
                db.SaveChanges();
                operId = operationDb.OperationId;
                if (notifi.ObjectID == -1)
                {
                    notifi.ObjectID = operId;
                    notifi.NotificationMsg = " New " + (operationDb.OrderFrom == 1 ? "Export " : "Import ") + " Operation Code: " + operationDb.OperationCode;
                    NotificationHelper.Create_Notification(notifi);
                }

                //update hb
                if (updateHB)
                {

                   
                }
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

        public static List<OperationContainerVm> GetOperationContainers(int id)
        {
            OperationsEntities db = new OperationsEntities();
            Dictionary<int, string> contList = ListCommonHelper.GetContainerList();
            Dictionary<int, string> packageList = ListCommonHelper.GetPackageTypeList();
            List<OperationContainerVm> oprContainers = new List<OperationContainerVm>();
            var operConDb = db.OperationContainers.Where(x => x.OperationId == id).ToList();
            Mapper.CreateMap<OperationContainer, OperationContainerVm>().IgnoreAllNonExisting();

            Mapper.Map(operConDb, oprContainers);
            foreach (var item in oprContainers)
            {
                try
                {
                    item.ContainerTypeName = contList[item.ContainerTypeId];
                }
                catch { }

                if (item.PackageTypeId != null)
                    item.PackageTypeName = packageList[item.PackageTypeId.Value];
            }

            if (oprContainers.Count == 0)
            {
                OperationContainerVm operContObj = new OperationContainerVm();
                oprContainers.Add(operContObj);
            }


            return oprContainers;
        }

        internal static OperationView GetOne(int id)
        {
            OperationsEntities db = new OperationsEntities();
            OperationView operationView = db.OperationViews.Where(x => x.OperationId == id).FirstOrDefault();
            return operationView;
        }

        /// <summary>
        /// Get container summary for operation
        /// </summary>
        /// <param name="operationId">int operation id</param>
        /// <returns>string like 2x20' 4xLCL 3x40'</returns>
        internal static string GetContainersSummary(int operationId)
        {
            OperationsEntities db = new OperationsEntities();
            var contList = db.OperationContainers.Where(x => x.OperationId == operationId)
                .GroupBy(x => x.ContainerTypeId, (key, values) => new { ContainerTypeId = key, count = values.Count() }).ToList();

            Dictionary<int, string> containerTypeList = ListCommonHelper.GetContainerList();
            StringBuilder result = new StringBuilder();
            foreach (var item in contList)
            {
                if (item.ContainerTypeId == 0)
                    result.Append("Air Container");
                else
                    result.Append(containerTypeList[item.ContainerTypeId]);

                result.Append("x");
                result.Append(item.count);
                result.Append("  ");
            }

            return result.ToString();
        }

        internal static bool CheckOperationIsConsolidation(int operationId)
        {
            OperationsEntities db = new OperationsEntities();
            return db.Operations.Where(x => x.OperationId == operationId).FirstOrDefault().IsConsolidation;
        }


        /// <summary>
        /// Get data for Concession Letter "خطاب تنازل"
        /// </summary>
        /// <param name="operationId">operation id</param>
        /// <param name="langCode">language code "en" or "ar"</param>
        /// <returns>Concession Letter Vm</returns>
        internal static ConcessionLetterVm GetConcessionLetter(int operationId, string langCode = "en")
        {
            ConcessionLetterVm concLetterVm = new ConcessionLetterVm();
            OperationsEntities db = new OperationsEntities();
            var operDb = //db.OperationViews
                //.Where(x => x.OperationId == operationId)
                db.HouseBillViews .Where(x => x.HouseBillId == operationId)
                .Select(x => new
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
                    x.OperationId
                })
                .FirstOrDefault();


            concLetterVm.CarrierName = operDb.CarrierNameEn;
            concLetterVm.FromPort = operDb.FromPort;
            concLetterVm.MBL = operDb.MBL;
            concLetterVm.GrossWeight = operDb.GrossWeight.Value;
            concLetterVm.CBM = operDb.CBM == null ? 0 : operDb.CBM.Value;
            concLetterVm.ConsigneeName = operDb.ConsigneeNameEn;
            concLetterVm.Containers = GetContainersSummary(operDb.OperationId);
            concLetterVm.NumberOfPackages = operDb.NumberOfPackages.Value;
            concLetterVm.ToPort = operDb.ToPort;
            if (operDb.CarrierType == 1) // sea
                concLetterVm.StaticLabels = CommonHelper.GetStaticLabels((int)StaticTextForScreenEnum.ConcessionLetter, langCode);
            else
            {
                concLetterVm.StaticLabels = CommonHelper.GetStaticLabels((int)StaticTextForScreenEnum.ConcessionLetterAir, langCode);
                concLetterVm.HouseBL = db.HouseBills.Where(x => x.HouseBillId == operationId).FirstOrDefault().HouseBL;
            }

            if (langCode == "ar")
                concLetterVm.CompanyName = CommonHelper.GetCompInfo().CompanyNameAr;
            else
                concLetterVm.CompanyName = CommonHelper.GetCompInfo().CompanyNameEn;
            return concLetterVm;
        }



        /// <summary>
        /// Check If any transportation or CC is opened for this operation 
        /// .. if so will show message that is must be closed first
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        private static bool CheckOpenedOperationTransactions(int operationId)
        {
            bool isOpened = false;
            EasyFreightEntities db1 = new EasyFreightEntities();
            isOpened = db1.TruckingOrders.Any(x => x.OperationId == operationId && (x.StatusId == 1 || x.StatusId == 2));
            if (!isOpened)
            {
                OperationsEntities db = new OperationsEntities();
                isOpened = db.CustomClearanceOrders.Any(x => x.OperationId == operationId && (x.StatusId == 1 || x.StatusId == 2));
            }


            return isOpened;
        }

        /// <summary>
        /// Close operation and all related HB
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        internal static string CloseOperation(int operationId)
        {
            string isClosed = "true";
            int operCostCount = GetOperationCostCount(operationId);
            if (operCostCount == 0)
                return "Please add operation cost before close it.";

            bool isTransOpened = CheckOpenedOperationTransactions(operationId);
            if (isTransOpened)
                return "Transportation or Custom Clearance orders are opened. Please close them first";

            OperationsEntities db = new OperationsEntities();
            bool anyHbIsOpended = db.HouseBills.Any(x => x.OperationId == operationId && (x.StatusId == 1 || x.StatusId == 2));
            if (anyHbIsOpended)
            {
                var hbList = db.HouseBills.Where(x => x.OperationId == operationId && (x.StatusId == 1 || x.StatusId == 2));
                foreach (var item in hbList)
                {
                    item.StatusId = 3;
                }
            }

            var operation = db.Operations.Where(x => x.OperationId == operationId).FirstOrDefault();
            operation.StatusId = 3;

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

        private static int GetOperationCostCount(int operationId)
        {
            OperationsEntities db = new OperationsEntities();
            int count = db.OperationCosts.Where(x => x.OperationId == operationId).Count();

            return count;
        }

        internal static string ChangeOperationStatus(int operationId, byte statusId)
        {
            string isClosed = "true";
            OperationsEntities db = new OperationsEntities();
            Operation hbObj = db.Operations.Where(x => x.OperationId == operationId).FirstOrDefault();
            hbObj.StatusId = statusId;
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


        public static OperationStatisticVm GetOperationStatistics(DateTime fromDate, DateTime toDate, int? orderFrom)
        {
            /* */
            OperationsEntities db5 = new OperationsEntities();

            var operationList = db5.OperationViews.Where(f => f.OrderFrom == orderFrom &&
                (f.OperationDate >= fromDate && f.OperationDate <= toDate))
                 .GroupBy(f => f.ShipperId)
            .Select(x => new
             {
                 ShipperId = x.Key,
                 OperationCount = x.Count(),
                 TotalCBM = x.Sum(f => f.CBM),
                 TotalWeight = x.Sum(f => f.NetWeight),
                 ShipperNameEn = x.Max(r => r.ShipperNameEn)
                 //x.ShipperId,
                 //x.OperationId,
                 //x.CarrierType,
                 //x.QuoteId,
                 //x.CreateDate,
                 //x.OperationCode,
                 //x.ShipperNameEn,
                 //x.ConsigneeNameEn,
                 //x.CarrierNameEn,
                 //x.FromPort,
                 //x.ToPort,
                 //x.DateOfDeparture,
                 //x.StatusName,
                 //x.StatusId,
                 //x.IsConsolidation,
                 //x.OrderFrom,
                 //x.AgentNameEn
             })
              .Take(5)
             .OrderByDescending(y => y.OperationCount)
             .ThenBy(s => s.TotalWeight)
             .ToList();
            var ee = operationList;
            /* */
            OperationsEntities db = new OperationsEntities();
            OperationStatisticVm operationStatistic = new OperationStatisticVm();
            GetOperationStatistics_Result spResult = db.GetOperationStatistics(orderFrom, fromDate, toDate).FirstOrDefault();

            Mapper.CreateMap<GetOperationStatistics_Result, OperationStatisticVm>().IgnoreAllNonExisting();
            Mapper.Map(spResult, operationStatistic);
            return operationStatistic;
        }

        //public static OperationStatisticVm GetOperationStatistics(System.Web.Mvc.FormCollection form)
        //{ 

        //OperationsEntities db5 = new OperationsEntities();

        //var operationList = db5.OperationViews.Where(f => f.OrderFrom == orderFrom &&
        //    (f.OperationDate >= fromDate && f.OperationDate <= toDate))
        // .Select(x => new
        // {
        //     x.OperationId,
        //     x.CarrierType,
        //     x.QuoteId,
        //     x.CreateDate,
        //     x.OperationCode,
        //     x.ShipperNameEn,
        //     x.ConsigneeNameEn,
        //     x.CarrierNameEn,
        //     x.FromPort,
        //     x.ToPort,
        //     x.DateOfDeparture,
        //     x.StatusName,
        //     x.StatusId,
        //     x.IsConsolidation,
        //     x.OrderFrom,
        //     x.AgentNameEn
        // })
        // .ToList();
        //    OperationsEntities db = new OperationsEntities(); 
        //    OperationStatisticVm operationStatistic = new OperationStatisticVm() ; 
        //        int orderfrom =  string.IsNullOrEmpty(form["OrderFrom"])? 1:int.Parse(form["OrderFrom"]);
        //        DateTime fromDate = string.IsNullOrEmpty(form["FromDate"]) ? DateTime.Now.AddDays(-7) : DateTime.Parse(form["FromDate"]);
        //        DateTime toDate = string.IsNullOrEmpty(form["ToDate"]) ? DateTime.Now.AddDays(-7) : DateTime.Parse(form["ToDate"]);
        //        operationStatistic = GetOperationStatistics(fromDate, toDate, orderfrom); 

        //        return operationStatistic; 
        //}


        internal static string DeleteOperation(int operationId)
        {
            string isClosed = "true";


            OperationsEntities db = new OperationsEntities();

            int statusId = db.Operations.Where(d => d.OperationId == operationId).FirstOrDefault().StatusId;
            if (statusId > 2)
            {
                isClosed = "false Cannot delete this operation it may be Closed or Invoice Issued ";
                return isClosed;
            }
            // delete all HB under this operation
            var hbList = db.HouseBills.Where(x => x.OperationId == operationId);// && (x.StatusId == 1 || x.StatusId == 2));
            foreach (var item in hbList)
            {
                item.StatusId = 4; //canceled
            }
            // delete all Custom Clearance under this operation
            var ccList = db.CustomClearanceOrders.Where(x => x.OperationId == operationId);// && (x.StatusId == 1 || x.StatusId == 2));
            foreach (var item in ccList)
            {
                item.StatusId = 4; //canceled
            }
            // Delete all Trucking orders
            EasyFreightEntities dbE = new EasyFreightEntities();
            var truckList = dbE.TruckingOrders.Where(x => x.OperationId == operationId);// && (x.StatusId == 1 || x.StatusId == 2));
            foreach (var item in truckList)
            {
                item.StatusId = 4; //canceled
            }

            var operation = db.Operations.Where(x => x.OperationId == operationId).FirstOrDefault();
            operation.StatusId = 4;

            try
            {
                db.SaveChanges();
                dbE.SaveChanges();
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

    }
}