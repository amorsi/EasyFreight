$(function () {
    $(".time24").timepicker({
        minuteStep: 5,
        showSeconds: false,
        showMeridian: false,
        defaultTime: '08:00'
    });
});

$(function () {

    $("#FromAreaId").change(function () {
        var countryId = $('option:selected', this).attr('cityId');
        $("#FromCityId").val(countryId);

    });

    $("#ToAreaId").change(function () {
        var countryId = $('option:selected', this).attr('cityId');
        $("#ToCityId").val(countryId);

    });



    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('TruckingForm');
        if (isValid) {
            SaveOrderDetails();

        }

    });

    function SaveOrderDetails() {
        $.ajax({
            url: ROOT + "Trucking/NewTruckingOrder",
            type: "POST",
            data: $("#TruckingForm").serialize(),
            async: false,
            dataType: "json",
            success: function (data) {
                if (data == "true") {
                    new PNotify({
                        title: 'Success!',
                        text: 'Order Saved',
                        type: 'success'
                    });
                    //ClearForm();
                    if ($.isFunction(GetTruckingList))
                        GetTruckingList();
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

});