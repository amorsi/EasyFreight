$(function () {
    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            SaveAgent();
        }

    });
});

function SaveAgent() {
    $.ajax({
        url: ROOT + "Employee/AddEditEmployee",
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
                ClearForm();
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

function ClearForm()
{
    $("input:text").each(function () {
        $(this).val("");
    });
    $("textarea").each(function () {
        $(this).val("");
    });
    $("input[type=email]").each(function () {
        $(this).val("");
    });
    $("#EmpId").val("0");
    $("#DepId").val("").trigger("change");

}