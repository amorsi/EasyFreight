$(function () {

    $("#AgentId").select2({
        allowClear: true
    });

    //In case of edit mode load NotifierList
    if ($("#ConsigneeId").val() != "" && $("#NotifierAsConsignee").val() == "False") {
        var consigneeId = $("#ConsigneeId").val();
        GetNotifierList(consigneeId);
    }

    if ($("#NotifierAsConsignee").val() == "True") {
        $("#NotifierId").attr('disabled', 'disabled');
        $("#NotifierId").select2();
    }

    // Vessel Edit
    if ($("#CarrierId").val() != "" && $("#CarrierId").val() != undefined) {
        var carrierId = $("#CarrierId").val();
        GetVesselList(0);
    }

    if ($("#carriertype").val() == "2") //air edit mode
    {
       // $("#flightdiv").toggle();
        $("#FlightTitle").text("Flight Number")
        $("#vesseldiv").toggle();
        $("#nwlbl").text("CH.W");
    }
    if ($("#carriertype").val() == "1") //Sea edit mode
    {
        $("#FlightTitle").text("Voyage Number")
    }
    if ($("#PickupNeeded").val() == "True")
    {
        $("#truckcost").toggle();
    }

    if ($("#CustomClearanceNeeded").val() == "True") {
        $("#cccost").toggle();
    }


    $("#ConsigneeId").change(function () {
       // var ConsigneeId = $(this).val();
      //  GetNotifierList(ConsigneeId);

    });

    $("#CarrierId").change(function () {
        var CarrierId = $(this).val();
        var length = $('#VesselId > option').length;
        if (length <= 1) {
            GetVesselList(0);
        }
    });

    $("#carriertype").change(function () {
        var carrType = $(this).val();
       // $("#flightdiv").toggle();
        
        $("#vesseldiv").toggle();
        if (carrType == "1") //sea
        {
            $("#nwlbl").text("N.W"); $("#FlightTitle").text("Voyage Number")
        }
        else {
            $("#nwlbl").text("CH.W"); $("#FlightTitle").text("Flight Number")
        }
    });


    $("#notifierCheck").change(function () {
        if ($(this).is(':checked')) {
            $("#NotifierAsConsignee").val("true");
            $("#NotifierId").val("");
            $("#NotifierId").attr('disabled', 'disabled');
            $("#NotifierId").select2();

        }
        else {
            $("#NotifierAsConsignee").val("false");
            $('#NotifierId').attr("disabled", false);
            $("#NotifierId").select2();
        }
    });
    $("#HazardousCheck").change(function () {
        if ($(this).is(':checked')) {
            $("#IsHazardous").val("true");
        }
        else {
            $("#IsHazardous").val("false");
        }
    });


    $("#PickupNeededCheck").change(function () {

        $("#truckcost").toggle();

        if ($(this).is(':checked')) {
            $("#PickupNeeded").val("true");
        }
        else {
            $("#PickupNeeded").val("false");
        }
    });

    $("#CustomClearanceNeededCheck").change(function () {

        $("#cccost").toggle();

        if ($(this).is(':checked')) {
            $("#CustomClearanceNeeded").val("true");
        }
        else {
            $("#CustomClearanceNeeded").val("false");
        }
    });

    $("#prepaid").click(function () { $("#IsPrepaid").val("true"); });
    $("#collected").click(function () { $("#IsPrepaid").val("false"); });

    $("#IsCareOfCheck").change(function () {
        if ($(this).is(':checked')) {
            $("#IsCareOf").val("true");
        }
        else {
            $("#IsCareOf").val("false");
        }
    });

});

function GetNotifierList(consigneeId) {
    var selectedNotifier = $("#NotitiferEdit").val();
    $.ajax({
        url: ROOT + "Quotation/GetNotifierList",
        type: "POST",
        data: { 'consigneeId': 0/*consigneeId*/ },
        async: false,
        dataType: "json",
        success: function (data) {
            $('#NotifierId').find('option').remove().end().append('<option value=""></option>');
            $.each(data, function (i, item) {
                if (selectedNotifier != item.Id) {
                    $("#NotifierId").append($("<option/>").val(item.Id).text(item.Value));
                }
                else {
                    $("#NotifierId").append($("<option selected />").val(item.Id).text(item.Value));
                }
            });
            $("#NotifierId").select2();
        }
    });
}

function GetVesselList(carrierId) {
    var selectedVessel = $("#VesselIdEdit").val();

    $.ajax({
        url: ROOT + "Quotation/GetVesselList",
        type: "POST",
        data: { 'carrierId': carrierId },
        async: false,
        dataType: "json",
        success: function (data) {
            $('#VesselId').find('option').remove().end().append('<option value=""></option>');
            $.each(data, function (i, item) {
                if (selectedVessel != item.Id) {
                    $("#VesselId").append($("<option/>").val(item.Id).text(item.Value));
                }
                else {
                    $("#VesselId").append($("<option selected />").val(item.Id).text(item.Value));
                }
            });
            $("#VesselId").select2();
        }
    });

}