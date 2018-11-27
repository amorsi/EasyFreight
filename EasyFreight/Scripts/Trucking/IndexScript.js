(function ($) {

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
    });


    $(function () {

        GetTbData();

        $('#datatable-default tbody').on('click', '.details', function () {
            var itemId = $(this).closest('tr').attr("id");
            GetTruckingDetails(itemId);
        });

        $('#datatable-default tbody').on('click', '.process', function () {
            var statusId = $(this).closest('tr').attr("statusId");

            if (statusId != 1 && statusId != 2) {
                //Check If super user .. allow edit
                var isSuper = CheckIfSuperUser();
                if (!isSuper) {
                new PNotify({
                    title: 'Sorry!',
                    text: "Trucking Order is closed",
                    type: 'error'
                });

                return true;
            }
            }
            var itemId = $(this).closest('tr').attr("id");
            window.location.href = ROOT + "Trucking/ManageOrder/" + itemId;
        });

        $('#datatable-default tbody').on('click', '.closeorder', function () {
            var itemId = $(this).closest('tr').attr("id");
            var statusId = $(this).closest('tr').attr("statusId");

            if (statusId != 1 && statusId != 2) {
                new PNotify({
                    title: 'Sorry!',
                    text: "Trucking Order is closed",
                    type: 'error'
                });

                return true;
            }

            $("#deletedId").val(itemId);
            $.magnificPopup.open({
                items: {
                    src: statusId > 2 ? '#modelUnavailable' : '#modelClose',
                    type: 'inline'
                },
                modal: true
            });
        });

        $('#datatable-default tbody').on('click', '.modal-roll', function () {
            var itemId = $(this).closest('tr').attr("id");
            var statusId = $(this).closest('tr').attr("statusId");

            if (statusId != 1 && statusId != 2) {
                //Check If super user .. allow edit
                var isSuper = CheckIfSuperUser();
                if (!isSuper) {
                new PNotify({
                    title: 'Sorry!',
                    text: "Trucking Order is closed",
                    type: 'error'
                });

                return true;
            }
            }

            $("#deletedId").val(itemId);
            $.magnificPopup.open({
                items: {
                    //if want to stop cancel after close
                    //src: statusId > 2 ? '#modelUnavailable' : '#modelRoll',
                    src:  '#modelRoll',
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
            var form = $("#searchform").serialize();
            GetTbData(form, $(this).attr("orderfrom"));
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

    });

    //Close order Modal Confirm
    $(document).on('click', '#btnCloseOrder', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var idToCancel = $("#deletedId").val()
        CloseOrder(idToCancel, 3); //in TruckingCommon.js
        $("#" + idToCancel + "").attr("statusId", "3")
    });

    $(document).on('click', '.modal-confirmRoll', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var idToRoll = $("#deletedId").val()
        var isrolled = RollBack(idToRoll);
        if (isrolled == "true") {
            new PNotify({
                title: 'Success!',
                text: 'Order has been Rolled.',
                type: 'success'
            });
            //Delete row from table
            //var table = $('#datatable-default').DataTable();
            // table.row('#' + idToDelete).remove().draw(false);

            //$('#' + portId).remove();
        }
        else {
            new PNotify({
                title: 'Sorry!',
                text: 'Can not Roll this order.',
                type: 'error'
            });
        }
    });

    $(document).on('click', '.printorder', function () {
        var itemId = $(this).closest('tr').attr("id");
        window.location.href = ROOT + "Trucking/PrintDetails?truckingOrderId=" + itemId;
    });

    

}).apply(this, [jQuery]);

var truckingOrdersJson;

function GetTbData(searchForm, orderFrom) {
    $.ajax({
        url: ROOT + "Trucking/GetTableJson",
        type: "POST",
        data: searchForm + "&OrderFrom=" + orderFrom,
        async: false,
        success: function (data) {
            truckingOrdersJson = data;
            datatableInit();
        }

    });
}


function datatableInit() {

    if ($.fn.dataTable.isDataTable('#datatable-default')) {
        table = $('#datatable-default').dataTable();
        table.fnClearTable();
        if(truckingOrdersJson.length>18)
        table.fnAddData(jQuery.parseJSON(truckingOrdersJson).data);
        return;
    }

    $('#datatable-default').dataTable({
        //destroy: true, 
        processing: true,
        data: jQuery.parseJSON(truckingOrdersJson).data,
        columns: [
            { "data": "OrderFromImg", "orderable": false, "width": "1%" },
            { data: "NeedArrive" },
            { data: "ArriveTime" },
            { data: "CreateDate", "sType": "date-uk" },
            { data: "OperationCode" },
            { data: "Client" },
            { data: "BookingNo" },
            { data: "ContractorName" },
            { data: "StatusName" },
            { data: null, "orderable": false, "width": "14%" }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<div class="btn-group"> ' +
                '<button type="button" class="mb-xs mt-xs mr-xs btn btn-default dropdown-toggle" data-toggle="dropdown">Actions <span class="caret"></span></button> ' +
                '<ul class="dropdown-menu" role="menu">' +
                '<li><a class="details modal-details" href="javascript:void(0)"><i class="fa fa-info-circle"></i>  Details</a></li> ' +
                '<li><a class="process" href="javascript:void(0)"><i class="fa fa-cog"></i> Process</a></li>' +
                '<li class="divider"></li><li><a class="roll modal-roll" href="javascript:void(0)"><i class="fa fa-trash"></i> Delete</a></li>' + 
                '<li class="success"><a class="closeorder" href="javascript:void(0)"><i class="fa fa-lock"></i> Close</a></li>'+
                '<li class="success"><a class="printorder" href="javascript:void(0)"><i class="fa fa-print"></i> Print</a></li></ul></div>'
        }],
        order: [[1, "desc"]],
        //buttons: ['copy', 'excel', 'pdf'],
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
                     exportOptions: {
                         columns: [0, ':visible'],
                     }
                     ,
                     title: function () { 
                         return "Trucking Orders";
                     }
        },
                  'colvis'

    ] ,


        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["TruckingOrderId"]);
            $(nRow).attr('statusId', data["StatusId"]);

            if (data["StatusId"] == "1") {
                $('td', nRow).eq(8).css('color', 'Green')//addClass('danger');
            }
            if (data["StatusId"] == "2") {
                $('td', nRow).eq(8).css('color', 'Red')//addClass('danger');
            }
        }
    });

};


//Delete order
function RollBack(orderId) {
    var isRolled = "true";
    $.ajax({
        url: ROOT + "Trucking/Roll",
        type: "GET",
        data: { 'truckingOrderId': orderId },
        async: false,
        dataType: "json",
        success: function (data) {
            isRolled = data;
        }
    });
    return isRolled;
}

function ClearForm() {
    $("#searchform input:text").each(function () {
        $(this).val("");
    });

    $("#searchform select").val("").trigger("change");

}

function GetSearchForm() {
    $.ajax({
        url: ROOT + "Trucking/AdvSearch",
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



