$(function () {

    $(document).on('click', '.viewrec', function () {
        var receiptId = $(this).attr("recid");
        var isOut = $(this).attr("isout");      
        ViewCashReceipt(receiptId, isOut);

        if(isOut == "True")
            $("#modalheader").html("Cash Out Receipt Details");
        else
            $("#modalheader").html("Cash In Receipt Details");
        $("#printbtn").show();
        $("#printbtninv").hide();
        $("#printbtn").attr("href", ROOT + "CashManagement/PrintCashIn?id=" + receiptId + "&isOut=" + isOut);
    });

});

function ViewCashReceipt(id, isOut) {
    $.ajax({
        url: ROOT + "CashManagement/ViewCashInPartial",
        type: "POST",
        data: { 'id': id, 'isOut': isOut },
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