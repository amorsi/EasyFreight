$(function () {

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    GetTbData('', 'cashin');

    $('#datatable-default tbody').on('click', '.print', function () {
        var recId = $(this).closest('tr').attr("id");
        var isOut = false;
        if ($(this).closest('.active').attr("id") == 'cashout') {
            isOut = true;
        }
        window.open(
          ROOT + "CashManagement/PrintCashIn?id=" + recId + '&isOut=' + isOut,
          '_blank'
        );

    });

    //delete receipt 
    $(document).on('click', '.deleterec', function () {
        var recId = $(this).closest('tr').attr("id");
        $("#receiptIdDele").val(recId);
       
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
        var invId = 0;

        var deleteReason = $("#DeleteReasonReceipt").val();
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
             $.magnificPopup.close(); 
            DeleteReceipt(receiptId, invId, deleteReason);
        } 
    });


    //View receipt
    $(document).on('click', '.details', function () {
        var recId = $(this).closest('tr').attr("id");
        var isOut = false;
        if ($(this).closest('.active').attr("id") == 'cashout') {
            isOut = true;
        }
        ViewReceipt(recId, isOut);
    });


    $("#cashoutlink").on('click', function () {
        table = $('#cashin #datatable-default').DataTable();
        table.destroy();

        GetTbData('', 'cashout');
        $("#cashtype").val("cashout");
        ClearForm();
    });

    $("#cashinlink").on('click', function () {
        table = $('#cashout #datatable-default').DataTable();
        table.destroy();

        GetTbData('', 'cashin');
        $("#cashtype").val("cashin");
        ClearForm();
    });

    //clear filter
    $(".filter0").click(function () {
        GetTbData('', $("#cashtype").val());
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
        GetTbData(form, $("#cashtype").val());

    });

});

var cashRecExpJson;

function GetTbData(searchForm, parentDivId) {

    $.ajax({
        url: ROOT + "CashManagement/GetTableJson",
        type: "POST",
        data: searchForm + '&cashType=' + parentDivId,
        async: false,
        success: function (data) {
            cashRecExpJson = data;
            if ((jQuery.parseJSON(cashRecExpJson).data) != "")
                datatableInit(parentDivId);
            else {
                
                table = $('#' + parentDivId + ' #datatable-default').dataTable();
                table.fnClearTable();
            }
        }

    });
}

function datatableInit(parentDivId) {
    if ($.fn.dataTable.isDataTable('#' + parentDivId + ' #datatable-default')) {
        table = $('#' + parentDivId + ' #datatable-default').dataTable();
        table.fnClearTable();
        table.fnAddData(jQuery.parseJSON(cashRecExpJson).data);
        return;
    }
    $('#' + parentDivId + ' #datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(cashRecExpJson).data,
        columns: [

            { "data": "ReceiptCode" },
            { "data": "PaymentTermEn" },
            { "data": "ReceiptDate", "sType": "date-uk" },
            { "data": "ReceiptAmount" },
            { "data": "ReceiptFor" },
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
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["ReceiptId"]);
        },
        dom: 'Blfrtip',
        buttons: [
               {
                   extend: 'excelHtml5',
                   exportOptions: {
                       columns: [0, ':visible']
                   }
               },
              {
                  extend: 'pdfHtml5',
                  exportOptions: {
                      columns: [0, ':visible']
                  } 
              },
               {
                   extend: 'print',
                   footer: true,
                   exportOptions: {
                       columns: [0, ':visible'],
                   },
                   text: '<i class="fa fa-print"></i> Print' 
               },
              'colvis'

        ]
    });

};



function ViewReceipt(recId, isOut) {
    $.ajax({
        url: ROOT + "CashManagement/ViewCashInPartial",
        type: "POST",
        data: { 'id': recId, 'isOut': isOut },
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

function GetSearchForm() {
    $.ajax({
        url: ROOT + "CashManagement/AdvSearch",
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

function DeleteReceipt(receiptId, invId, deleteReason) {
    var cashType = $("#cashtype").val();
    var methodName = "CashManagement/DeleteCashIn";

    if (cashType == "cashout")
        methodName = "CashManagement/DeleteCashOut";

    $.ajax({
        url: ROOT + methodName,
        type: "GET",
        data: { 'receiptId': receiptId, 'invId': invId, 'deleteReason': deleteReason },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                GetTbData('', cashType);
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
