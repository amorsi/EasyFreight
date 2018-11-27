$(function () {

    GetTbData();

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    $('#datatable-default tbody').on('click', '.paycheck', function () {
        var receiptId = $(this).closest('tr').attr("id");

        $("#receiptIdToPay").val(receiptId);

        $.magnificPopup.open({
            items: {
                src: '#modelConfirmCollect',
                type: 'inline'
            },
            modal: true
        });
    });

            /*
        collect Confirm
        */
    $(document).on('click', '#btnConfirmCollect', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var idToPay = $("#receiptIdToPay").val()
        var isPaid = PayCheck(idToPay);
        if (isPaid == "true") {
            new PNotify({
                title: 'Success!',
                text: 'Check Collected.',
                type: 'success'
            });
            //Delete row from table

            var row = $('#datatable-default').find('#' + idToPay);
            $(row).find(".paycheck").hide();
            $(row).find(".status").html('Paid');
        }
        else {
            new PNotify({
                title: 'Sorry!',
                text: isPaid,
                type: 'error'
            });
        }
    });

});

var aPChecksJson;

function GetTbData(searchForm) {

    $.ajax({
        url: ROOT + "CheckManagement/GetAPChecksJson",
        type: "POST",
        data: searchForm,
        async: false,
        success: function (data) {
            aPChecksJson = data;
            if ((jQuery.parseJSON(aPChecksJson).data) != "")
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
        table.fnAddData(jQuery.parseJSON(aPChecksJson).data);
        return;
    }
    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(aPChecksJson).data,
        columns: [
            { "data": "CheckNumber" },
            { "data": "CheckDueDate", "sType": "date-uk" },
            {
                "data": "CustomerName"
            },
            { "data": "ReceiptCode" },
            { "data": "ReceiptDate", "sType": "date-uk" },
            { "data": "ReceiptAmount" },
            { "data": "IsPaid", "class": "status" },

            { data: null, "orderable": false, "width": "14%" }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<a class="paycheck" href="javascript:void(0)"><i class="fa fa-credit-card"></i> Pay Check</a>'
        }],
        order: [[1, "asc"]],
        buttons: [

           'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["ReceiptId"]);
            $(nRow).attr('status', data["Status"]);

            if (data["Status"] == "1") {
                //If paid 4 .. or not approved 1
                $(nRow).find(".paycheck").hide();
            }
        }
    });

};


function PayCheck(idToPay) {

    var isPaid = "true";
    $.ajax({
        url: ROOT + "CheckManagement/PayAPCheck",
        type: "POST",
        data: { 'receiptId': idToPay },
        async: false,
        dataType: "json",
        success: function (data) {
            isPaid = data;
        }
    });
    return isPaid;
}