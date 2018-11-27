$(function () {


    $(".currncySign").text($("#ddlCostCurrency option:selected").text());

    $('#ddlCostCurrency').change(function () {
        $(".currncySign").text($("#ddlCostCurrency option:selected").text());
        var currencyId = $("#ddlCostCurrency option:selected").val();
        $(".currencyid").val(currencyId);
        $("#CostCurrencyId").val(currencyId);
        $("#CostCurrencyId2").val(currencyId);
    });

    //sum of net texts on blur
    $(".costnet").blur(function () {
        var sum = 0;
        $(".costnet").each(function () {
            sum += Number($(this).val());
        });
        var result = (Math.round(sum * 100) / 100).toFixed(2);
        $(".sumnet").text(result);
    });

    //sum of selling texts on blur
    $(".costsell").blur(function () {
        var sum = 0;
        $(".costsell").each(function () {
            sum += Number($(this).val());
        });
        var result = (Math.round(sum * 100) / 100).toFixed(2);
        $(".sumsell").text(result);
        // $(".sumsell").text(Math.round(sum * 100) / 100).toFixed(3);
    });

    //sum of net texts on load
    var netsum = 0;
    $(".costnet").each(function () {
        netsum += Number($(this).val());
    });
    var totalnetsum = (Math.round(netsum * 100) / 100).toFixed(2);
    $(".sumnet").text(totalnetsum);

    //sum of sell texts on load
    var sellsum = 0;
    $(".costsell").each(function () {
        sellsum += Number($(this).val());
    });
    var totalsellsum = (Math.round(sellsum * 100) / 100).toFixed(2);
    $(".sumsell").text(totalsellsum);


    $('.remove-contact').off('click');
    $('.remove-contact').on('click', function () {
        $(this).parents('.contactform').slideUp("normal", function () {
            $(this).remove();

            var lastIndex = parseInt($('.btnAdd').attr("lastIndex"));
            //reset all divs index
            $($('.contactform').get().reverse()).each(function () {
                var divContent = $(this);
                var firstInput = $(this).find('input.form-control').first().attr('name');
                
                if (firstInput.indexOf("[-1]") == -1 && firstInput.indexOf("[0]") == -1
                    && firstInput.indexOf("[") != -1 && lastIndex > 0) {
                    var xx = lastIndex - 1;
                    divContent.find('input').each(function () {
                        if (this.name.indexOf("[-1]") == -1 && this.name.indexOf("[0]") == -1
                            && this.name.indexOf("[") != -1 && lastIndex > 0) {

                            this.name = this.name.replace('[' + lastIndex + ']', '[' + xx + ']');
                        }
                    });

                    divContent.find('select').each(function () {
                        if (this.name.indexOf("[-1]") == -1 && this.name.indexOf("[0]") == -1
                            && this.name.indexOf("[") != -1 && lastIndex > 0) {

                            this.name = this.name.replace('[' + lastIndex + ']', '[' + xx + ']');
                        }
                    });

                    lastIndex--;

                }

            });

            var newIndex = parseInt($('.btnAdd').attr("lastIndex")) - 1;
            $('.btnAdd').attr("lastIndex", newIndex);

            //sum of sell texts on load
            var sellsum = 0;
            $(".costsell").each(function () {
                sellsum += Number($(this).val());
            });
            var totalsellsum = (Math.round(sellsum * 100) / 100).toFixed(2);
            $(".sumsell").text(totalsellsum);

            //sum of net texts on load
            var netsum = 0;
            $(".costnet").each(function () {
                netsum += Number($(this).val());
            });
            var totalnetsum = (Math.round(netsum * 100) / 100).toFixed(2);
            $(".sumnet").text(totalnetsum);


        });

    });

    $('.isagentcheck').off('change');
    $('.isagentcheck').on('change', function () {
        var agentID = $(".agentId").val();
        if ($('.agentId').val() != '') {
            var hidden = $(this).next('input[type=hidden]');
            if ($(this).is(':checked')) {
                $(hidden).val("true");
            }
            else {
                $(hidden).val("false");
            }
        }
        else {
            $(this).filter(':checkbox').prop('checked', false);
            new PNotify({
                title: 'Sorry!',
                text: 'This HB does not have an agent ',
                type: 'error'
            });
        }
    });

    //$('.costname').off('change');
    //$('.costname').on('change', function () {
    //    if ($('.costname').length > 2)
    //    {
    //        DisableOptions(); //disable selected values

    //    }
    //});

});

function DisableOptions() {
    $(".costname option").removeAttr("disabled"); //enable everything
    var arr = [];
    $(".costname option:selected").each(function () {
        if ($(this).val() != "")
            arr.push($(this).val());
    });
    $(".costname option").each(function () {
        if ($.inArray($(this).val(), arr) != -1 && !$(this).is(":selected"))
            $(this).attr("disabled", "disabled");
        //return $.inArray($(this).val(), arr) == -1;
    });
    //.attr("disabled", "disabled");

}