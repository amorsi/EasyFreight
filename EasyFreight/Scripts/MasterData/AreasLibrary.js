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
        var areaId = $("#DelAreaId").val()
        var isdeleted = DeleteArea($("#DelAreaId").val());
        if (isdeleted == "true") {
            new PNotify({
                title: 'Success!',
                text: 'Row Deleted.',
                type: 'success'
            });
            //Delete row from table
            var table = $('#datatable-default').DataTable();
            table.row('#' + areaId).remove().draw(false);
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

        $('#datatable-default').dataTable({
            buttons: ['copy', 'excel', 'pdf'],
            dom: 'Bfrtip'
        });

    };

    $(function () {
        datatableInit();

        $("#SaveForm").click(function () {
            var isValid = CheckFormIsValid('Form1');
            if (isValid) {
                SaveArea();
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
            $("#AreaId").val("0");
            $('#AddForm').fadeIn();
            $('#SaveBtnDiv').fadeIn();
        });



        $('body').on('click', '.edit-row', function () {
            $('#AddForm').fadeIn();
            $('#SaveBtnDiv').fadeIn();
            var counId = $(this).attr("countryId");
            var cityId = $(this).attr("cityId");
            var areaId = $(this).attr("areaId");

            $("#CountryId").val(counId);
            $("#AreaId").val(areaId);


            $("#ddlCountryCity").val(cityId).trigger("change");
            $("#AreaNameEn").val($(this).closest('tr').children('td#NameEn').text());
            $("#AreaNameAr").val($(this).closest('tr').children('td#NameAr').text());
        });

        $('body').on('click', '.remove-row', function () {
            $("#DelAreaId").val($(this).attr("areaId"));
        });

    });



}).apply(this, [jQuery]);


function SaveArea() {
    $.ajax({
        url: ROOT + "CountryLibrary/AddEditArea",
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
    if ($("#AreaId").val() != "0") //edit mode
    {
        $("#ddlCountryCity").val("").trigger("change");
        $('#AddForm').fadeOut();
        $('#SaveBtnDiv').fadeOut();
        $("#CountryId").val("0");
    }

    $("input:text").each(function () {
        $(this).val("");
    });

    $("#AreaId").val("0");

}

function DeleteArea(areaId) {
    var isDeleted = "true";
    $.ajax({
        url: ROOT + "CountryLibrary/DeleteArea",
        type: "POST",
        data: { 'areaId': areaId },
        async: false,
        dataType: "json",
        success: function (data) {
            isDeleted = data;
        }
    });
    return isDeleted;
}


