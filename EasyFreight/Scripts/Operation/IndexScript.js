(function ($) {
    /*
Modal Dismiss
*/
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

        $(".filter1").click(function () {
            $(".filter1").each(function () {
                $(this).removeClass("clicked-icon");
                $(this).addClass("default-icon");
            });

            $(this).removeClass("default-icon");
            $(this).addClass("clicked-icon");
            var form = $("#searchform").serialize();
            GetTbData(form, $(this).attr("carriertype"));
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
            $("#AllType").addClass("clicked-icon");
            GetTbData();
            ClearForm();
        });

        $('#datatable-default tbody').on('click', '.edit', function () {
            var itemId = $(this).closest('tr').attr("id");
            var orderFrom = $("#orderFromString").val();
            window.location.href = ROOT + "Operation/" + orderFrom + "/Add/" + itemId;
        });

        $('#datatable-default tbody').on('click', '.process', function () {
            var itemId = $(this).closest('tr').attr("id");
            var orderFrom = $("#orderFromString").val();
            window.location.href = ROOT + "Operation/" + orderFrom + "/HouseBill/" + itemId;
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

        $('#datatable-default tbody').on('click', '.cancel', function () {
            var itemId = $(this).closest('tr').attr("id");
            $("#operIdToClose").val(itemId);
            $.magnificPopup.open({
                items: {
                    src: '#modelClose',
                    type: 'inline'
                },
                modal: true
            });
        });

        $(document).on('click', '#closeConfirm', function () {
            CloseOperation($("#operIdToClose").val());
            $.magnificPopup.close();
        });

        $('#datatable-default tbody').on('click', '.cashdeposit', function () {
            var operId = $(this).closest('tr').attr("id");
            window.location.href = ROOT + "CashManagement/CCCashDeposit?operationId=" + operId;
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


        $('#datatable-default tbody').on('click', '.shippingdeclink', function () {
            var orderFrom = $("#orderFrom").val();
            var itemId = $(this).closest('tr').attr("id");
            $("#currentOpID").val(itemId);
            $.ajax({
                url: ROOT + "Operation/" + orderFrom + "/GetShippingDeclaration",
                type: "POST",
                data: { 'id': itemId },
                async: false,
                dataType: "html",
                success: function (data) {
                    $("#shippingdeclcontent").html(data);

                    $.magnificPopup.open({
                        items: {
                            src: '#shippingdeclmodal',
                            type: 'inline'
                        },
                        modal: true
                    });
                }
            });
        });

        $('#shippingdeclmodal').on('click', '.btn-info', function () {
            //var itemId = $("#currentOpID").val();//$(this).closest('tr').attr("id");
            // GetOperationContainers(itemId);
            var orderFrom = $("#orderFromString").val();
            window.location.href = ROOT + "Operation/" + orderFrom + "/PrintShippingDecl";
         });

        $('#datatable-default tbody').on('click', '.opercost', function () {
            var itemId = $(this).closest('tr').attr("id");
            GetOperOrHbCost(itemId, 0);
            $("#modalheader").html("Operation cost analysis");
            $("#printopercost").attr("href", ROOT + "Accounting/PrintOperCostDetails?operationId=" + itemId + "&hbId=0");
        });

        // delete section
        $('#datatable-default tbody').on('click', '.deletethis', function () {
            var itemId = $(this).closest('tr').attr("id");
            var isSuper = CheckIfSuperUser();
            var statusId = $(this).attr("statusid");
            if (statusId > 3) {
                new PNotify({
                    title: 'Sorry!',
                    text: "This Order cannot be deleted may be closed or Invoice Issued",
                    type: 'error'
                });
                return true;
            }
            else {
                if (!isSuper) {
                    new PNotify({
                        title: 'Sorry!',
                        text: "You don't have access to delete",
                        type: 'error'
                    });
                    return true;
                }
                else {
                    $("#operIdToDelete").val(itemId);
                    $.magnificPopup.open({
                        items: {
                            src: '#modelDelete',
                            type: 'inline'
                        },
                        modal: true
                    });
                }
            }
        });

        $(document).on('click', '#deleteConfirm', function () {
            DeleteOperation($("#operIdToDelete").val());
            $.magnificPopup.close();
        });




    });



}).apply(this, [jQuery]);

var operationOrdersJson;

function GetTbData(searchForm, CarrierType) {
    var orderFrom = $("#orderFrom").val();
    $.ajax({
        url: ROOT + "Operation/export/GetTableJson",
        type: "POST",
        data: searchForm + "&OrderFrom=" + orderFrom + "&CarrierType=" + CarrierType,
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
        "columns": [
             { "data": "CarrierTypeImg", "orderable": false, "width": "1%" },
             { "data": "ConsolidationImg", "orderable": false, "width": "1px" },
            { "data": "CreateDate", "sType": "date-uk"  },
            { "data": "OperationCode"  },
            { "data": "ShipperName"  },
            { "data": "ConsigneeName"  },
            { "data": "CarrierName" },
            { "data": "FromPort"  },
            { "data": "ToPort"},
            { "data": "DateOfDeparture", "sType": "date-uk" },
            { "data": "StatusName" },
            { "data": null, "orderable": false, "width": "14%" }
        ],
        "columnDefs": [{
            "targets": -1,
            "data": null,
            "defaultContent": '<div class="btn-group"> ' +
                '<button type="button" class="mb-xs mt-xs mr-xs btn btn-default dropdown-toggle" data-toggle="dropdown">Actions <span class="caret"></span></button> ' +
                '<ul class="dropdown-menu" role="menu">' +
                '<li><a class="details modal-details" href="javascript:void(0)"><i class="fa fa-info-circle"></i>  Details</a></li> ' +
                '<li><a class="shippingdeclink" href="javascript:void(0)" title="Shipping Declaration"><i class="fa fa-info-circle"></i>  Shipping Decl.</a></li> '
                + '<li><a class="opercost" href="javascript:void(0)"><i class="fa fa-usd"></i> Operation cost</a></li>'
                + '<li><a class="process" href="javascript:void(0)"><i class="fa fa-cog"></i> Process</a></li>'
                + '<li class="divider"></li>'
                + '<li><a class="cashdeposit" href="javascript:void(0)"><i class="fa fa-usd"></i> Add CC Cash Deposit</a></li>'
                + '<li class="divider"></li><li><a class="edit" href="javascript:void(0)"><i class="fa fa-pencil"></i>  Edit</a></li>'
                + '<li ></li><li><a class="deletethis" href="javascript:void(0)"><i class="fa fa-trash"></i>  Delete</a></li>'
                + '<li><a class="cancel" href="javascript:void(0)"><i class="fa fa-lock"></i> Close</a></li></ul></div>'
        }],
        "order": [[2, "desc"]],
        //buttons: [

        //  'copy', 'excel', 'pdf'
        //],
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
                        return ($("#orderFrom").val()=="1"?"Export ":"Import ") + "MBL ";
                    }
                },
                 'colvis'

        ],
        dom: 'Bfrtip',
        "fnCreatedRow": function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["OperationId"]);
            $(nRow).attr('statusid', data["StatusId"]);
            if (data["StatusName"] == "New") {
                $('td', nRow).eq(10).css('color', 'Green')//addClass('danger');
            }
            if (data["StatusName"] == "Opened") {
                $('td', nRow).eq(10).css('color', 'Red')//addClass('danger');
            }
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
        url: ROOT + "Operation/" + orderFrom + "/AdvSearch",
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

function CloseOperation(operationId) {
    var orderFrom = $("#orderFrom").val();
    $.ajax({
        url: ROOT + "Operation/export/CloseOperation",
        type: "POST",
        data: { "id": operationId, 'orderFrom': orderFrom },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Operation Is Closed',
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

function DeleteOperation(operationId) {
    var orderFrom = $("#orderFrom").val();
    $.ajax({
        url: ROOT + "Operation/export/DeleteOperation",
        type: "POST",
        data: { "id": operationId, 'orderFrom': orderFrom },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Operation Is Deleted',
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

}

 