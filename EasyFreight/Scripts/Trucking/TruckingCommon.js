function GetTruckingDetails(truckingOrderId) {
    $.ajax({
        url: ROOT + "Trucking/GetOrderDetails",
        type: "POST",
        data: { 'truckingOrder': truckingOrderId },
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

function CloseOrder(orderId, statusId, oprOrderFrom) {
    var params;
    if (oprOrderFrom != undefined) {
        params = { 'truckingOrderId': orderId, 'statusId': statusId, 'orderFrom': oprOrderFrom };
    }
    else {
        params = { 'truckingOrderId': orderId, 'statusId': statusId };
    }
    var isClosed = 'true';
    $.ajax({
        url: ROOT + "Trucking/CloseOrder",
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