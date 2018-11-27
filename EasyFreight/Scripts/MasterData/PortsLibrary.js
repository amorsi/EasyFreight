(function ($) {

    'use strict';

    $('.remove-row').magnificPopup({
        type: 'inline',
        preloader: false,
        modal: true
    });

    /*
    Modal Dismiss
*/
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    /*
	Modal Confirm
	*/
    $(document).on('click', '.modal-confirm', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var portId = $("#DelPortId").val()
        var isdeleted = DeletePort($("#DelPortId").val());
        if (isdeleted == "true") {
            new PNotify({
                title: 'Success!',
                text: 'Row Deleted.',
                type: 'success'
            });
            //Delete row from table
            var table = $('#datatable-default').DataTable();
            table.row('#' + portId).remove().draw(false);
            //$('#' + portId).remove();
        }
        else {
            new PNotify({
                title: 'Sorry!',
                text: isdeleted,
                type: 'error'
            });
        }
    });

    var datatableInit = function () {

        $('#datatable-default').dataTable();

    };

    $(function () {
        datatableInit();

        $("#SaveForm").click(function () {
            var isValid = CheckFormIsValid('Form1');
            if (isValid) {
                SavePort();
            }

        });

        $("#ddlCountryCity").change(function () {
            var countryId = $('option:selected', this).attr('countryId');
            $("#CountryId").val(countryId);

        });

        $("#ResetForm").click(function () {
            ClearForm();
            $('#AddForm').fadeOut();
            $('#SaveBtnDiv').fadeOut();
        });

        $("#addToTable").click(function () {
            $("#PortType").val("").trigger("change");
            $("#PortId").val("0");
            $('#AddForm').fadeIn();
            $('#SaveBtnDiv').fadeIn();
        });



        $('body').on('click', '.edit-row', function () {
            $('#AddForm').fadeIn();
            $('#SaveBtnDiv').fadeIn();
            var counId = $(this).attr("countryId");
            var cityId = $(this).attr("cityId");
            var portId = $(this).attr("portId");

            $("#CountryId").val(counId);
            $("#PortId").val(portId);


            $("#ddlCountryCity").val(cityId).trigger("change");
            $("#PortType").val($(this).attr("portType")).trigger("change");
            $("#PortNameEn").val($(this).closest('tr').children('td#NameEn').text());
            $("#PortNameAr").val($(this).closest('tr').children('td#NameAr').text());
        });

        $('body').on('click', '.remove-row', function () {
            $("#DelPortId").val($(this).attr("portId"));
        });

    });



}).apply(this, [jQuery]);


function SavePort() {
    $.ajax({
        url: ROOT + "CountryLibrary/AddEditPort",
        type: "POST",
        data: $("#Form1").serialize(),
        async: false,
        success: function (data) {
            if (data.indexOf("false") != -1) {
                new PNotify({
                    title: 'Sorry!',
                    text: data,
                    type: 'error'
                });
                return;
            }

            new PNotify({
                title: 'Success!',
                text: 'Row Added.',
                type: 'success'
            });
            $("#tbContent").html(data);
            ClearForm();
            $('#datatable-default').dataTable();
            $('.remove-row').magnificPopup({
                type: 'inline',
                preloader: false,
                modal: true
            });
        }
    });

}

function ClearForm() {
    if ($("#PortId").val() != "0") //edit mode
    {
        $("#ddlCountryCity").val("").trigger("change");
        $("#PortType").val("").trigger("change");
        $('#AddForm').fadeOut();
        $('#SaveBtnDiv').fadeOut();
        $("#CountryId").val("0");
    }
    else { // add new .. will keep the form opened to add another
        $("#PortType").val("").trigger("change");
    }

    $("input:text").each(function () {
        $(this).val("");
    });

    $("#PortId").val("0");


}

function DeletePort(portId) {
    var isDeleted = "true";
    $.ajax({
        url: ROOT + "CountryLibrary/DeletePort",
        type: "POST",
        data: { 'portId': portId },
        async: false,
        dataType: "json",
        success: function (data) {
            isDeleted = data;
        }
    });
    return isDeleted;
}


