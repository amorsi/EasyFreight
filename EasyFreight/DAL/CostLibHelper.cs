using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyFreight.Models;

namespace EasyFreight.DAL
{
    public static class CostLibHelper
    {
        internal static List<TruckingCostLib> GetTruckingCostLib()
        {
            EasyFreightEntities db = new EasyFreightEntities();
            List<TruckingCostLib> costLib = db.TruckingCostLibs.ToList();
            return costLib;
        }

        internal static string AddEditTruckCost(int costlId, string costNameEn,string costNameAr)
        {
            string isSaved = "true";
            EasyFreightEntities db = new EasyFreightEntities();
            TruckingCostLib costDb;
            if (costlId == 0)
                costDb = new TruckingCostLib();
            else
                costDb = db.TruckingCostLibs.Where(x => x.TruckingCostId == costlId).FirstOrDefault();

            costDb.TruckingCostName = costNameEn;
            costDb.TruckingCostNameAr = costNameAr;

            if (costlId == 0)
                db.TruckingCostLibs.Add(costDb);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        internal static string DeleteTruckingCost(int id)
        {
            EasyFreightEntities db = new EasyFreightEntities();
            string isDeleted = "true";

            var costObj = db.TruckingCostLibs.Where(x => x.TruckingCostId == id).FirstOrDefault();
            
            db.TruckingCostLibs.Remove(costObj);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                if ((ex.InnerException).InnerException.Message.Contains("DELETE statement conflicted"))
                    isDeleted = "false. Can't delete this row as it is linked with some operation orders";
                else
                    isDeleted = "false" + ex.Message;
            }

            return isDeleted;
        }

        internal static List<OperationCostLib> GetOperationCostLib()
        {
            OperationsEntities db = new OperationsEntities();
            List<OperationCostLib> costLib = db.OperationCostLibs.ToList();
            return costLib;
        }

        internal static string AddEditOperationCost(int costId, string costNameEn, string costNameAr)
        {
            string isSaved = "true";
            OperationsEntities db = new OperationsEntities();
            OperationCostLib costDb;
            if (costId == 0)
                costDb = new OperationCostLib();
            else
                costDb = db.OperationCostLibs.Where(x => x.OperCostLibId == costId).FirstOrDefault();

            costDb.OperCostNameEn = costNameEn;
            costDb.OperCostNameAr = costNameAr;

            if (costId == 0)
                db.OperationCostLibs.Add(costDb);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        internal static string DeleteOperationCost(int id)
        {
            OperationsEntities db = new OperationsEntities();
            string isDeleted = "true";

            var costObj = db.OperationCostLibs.Where(x => x.OperCostLibId == id).FirstOrDefault();

            db.OperationCostLibs.Remove(costObj);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                if ((ex.InnerException).InnerException.Message.Contains("DELETE statement conflicted"))
                    isDeleted = "false. Can't delete this row as it is linked with some operation orders";
                else
                    isDeleted = "false" + ex.Message;
            }

            return isDeleted;
        }

        #region CustomClearance Cost
        internal static List<CustomClearanceCostLib> GetCCCostLib()
        {
            OperationsEntities db = new OperationsEntities();
            List<CustomClearanceCostLib> costLib = db.CustomClearanceCostLibs.ToList();
            return costLib;
        }

        internal static string AddEditCCCost(int costId, string costNameEn, string costNameAr)
        {
            string isSaved = "true";
            OperationsEntities db = new OperationsEntities();
            CustomClearanceCostLib costDb;
            if (costId == 0)
                costDb = new CustomClearanceCostLib();
            else
                costDb = db.CustomClearanceCostLibs.Where(x => x.CCCostId == costId).FirstOrDefault();

            costDb.CostNameEn = costNameEn;
            costDb.CostNameAr = costNameAr;

            if (costId == 0)
                db.CustomClearanceCostLibs.Add(costDb);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSaved = "false" + ex.Message;
            }

            return isSaved;
        }

        internal static string DeleteCCCost(int id)
        {
            OperationsEntities db = new OperationsEntities();
            string isDeleted = "true";

            var costObj = db.CustomClearanceCostLibs.Where(x => x.CCCostId == id).FirstOrDefault();

            db.CustomClearanceCostLibs.Remove(costObj);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                if ((ex.InnerException).InnerException.Message.Contains("DELETE statement conflicted"))
                    isDeleted = "false. Can't delete this row as it is linked with some operation orders";
                else
                    isDeleted = "false" + ex.Message;
            }

            return isDeleted;
        }
        #endregion
    }
}