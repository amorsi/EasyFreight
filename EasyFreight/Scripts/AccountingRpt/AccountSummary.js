$(function () {

    GetTbData();

    $("#searchbtn").on('click', function () {
        BindTotalsTb();
        GetTbData();
    });

});

var AccountSummaryJson;

function GetTbData() {
    var parentAccId = $("#parentAccountId").val();
    var isCreditAccount = $("#IsCreditAccount").val();
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();
    var bId = $("#bId").val();

    $.ajax({
        url: ROOT + "AccountingRpt/GetAccSummaryJson",
        type: "POST",
        data: { "parentAccId": parentAccId, "isCreditAccount": isCreditAccount, "fromDate": fromDate, "toDate": toDate, "bId": bId },
        async: false,
        success: function (data) {
            AccountSummaryJson = data;
            if ((jQuery.parseJSON(AccountSummaryJson).data) != "")
                datatableInit();
            else {
                table = $('#accountsummarytb').dataTable();
                table.fnClearTable();
            }
        }

    });
}

function datatableInit() {

    if ($.fn.dataTable.isDataTable('#accountsummarytb')) {
        table = $('#accountsummarytb').dataTable();
        table.fnClearTable();
        table.fnAddData(jQuery.parseJSON(AccountSummaryJson).data);
        return;
    }

    $('#accountsummarytb').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(AccountSummaryJson).data,
        columns: [
            { data: "AccountId" },
            {
                data: "AccountNameEn", "width": "30%",
                "render": function (data, type, full, meta) {
                    return '<a href="AccTransByAccId?accId=' + full["AccountId"] + '">' + data + '</a>';
                }
            },
             { data: "EGP" },
             { data: "USD" },
               { data: "EUR" },
               { data: "GBP" },


        ],
        order: [[2, "desc"]]
        ,
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["AccountId"]);
        },
        dom: 'Blfrtip',
    buttons: [
           {
               extend: 'excelHtml5',
               exportOptions: {
                   columns: [0, ':visible']
               }
           },
          {
              extend: 'pdfHtml5',
              exportOptions: {
                  columns: [0, ':visible']
              },
              title: function () {
                  var title = $("#rpTitle").text();
                  return title;
              }
          },
           {
               extend: 'print',
               footer: true,
               exportOptions: {
                   columns: [0, ':visible'],
               },
               text: '<i class="fa fa-print"></i> Print'  ,
               title: function () {
                   var title = $("#rpTitle").text();
                        
                   return   + title+'</h4>';
               }
           },
          'colvis'

    ],
    footerCallback: function (row, data, start, end, display) {
        var api = this.api(), data;
        var colNumber = [2,3,4,5];

        var intVal = function (i) {
            return typeof i === 'string' ?
                   // i.replace(/[, EGP]|(\.\d{3})/g, "") * 1 :
                 i.replace(/[\$,]/g, '')*1:
                    typeof i === 'number' ?
                i : 0;
        };
        for (i = 0; i < colNumber.length; i++) {
            var colNo = colNumber[i];
            var total2 = api
                    .column(colNo )
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0);
            $(api.column(colNo).footer()).html(parseFloat(total2).toFixed(2) + (colNo == 2 ? ' EGP' : (colNo == 3 ? ' USD' : (colNo == 4 ? ' EUR' : ' GBP'))));
        }
    }
        //,
        //dom: 'Bfrtip',
        //buttons: [
        //    {
        //        extend: 'copyHtml5',
        //        exportOptions: { orthogonal: 'export' }
        //    },
        //    {
        //        extend: 'excelHtml5',
        //        exportOptions: { orthogonal: 'export' }
        //    },
        //    {
        //        extend: 'pdfHtml5',
        //        exportOptions: { orthogonal: 'export' }
        //    }
        //]
    });

};

function BindTotalsTb() {
    var parentAccId = $("#parentAccountId").val();
    var isCreditAccount = $("#IsCreditAccount").val();
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();

    $.ajax({
        url: ROOT + "AccountingRpt/GetAccSummaryTotalPartial",
        type: "POST",
        data: { "parentAccId": parentAccId, "isCreditAccount": isCreditAccount, "fromDate": fromDate, "toDate": toDate },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#totalsection").html(data);
        }
    });
}