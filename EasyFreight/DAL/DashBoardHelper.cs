using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;

namespace EasyFreight.DAL
{
    public static class DashBoardHelper
    {
        public static JObject GetCountByOrderTypePie(int year = 0)
        {
            OperationsEntities db = new OperationsEntities();
            var operationsGrouped = (Dictionary<string, int>)null;
            if (year == 0)
            {
                operationsGrouped = db.Operations.GroupBy(x => x.OrderFrom)
                .Select(x => new { key = x.Key, Count = x.Count() })
                .ToDictionary(x => x.key == 1 ? "Export" : "Import", x => x.Count);
            }
            else
            {
                operationsGrouped = db.Operations.Where(x => x.OperationDate.Year == year).GroupBy(x => x.OrderFrom)
                .Select(x => new { key = x.Key, Count = x.Count() })
                .ToDictionary(x => x.key == 1 ? "Export" : "Import", x => x.Count);
            }

            var pieChartData = CommonHelper.GetPieChartData(operationsGrouped);
            return pieChartData;

        }

        public static JObject GetCountByCarrierTypePie(int year = 0)
        {
            OperationsEntities db = new OperationsEntities();
            var operationsGrouped = (Dictionary<string, int>)null;
            if (year == 0)
            {
                operationsGrouped = db.Operations.GroupBy(x => x.CarrierType)
                .Select(x => new { key = x.Key, Count = x.Count() })
                .ToDictionary(x => x.key == 1 ? "Sea" : "Air", x => x.Count);
            }
            else
            {
                operationsGrouped = db.Operations.Where(x => x.OperationDate.Year == year).GroupBy(x => x.CarrierType)
                .Select(x => new { key = x.Key, Count = x.Count() })
                .ToDictionary(x => x.key == 1 ? "Sea" : "Air", x => x.Count);
            }

            var pieChartData = CommonHelper.GetPieChartData(operationsGrouped);
            return pieChartData;

        }

        public static JObject GetCountByStatusPie(int year = 0)
        {
            OperationsEntities db = new OperationsEntities();
            var operationsGrouped = (Dictionary<string, int>)null;
           //if (year == 0)
           // {
           //     operationsGrouped = db.Operations.Include("StatusLib").GroupBy(x => x.StatusLib.StatusName)
           //     .Select(x => new { key = x.Key, Count = x.Count() })
           //     .ToDictionary(x => x.key, x => x.Count);
           // }
           // else
           // {
           //     operationsGrouped = db.Operations.Include("StatusLib").Where(x => x.OperationDate.Year == year)
           //        .GroupBy(x => x.StatusLib.StatusName)
           //     .Select(x => new { key = x.Key, Count = x.Count() })
           //     .ToDictionary(x => x.key, x => x.Count);
           // }
          
            var pieChartData = CommonHelper.GetPieChartData(operationsGrouped);
            return pieChartData;

        }

        public static JObject GetOperationsCountLineChart(int year = 0)
        {
            OperationsEntities db = new OperationsEntities();
            var serviceUsageList = db.Operations
                .Where(x => x.StatusId != 4).Select(x => new { x.OperationDate, x.CreateDate }).ToList();
            if (year == 0)
                year = DateTime.Now.Year;

            int[] monthsNums = serviceUsageList.Where(x => x.OperationDate.Year == year)
            .Select(X => X.OperationDate.Month).Distinct().OrderBy(x => x).ToArray();

            JTokenWriter pJTokenWriter = new JTokenWriter();
            JObject ordersJson = new JObject();
            pJTokenWriter.WriteStartObject();
            pJTokenWriter.WritePropertyName("labels");
            pJTokenWriter.WriteStartArray();
            // pJTokenWriter.WriteValue(minDate.ToString("dd/MM/yyyy"));

            for (int i = 0; i < monthsNums.Length; i++)
            {
                pJTokenWriter.WriteValue(((MonthNameEnum)monthsNums[i]).ToString());
            }

            pJTokenWriter.WriteEndArray();

            pJTokenWriter.WritePropertyName("datasets");
            pJTokenWriter.WriteStartArray(); //datasets array
            pJTokenWriter.WriteStartObject();//datasets Object

            pJTokenWriter.WritePropertyName("label");
            pJTokenWriter.WriteValue(" عدد العمليات");
            pJTokenWriter.WritePropertyName("fill");
            pJTokenWriter.WriteValue(false);
            pJTokenWriter.WritePropertyName("lineTension");
            pJTokenWriter.WriteValue(0.1);
            pJTokenWriter.WritePropertyName("backgroundColor");
            pJTokenWriter.WriteValue("rgba(75,192,192,0.4)");
            pJTokenWriter.WritePropertyName("borderColor");
            pJTokenWriter.WriteValue("rgba(75,192,192,1)");
            pJTokenWriter.WritePropertyName("borderCapStyle");
            pJTokenWriter.WriteValue("butt");
            //pJTokenWriter.WritePropertyName("borderDash");
            //pJTokenWriter.WriteValue([]);
            pJTokenWriter.WritePropertyName("borderDashOffset");
            pJTokenWriter.WriteValue(0.0);
            pJTokenWriter.WritePropertyName("borderJoinStyle");
            pJTokenWriter.WriteValue("miter");
            pJTokenWriter.WritePropertyName("pointBorderColor");
            pJTokenWriter.WriteValue("rgba(75,192,192,1)");
            pJTokenWriter.WritePropertyName("pointBackgroundColor");
            pJTokenWriter.WriteValue("#fff");
            pJTokenWriter.WritePropertyName("pointBorderWidth");
            pJTokenWriter.WriteValue(1);
            pJTokenWriter.WritePropertyName("pointHoverRadius");
            pJTokenWriter.WriteValue(5);
            pJTokenWriter.WritePropertyName("pointHoverBackgroundColor");
            pJTokenWriter.WriteValue("rgba(75,192,192,1)");
            pJTokenWriter.WritePropertyName("pointHoverBorderColor");
            pJTokenWriter.WriteValue("rgba(220,220,220,1)");
            pJTokenWriter.WritePropertyName("pointHoverBorderWidth");
            pJTokenWriter.WriteValue(2);
            pJTokenWriter.WritePropertyName("pointRadius");
            pJTokenWriter.WriteValue(1);
            pJTokenWriter.WritePropertyName("pointHitRadius");
            pJTokenWriter.WriteValue(10);
            pJTokenWriter.WritePropertyName("spanGaps");
            pJTokenWriter.WriteValue(false);

            pJTokenWriter.WritePropertyName("data");
            pJTokenWriter.WriteStartArray();
            int count;

            for (int i = 0; i < monthsNums.Length; i++)
            {

                count = serviceUsageList
                .Where(x => x.OperationDate.Month == monthsNums[i] && x.OperationDate.Year == DateTime.Now.Year).Count();

                pJTokenWriter.WriteValue(count);
            }


            pJTokenWriter.WriteEndArray();

            pJTokenWriter.WriteEndObject();//datasets Object
            pJTokenWriter.WriteEndArray();//datasets array

            pJTokenWriter.WriteEndObject();
            ordersJson = (JObject)pJTokenWriter.Token;
            return ordersJson;
        }
    }

    enum MonthNameEnum
    {
        January = 1,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }
}