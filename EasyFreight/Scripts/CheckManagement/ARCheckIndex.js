$(function () {

    GetTbData();

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    $('#datatable-default tbody').on('click', '.collectcheck', function () {
        var receiptId = $(this).closest('tr').attr("id");

        $("#receiptIdToCollect").val(receiptId);

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
        var idToCollect = $("#receiptIdToCollect").val()
        var isCollected = CollectCheck(idToCollect);

        if (isCollected == "true") {
            new PNotify({
                title: 'Success!',
                text: 'Check Collected.',
                type: 'success'
            });
            //Delete row from table

            var row = $('#datatable-default').find('#' + idToCollect);
            $(row).find(".collectcheck").hide();
            $(row).find(".status").html('Collected');
        }
        else {
            new PNotify({
                title: 'Sorry!',
                text: isCollected,
                type: 'error'
            });
        }
    });

});

var aRChecksJson;

function GetTbData(searchForm) {

    $.ajax({
        url: ROOT + "CheckManagement/GetARChecksJson",
        type: "POST",
        data: searchForm,
        async: false,
        success: function (data) {
            aRChecksJson = data;
            if ((jQuery.parseJSON(aRChecksJson).data) != "")
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
        table.fnAddData(jQuery.parseJSON(aRChecksJson).data);
        return;
    }
    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(aRChecksJson).data,
        columns: [
            { "data": "CheckNumber" },
            { "data": "CheckDueDate", "sType": "date-uk" },
            { "data": "CustomerName" },
            { "data": "ReceiptCode" },
            { "data": "ReceiptDate", "sType": "date-uk" },
            { "data": "ReceiptAmount" },
            { "data": "IsCollected", "class":"status" },

            { data: null, "orderable": false, "width": "14%", "class": "actions" }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<a class="collectcheck" href="javascript:void(0)"><i class="fa fa-credit-card"></i> Collect Check</a>'
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
                $(nRow).find(".collectcheck").hide();
            }
        }
    });

};


function CollectCheck(idToCollect) {

    var isCollected = "true";
    $.ajax({
        url: ROOT + "CheckManagement/CollectARCheck",
        type: "POST",
        data: { 'receiptId': idToCollect },
        async: false,
        dataType: "json",
        success: function (data) {
            isCollected = data;
        }
    });
    return isCollected;
}