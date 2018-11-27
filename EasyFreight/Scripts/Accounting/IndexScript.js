$(function () {

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
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

        $(".filter2").each(function () {
            $(this).removeClass("clicked-icon");
            $(this).addClass("default-icon");
        });

        $(".filter1").closest("#AllType").addClass("clicked-icon");
        $(".filter2").closest("#AllType").addClass("clicked-icon");

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
        $("#AllType").addClass("clicked-icon");
        GetTbData();
        ClearForm();
    });


    $('#datatable-default tbody').on('click', '.details', function () {
        var itemId = $(this).closest('tr').attr("id");
        $.ajax({
            url: ROOT + "Operation/export/GetOperationDetails/" + itemId,
            type: "POST",
            data: { 'id': itemId },
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
    });

    $('#datatable-default tbody').on('click', '.opercost', function () {
        var itemId = $(this).closest('tr').attr("id");
        GetOperOrHbCost(itemId, 0);
        $("#modalheader").html("Operation cost analysis");
        $("#printopercost").attr("href", ROOT + "Accounting/PrintOperCostDetails?operationId=" + itemId + "&hbId=0");
    });

    $('#datatable-default tbody').on('click', '.agentnote', function () {
        var operId = $(this).closest('tr').attr("id");
        var noteType = $(this).attr("notetype");

        window.location.href = ROOT + "AgentNote/AddAgentNote?operId=" + operId + "&noteType=" + noteType;
    });

    $('#datatable-default tbody').on('click', '.process', function () {
        var itemId = $(this).closest('tr').attr("id");
        var orderFrom = $(this).closest('tr').attr("orderfrom");
        window.location.href = ROOT + "Accounting/ManageOrder?id=" + itemId + "&orderFrom=" + orderFrom;
    });



    $('#datatable-default tbody').on('click', '.addapinv', function () {
        var operId = $(this).closest('tr').attr("id");
        var invFor = $(this).attr("invfor");
        window.location.href = ROOT + "APInvoice/AddInvoice?hbId=0&operId=" + operId + "&invFor=" + invFor;
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

    $(document).on('click', '.hbcost', function () {
        var hbId = $(this).attr("hbid");
        GetOperOrHbCost(0, hbId);
        $("#modalheader").html("House Bill cost analysis");
        $("#printopercost").attr("href", ROOT + "Accounting/PrintOperCostDetails?operationId=0&hbId=" + hbId);
    });

    $(document).on('click', '.hbdetails', function () {
        var hbId = $(this).attr("hbid");
        GetHBForView(hbId);
        $("#modalheader").html("House Bill Details");
        $("#printopercost").attr("href", ROOT + "Operation/Export/PrintHBDetails?houseBillId=" + hbId);
    });

    $(document).on('click', '.invlist', function () {
        var hbId = $(this).attr("hbid");
        GetHbInvList(hbId);
        $("#modalheader").html("House Bill Invoices");
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

});

var operationOrdersJson;

function GetTbData(searchForm, orderFrom, CarrierType) {
    if (orderFrom == undefined)
        orderFrom = $("#orderFrom").val();

    $.ajax({
        url: ROOT + "Operation/export/GetTableJson",
        type: "POST",
        data: searchForm + "&OrderFrom=" + orderFrom + "&CarrierType=" + CarrierType + "&ScreenName=accoper",
        async: false,
        success: function (data) {
            operationOrdersJson = data;

            if ((jQuery.parseJSON(operationOrdersJson).data) != "")
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
        table.fnAddData(jQuery.parseJSON(operationOrdersJson).data);
        return;
    }
    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(operationOrdersJson).data,
        columns: [
             {
                 "className": 'details-control',
                 "orderable": false,
                 "data": null,
                 "defaultContent": '',
                  "width": "2%"
             },
            { "data": "OrderFromImg", "orderable": false, "width": "2%" },
            { "data": "CarrierTypeImg", "orderable": false, "width": "2%" },
            { "data": "ConsolidationImg", "orderable": false, "width": "2%" },
            { "data": "CreateDate", "sType": "date-uk" },
            { "data": "OperationCode" },
            { "data": "ClientName" },
            { "data": "CarrierName" },
            { "data": "AgentName" },
            { "data": "FromPort" },
            { "data": "ToPort" },
            { "data": "DateOfDeparture", "sType": "date-uk" },
            { "data": "StatusName" },
            { data: null, "orderable": false, "width": "14%" }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<div class="btn-group"> ' +
                '<button type="button" class="mb-xs mt-xs mr-xs btn btn-default dropdown-toggle" data-toggle="dropdown">Actions <span class="caret"></span></button> ' +
                '<ul class="dropdown-menu" role="menu">' +
                '<li><a class="details modal-details" href="javascript:void(0)"><i class="fa fa-info-circle"></i>  Details</a></li> ' 
                + '<li><a class="opercost" href="javascript:void(0)"><i class="fa fa-usd"></i> Operation cost</a></li>'
                + '<li class="divider"></li>'
                + '<li><a class="process" href="javascript:void(0)"><i class="fa fa-cog"></i> Process</a></li>'
                + '<li class="divider"></li>'
                + '<li><a class="agentnote" notetype="1" href="javascript:void(0)"><i class="fa fa-newspaper-o"></i>  Agent Debit Note</a></li>'
                + '<li><a class="agentnote" notetype="2" href="javascript:void(0)"><i class="fa fa-newspaper-o"></i>  Agent Credit Note</a></li>'
                + '<li class="divider"></li>'
                + '<li><a class="addapinv" invfor="1" href="javascript:void(0)"><i class="fa fa-ship"></i> (A/P) Add Carrier Invoice</a></li>'
                + '<li><a class="addapinv" invfor="2" href="javascript:void(0)"><i class="fa fa-truck"></i> (A/P) Add Contractor Invoice</a></li>'
                + '<li><a class="addapinv" invfor="3" href="javascript:void(0)"><i class="fa fa-newspaper-o"></i> (A/P) Add CC Invoice</a></li>'
                + '</ul></div>'
        }],
        order: [[4, "desc"]],
        buttons: [
            'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["OperationId"]);
            $(nRow).attr('statusId', data["StatusId"]);
            $(nRow).attr('orderfrom', data["OrderFrom"]);
            if (data["AgentName"] == "") {
                $(nRow).find(".agentnote").hide();
            }
            //else {
            //    if (data["OrderFrom"] == "1")
            //        $(nRow).find('[notetype="2"]').hide(); //hide credit note for export
            //    else
            //        $(nRow).find('[notetype="1"]').hide(); //hide debit note for import
            //}
        }
    });

};

/* Formatting function for row details - modify as you need */
function format(d) {
    // d is the original data object for the row
    var newRowHtml;

    $.ajax({
        url: ROOT + "Accounting/GetHbList",
        type: "POST",
        data: { 'operationId': d["OperationId"], 'orderFrom': d["OrderFrom"] },
        async: false,
        success: function (data) {
            newRowHtml = data;
        }

    });

    return newRowHtml;
}

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

function GetHbInvList(hbId) {
    $.ajax({
        url: ROOT + "Accounting/GetHbInvList",
        type: "POST",
        data: { 'hbId': hbId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#ModalContent").html(data);
            // $("#tbinvlist").dataTable();
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

function GetHBForView(houseBillId) {
    $.ajax({
        url: ROOT + "Operation/export/GetHBForView",
        type: "POST",
        data: { 'houseBillId': houseBillId },
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
        url: ROOT + "Accounting/AdvSearchOperations",
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