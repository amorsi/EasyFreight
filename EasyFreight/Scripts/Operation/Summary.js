(function ($) {
   

    $(function () {
        GetOperationStatistics();

        $(".filter").click(function () { 
             GetOperationStatistics();
        }); 
     });
 
}).apply(this, [jQuery]);

function GetOperationStatistics() {
    var orderfrom = $("#orderFrom").val();
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();
    $.ajax({
        url: ROOT + "Operation/Export/OperationStatistics",
        type: "POST",
        data: { 'orderfrom': orderfrom, 'fromDate': fromDate, 'toDate': toDate },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#Widget").html(data);
        }
    });
}
//orderfrom, DateTime fromDate, DateTime toDate