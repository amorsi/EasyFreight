$(function () {

    GetTbData();

    $("#searchbtn").on('click', function () {
        BindTotalsTb();
        GetTbData();
    });
});

var EmployeeJson;

function GetTbData() {
    var accId = $("#accountId").val();
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();

    $.ajax({
        url: ROOT + "AccountingRpt/GetAccTransJson",
        type: "POST",
        data: { "accId": accId, "fromDate": fromDate, "toDate": toDate },
        async: false,
        success: function (data) {
            EmployeeJson = data;

            if ((jQuery.parseJSON(EmployeeJson).data) != "")
                datatableInit();
            else {
                table = $('#acctrans').dataTable();
                table.fnClearTable();  
            }
        }
    });

    //table = $('#acctrans').dataTable();
    //table.rows.add([
    //        ["", "", "Total:", $('.totaltbl tr:eq(1) td:eq(1)').html(), $('.totaltbl tr:eq(2) td:eq(1)').html(), "EGP"],
    //        ["", "", "", ($('.totaltbl tr:eq(1) td:eq(2)').html() ? $('.totaltbl tr:eq(1) td:eq(2)').html() : 0), $('.totaltbl tr:eq(2) td:eq(2)').html() ? $('.totaltbl tr:eq(2) td:eq(2)').html() : 0, "USD"],
    //        ["", "", "", ($('.totaltbl tr:eq(1) td:eq(3)').html() ? $('.totaltbl tr:eq(1) td:eq(3)').html() : 0), $('.totaltbl tr:eq(2) td:eq(3)').html() ? $('.totaltbl tr:eq(2) td:eq(3)').html() : 0, "EUR"],
    //        ["", "", "", ($('.totaltbl tr:eq(1) td:eq(4)').html() ? $('.totaltbl tr:eq(1) td:eq(4)').html() : 0), $('.totaltbl tr:eq(2) td:eq(4)').html() ? $('.totaltbl tr:eq(2) td:eq(4)').html() : 0, "GBP"]
    //]).draw();

}

function datatableInit() {
    if ($.fn.dataTable.isDataTable('#acctrans')) {
        table = $('#acctrans').dataTable();
        table.fnClearTable();
        table.fnAddData(jQuery.parseJSON(EmployeeJson).data);
        return;
    }

    $('#acctrans').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(EmployeeJson).data,
        columns: [
             { data: "CreateDate","sType": "date-uk" , "width": "10%" },
            { data: "CreateBy", "width": "10%" },
            { data: "TransactionName", "width": "25%" },
            { data: "ReceiptNotes", "width": "25%" },
            { data: "DebitAmount", "width": "10%"},
            { data: "CreditAmount", "width": "10%" },
            { data: "CurrencySign", "width": "10%" }
          
        ],
        order: [[0, "asc"]],
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["EmpId"]); 
            $('td', nRow).eq(5).addClass('text-danger');
            $('td', nRow).eq(4).addClass('text-success');
            
        } ,
        dom: 'Blfrtip',
        buttons: [
               {
                   extend: 'excelHtml5',
                   exportOptions: {  columns: [0, ':visible']  }
               },
              {
                  extend: 'pdfHtml5',
                  exportOptions: {  columns: [0, ':visible'] }  ,
                  title: function () {
                      var title = $("#rpTitle").text();

                      return   title ;
                  }
              },
               {
                   extend: 'print',
                   footer: true,
                   exportOptions: {  columns: [0, ':visible'], },
                   text: '<i class="fa fa-print"></i> Print'  ,
                   title: function () { var title = $("#rpTitle").text();  return   title ;  } ,
                   customize: function (win) {
                       $(win.document.body).css('font-size', '10pt')
                       $(win.document.body).find('table')
                           .addClass('compact')
                           .css('font-size', 'inherit');
                   } ,
                   message: function () {
                       var d = new Date();
                       var head = "<div style='height: 180px; font-family: Arial; font-weight: bold;'>";
                       head += "<div style='float: left; height: 60px;'> ";
                       head += "<span style='font-size: 12pt; color: black'><br />" + d.toDateString() + " \n </span></div>";
                       // head += "<div style='float: right; height: 60px;'> <img src='/images/EnglishLogo.png'/></div>";
                       head += "<div style='margin-left: auto; margin-right: auto;text-align:center'>" + $("#rpTitle").text() + "</div>";
                       head += "<div style='clear: both' />";
                       head += "<div style='height: 2px; width: 100%; background-color: Black;'></div></div>";
                       return head;
                   }
               },
              'colvis'

        ]
        //,footerCallback: function (row, data, start, end, display) {
        //    var api = this.api(), data;
        //    $(api.column(3).footer()).html($('.totaltbl tr:eq(1) td:eq(1)').html());
        //    $(api.column(4).footer()).html($('.totaltbl tr:eq(2) td:eq(1)').html());
        //    $(api.column(5).footer()).html("EGP");
        //    // $(api.table().footer()).append("<tr><td  colspan='3'>Total</td><td>" + $('.totaltbl tr:eq(1) td:eq(1)').html() + "</td><td>" + $('.totaltbl tr:eq(2) td:eq(1)').html() + "</td><td>EGP</td></tr>");
        //    $(api.table().footer()).append("<th><td  colspan='3'></td><td>" + ($('.totaltbl tr:eq(1) td:eq(2)').html() ? $('.totaltbl tr:eq(1) td:eq(2)').html() : 0) + "</td><td>" + ($('.totaltbl tr:eq(2) td:eq(2)').html() ? $('.totaltbl tr:eq(2) td:eq(2)').html() : 0) + "</td><td>USD</td></th>");
        //    $(api.table().footer()).append("<tr><td  colspan='3'></td><td>" + ($('.totaltbl tr:eq(1) td:eq(3)').html() ? $('.totaltbl tr:eq(1) td:eq(3)').html() : 0) + "</td><td>" + ($('.totaltbl tr:eq(2) td:eq(3)').html() ? $('.totaltbl tr:eq(2) td:eq(3)').html() : 0) + "</td><td>EUR</td></tr>");
        //    $(api.table().footer()).append("<tr><td  colspan='3'></td><td>" + ($('.totaltbl tr:eq(1) td:eq(4)').html() ? $('.totaltbl tr:eq(1) td:eq(4)').html() : 0) + "</td><td>" + ($('.totaltbl tr:eq(2) td:eq(4)').html() ? $('.totaltbl tr:eq(2) td:eq(4)').html() : 0) + "</td><td>GBP</td></tr>");
        //}
    });
 };

function BindTotalsTb() {
    var accId = $("#accountId").val();
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();

    $.ajax({
        url: ROOT + "AccountingRpt/GetAccTotalPartial",
        type: "POST",
        data: { "accountId": accId, "fromDate": fromDate, "toDate": toDate },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#totalsection").html(data);
        }
    });
}