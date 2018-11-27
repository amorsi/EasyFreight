$(function () {
    $(".invlist").hide();

    $("#cashinlink").on('click', function () {
        
        if (!$('#cashincontent').length) {
                GetCashInList();
        }
    });

    $("#agentnotelink").on('click', function () {

        if (!$('#agentnotecontent').length) {
            GetAgentNoteList();
        }
    });

    $("#aplink").on('click', function () {

        if (!$('#apcontent').length) {
            GetAPInvoiceList();
        }
    });

    $("#cccashlink").on('click', function () {

        if (!$('#tbcccashdeplist').length) {
            GetCCCashList();
        }
    });


    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });


    $(document).on('click', '.hbcost', function () {
        var hbId = $(this).attr("hbid");
        GetOperOrHbCost(0, hbId);
        $("#modalheader").html("House Bill cost analysis");
        $("#printopercost").attr("href", ROOT + "Accounting/PrintOperCostDetails?operationId=0&hbId=" + hbId);
    });

    $(document).on('click', '#ccsettlementbtn', function () {
        var ccSettlAmount = $(this).parent('td').find(".cccashdepdiff").val();

        if (ccSettlAmount > 0) // Deposit Total > AP invoices .. will need cash in receipt for that amount
        {
            
            var operId = $(this).closest('tr').attr("id");
            window.location.href = ROOT + "CashManagement/CCCashSettlement?operationId=" + operId + "&receiptAmount=" + ccSettlAmount;
        }
        else {
            ccSettlAmount = ccSettlAmount * -1;
            // will need cash out receipt for that amount .. increase the cc cash deposit
            var operId = $(this).closest('tr').attr("id");
            window.location.href = ROOT + "CashManagement/CCCashDeposit?operationId=" + operId + "&receiptAmount=" + ccSettlAmount;
        }
    });
});

function GetOperOrHbCost(operId, hbId) {
    $.ajax({
        url: ROOT + "Accounting/GetOperationFullCost",
        type: "POST",
        data: { 'operationId': operId, 'hbId': hbId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#ModalContent").html(data);
            // $("#OperCostFullTb").dataTable();
            $.magnificPopup.open({
                items: {
                    src: '#modalAnim',
                    type: 'inline'
                },
                modal: true
            });
        }
    });
}

function GetCashInList()
{
    $.ajax({
        url: ROOT + "Accounting/GetCashIn",
        type: "POST",
        data: { 'operationId': 1 },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#cashin").html(data);

        }
    });
}

function GetAgentNoteList() {
    var operId = $("#operationid").val();
    $.ajax({
        url: ROOT + "Accounting/GetAgNoteTab",
        type: "POST",
        data: { 'operId': operId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#agentnote").html(data);

        }
    });
}

function GetAPInvoiceList() {
    var operId = $("#operationid").val();
    $.ajax({
        url: ROOT + "Accounting/GetAPInvoiceTab",
        type: "POST",
        data: { 'operId': operId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#accpayable").html(data);

        }
    });
}

function GetCCCashList()
{
    var operId = $("#operationid").val();
    $.ajax({
        url: ROOT + "CashDeposit/GetCCCashDepositList",
        type: "POST",
        data: { 'operId': operId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#cccash").html(data);

        }
    });
}