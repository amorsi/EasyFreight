$(function () {

    $(document).on('click', '.currencyitem', function () {
        var currId = $(this).attr("currencyId");
        var currSign = $(this).text();
        $(this).parent().closest('div').find('.currSelected').text(currSign);
        $(this).parent().closest('div').find('.currIdHidden').val(currId);
    });

    $("#checkExp").change(function () {
        if ($(this).is(':checked'))
            $("#ValidToChecked").val("true");
        else
            $("#ValidToChecked").val("false");

        $("#dateExp").toggle("slow");
    });

    $("#FromPortId").change(function () {
        var countryId = $('option:selected', this).attr('countryId');
        $("#FromCountryId").val(countryId);

    });

    $("#ToPortId").change(function () {
        var countryId = $('option:selected', this).attr('countryId');
        $("#ToCountryId").val(countryId);

    });

    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid)
        {
            Save();
        }

    });

    if ($("#TransitList").val() != "") {
        var selectedValues = $("#TransitList").val().split(',');
        $("#TransitPortId").select2('val', selectedValues);
    }


});

function Save() {

    $.ajax({
        url: ROOT + "CarrierRate/AddEditCarrierRate",
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
   
    $("select").val("").trigger("change");
    $("#TransitPortId").select2('val', "");
}