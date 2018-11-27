$(function () {

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    GetTbData();

    $('#datatable-default tbody').on('click', '.details', function () {
        var agNoteId = $(this).closest('tr').attr("id");
        ViewAgNote(agNoteId);
    });

    // Add event listener for opening and closing details
    $('#datatable-default tbody').on('click', 'td.details-control', function () {
        if ($.fn.dataTable.isDataTable('#datatable-default')) {
            var tr = $(this).closest('tr');
            var table = $('#datatable-default').DataTable();
            var row = table.row(tr);

            if (row.child.isShown()) {
                // This row is already open - close it
                row.child.hide();
                tr.removeClass('shown');
            }
            else {
                // Open this row
                row.child(format(row.data())).show();
                tr.addClass('shown');
            }
        }
    });

    $('#datatable-default tbody').on('click', '.payment', function () {
        var agNoteId = $(this).closest('tr').attr("id");
        var cashType = $(this).attr("cashtype");
        if (cashType == "cashin")
            window.location.href = ROOT + "CashManagement/CashInReceipt?invId=0&agNoteId=" + agNoteId;
        else
            window.location.href = ROOT + "CashManagement/CashOutReceipt?invId=0&agNoteId=" + agNoteId;
    });


    $(".filter1").click(function () {
        $(".filter1").each(function () {
            $(this).removeClass("clicked-icon");
            $(this).addClass("default-icon");
        });

        $(this).removeClass("default-icon");
        $(this).addClass("clicked-icon");
        var form = $("#searchform").serialize() + "&OrderFrom=" + $(this).attr("orderfrom");
        GetTbData(form);
    });


    //clear filter
    $(".filter0").click(function () {
        $(".filter1").each(function () {
            $(this).removeClass("clicked-icon");
            $(this).addClass("default-icon");
        });
        $("#AllType").addClass("clicked-icon");
        GetTbData();
        ClearForm();
    });

    $("#advsearchlink").click(function () {
        if (!$('#searchform').length) {
            GetSearchForm();
        }
        $.magnificPopup.open({
            items: {
                src: '#modalForm',
                type: 'inline'
            },
            modal: true
        });
    });

    //Search form submit
    $(document).on('click', '.search-modal-confirm', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var form = $("#searchform").serialize();
        GetTbData(form);

        $(".filter1").each(function () {
            $(this).removeClass("clicked-icon");
            $(this).addClass("default-icon");
        });

        $(".filter1").closest("#AllType").addClass("clicked-icon");

    });


    //Cash in Confirm Delete
    $('#datatable-default tbody').on('click', '.deleteCashInv', function () {
        var itemId = $(this).attr("recid");
        $("#receiptIdDele").val(itemId);
        var invId = $(this).attr("agnid");
        $("#agentNoteId").val(invId);
        $("#isOut").val($(this).attr("isOut"));
         

        $.magnificPopup.open({
            items: {
                src: '#modelConfirmDeleteReceipt',
                type: 'inline'
            },
            modal: true
        });
    });

    $('#modelConfirmDeleteReceipt').on('click', '.modal-confirmReceipt', function (e) {

        e.preventDefault();
        $.magnificPopup.close();

        $.magnificPopup.open({
            items: {
                src: '#modalDeleteReceipt',
                type: 'inline'
            },
            modal: true
        });
    });


    $('#modalDeleteReceipt').on('click', '.delete-modal-confirmReceipt', function () {
        var receiptId = $("#receiptIdDele").val();
        var invId = $("#agentNoteId").val();

        var deleteReason = $("#DeleteReasonReceipt").val();
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {

            $.magnificPopup.close();

            DeleteReceipt(receiptId, invId, deleteReason);
        }


    });


});

var invoiceOrdersJson;

function GetTbData(searchForm) {

    $.ajax({
        url: ROOT + "AgentNote/GetTableJson",
        type: "POST",
        data: searchForm,
        async: false,
        success: function (data) {
            invoiceOrdersJson = data;
            if ((jQuery.parseJSON(invoiceOrdersJson).data) != "")
                datatableInit();
            else {
                table = $('#datatable-default').dataTable();
                table.fnClearTable();
            }
        }

    });
}

function datatableInit() {
    if ($.fn.dataTable.isDataTable('#datatable-default')) {
        table = $('#datatable-default').dataTable();
        table.fnClearTable();
        table.fnAddData(jQuery.parseJSON(invoiceOrdersJson).data);
        return;
    }
    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(invoiceOrdersJson).data,
        columns: [
             {
                 "className": 'details-control',
                 "orderable": false,
                 "data": null,
                 "defaultContent": '',
                 "width": "2%"
             },
            { "data": "OrderFromImg", "orderable": false, "width": "2%" },
            { "data": "AgentNoteCode" },
            { "data": "DueDate", "sType": "date-uk" },
            { "data": "OperationCode" },
            { "data": "MBL" },
            { "data": "AgentNoteDate", "sType": "date-uk" },
            { "data": "AgentName" },
            { "data": "TotalAmount" },
            { "data": "AmountDue" },
            { "data": "InvStatusName" },
            { data: null, "orderable": false, "width": "14%" }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<div class="btn-group"> ' +
                '<button type="button" class="mb-xs mt-xs mr-xs btn btn-default dropdown-toggle" data-toggle="dropdown">Actions <span class="caret"></span></button> ' +
                '<ul class="dropdown-menu" role="menu">' +
                '<li><a class="details modal-details" href="javascript:void(0)">'
                + '<i class="fa fa-info-circle"></i>  View Note</a></li> '
                + '<li><a class="payment" cashtype="cashin" href="javascript:void(0)"><i class="fa fa-usd"></i> Add Cash In</a></li>'
                + '<li><a class="payment" cashtype="cashout" href="javascript:void(0)"><i class="fa fa-usd"></i> Add Cash Out</a></li>'
                + '</ul></div>'
        }],
        order: [[3, "asc"]],
        buttons: [

           'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["AgentNoteId"]);
            $(nRow).attr('operid', data["OperationId"]);
            $(nRow).attr('invstatus', data["InvStatusId"]);
            $(nRow).attr('agnotetype', data["AgentNoteType"]);
            if (data["InvStatusId"] == "4" || data["InvStatusId"] == "1") {
                //If paid 4 .. or not approved 1
                $(nRow).find(".payment").hide();
            }

            if (data["AgentNoteType"] == "1")
                $(nRow).find('[cashtype="cashout"]').hide(); //hide cash out for debit note
                else
                $(nRow).find('[cashtype="cashin"]').hide(); //hide cash in for credit note
        }
    });

};

/* Formatting function for row details - modify as you need */
function format(d) {
    // d is the original data object for the row
    var newRowHtml;
    var isOut = false;
    if (d["AgentNoteType"] == "2")
    {
        isOut = true;
    }
    $.ajax({
        url: ROOT + "CashManagement/GetCashReceiptsForNote",
        type: "POST",
        data: { 'agNoteId': d["AgentNoteId"] ,isOut : isOut},
        async: false,
        success: function (data) {
            newRowHtml = data;
        }

    });

    return newRowHtml;
}


function ClearForm() {
    $("#searchform input:text").each(function () {
        $(this).val("");
    });

    $("#searchform select").val("").trigger("change");

}

function ViewAgNote(agNoteId) {
    $.ajax({
        url: ROOT + "AgentNote/ViewAgNotePartial",
        type: "POST",
        data: { 'id': agNoteId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#ModalContent").html(data);

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

function GetSearchForm() {
    $.ajax({
        url: ROOT + "AgentNote/AdvSearch",
        type: "POST",
        async: false,
        dataType: "html",
        success: function (data) {
            $("#searchdiv").html(data);
            $("select").select2({
                allowClear: true
            });
            $(".date").datepicker();

        }
    });
}

function DeleteReceipt(receiptId, agnid, deleteReason) {
    var isOut = $("#isOut").val();

    var funurl='';
    if (isOut == "True")
        funurl = "AgentNote/DeleteAgentOut";
    else
        funurl = "AgentNote/DeleteAgentIn";

     $.ajax({
         url: ROOT + funurl,
        type: "GET",
        data: { 'receiptId': receiptId, 'agnid': agnid, 'deleteReason': deleteReason },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                GetTbData();
                new PNotify({
                    title: 'Success!',
                    text: 'Deleted has been done.',
                    type: 'success'
                });
                ClearForm();
            }
            else {
                new PNotify({
                    title: 'Sorry!',
                    text: data,
                    type: 'error'
                });
            }
        }
    });

    //return isDeleted;
}


