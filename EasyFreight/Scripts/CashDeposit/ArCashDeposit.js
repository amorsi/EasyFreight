$(function () {

    $("#PaymentTermId").change(function () {
        if ($(this).val() == "3") //Bank Cash Deposit
        {
            $("#forbank").toggle();
            $("#forbank").find('select').attr('required', 'required');
            $("#forcheck").hide();
            $("#forcheck").find('input[type="text"]').removeAttr('required');
            $("#bankAccountNumb").show();
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
            $("#bankAccountNumb").hide();
        }
    });
    //$('#invcurrency').change(function () {
        $("#BankId,#CheckBankId").change(function () {

            var bankId = $(this).val();
            if (bankId == "")
                return;
            var currId = $("#CurrencyId").val();
            if (currId == "" || currId == undefined) {
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

      //  });
    });

    $('input[type=radio][name=clientradio]').change(function () {
        if (this.id == 'shipper') {
            $("#ShipperId").attr('required', 'required');
            $("#ConsigneeId").removeAttr('required');
            $("#ConsigneeId").val("").trigger("change");
            $("#shipperdiv").toggle();
            $("#consigneediv").toggle();
        }
        else if (this.id == 'consignee') {
            $("#ConsigneeId").attr('required', 'required');
            $("#ShipperId").removeAttr('required');
            $("#ShipperId").val("").trigger("change");
            $("#shipperdiv").toggle();
            $("#consigneediv").toggle();
        }
    });

    $("#savereceipt").click(function () {
        var isValid = CheckFormIsValid('invform');
        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveCashReceipt();
            $(this).attr('disabled', '');
        }

    });


   // $("#BankId,#CheckBankId").trigger("change");
});

function SaveCashReceipt() {
    $.ajax({
        url: ROOT + "CashDeposit/AddArCashDeposit",
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

                window.location.href = ROOT + "CashDeposit";

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
