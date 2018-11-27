$(function () {

    $(".saveform").click(function () {
        if ($(this).attr('id') == 'SaveProcess')
            $("#process").val('true');

        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveOperation();
            $(this).attr('disabled', '');
        }
        else {
            new PNotify({
                title: 'Sorry!',
                text: 'Please check invalid inputs',
                type: 'error'
            });
        }
    });

    
    //if ($("#AgentId").val() != "" && $("#AgentId").val() != undefined) {
    //    $("#agentcost").show();
    //    if (!$("#IsFixedCostCheck").is(":checked"))
    //    {
    //        $("#fixedcostdiv").toggle();
    //        $("#precdiv").toggle();

    //        $("#PercentageOfAmount").attr("required", "required");
    //        $("#FixedCost").removeAttr("required");
    //    }
    //}

    $("#carriertype").change(function () {
        var carrType = $(this).val();
        GetContainersDetails(carrType);
    });

    //$("#AgentId").change(function () {
        
    //    var agentId = $(this).val();
    //    if (agentId != "") {
    //        $("#agentcost").show();
    //        $("#FixedCost").attr("required", "required");
    //    }
    //    else {
    //        $("#agentcost").hide();
    //        $("#FixedCost").removeAttr("required");
    //    }
        
    //});

    $("#IsFixedCostCheck").change(function () {

        $("#fixedcostdiv").toggle();
        $("#precdiv").toggle();

        if ($(this).is(':checked')) {
            $("#IsFixedCost").val("true");
            $("#FixedCost").attr("required", "required");
            $("#PercentageOfAmount").removeAttr("required");
        }
        else {
            $("#IsFixedCost").val("false");
            $("#PercentageOfAmount").attr("required", "required");
            $("#FixedCost").removeAttr("required");
        }
    });

    //sum summary section
    $(document).off('blur','.gw')
    $(document).on('blur','.gw',function () {
        var sum = 0;
        $(".gw").each(function () {
            sum += Number($(this).val());
        });
        var result = (Math.round(sum * 100) / 100).toFixed(2);
        $(".tgw").val(result);
    });

    $(document).off('blur', '.nw')
    $(document).on('blur', '.nw', function () {
        var sum = 0;
        $(".nw").each(function () {
            sum += Number($(this).val());
        });
        var result = (Math.round(sum * 100) / 100).toFixed(2);
        $(".tnw").val(result);
    });

    $(document).off('blur', '.cbm')
    $(document).on('blur', '.cbm', function () {
        var sum = 0;
        $(".cbm").each(function () {
            sum += Number($(this).val());
        });
        var result = (Math.round(sum * 100) / 100).toFixed(2);
        $(".tcbm").val(result);
    });

    $(document).off('blur', '.packagesnum')
    $(document).on('blur', '.packagesnum', function () {
        var sum = 0;
        $(".packagesnum").each(function () {
            sum += Number($(this).val());
        });
        var result = (Math.round(sum * 100) / 100).toFixed(0);
        $(".tpackagesnum").val(result);
    });

    $("#IsConsolidationCheck").change(function () {

        if ($(this).is(':checked')) {
            $("#IsConsolidation").val("true");
        }
        else {
            $("#IsConsolidation").val("false");
        }
    });
    

});

function SaveOperation() {
    var orderFrom = $("#orderFromString").val();
    $.ajax({
        url: ROOT + "Operation/export/AddEditOperation",
        type: "POST",
        data: $("#Form1").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data.isSaved == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Row Added.',
                    type: 'success'
                });
                if ($("#process").val() == 'false')
                    window.location.href = ROOT + "Operation/" + orderFrom;
                else
                    window.location.href = ROOT + "Operation/" + orderFrom + "/HouseBill/" + data.operId;
            }
            else {
                new PNotify({
                    title: 'Sorry!',
                    text: data.isSaved,
                    type: 'error'
                });
            }
        }
    });

}

function ClearForm() {

}

function GetContainersDetails(carrierType)
{
    var operationId = $("#OperationId").val();
    $.ajax({
        url: ROOT + "Operation/export/GetOperationContainers",
        type: "POST",
        data: { 'operationId': operationId, 'carrierType': carrierType },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#containersdata").html(data);
            $("select").not("[name*=-1]").select2();
        }
    });
}
