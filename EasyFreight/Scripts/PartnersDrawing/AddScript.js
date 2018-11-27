$(function () {
    $("#savepartnerreceipt").click(function () {
        var isValid = CheckFormIsValid('invform');

        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveReceipt(false);
            $(this).attr('disabled', '');
        }

    });
    $("#saveAndPrint").click(function () {
        var isValid = CheckFormIsValid('invform');

        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveReceipt(true);
            $(this).attr('disabled', '');
        }

    });
    

})

function SaveReceipt(isPrint) {
    $.ajax({
        url: ROOT + "PartnersDrawing/AddEditCashReceipt",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data.indexOf('true') != -1) //if (data == "true")
            {
                new PNotify({
                    title: 'Success!',
                    text: 'Cash Receipt Saved',
                    type: 'success'
                });
                var cashTypr = $("#cashtype").val();
                if (isPrint == true) {
                    //Print
                    var recId = data.replace('true_', '');
                    var isOut = false;
                    if (cashTypr == 'cashout')
                        isOut = true;

                    window.open(
                      ROOT + "CashManagement/PrintCashIn?id=" + recId + '&isOut=' + isOut,
                      '_blank'
                    );
                    window.location.href = ROOT + "PartnersDrawing";
                }
                else {
                    window.location.href = ROOT + "PartnersDrawing";
                }
                return false;
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