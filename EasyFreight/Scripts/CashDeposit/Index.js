$(document).ready(function () {

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    $(document).on('click', '.print', function () {
        var recId = $(this).closest('tr').attr("id");
        window.open(
          ROOT + "CashDeposit/PrintCashIn?id=" + recId ,
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
      

        var deleteReason = $("#DeleteReasonReceipt").val();
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            $.magnificPopup.close();
            DeleteReceipt(receiptId, deleteReason);
        }
    });

    //View receipt
    $(document).on('click', '.details', function () {
        var recId = $(this).closest('tr').attr("id");
        ViewReceipt(recId);
    });

    datatableInit();

});

    function datatableInit() {
        $('#datatable-default').DataTable({
            "processing": true,
            "serverSide": false,
            "ajax": {
                "url": ROOT + "CashDeposit/GetTableJson"
            },
            "columns": [
                { "data": "ReceiptCode" },
                { "data": "PaymentTermEn" },
                { "data": "ReceiptDate", "sType": "date-uk" },
                { "data": "ReceiptAmount" },
                { "data": "Client" },
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
    }


function ViewReceipt(recId) {
    $.ajax({
        url: ROOT + "CashDeposit/ViewCashInPartial",
        type: "POST",
        data: { 'id': recId },
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



function DeleteReceipt(receiptId, deleteReason) {
  $.ajax({
        url: ROOT + "CashDeposit/DeleteCashDeposit",
        type: "GET",
        data: { 'receiptId': receiptId, 'deleteReason': deleteReason },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                datatableInit();
                new PNotify({
                    title: 'Success!',
                    text: 'Deleted has been done.',
                    type: 'success'
                });
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


