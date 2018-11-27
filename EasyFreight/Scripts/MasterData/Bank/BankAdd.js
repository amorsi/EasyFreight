$(function () {

    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            SaveBank();
        }

    });
});

function SaveBank() {
    $.ajax({
        url: ROOT + "Bank/AddEditBank",
        type: "POST",
        data: $("#Form1").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Row Added.',
                    type: 'success'
                });
              //  ClearForm();
            }
            else {
                new PNotify({
                    title: 'Sorry!',
                    text: 'Error Accrues.',
                    type: 'error'
                });
            }
        }
    });

}
