using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyFreight.DAL
{
    public enum AccountingChartEnum
    {
        Inventory = 16,
        AccountsRecievable = 171,
        NotesReceivable = 172,
        CashInBanks = 193,
        Cash = 194,
        AccountsPayable = 281,
        NotesPayable = 282,
        SoldServices = 414,
        CarrierCostOfSales = 361,
        TruckingCostOfSales = 362,
        CCCostOfSales = 363,
        CashDepositTemp = 17711,
        TaxDepositDebit = 1743, //مصلحة الضرائب العامة (مبالغ مخصومة من الشركة بمعرفة الغير
        GeneralAndAdministrativeExpenses = 33,  //مصروفات ادارية وعمومية
        Agents = 178,  //وكلاء الشحن
        PartnersDrawingAccounts = 283, //جاري الشركاء
        EngAshrafDrawingAccounts = 2831,  //جاري مهندس اشرف
        MadamMarwaDrawingAccounts = 2832,  //جاري مدام مروة
        CustomClearanceSupplier = 284,  //تكاليف التخليص الجمركى
        APCarriers = 2811,  //شركات الشحن 
        APContractors = 2812,  //مقاولون النقل
        CurrencyDifferences = 445,  //فروق العملة
        DepositRevenues = 287, //إيرادات محصلة مقدما
        VAT = 285


    }
}