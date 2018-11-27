﻿$(function () {

    $("#ddlCountryCity").change(function () {
        var countryId = $('option:selected', this).attr('countryId');
        $("#CountryId").val(countryId);

    });

    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            SaveConsignee();
        }

    });
});

function SaveConsignee() {
    $.ajax({
        url: ROOT + "Consignee/AddEditConsignee",
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
    $("#ConsigneeId").val("0");
    $("#ConsigneeCode").val("");
    $("#ddlCountryCity").val("").trigger("change");

}