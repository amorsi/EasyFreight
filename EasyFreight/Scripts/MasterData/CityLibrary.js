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
        var cityId = $("#DelCityId").val()
        var isdeleted = DeleteCity( $("#DelCityId").val());
        if (isdeleted == "true") {
            new PNotify({
                title: 'Success!',
                text: 'Row Deleted.',
                type: 'success'
            });
            //Delete row from table
            var table = $('#datatable-default').DataTable();
            table.row('#' + cityId).remove().draw(false);
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
                SaveCity();
            }

        });

        $("#ddlCountry").change(function () {
            var countryId = $('option:selected', this).attr('countryId');
            $("#CountryId").val(countryId);

        });

        $("#ResetForm").click(function () {
            ClearForm();
            $('#AddForm').fadeOut();
            $('#SaveBtnDiv').fadeOut();
        });

        $("#addToTable").click(function () { 
            $("#CityId").val("0");
            $('#AddForm').fadeIn();
            $('#SaveBtnDiv').fadeIn();
        });



        $('body').on('click', '.edit-row', function () {
            $('#AddForm').fadeIn();
            $('#SaveBtnDiv').fadeIn();
            var counId = $(this).attr("countryId");
            var cityId = $(this).attr("cityId");
            

            $("#CountryId").val(counId);
            $("#CityId").val(cityId);


            $("#ddlCountry").val(counId).trigger("change");
           
            $("#CityNameEn").val($(this).closest('tr').children('td#NameEn').text());
            $("#CityNameAr").val($(this).closest('tr').children('td#NameAr').text());
        });

        $('body').on('click', '.remove-row', function () {
            $("#DelCityId").val($(this).attr("cityId"));
        });

    });



}).apply(this, [jQuery]);


function SaveCity() {
    $.ajax({
        url: ROOT + "CountryLibrary/AddEditCity",
        type: "POST",
        data: $("#Form1").serialize(),
        async: false,
        success: function (data) {
            
            if (data.indexOf("false") != -1 )
            {
                new PNotify({
                    title: 'Sorry!',
                    text: data,
                    type: 'error'
                });
                return;
            }

            new PNotify({
                title: 'Success!',
                text: 'Row Saved.',
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
    if ($("#CityId").val() != "0") //edit mode
    {
        $("#ddlCountry").val("").trigger("change"); 
        $('#AddForm').fadeOut();
        $('#SaveBtnDiv').fadeOut();
        $("#CountryId").val("0");
    } 
    $("input:text").each(function () {
        $(this).val("");
    });

    $("#CityId").val("0");


}

function DeleteCity(cityId) {
    var isDeleted = "true";
    $.ajax({
        url: ROOT + "CountryLibrary/DeleteCity",
        type: "POST",
        data: { 'cityId': cityId },
        async: false,
        dataType: "json",
        success: function (data) {
            isDeleted = data;
        }
    });
    return isDeleted;
}


