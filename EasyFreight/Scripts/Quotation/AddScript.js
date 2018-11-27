$(function () {

    if ($("#carriertype").val() == "2") //air edit mode
    {
        $(".btnAdd").toggle();
        $("select[name*=ContainerTypeId]").prop("readonly", true);
        $("select[name*=ContainerTypeId]").append($("<option selected />").val("0").text("Air"));
        $("select[name*=ContainerTypeId][name*=0]").select2();
    }

    $("#carriertype").change(function () {
        var carrType = $(this).val();
        $(".btnAdd").toggle();

        if (carrType == "1") //sea
        {
            $("select[name*=ContainerTypeId][name*=0]").prop("readonly", false);
            $("select[name*=ContainerTypeId][name*=0] option[value='0']").remove();

            $(".btnAdd").attr("lastindex", "0");

        }
        else {
            //air
            $("select[name*=ContainerTypeId][name*=0]").prop("readonly", true);
            $("select[name*=ContainerTypeId][name*=0]").append($("<option selected />").val("0").text("Air"));

            $(".remove-contact").each(function () {
                if (!$(this).parents('.form-template').length) {
                    $(this).click();
                }

            });

        }

        $("select[name*=ContainerTypeId][name*=0]").select2();
    });

    if ($("#HasExpireDateCheck").is(':checked')) {
        $('#ExpirationInDays').attr("disabled", false);
    }

    $("#HasExpireDateCheck").change(function () {
        if ($(this).is(':checked')) {
            $("#HasExpireDate").val("true");
            $('#ExpirationInDays').attr("disabled", false);
        }
        else {
            $("#HasExpireDate").val("false");
            $("#ExpirationInDays").val("");
            $("#ExpirationInDays").attr('disabled', 'disabled');
        }
    });

    $("#SaveForm").click(function () {
        var isValid = CheckFormIsValid('Form1');
        if (isValid) {
            $(this).attr('disabled', 'disabled');
            SaveQuotation();
            $(this).attr('disabled', '');
        }
        else {
            new PNotify({
                title: 'Sorry!',
                text: 'Please check invalid inputs',
                type: 'error'
            });
        }

    });

    /*
Modal Dismiss
*/
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    /*
    Remove row Modal 
    */
    $('.quickadd').magnificPopup({
        type: 'inline',
        preloader: false,
        modal: true
    });

    $(".quickadd").click(function () {
        var addType = $(this).attr("addtype");
        $("#addtype").val(addType);
        if (addType == 1)
            $("#addheader").text("Add New Shipper");
        else
            $("#addheader").text("Add New Consignee");

    });

    $(document).on('click', '.modal-confirm', function (e) {
        e.preventDefault();      
        var addType = $("#addtype").val();
        var code = $("#code").val();
        var name = $("#name").val();
        if (addType == 1)
            AddShipper(code, name);
        else
            AddConsignee(code, name);

        $.magnificPopup.close();

    });

});

function SaveQuotation() {

    $.ajax({
        url: ROOT + "Quotation/AddEditQuotation",
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
                window.location.href = ROOT + "Quotation/Index?orderFrom=" + $("#OrderFrom").val();
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

function AddShipper(code, name) {
    $.ajax({
        url: ROOT + "MasterData/Shipper/AddShipperQuick",
        type: "POST",
        data: { "code": code, "name": name },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "0") {
                new PNotify({
                    title: 'Sorry!',
                    text: data,
                    type: 'error'
                });
            }
            else {
                $("#ShipperId").append($("<option selected />").val(data).text(name));
                $("#ShipperId").select2();
            }
        }
    });
}

function AddConsignee(code, name) {
    $.ajax({
        url: ROOT + "MasterData/Consignee/AddConsigneeQuick",
        type: "POST",
        data: { "code": code, "name": name },
        async: false,
        dataType: "json",
        success: function (data) {
            if (data == "0") {
                new PNotify({
                    title: 'Sorry!',
                    text: data,
                    type: 'error'
                });
            }
            else {
                $("#ConsigneeId").append($("<option selected />").val(data).text(name));
                $("#ConsigneeId").select2();
            }
        }
    });
}

function ClearForm() {

}
 

