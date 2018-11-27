

$(function () {

    $(".currencyid").val($("#ddlCostCurrency").val());

    if ($("#ddlContractors").val() != "")
    {
        $(".contractorId").val($("#ddlContractors").val());
    }

    $(".time24").timepicker({
        minuteStep: 5,
        showSeconds: false,
        showMeridian: false,
        defaultTime: '08:00'
    });

    $("#ddlContractors").change(function () {
        $(".contractorId").val($(this).val());
    });

    $('.ratesmodal').magnificPopup({
        type: 'inline',

        fixedContentPos: false,
        fixedBgPos: true,
        overflowY: 'auto',
        closeBtnInside: true,
        preloader: false,

        midClick: true,
        removalDelay: 300,
        mainClass: 'my-mfp-slide-bottom',
        modal: true
    });

    /*
 Modal Dismiss
 */
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

      
});
 


$(function () {

    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            SaveOrderDetails();

        }

    });

    $("#SaveExit").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            
            $.magnificPopup.open({
                items: {
                    src: '#modelRoll',
                    type: 'inline'
                },
                modal: true
            });

        }

    });

    //Close order Modal Confirm
    $(document).on('click', '#dialogConfirmRoll', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        //change the status
         $("#StatusId").val(3);
        SaveOrderDetails();
    });

    $("#showrates").click(function () {
        var FromAreaId = $("#FromAreaId").val();
        var ToAreaId = $("#ToAreaId").val();
        var FromCityId = $("#FromCityId").val();
        var ToCityId = $("#ToCityId").val();
        $.ajax({
            url: ROOT + "ContractorRateInquiry/GetRatesForArea",
            type: "POST",
            data: { 'fromCityId': FromCityId, 'toCityId': ToCityId },
            async: false,
            dataType: "html",
            success: function (data) {
                $("#ModalContent").html(data);
                $('#table table-striped mb-none').dataTable();

            }
        });
    });

    function SaveOrderDetails() {
        $.ajax({
            url: ROOT + "Trucking/AddEditTruckingOrderDetails",
            type: "POST",
            data: $("#Form1").serialize(),
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
                    window.location.href = ROOT + "Trucking";
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

});