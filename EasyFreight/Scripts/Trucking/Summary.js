(function ($) {
   

    $(function () {
        GetTruckingStatistics();

        $(document).on('click', '.filter', function (e){
        //$(".filter").click(function () { 
             GetTruckingStatistics();
        }); 
     });
 
}).apply(this, [jQuery]);

function GetTruckingStatistics() { 
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();
    $.ajax({
        url: ROOT + "Trucking/TruckingStatistics",
        type: "POST",
        data: { 'fromDate': fromDate, 'toDate': toDate },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#Widget").html(data);
        }
    });
}
 