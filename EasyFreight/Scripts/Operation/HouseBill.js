$(function () {

    /*
Modal Dismiss
*/
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });


    $("#hblink").on('click', function () {
        var hbCount = $("#hbcount").val();

        if (!$('#Form1').length && !$('.hblist').length) {
            if (hbCount == "0")
                GetHbContent();
            else
                GetHbList();
        }
    });

    $("#trucklink").on('click', function () {
        if (!$('#TruckingForm').length && !$('.trucklist').length) {
            GetTruckingList();
        }
    });

    $("#cclink").on('click', function () {
        if (!$('#ccsection').length && !$('.cclist').length) {
            GetCCList();
        }
    });


    $(document).on('click', '#SaveHouseForm', function () {
         
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            SaveHouseBillForm();
            GetHbList();
        } else {
            new PNotify({
                title: 'Sorry!',
                text: 'Please check invalid fields',
                type: 'error'
            });
        }
    });

    //Edit trucking order
    $(document).on('click', '.edittruck', function () {
        var hbId = $(this).attr("hbid");

        var statusId = $(this).attr("statusid");
        if (statusId == 2) {
            new PNotify({
                title: 'Sorry!',
                text: "Trucking Order is opened and modified by trucking department",
                type: 'error'
            });
            return true;
        }
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
        GetTruckContent(hbId);

    });

    //view truck
    $(document).on('click', '.viewtruck', function () {
        var truckId = $(this).attr("truckid");
        GetTruckingDetails(truckId);
        $("#modalheader").text("Trucking Order Information");
        $("#printtrucking").attr("href", ROOT + "Trucking/PrintDetails?truckingOrderId=" + truckId);
    });

    //Delete trucking order
    $(document).on('click', '.canceltruck', function () {
        var truckId = $(this).attr("truckid");
        //if opened .. modified by trucking can't cancel it
        $("#IdToClose").val(truckId);
        $("#ModelType").val('Trucking');
        $("#p1").html("Are you sure that you want to cancel this Trucking Order?");
        $.magnificPopup.open({
            items: {
                src: '#modelClose',
                type: 'inline'
            },
            modal: true
        });
    });

    //Delete Custom Clearance order
    $(document).on('click', '.cancelcc', function () {
        var ccId = $(this).attr("ccid");
        //if opened .. modified by trucking can't cancel it
        $("#IdToClose").val(ccId);
        $("#ModelType").val('CC');
        $("#p1").html("Are you sure that you want to cancel this Custom Clearance Order?");
        $.magnificPopup.open({
            items: {
                src: '#modelClose',
                type: 'inline'
            },
            modal: true
        });
    });



    //Reset Truck Form
    $(document).on('click', '#ResetTruckForm', function () {
        GetTruckingList();
    });


    //Add trucking order
    $(document).on('click', '.addtro', function () {
        var hbId = $(this).attr("hbid");

        var statusId = $(this).attr("statusid");
        if (statusId != 1 && statusId != 2) {
            //Check If super user .. allow edit
            var isSuper = CheckIfSuperUser();
            if (!isSuper) {
                new PNotify({
                    title: 'Sorry!',
                    text: "House Bill is closed",
                    type: 'error'
                });
                return true;
            }
        }

        GetTruckContent(hbId);
        $('.nav-tabs a[href="#truck"]').tab('show')
    });
    //Add Custom Clearance order
    $(document).on('click', '.addccorder', function () {
        var hbId = $(this).attr("hbid");
        var statusId = $(this).attr("statusid");
        if (statusId != 1 && statusId != 2) {
            //Check If super user .. allow edit
            var isSuper = CheckIfSuperUser();
            if (!isSuper) {
                new PNotify({
                    title: 'Sorry!',
                    text: "House Bill is closed",
                    type: 'error'
                });
                return true;
            }
        }
        GetCCContent(hbId);
        $('.nav-tabs a[href="#cs"]').tab('show')
    });
    //view house bill
    $(document).on('click', '.viewhb', function () {
        var hbId = $(this).attr("hbid");
        GetHBForView(hbId);
        $("#modalheader").text("House Bill Information");
        $("#printtrucking").attr("href", ROOT + "Operation/Export/PrintHBDetails?houseBillId=" + hbId);
    });

    //edit house bill
    $(document).on('click', '.edithb', function () {
        var hbId = $(this).attr("hbid");
        var statusId = $(this).attr("statusid");
        if (statusId != 1 && statusId != 2) {
            //Check If super user .. allow edit
            var isSuper = CheckIfSuperUser();
            if (!isSuper) {
                new PNotify({
                    title: 'Sorry!',
                    text: "House Bill is closed",
                    type: 'error'
                });
                return true;
            }
        }
        GetHbContent(hbId);

    });

    $(document).on('click', '.closehb', function () {
        var hbId = $(this).attr("hbid");
        $("#IdToClose").val(hbId);
        $("#ModelType").val('HB');
        $("#p1").html("Are you sure that you want to close this House Bill?");
        $("#p2").html("You will not be able to edit the HB Info or its cost details");
        $.magnificPopup.open({
            items: {
                src: '#modelClose',
                type: 'inline'
            },
            modal: true
        });
    });

    $(document).on('click', '#closeHbConfirm', function () {
        var idToClose = $("#IdToClose").val();
        var oprOrderFrom = $("#oprOrderFrom").val();

        if ($("#ModelType").val() == 'HB') {
            CloseHb(idToClose);
        }
        else if ($("#ModelType").val() == 'Trucking') {
            isClosed = CloseOrder(idToClose, 4, oprOrderFrom); //in TruckingCommon.js
            if (isClosed == 'true')
                GetTruckingList();
        }
        else if ($("#ModelType").val() == 'CC') {
            isClosed = CloseCCOrder(idToClose, 4, oprOrderFrom); //in CCCommon.js
            if (isClosed == 'true')
                GetCCList();
        }

        $.magnificPopup.close();
    });

    $(document).on('click', '.opercost', function () {
        var hbId = $(this).attr("hbid");

        var statusId = $(this).attr("statusid");
        if (statusId != 1 && statusId != 2) {
            //Check If super user .. allow edit
            var isSuper = CheckIfSuperUser();
            if (!isSuper) {
                new PNotify({
                    title: 'Sorry!',
                    text: "House Bill is closed",
                    type: 'error'
                });
                return true;
            }
        }

        GetHBCost(hbId);
        $("#modalheader").text("Opreation Cost");
        $("#printtrucking").attr("href", ROOT + "Operation/export/PrintDetails?HBId=" + hbId);
    });

    $(document).on('click', '#SaveHBCostForm', function () {
        var isValid = CheckFormIsValid('opercostform');
        if (isValid) {
            SaveHouseBillCost();
            GetHbList();
        } 
    });

    $(document).on('click', '#printhbCost', function () {
        var hbId = $(this).attr("hbid");
        window.location.href = ROOT + "Operation/export/PrintHbCost/" + hbId;
    });

    //Add house bill
    $(document).on('click', '.addhb', function () {
        GetHbContent();
    });
    //reset house bill form 
    $(document).on('click', '#ResetHbForm', function () {
        GetHbList();
    });

    //save custom clearance
    $(document).on('click', '#SaveCcForm', function () {
        var isValid = CheckFormIsValid('ccform');
        if (isValid) {
            SaveCustomClearForm();
            GetCCList();
        }
    });
    //Edit custom clearance order
    $(document).on('click', '.editcc', function () {
        var hbId = $(this).attr("hbid");
        GetCCContent(hbId);

    });
    //View custom clearance order
    $(document).on('click', '.viewcc', function () {
        var ccId = $(this).attr("ccid");
        var operId = $(this).attr("operid");
        GetCCDetails(ccId, operId);

    });

    //reset custom clearance form
    $(document).on('click', '#ResetCCForm', function () {
        GetCCList();
    });


    $(document).on('click', '#original', function () { $("#DeliveryType").val("1"); });
    $(document).on('click', '#express', function () { $("#DeliveryType").val("2"); });

    $(document).on('click', '.shippingdeclink', function () {
        var orderFrom = $("#orderFrom").val();
        if (orderFrom.length < 1)
        {
            orderFrom = $(this).attr("optype");
        }
        var itemId = $(this).attr("id");
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

    $(document).on('click', '.oper_cost', function () {
        var itemId = $(this).attr("id");
        GetOperOrHbCost(itemId, 0);
        $("#modalheader").html("Operation cost analysis");
        $("#printtrucking").attr("href", ROOT + "Accounting/PrintOperCostDetails?operationId=" + itemId + "&hbId=0");
    });

    $(document).on('click', '.cancelOp', function () {
        var itemId = $(this).attr("id");
        $("#operIdToClose").val(itemId);
        $.magnificPopup.open({
            items: {
                src: '#modelCloseOp',
                type: 'inline'
            },
            modal: true
        });
    });
    $(document).on('click', '#closeConfirmOP', function () {
        CloseOperation($("#operIdToClose").val());
        $.magnificPopup.close();
    });


    $('#shippingdeclmodal').on('click', '.btn-info', function () {
        //var itemId = $("#currentOpID").val();//$(this).closest('tr').attr("id");
        // GetOperationContainers(itemId);
        var orderFrom = $("#orderFromString").val();
        window.location.href = ROOT + "Operation/" + orderFrom + "/PrintShippingDecl";
    });
});

function GetHbContent(houseBillId) {
    var operationId = $("#oprId").val();
    var oprOrderFrom = $("#oprOrderFrom").val();
    $.ajax({
        url: ROOT + "Operation/export/GetHbContent",
        type: "POST",
        data: { 'operationId': operationId, 'oprOrderFrom': oprOrderFrom, 'hbId': houseBillId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#hb").html(data);
            $.getScript(ROOT + "Scripts/Operation/OperationCommon.js");
            $("select").select2();
            $(".date").datepicker()
        }
    });
}

function GetHbList() {
    var operationId = $("#oprId").val();
    var oprOrderFrom = $("#oprOrderFrom").val();
    $.ajax({
        url: ROOT + "Operation/export/GetHbList",
        type: "POST",
        data: { 'operationId': operationId, 'oprOrderFrom': oprOrderFrom },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#hb").html(data);

        }
    });
}

function GetTruckContent(houseBillId) {
    var operationId = $("#oprId").val();
    var oprOrderFrom = $("#oprOrderFrom").val();
    $.ajax({
        url: ROOT + "Trucking/AddTruckingOrder",
        type: "POST",
        data: { 'operationId': operationId, 'orderFrom': oprOrderFrom, 'houseBillId': houseBillId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#truck").html(data);
            $.getScript(ROOT + "Scripts/Trucking/CreateOrder.js");
            $("select").select2();
            $(".date").datepicker();
        }
    });
}

function GetTruckingList() {
    var operationId = $("#oprId").val();
    var oprOrderFrom = $("#oprOrderFrom").val();
    $.ajax({
        url: ROOT + "Trucking/GetTruckingOrderList",
        type: "POST",
        data: { 'operationId': operationId, 'orderFrom': oprOrderFrom },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#truck").html(data);
            $.getScript(ROOT + "Scripts/Trucking/TruckingCommon.js");

        }
    });
}

function GetCCList() {
    var operationId = $("#oprId").val();
    var oprOrderFrom = $("#oprOrderFrom").val();
    $.ajax({
        url: ROOT + "CustomClearance/GetCCOrderList",
        type: "POST",
        data: { 'operationId': operationId, 'orderFrom': oprOrderFrom },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#cs").html(data);
            $.getScript(ROOT + "Scripts/CustomClear/CCCommon.js");

        }
    });
}

function GetCCContent(houseBillId) {
    var operationId = $("#oprId").val();
    var oprOrderFrom = $("#oprOrderFrom").val();
    $.ajax({
        url: ROOT + "CustomClearance/CustomClearanceOrder",
        type: "POST",
        data: { 'id': operationId, 'houseBillId': houseBillId, 'orderFrom': oprOrderFrom },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#cs").html(data);
            //$.getScript(ROOT + "Scripts/Trucking/CreateOrder.js");
            $("select").select2();
            $(".date").datepicker();
            $(".time24").timepicker({
                minuteStep: 5,
                showSeconds: false,
                showMeridian: false,
                defaultTime: '08:00'
            });

        }
    });
}

function SaveHouseBillForm() {

    $.ajax({
        url: ROOT + "Operation/export/AddEditHouseBill",
        type: "POST",
        data: $("#Form1").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data.indexOf("true")>-1) {
                new PNotify({
                    title: 'Success!',
                    text: 'Row Saved.',
                    type: 'success'
                });

                var hbCount = $("#hbcount").val();
                var hbNewCount = parseInt(hbCount) + 1;
                $("#hbcount").val(hbNewCount);

                var hbId = data.replace('true_', '');
                var operationId = $("#oprId").val();

                if ($("select#OperConId").val() != null) {
                    $.post(ROOT + 'Operation/export/AddHbContainer', $.param({ data: $("select#OperConId").val(), hbID: hbId, operationId: operationId }, true), function (data) { });
                }
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

function SaveCustomClearForm() {

    $.ajax({
        url: ROOT + "CustomClearance/AddEditCustClear",
        type: "POST",
        data: $("#ccform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Row Saved.',
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

function GetHBCost(houseBillId) {
    var operationId = $("#oprId").val();
    $.ajax({
        url: ROOT + "Operation/export/GetHBCost",
        type: "POST",
        data: { 'houseBillId': houseBillId, 'operationId': operationId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#hb").html(data);
            $.getScript(ROOT + "Scripts/MasterData/ContactPerson.js");
            $.getScript(ROOT + "Scripts/AddCostCommon.js");
            $("select:not([name*=-1])").select2();

        }
    });
}

function SaveHouseBillCost() {
    $.ajax({
        url: ROOT + "Operation/export/AddEditHbCost",
        type: "POST",
        data: $("#opercostform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Row Saved.',
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

function CloseHb(hbId) {
    $.ajax({
        url: ROOT + "Operation/export/CloseHB",
        type: "POST",
        data: { "hbId": hbId },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'House Bill Is Closed',
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

$('#bol').on('click', '.editop', function () {
     var url = window.location.href;
     window.location.href = url.replace("HouseBill", "Add");
});
 

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

function CloseOperation(operationId) {
    var orderFrom = $("#oprOrderFrom").val();
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




