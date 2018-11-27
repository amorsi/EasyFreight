$(function () {

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    GetTbData();

    $('#datatable-default tbody').on('click', '.details', function () {
        var invId = $(this).closest('tr').attr("id");
        var hbId = $(this).closest('tr').attr("hbid");
        ViewInvoice(invId, hbId);
    });

    $('#datatable-default tbody').on('click', '.payment', function () {
        var invId = $(this).closest('tr').attr("id");
        window.location.href = ROOT + "CashManagement/CashInReceipt?invId=" + invId;
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


    $('#datatable-default tbody').on('click', '.deleteInv', function () {
        var itemId = $(this).closest('tr').attr("id");
        $("#ivnDele").val(itemId);

        $.magnificPopup.open({
            items: {
                src: '#modelConfirmDelete',
                type: 'inline'
            },
            modal: true
        });
    });


    $('#modelConfirmDelete').on('click', '.modal-confirm', function (e) {

        e.preventDefault();
        $.magnificPopup.close();

        $.magnificPopup.open({
            items: {
                src: '#modalDelete',
                type: 'inline'
            },
            modal: true
        });
    });

    $('#modalDelete').on('click', '.delete-modal-confirm', function () {
        var invId = $("#ivnDele").val();

        var deleteReason = $("#DeleteReason").val();
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {

            $.magnificPopup.close();

            DeleteInv(invId, deleteReason);
        }


    });

    //Cash in Confirm Delete
    $('#datatable-default tbody').on('click', '.deleteCashInv', function () {
        var itemId = $(this).attr("recid");
         $("#receiptIdDele").val(itemId);
         var invId = $(this).attr("invid");
         $("#receiptIvn").val(invId);


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
        var invId = $("#receiptIvn").val();

        var deleteReason = $("#DeleteReasonReceipt").val();
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {

            $.magnificPopup.close();

            DeleteReceipt(receiptId, invId, deleteReason);
        }


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


});

var invoiceOrdersJson;

function GetTbData(searchForm) {

    $.ajax({
        url: ROOT + "Invoice/GetTableJson",
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
            { "data": "InvoiceCode", "sType": "int" },
            { "data": "DueDate", "sType": "date-uk" },
            { "data": "OperationCode" },
            { "data": "HouseBL" },
            { "data": "InvoiceDate", "sType": "date-uk" },
            { "data": "ClientName" },
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
                + '<i class="fa fa-info-circle"></i>  View Invoice</a></li> '
                + '<li><a class="payment" href="javascript:void(0)"><i class="fa fa-usd"></i> Add Payment</a></li>'
                + '<li><a class="deleteInv" href="javascript:void(0)"><i class="fa fa-trash"></i> Delete Invoice</a></li>'
                + '</ul></div>'
        }],
        order: [[3, "asc"]],
        buttons: [
            
           'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["InvoiceId"]);
            $(nRow).attr('hbid', data["HouseBillId"]);
            $(nRow).attr('invstatus', data["InvStatusId"]);
            $(nRow).find(".deleteInv").hide();
            if (data["InvStatusId"] == "4" || data["InvStatusId"] == "1") {
                //If paid 4 .. or not approved 1
                $(nRow).find(".payment").hide();
            }
            if (data["InvStatusId"] == "1" || data["InvStatusId"] == "2") {
                //If  Waiting For Approval 1  or Approved 2 can delete
                $(nRow).find(".deleteInv").show();
            }
        }
    });

};

/* Formatting function for row details - modify as you need */
function format(d) {
    // d is the original data object for the row
    var newRowHtml;

    $.ajax({
        url: ROOT + "CashManagement/GetCashReceiptsForInv",
        type: "POST",
        data: { 'invId': d["InvoiceId"], 'isOut': false },
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

function ViewInvoice(invId, hbId) {
    $.ajax({
        url: ROOT + "Invoice/ViewInvoicePartial",
        type: "POST",
        data: { 'id': invId, 'hbId': hbId },
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

function DeleteInv(invId, deleteReason) {
    //var isDeleted = "true";

    $.ajax({
        url: ROOT + "Invoice/Delete",
        type: "GET",
        data: { 'invId': invId, 'deleteReason': deleteReason },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                var table = $('#datatable-default').DataTable();
                table.row('#' + invId).remove().draw(false);

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


function DeleteReceipt(receiptId, invId, deleteReason) {
    $.ajax({
        url: ROOT + "CashManagement/DeleteCashIn",
        type: "GET",
        data: { 'receiptId': receiptId, 'invId': invId, 'deleteReason': deleteReason },
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

function GetSearchForm() {
    $.ajax({
        url: ROOT + "Invoice/AdvSearchInv",
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
