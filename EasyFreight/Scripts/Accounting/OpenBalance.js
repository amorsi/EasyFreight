$(function () {

    $(document).off('change', '#LibItemId');
    $(document).on('change', '#LibItemId', function () {
        var parentDivId = $(this).closest('.tab-pane').attr('id');
        
        LoadBalanceForm($(this).val(), parentDivId);

    });

    $(document).off('click', '#SaveForm');
    $(document).on('click', '#SaveForm', function () {
        
            var parentDivId = $(this).closest('.tab-pane').attr('id');
            SaveBalanceForm(parentDivId);
    });

    //Shipper tab
    $("#shipperlink").on('click', function () {
        if (!$('#shipper').children("#formcontent").length) {
            GetOtherTabForm('Shipper', 'shipper');
        }
    });

    //Consignee tab
    $("#conslink").on('click', function () {
        if (!$('#consignee').children("#formcontent").length) {
            GetOtherTabForm('Consignee', 'consignee');
        }
    });

    //Carrier Tab
    $("#carrlink").on('click', function () {
        if (!$('#carrier').children("#formcontent").length) {
            GetOtherTabForm('Carrier', 'carrier');
        }
    });

    //Contractor Tab
    $("#contlink").on('click', function () {
        if (!$('#contractor').children("#formcontent").length) {
            GetOtherTabForm('Contractor', 'contractor');
        }
    });

    //Cash Tab
    $("#cashlink").on('click', function () {
        if (!$('#cash').children("#formcontent").length) {
            GetOtherTabForm('Currency', 'cash');
        }
    });

    //Cash Tab
    $("#banklink").on('click', function () {
        if (!$('#bank').children("#formcontent").length) {
            GetOtherTabForm('BankAccount', 'bank');
        }
    });

    //Partner Tab
    $("#Partnerslink").on('click', function () {
        if (!$('#partner').children("#formcontent").length) {
            GetOtherTabForm('PartnerAccount', 'partner');
        }
    });

    //CC Cash Deposit Tab
    $("#CCCashDeplink").on('click', function () {
        if (!$('#cccashDep').children("#formcontent").length) {
            GetOtherTabForm('CashDepositTempAccount', 'cccashDep');
        }
    });
});

function LoadBalanceForm(libId,parentDivId) {
    var tbName = $("#" + parentDivId).find('#TbName').val();
    var pkName =  $("#" + parentDivId).find('#PkName').val();
    $.ajax({
        url: ROOT + "OpenBalance/GetBalancePartial",
        type: "POST",
        data: { 'itemId': libId, 'tbName': tbName, 'pkName': pkName },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#" + parentDivId).find("#baldiv").html(data);

            if (tbName == "Currency")
            {
                
                $("#" + parentDivId).find("tbody tr").each(function () {
                    if ($(this).attr('id') != libId)
                    {
                        $(this).hide();
                    }
                    else
                    {
                        $(this).show();
                    }
                })
            }

        }
    });

}

function SaveBalanceForm(parentDivId) {
    $.ajax({
        url: ROOT + "OpenBalance/AddEditBalance",
        type: "POST",
        data: $("#" + parentDivId).find("#balanceform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data.IsSaved == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Open Balance Saved',
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

function GetOtherTabForm(tbName, divId) {

    $.ajax({
        url: ROOT + "OpenBalance/GetOtherTabForm",
        type: "POST",
        data: { 'tbName': tbName },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#" + divId).html(data);
            $("#" + divId).find("#LibItemId").select2();
        }
    });

}