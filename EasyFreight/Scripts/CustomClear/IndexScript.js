$(function () {

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    GetTbData();

    $(".filter1").click(function () {
        $(".filter1").each(function () {
            $(this).removeClass("clicked-icon");
            $(this).addClass("default-icon");
        });

        $(this).removeClass("default-icon");
        $(this).addClass("clicked-icon");
        var form = $("#searchform").serialize();
        var carrType = $(".filter2.clicked-icon").attr("carriertype");
        GetTbData(form, $(this).attr("orderfrom"), carrType);
    });

    $(".filter2").click(function () {
        $(".filter2").each(function () {
            $(this).removeClass("clicked-icon");
            $(this).addClass("default-icon");
        });

        $(this).removeClass("default-icon");
        $(this).addClass("clicked-icon");
        var form = $("#searchform").serialize();
        var orderFrom = $(".filter1.clicked-icon").attr("orderfrom");
        GetTbData(form, orderFrom, $(this).attr("carriertype"));
    });

    //clear filter
    $(".filter0").click(function () {
        $(".filter1").each(function () {
            $(this).removeClass("clicked-icon");
            $(this).addClass("default-icon");
        });

        $(".filter2").each(function () {
            $(this).removeClass("clicked-icon");
            $(this).addClass("default-icon");
        });

        $("#AllType.filter1").addClass("clicked-icon");
        $("#AllType.filter2").addClass("clicked-icon");
        
        GetTbData();
        ClearForm();
    });

    $('#datatable-default tbody').on('click', '.process', function () {
        var itemId = $(this).closest('tr').attr("id");
        window.location.href = ROOT + "CustomClearance/ManageOrder/" + itemId;
    });

    $('#datatable-default tbody').on('click', '.details', function () {
        var itemId = $(this).closest('tr').attr("id");
        var operationId = $(this).closest('tr').attr("operationid");
        
        GetCCDetails(itemId,operationId);
    });

    $('#datatable-default tbody').on('click', '.closeorder', function () {
        var itemId = $(this).closest('tr').attr("id");
        var ststusID = $(this).closest('tr').attr("statusId");
        $("#deletedId").val(itemId);
        $.magnificPopup.open({
            items: {
                src: ststusID > 2 ? '#modelUnavailable' : '#modelClose',
                type: 'inline'
            },
            modal: true
        });
    });

    //Close order Modal Confirm
    $(document).on('click', '#btnCloseOrder', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var idToCancel = $("#deletedId").val()
        CloseCCOrder(idToCancel, 3); //in TruckingCommon.js

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

    });

});

var ccOrders;

function GetTbData(searchForm, orderFrom, carrierType) {
    $.ajax({
        url: ROOT + "CustomClearance/GetTableJson",
        type: "POST",
        data: searchForm + "&OrderFrom=" + orderFrom + "&CarrierType=" + carrierType,
        async: false,
        success: function (data) {
            ccOrders = data;
            if ((jQuery.parseJSON(ccOrders).data) != "")
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
        table.fnAddData(jQuery.parseJSON(ccOrders).data);
        return;
    }
    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(ccOrders).data,
        columns: [
            { "data": "OrderFromImg", "orderable": false, "width": "2%" },
            { "data": "CarrierTypeImg", "orderable": false, "width": "2%" },
            { data: "NeedArrive" },
            { data: "OperationCode" },
            { data: "BookingNumber" },
            { data: "MBL" },
            { data: "Client" },
            { data: "Port" },
            { data: "StatusName" },
            { data: null, "orderable": false,"width":"14%" }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<div class="btn-group"> ' +
                '<button type="button" class="mb-xs mt-xs mr-xs btn btn-default dropdown-toggle" data-toggle="dropdown">Actions <span class="caret"></span></button> ' +
                '<ul class="dropdown-menu" role="menu">' +
                '<li><a class="details modal-details" href="javascript:void(0)"><i class="fa fa-info-circle"></i>  Details</a></li> ' +
                '<li><a class="process" href="javascript:void(0)"><i class="fa fa-cog"></i> Process</a></li>'
                +'<li class="divider"></li>'
                +'<li><a class="roll modal-roll" href="javascript:void(0)"><i class="fa fa-undo"></i>  Roll</a></li>'
                + '<li><a class="closeorder" href="javascript:void(0)"><i class="fa fa-lock"></i>  Close</a></li></ul></div>'
        }],
        order: [[2, "desc"]],
        buttons: [

          'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["CCId"]);
            $(nRow).attr('statusId', data["StatusId"]);
            $(nRow).attr('operationid', data["OperationId"]);
            
        }
    });

};

function ClearForm() {
    $("#searchform input:text").each(function () {
        $(this).val("");
    });

    $("#searchform select").val("").trigger("change");

}

function GetSearchForm() {
    $.ajax({
        url: ROOT + "CustomClearance/AdvSearch",
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