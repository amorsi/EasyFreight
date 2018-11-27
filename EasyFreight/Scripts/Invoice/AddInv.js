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

        $(".invamount").blur();
    });

    $('#invcurrency').change(function () {
        $('.invcurrid').val($(this).val());
        var txt = $("#invcurrency option:selected").text();
        $('.currsign').text(txt);
        $('.currsign').val(txt);
    });


    $("#saveinv").on("click", function () {
        var isValid = CheckInvFormValid();
        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveInvoice();
        }
    });
    $("#saveapinv").on("click", function () {
        var isValid = CheckInvFormValid();
        if (isValid) {
            SaveAPInvoice();
        }

    });
    

    //sum of net texts on blur
    $(".invamount").blur(function () {

        var sum = 0;
        $(".invamount").each(function () {
            if ($(this).attr('disabled') === undefined)
                sum += Number($(this).val());
        });

        var result = (Math.round(sum * 100) / 100).toFixed(2);

        var resultAfterTax = 0;
        if ($(".taxdeposit").val() == "" || $(".taxdeposit").val() == undefined)
        {
            resultAfterTax = result;
        }
        else
        {
            var taxAmount = Number($(".taxdeposit").val());
            resultAfterTax = result - taxAmount;
        }

        if ($(".vat").val() == "" || $(".vat").val() == undefined) {
            //resultAfterTax = resultAfterTax;
        }
        else {
            var vatAmount = Number($(".vat").val());
            resultAfterTax = resultAfterTax + vatAmount;
        }

        $(".suminv").text(result);
        $(".suminvaftertax").text(resultAfterTax);
        $(".suminvhidden").val(result);
        $(".suminvhiddenaftertax").val(resultAfterTax);

    });

    $(".exchange").blur(function () {
        var row = $(this).closest('tr');
        var exrate = Number($(this).val());
        var mainAmoutn = Number($(row).find('.mainamount').val());

        var result = (Math.round(exrate * mainAmoutn * 100) / 100).toFixed(2);
        $(row).find('.invamount').val(result);

        $(".invamount").blur();
    });

    $(".taxdeposit").blur(function () {
        CalcNewTotal();       
    });

    $(".vat").blur(function () {
        CalcNewTotal();
    });

    $('#invcurrency').change(function () {
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
        if ($("#BankId").length > 0) {
            $("#BankId,#CheckBankId").trigger("change");
        }

    });

    var receiptCurr =  $('#invcurrency').val();
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

});

function CalcNewTotal()
{
    var taxAmount = Number($(".taxdeposit").val());
    var vatAmount = Number($(".vat").val());

    var totalBeforeTax = Number($('.suminvhidden').val());

    var result = (Math.round((totalBeforeTax - taxAmount + vatAmount) * 100) / 100).toFixed(2);

    $('.suminvhiddenaftertax').val(result);
    $(".suminvaftertax").text(result);
}

function SaveInvoice() {
    $.ajax({
        url: ROOT + "Invoice/AddEditInvoice",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Invoice Saved',
                    type: 'success'
                });
                var operId = $("#OperationId").val();
                var orderFrom = $("#OrderFrom").val();
                history.go(-1);
                // window.location.href = ROOT + "Accounting/ManageOrder?id=" + operId + "&orderFrom=" + orderFrom;
            }
            else {
                new PNotify({
                    title: 'Sorry!',
                    text: data,
                    type: 'error'
                });
                $(this).attr('disabled', '');
            }
        }
    });

}

function CheckInvFormValid()
{
    var isValid = CheckFormIsValid('invform');
    if ($("#tbinvdetail input[type='checkbox']:checked").length == 0)
    {
        new PNotify({
            title: 'Sorry!',
            text: "Please Select at least one row from the Invoices List",
            type: 'error'
        });

        isValid = false;

    }

    return isValid;
}

function SaveAPInvoice() {
    $.ajax({
        url: ROOT + "APInvoice/AddEditInvoice",
        type: "POST",
        data: $("#invform").serialize(),
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "true") {
                new PNotify({
                    title: 'Success!',
                    text: 'Invoice Saved',
                    type: 'success'
                });
                var operId = $("#OperationId").val();
                var orderFrom = $("#OrderFrom").val();
                history.go(-1);
                //window.location.href = ROOT + "Accounting/ManageOrder?id=" + operId + "&orderFrom=" + orderFrom;
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

var fixHelperModified = function (e, tr) {
    var $originals = tr.children();
    var $helper = tr.clone();
    $helper.children().each(function (index) {
        $(this).width($originals.eq(index).width())
    });
    return $helper;
},

  updateIndex = function (e, ui) {
      $('td.index', ui.item.parent()).each(function (i) {
          $(this).html(i + 1);
      });
      $('td.index2', ui.item.parent()).each(function (i) {
          $(this).find('.tdIndx').val(i + 1);
      });
  };

$("#tbinvdetail tbody").sortable({
    helper: fixHelperModified,
    stop: updateIndex
}).disableSelection();