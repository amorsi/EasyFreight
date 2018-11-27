$(function () {

    $(document).on('click', '.currencyitem', function () {
        var currId = $(this).attr("currencyId");
        var currSign = $(this).text();
        $(this).parent().closest('div').find('.currSelected').text(currSign);
        $(this).parent().closest('div').find('.currIdHidden').val(currId);
    });

});