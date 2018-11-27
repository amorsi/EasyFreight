$(function () {
    $('#tbinvdetail').on('change', '.rowselect', function () {
        var row = $(this).closest('tr');
        if ($(this).is(':checked')) {
            $(row).find('input[type="text"]').removeAttr("disabled").attr("required", "required");;
            $(row).find('.selected').val("true");
        }
        else {
            $(row).find('input[type="text"]').attr("disabled", "disabled").removeAttr("required");;
            $(row).find('.selected').val("false");
        }

    });

    $('#CurrencyId').change(function () {
        var receiptCurr = $(this).val();
        $('.rowselect').each(function () {
            var row = $(this).closest('tr');
            var invCurr = $(row).find('.invcurr').val();
            if (receiptCurr == invCurr) {
                $(this).removeAttr("disabled");

            }
            else {
                $(this).attr("disabled", "disabled");
                $(this).removeAttr("checked");
                $(row).find('input[type="text"]').attr("disabled", "disabled");
                $(row).find('.selected').val("false");
            }
        });
        $("#BankId,#CheckBankId").trigger("change");

    });

    $("#PaymentTermId").change(function () {
        if ($(this).val() == "3") //Bank Cash Deposit
        {
            $("#forbank").toggle();
            $("#forbank").find('select').attr('required', 'required');
            $("#forcheck").hide();
            $("#forcheck").find('input[type="text"]').removeAttr('required');
        }
        else if ($(this).val() == "4") {
            $("#forbank").find('select').removeAttr('required');
            $("#forbank").hide();
            $("#forcheck").toggle();
            $("#forcheck").find('input[type="text"]').attr("required", "required");
            $("#forcheck").find('select').attr('required', 'required');
        }
        else {
            $("#forcheck").hide();
            $("#forbank").hide();
            $("#forcheck").find('input[type="text"]').removeAttr('required');
            $("#forbank").find('select').removeAttr('required');
            $("#forcheck").find('select').removeAttr('required');
        }
    });

    $("#BankId,#CheckBankId").change(function () {

        var bankId = $(this).val();
        if (bankId == "")
            return;
        var currId = $("#CurrencyId").val();
        if (currId == "" || currId == undefined)
        {
            new PNotify({
                title: 'Sorry!',
                text: "Please select currency first.",
                type: 'error'
            });

            $("#BankId,#CheckBankId").val("").trigger("change");
            return;
            
        }
        $.ajax({
            url: ROOT + "AgentNote/GetBankDetails",
            type: "POST",
            data: { 'bankId': bankId, 'currencyId': currId },
            async: false,
            dataType: "json",
            success: function (data) {
                if (data.AccountName != "false") {
                    $("#bankdetails").show();
                    $("#bankname").text(data.BankNameEn);
                    $("#bankaddress").text(data.BankAddressEn);
                    $("#accountname").text(data.AccountName);
                    $("#accountnum").text(data.AccountNumber);
                    $("#swiftcode").text(data.SwiftCode);
                    $("#BankAccId").val(data.BankAccId);
                }
                else {
                    $("#BankAccId").val("");
                    $("#bankdetails").hide();
                    $("#BankId,#CheckBankId").val("").trigger("change");
                    new PNotify({
                        title: 'Sorry!',
                        text: "No Bank Account in this bank with the agent currency",
                        type: 'error'
                    });
                }
            }
        });

    });

    $("#savereceipt").click(function () {
        var isValid = CheckFormIsValid('invform');

        if($("#tbinvdetail").length != 0)
        {
            if ($("#tbinvdetail input[type='checkbox']:checked").length == 0) {
                new PNotify({
                    title: 'Sorry!',
                    text: "Please Select at least one row from the Invoices List",
                    type: 'error'
                });

                isValid = false;

            }
            var sum = 0;
            $("#tbinvdetail input[type='text']").each(function () {
                var isDisabled = $(this).prop('disabled');

                if (!isDisabled)
                {
                    sum += Number($(this).val());
                }
            });
            var total = (Math.round(sum * 100) / 100).toFixed(0);

            var resAmount = Number($("#ReceiptAmount").val());
            var resTotal = (Math.round(resAmount * 100) / 100).toFixed(0);

            if(total != resTotal)
            {
                new PNotify({
                    title: 'Sorry!',
                    text: "Total invoice(s) amount must equals Receipt Amount",
                    type: 'error'
                });

                isValid = false;
            }
        }
        


        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveCashReceipt(false);
            $(this).attr('disabled', '');
        }

    });

    $("#savePrintreceipt").click(function () {
        var isValid = CheckFormIsValid('invform');

        if ($("#tbinvdetail").length != 0) {
            if ($("#tbinvdetail input[type='checkbox']:checked").length == 0) {
                new PNotify({
                    title: 'Sorry!',
                    text: "Please Select at least one row from the Invoices List",
                    type: 'error'
                });

                isValid = false;

            }
            var sum = 0;
            $("#tbinvdetail input[type='text']").each(function () {
                var isDisabled = $(this).prop('disabled');

                if (!isDisabled) {
                    sum += Number($(this).val());
                }
            });
            var total = (Math.round(sum * 100) / 100).toFixed(0);

            var resAmount = Number($("#ReceiptAmount").val());
            var resTotal = (Math.round(resAmount * 100) / 100).toFixed(0);

            if (total != resTotal) {
                new PNotify({
                    title: 'Sorry!',
                    text: "Total invoice(s) amount must equals Receipt Amount",
                    type: 'error'
                });

                isValid = false;
            }
        }



        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveCashReceipt(true);
            $(this).attr('disabled', '');
        }

    });

    $(".payamount").blur(function () {
        var row = $(this).closest('tr');
        //Get current amount due
        var currAmountDue = Number($(row).find(".maindue").text());
        //If payment amount > current amount due .. STOP ..
        if(Number($(this).val() > currAmountDue))
        {
            new PNotify({
                title: 'Sorry!',
                text: "Payment amount must be less than or equal  amount due.",
                type: 'error'
            });

            $(this).val(currAmountDue);

            return;
        }
        var amountdue = Number($(row).find(".maindue").text()) - Number($(this).val());
        var collectedamount = Number($(row).find(".maincollected").text()) + Number($(this).val());

        $(".amountdue").val((Math.round(amountdue * 100) / 100).toFixed(2));
        $(".collectedamount").val((Math.round(collectedamount * 100) / 100).toFixed(2));
    });

 
    $("#saveOpenReceipt").click(function () {
        var isValid = CheckFormIsValid('invform');

        if($("#tbinvdetail").length != 0)
        {
             
            var sum = 0;
            $("#tbinvdetail input[type='text']").each(function () {
                var isDisabled = $(this).prop('disabled');

                if (!isDisabled)
                {
                    sum += Number($(this).val());
                }
            });
            var total = (Math.round(sum * 100) / 100).toFixed(0);

            var resAmount = Number($("#ReceiptAmount").val());
            var resTotal = (Math.round(resAmount * 100) / 100).toFixed(0);

            if(total != resTotal || total==0)
            {
                new PNotify({
                    title: 'Sorry!',
                    text: "Total invoice(s) amount must equals Receipt Amount",
                    type: 'error'
                });

                isValid = false;
            }
        } 
        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveCashOpenBalanceReceipt(false);
            $(this).attr('disabled', '');
         }

    });

});

function SaveCashReceipt(isPrint)
{
    $.ajax({
        url: ROOT + "CashManagement/AddEditCashReceipt",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            var result = data;
            if (result.indexOf('true') != -1)// == "true") 
            {
                new PNotify({
                    title: 'Success!',
                    text: 'Cash Receipt Saved',
                    type: 'success'
                });
                var cashTypr = $("#cashtype").val();
            
                if (isPrint == true) {
                    //Print
                    var recId = result.replace('true_', '');
                    var isOut = false;
                    if (cashTypr == 'cashout')
                        isOut = true;

                    window.open(
                      ROOT + "CashManagement/PrintCashIn?id=" + recId + '&isOut=' + isOut,
                      '_blank'
                    );
                    //end Print
                }

                history.go(-1);

                //if (cashTypr == "cashin")
                //    window.location.href = ROOT + "Invoice";
                //else
                //    window.location.href = ROOT + "APInvoice";

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


function SaveCashOpenBalanceReceipt(isPrint) {
    $.ajax({
        url: ROOT + "CashManagement/AddEditOpenCashReceipt",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            var result = data;
            if (result.indexOf('true') != -1)// == "true") 
            {
                new PNotify({
                    title: 'Success!',
                    text: 'Cash Receipt Saved',
                    type: 'success'
                });
                var cashTypr = $("#cashtype").val();

                if (isPrint == true) {
                    //Print
                    var recId = result.replace('true_', '');
                    var isOut = false;
                    if (cashTypr == 'cashout')
                        isOut = true;

                    window.open(
                      ROOT + "CashManagement/PrintCashIn?id=" + recId + '&isOut=' + isOut,
                      '_blank'
                    );
                    //end Print
                }

                history.go(-1);

                //if (cashTypr == "cashin")
                //    window.location.href = ROOT + "Invoice";
                //else
                //    window.location.href = ROOT + "APInvoice";

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
