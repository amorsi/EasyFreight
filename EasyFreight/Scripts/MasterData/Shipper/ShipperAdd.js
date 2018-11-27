$(function () {

    $("#ddlCountryCity").change(function () {
        var countryId = $('option:selected', this).attr('countryId');
        $("#CountryId").val(countryId);

    });

    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            SaveShipper();
        }

    });
});

function SaveShipper() {
    $.ajax({
        url: ROOT + "Shipper/AddEditShipper",
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
                    text: 'Error Accrues.',
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
    $("#ShipperId").val("0");
    $("#ShipperCode").val("");
    $("#ddlCountryCity").val("").trigger("change");

}