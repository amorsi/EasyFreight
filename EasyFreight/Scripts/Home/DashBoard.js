$(function () {

    BindByOrderTypePie(0);

    $("#year1").on("change", function () {
        var year = $(this).val();
        BindByOrderTypePie(year);
    });

    BindByCarrierTypePie(0);

    $("#year2").on("change", function () {
        var year = $(this).val();
        BindByCarrierTypePie(year);
    });

    BindStatusPie(0);

    $("#year3").on("change", function () {
        var year = $(this).val();
        BindStatusPie(year);
    });

    BindOperationsLine(0);
    $("#year4").on("change", function () {
        var year = $(this).val();
        BindOperationsLine(year);
    });

});


//ByOrderTypePie Start
function GetByOrderTypePieData(year) {
    var usagePieData;
    $.ajax({
        url: ROOT + "Home/GetByOrderType",
        data: { 'year': year },
        type: "POST",
        async: false,
        success: function (data) {
            usagePieData = data;
        }

    });

    if (usagePieData == '')
        return usagePieData;

    return jQuery.parseJSON(usagePieData);
}

function BindByOrderTypePie(year) {

    var usagePieData = GetByOrderTypePieData(year);
    var ctx = document.getElementById("piebyordertype");
    if (usagePieData != '') {
        var myChart = new Chart(ctx, {
            type: 'pie',
            data: usagePieData
        });
        $("#piebyordertype").show();

    }
    else
    {
        $("#piebyordertype").hide();
    }

}
//ByOrderTypePie End

//ByCarrierTypePie Start
function GetByCarrierTypePieData(year) {
    var usagePieData;
    $.ajax({
        url: ROOT + "Home/GetByCarrierType",
        data: { 'year': year },
        type: "POST",
        async: false,
        success: function (data) {
            usagePieData = data;
        }

    });

    if (usagePieData == '')
        return usagePieData;

    return jQuery.parseJSON(usagePieData);
}

function BindByCarrierTypePie(year) {

    var usagePieData = GetByCarrierTypePieData(year);
    var ctx = document.getElementById("piebycarriertype");
    if (usagePieData != '') {
        var myChart = new Chart(ctx, {
            type: 'pie',
            data: usagePieData
        });
        $("#piebycarriertype").show();

    }
    else {
        $("#piebycarriertype").hide();
    }

}
//ByCarrierTypePie End


//By Status Pie Start
function GetByStatusPieData(year) {
    var usagePieData;
    $.ajax({
        url: ROOT + "Home/GetByStatus",
        data: { 'year': year },
        type: "POST",
        async: false,
        success: function (data) {
            usagePieData = data;
        }

    });

    if (usagePieData == '')
        return usagePieData;

    return jQuery.parseJSON(usagePieData);
}

function BindStatusPie(year) {

    var usagePieData = GetByStatusPieData(year);
    var ctx = document.getElementById("piebystatus");
    if (usagePieData != '') {
        var myChart = new Chart(ctx, {
            type: 'pie',
            data: usagePieData
        });
        $("#piebystatus").show();

    }
    else {
        $("#piebystatus").hide();
    }

}
//By Status Pie End

//Operations Line Start
function GetOperationsLineData(year) {
    var usagePieData;
    $.ajax({
        url: ROOT + "Home/GetOperationsCountLine",
        data: { 'year': year },
        type: "POST",
        async: false,
        success: function (data) {
            usagePieData = data;
        }

    });

    if (usagePieData == '')
        return usagePieData;

    return jQuery.parseJSON(usagePieData);
}

function BindOperationsLine(year) {

    var usagePieData = GetOperationsLineData(year);
    var ctx = document.getElementById("operationschartLine");
    if (usagePieData != '') {
        var myChart = new Chart(ctx, {
            type: 'line',
            data: usagePieData
        });
        $("#operationschartLine").show();

    }
    else {
        $("#operationschartLine").hide();
    }

}
//Operations Line End