using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public enum ScreenEnum
{
    Agent = 1,
    Carrier,
    Consignee,
    Contractor,
    Notifier,
    Shipper,
    Bank,
    ContainersTypes,
    PackagesTypes,
    VesselsLibrary,
    CountriesLibrary,
    CitiesLibrary,
    PortsLibrary,
    AreasLibrary, 
    CarrierRate,
    ContractorRate,
    CarrierRatePriceList,
    ContractorRateList,
    ImportQuotation,
    ImportMBL,
    ImportSummary,
    ExportQuotation,
    ExportMBL,
    ExportSummary,
    TruckingOrders,
    TruckingSummary,
    CustomClearanceOrders,
    CustomClearanceSummary,
    CompanySetup,
    PrefixSetup,
    Security,
    Employee,
    ImportHB,
    ExportHB,
    AccOperationsList,
    InvoiceList,
    TruckingCostLib,
    OperationCostLib,
    CustomClearanceCostLib,
    AgentNote,
    CashManagment, //41
    APInvoice,
    IncotermLibrary,
    OpenBalance,
    ExpensesLibrary, //45
    ManageExpenses

}

public enum ActionEnum
{
    ViewAll = 1,
    Add,
    Edit,
    Delete,
    ProcessToMBL,
    CloseOrder,
    ProcessToHB,
    ManageOrder,
    RollOrder,
    SetRights,
    AddEditTruckingOrder,
    AddEditCCOrder,
    CancelTruckingOrder,
    CancelCCOrder,
    Print

}