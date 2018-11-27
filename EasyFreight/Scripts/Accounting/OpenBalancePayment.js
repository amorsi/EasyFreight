$(function () {

    GetTbData();


    $('#datatable-default tbody').on('click', '.payment', function () {
        var accid = $(this).closest('tr').attr("accid");
        var cid = $(this).closest('tr').attr("cid");
        window.location.href = ROOT + "CashManagement/CashOpenBalance?accid=" + accid + "&cid=" + cid;
    });

});

var OpenBalanceJson;


function GetTbData() {

    $.ajax({
        url: ROOT + "Accounting/GetOpenBalanceTableJson",
        type: "POST",
        //data: searchForm,
        async: false,
        success: function (data) {
            OpenBalanceJson = data;
            if ((jQuery.parseJSON(OpenBalanceJson).data) != "")
                datatableInit();
            else {
                table = $('#datatable-default').dataTable();
                table.fnClearTable();
            }
        }

    });
}

function datatableInit() {
    if ($.fn.dataTable.isDataTable('#datatable-default')) {
        table = $('#datatable-default').dataTable();
        table.fnClearTable();
        table.fnAddData(jQuery.parseJSON(OpenBalanceJson).data);
        return;
    }
    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(OpenBalanceJson).data,
        columns: [
             //{
             //    "render": function (data, type, JsonResultRow, meta) {
             //        return '<span>' + JsonResultRow.AccountNameEn + '</span> <br />' + '<span>' + JsonResultRow.AccountNameAr + ' </span>';
             //    },
             //},

            { "data": "AccountNameAr" },
            { "data": "DebitAmount" }, 
            { "data": "CreditAmount" },
            {
                "render": function (data, type, JsonResultRow, meta) {
                    if (JsonResultRow.Amount <0)
                        return '<span class="text-danger">(' + JsonResultRow.Amount + ')</span>';
                    else
                        return '<span  >' + JsonResultRow.Amount + '</span>';
                 },
             },
            //{ "data": "Amount" },
            { "data": "CurrencySign" }, 
            { data: null, "orderable": false, "width": "14%" }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<div class="btn-group"> ' +
                '<button type="button" class="mb-xs mt-xs mr-xs btn btn-default dropdown-toggle" data-toggle="dropdown">Actions <span class="caret"></span></button> ' +
                '<ul class="dropdown-menu" role="menu">' +
                '<li><a class="details modal-details" href="javascript:void(0)">'
                //+ '<i class="fa fa-info-circle"></i>  View Invoice</a></li> ' 
                + '<li><a class="payment" href="javascript:void(0)"><i class="fa fa-usd"></i> Add Payment</a></li>'
               // + '<li><a class="deleteInv" href="javascript:void(0)"><i class="fa fa-trash"></i> Delete Invoice</a></li>'
                + '</ul></div>'
        }],
        order: [[0, "asc"]],
        buttons: [

           'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('accid', data["AccountId"]);
            $(nRow).attr('cid', data["CurrencyId"]);
            $(nRow).attr('credit', data["IsCredit"]);
            //$(nRow).find(".deleteInv").hide();
            if (data["Ammount"] == "0" ) {
            //    //If paid hide
                $(nRow).find(".payment").hide();
             } 
        }
    });

};
