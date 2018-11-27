using AutoMapper;
using EasyFreight.Models;
using EasyFreight.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;


namespace EasyFreight.DAL
{
    public static class CommonHelper
    {
        public static string AdvancedSearch<T>(FormCollection form, T obj)
        {
            System.Text.StringBuilder where = new System.Text.StringBuilder();
            string value;
            foreach (var key in form.AllKeys)
            {
                value = form[key];
                if (value.Contains(","))
                    value = value.Split(',').Where(x => x != "").FirstOrDefault();
                if (!string.IsNullOrEmpty(value) && value != "undefined" && value != "0")
                {
                    if (obj.GetType().GetProperties().Any(x => x.Name == key))
                    {
                        where.Append(key);
                        if (key == "OperationCode")
                            where.Append(".ToLower().Contains(");
                        else
                            where.Append(" == ");
                        // if (obj.GetType().GetProperty(key).GetType().ToString().ToLower().Contains("string"))
                        if (obj.GetType().GetProperty(key).PropertyType.ToString().ToLower().Contains("string"))
                        {
                            if (key == "OperationCode")
                                where.Append('"' + value + '"' + ")");
                            else
                                where.Append('"' + value + '"');

                        }
                        else
                            where.Append(value);

                        where.Append(" and ");
                    }
                    else
                    {
                        #region Date Filter
                        //Create Date filter
                        if (key == "CreateDateStart" && !string.IsNullOrEmpty(form["CreateDateStart"]))
                        {
                            DateTime fromDate = DateTime.Parse(form["CreateDateStart"]);
                            where.Append("CreateDate >= DateTime(");
                            where.Append(fromDate.Year + "," + fromDate.Month + "," + fromDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        else if (key == "CreateDateEnd" && !string.IsNullOrEmpty(form["CreateDateEnd"]))
                        {
                            //as CreateDate save date and time in DB will add one day to get the last day in the between
                            DateTime toDate = DateTime.Parse(form["CreateDateEnd"]).AddDays(1);
                            where.Append("CreateDate <= DateTime(");
                            where.Append(toDate.Year + "," + toDate.Month + "," + toDate.Day); 
                            where.Append(")");
                            where.Append(" and ");
                        }

                        //Departure Date filter
                        else if (key == "DepartureDateStart" && !string.IsNullOrEmpty(form["DepartureDateStart"]))
                        {
                            DateTime fromDate = DateTime.Parse(form["DepartureDateStart"]);
                            where.Append("DepartureDate >= DateTime(");
                            where.Append(fromDate.Year + "," + fromDate.Month + "," + fromDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        else if (key == "DepartureDateEnd" && !string.IsNullOrEmpty(form["DepartureDateEnd"]))
                        {
                            DateTime toDate = DateTime.Parse(form["DepartureDateEnd"]);
                            where.Append("DepartureDate <= DateTime(");
                            where.Append(toDate.Year + "," + toDate.Month + "," + toDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        //Invoice Date filter
                        else if (key == "InvoiceDateStart" && !string.IsNullOrEmpty(form["InvoiceDateStart"]))
                        {
                            DateTime fromDate = DateTime.Parse(form["InvoiceDateStart"]);
                            where.Append("InvoiceDate >= DateTime(");
                            where.Append(fromDate.Year + "," + fromDate.Month + "," + fromDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        else if (key == "InvoiceDateEnd" && !string.IsNullOrEmpty(form["InvoiceDateEnd"]))
                        {
                            DateTime toDate = DateTime.Parse(form["InvoiceDateEnd"]);
                            where.Append("InvoiceDate <= DateTime(");
                            where.Append(toDate.Year + "," + toDate.Month + "," + toDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        //Due Date filter
                        else if (key == "DueDateStart" && !string.IsNullOrEmpty(form["DueDateStart"]))
                        {
                            DateTime fromDate = DateTime.Parse(form["DueDateStart"]);
                            where.Append("DueDate >= DateTime(");
                            where.Append(fromDate.Year + "," + fromDate.Month + "," + fromDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        else if (key == "DueDateEnd" && !string.IsNullOrEmpty(form["DueDateEnd"]))
                        {
                            DateTime toDate = DateTime.Parse(form["DueDateEnd"]);
                            where.Append("DueDate <= DateTime(");
                            where.Append(toDate.Year + "," + toDate.Month + "," + toDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        //Agent Note Date filter
                        else if (key == "AgentNoteDateStart" && !string.IsNullOrEmpty(form["AgentNoteDateStart"]))
                        {
                            DateTime fromDate = DateTime.Parse(form["AgentNoteDateStart"]);
                            where.Append("AgentNoteDate >= DateTime(");
                            where.Append(fromDate.Year + "," + fromDate.Month + "," + fromDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        else if (key == "AgentNoteDateEnd" && !string.IsNullOrEmpty(form["AgentNoteDateEnd"]))
                        {
                            DateTime toDate = DateTime.Parse(form["AgentNoteDateEnd"]);
                            where.Append("AgentNoteDate <= DateTime(");
                            where.Append(toDate.Year + "," + toDate.Month + "," + toDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        //Receipt Note Date filter
                        else if (key == "ReceiptDateStart" && !string.IsNullOrEmpty(form["ReceiptDateStart"]))
                        {
                            DateTime fromDate = DateTime.Parse(form["ReceiptDateStart"]);
                            where.Append("ReceiptDate >= DateTime(");
                            where.Append(fromDate.Year + "," + fromDate.Month + "," + fromDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }

                        else if (key == "ReceiptDateEnd" && !string.IsNullOrEmpty(form["ReceiptDateEnd"]))
                        {
                            DateTime toDate = DateTime.Parse(form["ReceiptDateEnd"]);
                            where.Append("ReceiptDate <= DateTime(");
                            where.Append(toDate.Year + "," + toDate.Month + "," + toDate.Day);
                            where.Append(")");
                            where.Append(" and ");
                        }
                        #endregion
                    }
                }
            }

            if (!string.IsNullOrEmpty(form["ScreenName"]))
            {
                if (form["ScreenName"] == "accoper" && string.IsNullOrEmpty(form["StatusId"]))
                    where.Append(" (StatusId == 3 OR StatusId == 6) ");
            }

            if (string.IsNullOrEmpty(form["StatusId"])
                && obj.GetType().GetProperty("StatusId") != null && !where.ToString().Contains("StatusId"))
                where.Append(" (StatusId == 1 OR StatusId == 2 ) ");

            if (where.ToString().Trim().EndsWith("and"))
                where.Remove(where.Length - 4, 4);



            return where.ToString();

        }

        public static StaticText GetStaticTextById(int staticTextId)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            StaticText staticText = db.StaticTexts.Where(x => x.StaticTextId == staticTextId).FirstOrDefault();
            return staticText;
        }

        public static CompanyInfoVm GetCompInfo()
        {
            CompanyInfoVm compInfo = new CompanyInfoVm();
            EasyFreightEntities db = new EasyFreightEntities();
            CompanySetup compSetupDb = db.CompanySetups.FirstOrDefault();
            Mapper.CreateMap<CompanySetup, CompanyInfoVm>();
            Mapper.Map(compSetupDb, compInfo);
            return compInfo;
        }

        public static Dictionary<string, string> GetStaticLabels(int staticTextScreenId, string langCode)
        {
            Dictionary<string, string> staticLabels = new Dictionary<string, string>();
            EasyFreightEntities db = new EasyFreightEntities();
            var staticLabelsDb = db.StaticTextLabels.Where(x => x.StaticTextId == staticTextScreenId)
                .Select(x => new { x.FieldPrefix, x.TextAr, x.TextEn }).ToList();
            if (langCode == "en")
                staticLabels = staticLabelsDb.ToDictionary(x => x.FieldPrefix, x => x.TextEn);
            else
                staticLabels = staticLabelsDb.ToDictionary(x => x.FieldPrefix, x => x.TextAr);

            return staticLabels;
        }

        /// <summary>
        /// Use this to get json format for Pie chat used in charts.js
        /// </summary>
        /// <param name="chartData">Dictionary string "For Label Name", int "For Label count"</param>
        /// <returns>Json</returns>
        public static JObject GetPieChartData(Dictionary<string, int> chartData)
        {
            if (chartData == null || chartData.Count == 0)
                return null;

            Random randomGen = new Random();
            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("labels");
            pJTokenWriter.WriteStartArray();
            foreach (var item in chartData)
            {
                pJTokenWriter.WriteValue(item.Key);
            }
            pJTokenWriter.WriteEndArray();

            pJTokenWriter.WritePropertyName("datasets");
            pJTokenWriter.WriteStartArray(); //datasets array
            pJTokenWriter.WriteStartObject();//datasets Object
            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray(); //data array
            foreach (var item in chartData)
            {
                pJTokenWriter.WriteValue(item.Value);
            }
            pJTokenWriter.WriteEndArray();//data array

            pJTokenWriter.WritePropertyName("backgroundColor");
            pJTokenWriter.WriteStartArray(); //backgroundColor array
            foreach (var item in chartData)
            {
                pJTokenWriter.WriteValue("rgb(" + randomGen.Next(256) + "," + randomGen.Next(256) + "," + randomGen.Next(256) + ")");
            }
            pJTokenWriter.WriteEndArray();//backgroundColor array

            pJTokenWriter.WriteEndObject();//datasets Object
            pJTokenWriter.WriteEndArray();//datasets array

            pJTokenWriter.WriteEndObject();

            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }

    }
}