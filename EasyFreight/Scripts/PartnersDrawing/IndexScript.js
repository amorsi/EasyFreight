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
        var isOut = false;
        if ($(this).closest('tr').attr("rectype") == 'out') {
            isOut = true;
        }
        ViewReceipt(recId, isOut);
    });

    $('#datatable-default tbody').on('click', '.print', function () {
        var recId = $(this).closest('tr').attr("id");
        var isOut = false;
        if ($(this).closest('tr').attr("rectype") == 'out') {
            isOut = true;
        }
        window.open(
          ROOT + "CashManagement/PrintCashIn?id=" + recId + '&isOut=' + isOut,
          '_blank'
        );

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

var cashRecPartnersJson;

function GetTbData(searchForm) {

    $.ajax({
        url: ROOT + "PartnersDrawing/GetTableJson",
        type: "POST",
        data: searchForm,
        async: false,
        success: function (data) {
            cashRecPartnersJson = data;
            if ((jQuery.parseJSON(cashRecPartnersJson).data) != "")
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
        table.fnAddData(jQuery.parseJSON(cashRecPartnersJson).data);
        return;
    }
    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(cashRecPartnersJson).data,
        columns: [
            { "data": "ReceiptCode" },
            { "data": "ReceiptDate", "sType": "date-uk" },
            { "data": "ReceiptType" },
            { "data": "AccountNameEn" },
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
        order: [[1, "desc"]],
        buttons: [

           'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["ReceiptId"]);
            $(nRow).attr('rectype', data["ReceiptTypeShort"]);
        }
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