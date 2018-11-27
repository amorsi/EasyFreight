$(function () {

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    GetTbData();

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

    $('#datatable-default tbody').on('click', '.details', function () {
        var recId = $(this).closest('tr').attr("id");
        ViewReceipt(recId);
    });

    $('#datatable-default tbody').on('click', '.print', function () {
        var recId = $(this).closest('tr').attr("id");

        window.open(
          ROOT + "CashOutExpense/PrintReceipt?receiptId=" + recId,
          '_blank' 
        );

    });

    $('#datatable-default tbody').on('click', '.deleterec', function () {
        var itemId = $(this).closest('tr').attr("id");
        $("#ivnDele").val(itemId);

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
        var receiptId = $("#ivnDele").val();

        var deleteReason = $("#DeleteReasonReceipt").val();
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {

            $.magnificPopup.close();

            DeleteReceipt(receiptId, deleteReason);
        }


    });

    //clear filter
    $(".filter0").click(function () {
        GetTbData();
        ClearForm();
    });

    $("#advsearchlink").click(function () {
       
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
    });

});

var cashRecExpJson;

function GetTbData(searchForm) {

    $.ajax({
        url: ROOT + "CashOutExpense/GetTableJson",
        type: "POST",
        data: searchForm,
        async: false,
        success: function (data) {
            cashRecExpJson = data;
            if ((jQuery.parseJSON(cashRecExpJson).data) != "")
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
        table.fnAddData(jQuery.parseJSON(cashRecExpJson).data);
        return;
    }
    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(cashRecExpJson).data,
        columns: [
             {
                 "className": 'details-control',
                 "orderable": false,
                 "data": null,
                 "defaultContent": '',
                 "width": "2%"
             },
            { "data": "ReceiptCode" },
            { "data": "ReceiptDate", "sType": "date-uk" },
            { "data": "ReceiptAmount" },

            { "data": "Notes" },
            { "data": "CreateDate", "sType": "date-uk" },
            { "data": "UserName" },
            { data: null, "orderable": false, "width": "12%" }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<div class="btn-group"> ' +
                '<button type="button" class="mb-xs mt-xs mr-xs btn btn-default dropdown-toggle" data-toggle="dropdown">Actions <span class="caret"></span></button> ' +
                '<ul class="dropdown-menu" role="menu">' +
                '<li><a class="details modal-details" href="javascript:void(0)">'
                + '<i class="fa fa-info-circle"></i>  View Receipt</a></li> '
                + '<li><a class="print" href="javascript:void(0)"><i class="fa fa-print"></i> Print Receipt</a></li>'
                + '<li><a class="deleterec" href="javascript:void(0)"><i class="fa fa-trash"></i> Delete Receipt</a></li>'
                + '</ul></div>'
        }],
        order: [[2, "desc"]],
        buttons: [

           'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["ReceiptId"]);
        }
    });

};

/* Formatting function for row details - modify as you need */
function format(d) {
    // d is the original data object for the row
    var newRowHtml;

    $.ajax({
        url: ROOT + "CashOutExpense/GetExpensesForReceipt",
        type: "POST",
        data: { 'receiptId': d["ReceiptId"] },
        async: false,
        success: function (data) {
            newRowHtml = data;
        }

    });

    return newRowHtml;
}

function ViewReceipt(recId) {
    $.ajax({
        url: ROOT + "CashOutExpense/ViewReceipt",
        type: "POST",
        data: { 'receiptId': recId },
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

function ClearForm() {
    $("#searchform input:text").each(function () {
        $(this).val("");
    });

    $("#searchform select").val("").trigger("change");

}


function DeleteReceipt(receiptId, deleteReason) {
    $.ajax({
        url: ROOT + "CashManagement/DeleteCashOutReceipt",
        type: "GET",
        data: { 'receiptId': receiptId,  'deleteReason': deleteReason },
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
