$(function () {

    $("#saveexchange").click(function () {
        var isValid = CheckFormIsValid('invform');
        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveExchange();
            $(this).attr('disabled', '');
        }

    });

    $("#saveCashTransfer").click(function () {
        var isValid = CheckFormIsValid('invform');
        if (isValid) {
            $(this).attr('disabled', 'disabled');
            saveCashTransfer();
            $(this).attr('disabled', '');
        }

    });

    $("#saveBankTransfer").click(function () {
        var isValid = CheckFormIsValid('invform');
        if (isValid) {
            $(this).attr('disabled', 'disabled');
            saveBankTransfer();
            $(this).attr('disabled', '');
        }

    });



    
    $("#BankAccId").change(function () {
        var accountId = $('option:selected', this).attr('accountId');
        $("#hd_AccountID").val(accountId);

    });


});


function SaveExchange()
{
    $.ajax({
        url: ROOT + "CashMovement/CurrencyExchange",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Row Added.',
                    type: 'success'
                });
                setInterval(function () {
                    window.location.reload();
                }, 1000);
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


function saveCashTransfer() {
    $.ajax({
        url: ROOT + "CashMovement/CashToBank",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Row Added.',
                    type: 'success'
                });
                setInterval(function () {
                    window.location.reload();
                }, 1000);
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

function saveBankTransfer() {
    $.ajax({
        url: ROOT + "CashMovement/BankToCash",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Row Added.',
                    type: 'success'
                });
                setInterval(function () {
                    window.location.reload();
                }, 1000);
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