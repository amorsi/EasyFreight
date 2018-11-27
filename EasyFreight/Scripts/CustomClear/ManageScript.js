$(function () {
    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            SaveOrderDetails();

        }

    });

    $("#SaveExit").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            SaveOrderDetails_Close();

        }

    });


    $(".currencyid").val($("#ddlCostCurrency").val());
});

function SaveOrderDetails() {
    var notes = $("#Notes").val();
    $.ajax({
        url: ROOT + "CustomClearance/AddEditCustClearDet",
        type: "POST",
        data: $("#Form1").serialize()+ "&Notes="+notes ,
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Order Saved',
                    type: 'success'
                });
                //ClearForm();
              //  window.location.href = ROOT + "CustomClearance";
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

function CloseCCOrder(orderId, statusId, oprOrderFrom) {
    var params;
    if (oprOrderFrom != undefined) {
        params = { 'ccId': orderId, 'statusId': statusId, 'orderFrom': oprOrderFrom };
    }
    else {
        params = { 'ccId': orderId, 'statusId': statusId };
    }
    var isClosed = 'true';
    $.ajax({
        url: ROOT + "CustomClearance/CloseCC",
        type: "POST",
        data: params,
        async: false,
        dataType: "json",
        success: function (data) {

            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: statusId == '4' ? 'Order has been Deleted.' : 'Order has been Closed.',
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
            isClosed = data;
        }
    });

    return isClosed;
}


function SaveOrderDetails_Close() {
    SaveOrderDetails();
    CloseCCOrder($("#hd_CCID").val(), 3);
    $("#SaveForm").attr('disabled', 'disabled');
    $("#SaveExit").attr('disabled', 'disabled');
}