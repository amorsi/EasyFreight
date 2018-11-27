function GetCCDetails(ccId, operId) {
    $.ajax({
        url: ROOT + "CustomClearance/GetOrderDetails",
        type: "POST",
        data: { 'ccId': ccId, 'operationId': operId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#ModalContent").html(data);
            $("#modalheader").html("Custom Clearance Order");
            $("#printtrucking").attr("href", ROOT + "CustomClearance/PrintDetails?operationId=" + operId + "&ccId=" + ccId);
            $("#printtruckingOrder").attr("href", ROOT + "CustomClearance/PrintWorkOrder?operationId=" + operId + "&ccId=" + ccId);
            $.magnificPopup.open({
                items: {
                    src: '#modalAnim',
                    type: 'inline'
                },
                modal: true
            });
            SumNumbers();
        }
    });
}

function SumNumbers() {
    //sum of net texts  
    var netsum = 0;
    $(".costnet").each(function () {
        netsum += Number($(this).val());
    });
    var totalnetsum = (Math.round(netsum * 100) / 100).toFixed(3);
    $(".sumnet").text(totalnetsum);

    //sum of sell texts  
    var sellsum = 0;
    $(".costsell").each(function () {
        sellsum += Number($(this).val());
    });
    var totalsellsum = (Math.round(sellsum * 100) / 100).toFixed(3);
    $(".sumsell").text(totalsellsum);

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

