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

    $("#FromAreaId").change(function () {
        var countryId = $('option:selected', this).attr('cityId');
        $("#FromCityId").val(countryId);

    });

    $("#ToAreaId").change(function () {
        var countryId = $('option:selected', this).attr('cityId');
        $("#ToCityId").val(countryId);

    });
    
    $('select[name=ddlFromCityId]').change(function () {
        var cityId = $('option:selected', this).val();
        $("#FromCityId").val(cityId);

    });

    $('select[name=ddlToCityId]').change(function () {
        var cityId = $('option:selected', this).val();
        $("#ToCityId").val(cityId);

    });


    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid)
        {
            Save();
        }

    });


    $("#selectByArea").click(function () {
        $("#divByArea").show();
        $("#divByCity").hide();
        $("#FromCityId").val('');
        $("#ToCityId").val('');
        $("#isbyArea").val(true);
    });
    $("#selectByCity").click(function () {
        $("#divByArea").hide();
        $("#divByCity").show();
        $("#FromCityId").val('');
        $("#ToCityId").val('');
        $("#isbyArea").val(false);
    });


});

function Save() {
    var byarea = $("#isbyArea").val();
    $.ajax({
        url: ROOT + "ContractorRate/AddEditContractorRate",
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
   
    //$("select").val("").trigger("change");

}