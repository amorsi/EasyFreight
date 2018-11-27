$(function () {

    $("#savereceipt").click(function () {
        var isValid = CheckFormIsValid('invform');

        var sum = 0;
        //$("#tbinvdetail input[type='text']").each(function () {
        //    var isDisabled = $(this).prop('disabled');

        //    if (!isDisabled) {
        //        sum += Number($(this).val());
        //    }
        //});

        sum = $(".sumnet").text();

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



        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveCashReceipt();
            $(this).attr('disabled', '');
        }

    });

    $('#ddlCostCurrency').change(function () {
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
        $("#BankId ").trigger("change");

    });

    $("#PaymentTermId").change(function () {
        if ($(this).val() == "3") //Bank Cash Deposit
        {
            $("#forbank").toggle();
            $("#forbank").find('select').attr('required', 'required'); 
        }
        else if ($(this).val() == "4") {
            $("#forbank").find('select').removeAttr('required');
            $("#forbank").hide(); 
        }
        else { 
            $("#forbank").hide(); 
            $("#forbank").find('select').removeAttr('required'); 
        }
    });

    $("#BankId").change(function () {

        var bankId = $(this).val();
        if (bankId == "")
            return;
        var currId = $("#ddlCostCurrency").val();
        if (currId == "" || currId == undefined) {
            new PNotify({
                title: 'Sorry!',
                text: "Please select currency first.",
                type: 'error'
            });

            $("#BankId").val("").trigger("change");
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
                    $("#BankId").val("").trigger("change");
                    new PNotify({
                        title: 'Sorry!',
                        text: "No Bank Account in this bank with the agent currency",
                        type: 'error'
                    });
                }
            }
        });

    });

});

function SaveCashReceipt() {
    $.ajax({
        url: ROOT + "CashOutExpense/AddEditCashReceipt",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Cash Receipt Saved',
                    type: 'success'
                });
                var cashTypr = $("#cashtype").val();
                history.go(-1);
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