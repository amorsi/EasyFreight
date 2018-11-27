$(function () {
    $("#ddlCountryCity").change(function () {
        
        var countryId = $('option:selected', this).attr('countryId');
        $("#CountryId").val(countryId);
    });

});