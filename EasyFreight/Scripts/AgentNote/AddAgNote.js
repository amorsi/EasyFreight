$(function () {

    $("#BankId").change(function () {
        var bankId = $(this).val();
        var currId = $("#CurrencyId").val();
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
                    new PNotify({
                        title: 'Sorry!',
                        text: "No Bank Account in this bank with the agent currency",
                        type: 'error'
                    });
                }
            }
        });

    });

    $("#savenote , #savePrintnote").click(function () {
        var isValid = CheckFormIsValid('invform');
        if ($("#tbinvdetail input[type='checkbox']:checked").length == 0) {
            new PNotify({
                title: 'Sorry!',
                text: "Please Select at least one row from the Invoices List",
                type: 'error'
            });

            isValid = false;

        }
        if (isValid) {
            if ($(this).attr('id') == 'savenote')
                SaveAgentNote(0);

            if ($(this).attr('id') == 'savePrintnote')
                SaveAgentNote(1);



                //SaveAgentNote();

        }

    });
})

function SaveAgentNote(print) {
    $.ajax({
        url: ROOT + "AgentNote/AddEditAgentNote",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data.indexOf("true")>-1) {
                new PNotify({
                    title: 'Success!',
                    text: 'Order Saved',
                    type: 'success'
                });
                if (print == "0")
                    window.location.href = ROOT + "AgentNote";
                else {
                    var agentID = data.replace('true', '');
                    window.location.href = ROOT + "AgentNote/PrintNote?id=" + agentID;
                    //window.location.href = ROOT + "AgentNote"; 
                    $("#savenote").attr('disabled', 'disabled');
                    $("#savePrintnote").attr('disabled', 'disabled');
                }
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