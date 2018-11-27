using System;
using System.Collections.Generic;
using System.Linq;
using EasyFreight.ViewModel;
using EasyFreight.Models;
using AutoMapper;
using System.Data.Entity.Validation;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;


namespace EasyFreight.DAL
{
    public static class QuotationHelper
    {
        public static QuotationVm GetQuotationInfo(int quotationId, byte orderFrom)
        {
            QuotationVm quotationVm = new QuotationVm(orderFrom);
            if (quotationId == 0)
            {
                QuotationContainerVm quotContVm = new QuotationContainerVm();
                quotationVm.QuotationContainers.Add(quotContVm);
            }
            else
            {
                OperationsEntities db = new OperationsEntities();
                var quotDb = db.Quotations.Include("QuotationContainers")
                    .Where(x => x.QuoteId == quotationId).FirstOrDefault();

                Mapper.CreateMap<Quotation, QuotationVm>().IgnoreAllNonExisting();
                Mapper.CreateMap<QuotationContainer, QuotationContainerVm>().IgnoreAllNonExisting();
                Mapper.Map(quotDb, quotationVm);

                if (quotationVm.QuotationContainers.Count == 0)
                {
                    QuotationContainerVm quotContVm = new QuotationContainerVm();
                    quotationVm.QuotationContainers.Add(quotContVm);
                }
            }
            return quotationVm;
        }

        public static string AddEditQuotation(QuotationVm quoteVm)
        {

            string isSaved = "true";
            int quoteId = quoteVm.QuoteId;
            OperationsEntities db = new OperationsEntities();
            Quotation quotationDb;
            List<QuotationContainer> quotationContListDb;
            if (quoteId == 0)
            {
                quotationDb = new Quotation();
                quotationContListDb = new List<QuotationContainer>();
            }
            else
            {
                quotationDb = db.Quotations.Include("QuotationContainers")
                    .Where(x => x.QuoteId == quoteId).FirstOrDefault();
                quotationContListDb = quotationDb.QuotationContainers.ToList();

                //Get quotContainers Ids sent from the screen
                List<int> containerVmIds = quoteVm.QuotationContainers.Select(x => x.ContainerTypeId.Value).ToList();
                var containerDel = quotationContListDb.Where(x => !containerVmIds.Contains(x.ContainerTypeId)).ToList();

                foreach (var item in containerDel)
                {
                    db.QuotationContainers.Remove(item);
                }
            }

            Mapper.CreateMap<QuotationVm, Quotation>().IgnoreAllNonExisting();
            Mapper.CreateMap<QuotationContainerVm, QuotationContainer>().IgnoreAllNonExisting();
            Mapper.Map(quoteVm, quotationDb);



            if (quoteId == 0)
            {
                if (quotationDb.OrderFrom == 1)
                    quotationDb.QuoteCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.QuoteExport, true);
                else
                    quotationDb.QuoteCode = AdminHelper.GeneratePrefixCode(PrefixForEnum.QuoteImport, true);

                db.Quotations.Add(quotationDb);

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

        public static JObject GetQuotationsOrders(FormCollection form = null)
        {
            OperationsEntities db = new OperationsEntities();
            QuotationView quotationObj = new QuotationView();

            string where = CommonHelper.AdvancedSearch<QuotationView>(form, quotationObj);
            var quotationList = db.QuotationViews.Where(where.ToString())
                  .Select(x => new
                  {
                      x.QuoteId,
                      x.CarrierType,
                      x.OrderFrom,
                      x.CreateDate,
                      x.QuoteCode,
                      x.ShipperNameEn,
                      x.ConsigneeNameEn,
                      x.CarrierNameEn,
                      x.FromPort,
                      x.ToPort,
                      x.DateOfDeparture,
                      x.StatusName,
                      x.StatusId
                  })
                  .ToList();


            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            foreach (var item in quotationList)
            {
                pJTokenWriter.WriteStartObject();
                pJTokenWriter.WritePropertyName("QuoteId");
                pJTokenWriter.WriteValue(item.QuoteId);

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

                pJTokenWriter.WritePropertyName("OrderFromText");
                pJTokenWriter.WriteValue(item.OrderFrom == 1 ? "Export" : "Import");

                pJTokenWriter.WritePropertyName("OrderFrom");
                pJTokenWriter.WriteValue(item.OrderFrom);

                pJTokenWriter.WritePropertyName("CarrierType");
                pJTokenWriter.WriteValue(item.CarrierType);

                pJTokenWriter.WritePropertyName("CreateDate");
                pJTokenWriter.WriteValue(item.CreateDate.ToString("dd/MM/yyyy"));

                pJTokenWriter.WritePropertyName("QuoteCode");
                pJTokenWriter.WriteValue(item.QuoteCode);

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

                pJTokenWriter.WriteEndObject();
            }

            pJTokenWriter.WriteEndArray();
            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

        public static string ChangeStatus(int quotationId, byte statusId)
        {
            string isSaved = "true";
            OperationsEntities db = new OperationsEntities();
            Quotation quotationDb;

            quotationDb = db.Quotations.Where(x => x.QuoteId == quotationId).FirstOrDefault();
            quotationDb.StatusId = statusId;

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
        public static QuotationView GetOneQuote(int id)
        {
            OperationsEntities db = new OperationsEntities();
            QuotationView quoteView = db.QuotationViews.Where(x => x.QuoteId == id).FirstOrDefault();
            return quoteView;
        }

        internal static List<QuotationContainerVm> GetQuotationContainers(int id)
        {
            OperationsEntities db = new OperationsEntities();
            Dictionary<int, string> contList = ListCommonHelper.GetContainerList();
            List<QuotationContainerVm> quoteContainers = new List<QuotationContainerVm>();
            var qouteConDb = db.QuotationContainers.Where(x => x.QuoteId == id).ToList();
            Mapper.CreateMap<QuotationContainer, QuotationContainerVm>().IgnoreAllNonExisting();

            Mapper.Map(qouteConDb, quoteContainers);
            foreach (var item in quoteContainers)
            {
                if (item.ContainerTypeId != 0)
                    item.ContainerTypeName = contList[item.ContainerTypeId.Value];
                else
                    item.ContainerTypeName = "Air";
            }

            return quoteContainers;
        }
    }
}

