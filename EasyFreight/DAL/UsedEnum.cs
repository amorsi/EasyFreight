using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public enum PrefixForEnum
{
    Export = 1,
    Import = 2,
    QuoteExport = 3,
    QuoteImport = 4,
    TruckingOrder = 5,
    AccountingInvoice = 6,
    AgentNote = 7,
    CashIn = 8,
    APInvoice,
    CashOut
}

public enum StatusEnum : byte
{
    New = 1,
    Opened = 2,
    Closed = 3,
    Canceled = 4,
    Rollback = 5,
    InvoiceIssued = 6
}

public enum CostType : byte
{
    OperationCost = 1,
    TruckingCost,
    CCCost
}

public enum InvStatusEnum : byte
{
    WaitingForApproval = 1,
    Approved,
    PartiallyPaid,
    Paid
}

public enum PaymentTermEnum : byte
{
    Cash = 1,
    Credit,
    BankCashDeposit,
    Check,
    FromDeposit,
    CashToBank,
    BankToCash,
    CurrencyExchange,
    BankToBank
}

public enum StaticTextForScreenEnum
{
    ConcessionLetter = 2,
    ConcessionLetterAir,
    DeliveryNote
}


public enum UsedCurrencies : byte
{
    EGP,
    USD,
    EUR,
    GBP
}