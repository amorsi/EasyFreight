$(function () {
    $("#saveinv").on("click", function () {
        var isValid = CheckFormIsValid('invform');
        if (isValid) {
            SaveInvoice();

        }
    });
})


function SaveInvoice() {
    $.ajax({
        url: ROOT + "Accounting/AddEditInvoice",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Invoice Saved',
                    type: 'success'
                });
                var operId = $("#OperationId").val();
                var orderFrom = $("#OrderFrom").val();
                window.location.href = ROOT + "Accounting/ManageOrder?id=" + operId + "&orderFrom=" + orderFrom;
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