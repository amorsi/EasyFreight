(function ($) {

    /*
   Modal Dismiss
   */
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    //Search form submit
    $(document).on('click', '.search-modal-confirm', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var form = $("#searchform").serialize();
        GetTbData(form);       
    });

    $(function () {
        GetTbData();

        $(".filter1").click(function () {
            $(".filter1").each(function () {
                $(this).removeClass("clicked-icon");
                $(this).addClass("default-icon");
            });

            $(this).removeClass("default-icon");
            $(this).addClass("clicked-icon");
            var form = $("#searchform").serialize();
            GetTbData(form,$(this).attr("carrierType"));
        });

        //clear filter
        $(".filter0").click(function () {
            $(".filter1").each(function () {
                $(this).removeClass("clicked-icon");
                $(this).addClass("default-icon");
            });
            $("#AllType").addClass("clicked-icon");
            GetTbData();
            ClearForm();
        });


        $('#datatable-default tbody').on('click', '.details', function () {
            var itemId = $(this).closest('tr').attr("id");
            $.ajax({
                url: ROOT + "Quotation/GetQuotationDetails",
                type: "POST",
                data: { 'id': itemId },
                async: false,
                dataType: "html",
                success: function (data) {
                    $("#ModalContent").html(data);

                    $.magnificPopup.open({
                        items: {
                            src: '#modalAnim',
                            type: 'inline'
                        },
                        modal: true
                    });
                }
            });
        });

        $('#datatable-default tbody').on('click', '.shippingdeclink', function () {
            var itemId = $(this).closest('tr').attr("id");
            $.ajax({
                url: ROOT + "Quotation/GetShippingDeclaration",
                type: "POST",
                data: { 'id': itemId },
                async: false,
                dataType: "html",
                success: function (data) {
                    $("#shippingdeclcontent").html(data);

                    $.magnificPopup.open({
                        items: {
                            src: '#shippingdeclmodal',
                            type: 'inline'
                        },
                        modal: true
                    });
                }
            });
        });

        $('#datatable-default tbody').on('click', '.process', function () {
            var itemId = $(this).closest('tr').attr("id");
            var orderFromText = $(this).closest('tr').attr("orderfromtext");
            window.location.href = ROOT + "Operation/" + orderFromText + "/Add?quoteId=" + itemId;
        });

        $('#datatable-default tbody').on('click', '.edit', function () {
            var statusId = $(this).closest('tr').attr("statusId");
            if (statusId != "1") {
                new PNotify({
                    title: 'Sorry!',
                    text: 'Can not edit this quotation. Only new quotations can be edited.',
                    type: 'error'
                });
                return;
            }
            var itemId = $(this).closest('tr').attr("id");
            var orderFrom = $(this).closest('tr').attr("orderfrom");
            window.location.href = ROOT + "Quotation/Add?id=" + itemId + "&orderFrom=" + orderFrom;
        });

        $('#datatable-default tbody').on('click', '.modal-delete', function () {
            var itemId = $(this).closest('tr').attr("id");
            var ststusID = $(this).closest('tr').attr("statusId");
            if (ststusID != "1") {
                new PNotify({
                    title: 'Sorry!',
                    text: 'Can not delete this quotation. Only new quotations can be deleted.',
                    type: 'error'
                });
                return;
            }
            $("#deletedId").val(itemId);
            $.magnificPopup.open({
                items: {
                    src: ststusID > 2 ? '#modelUnavailable' : '#modelDelete',
                    type: 'inline'
                },
                modal: true
            });
        });

        $("#advsearchlink").click(function () {
            if (!$('#searchform').length) {
                GetSearchForm();
            }
            $.magnificPopup.open({
                items: {
                    src: '#modalForm',
                    type: 'inline'
                },
                modal: true
            });
        });
    });

    /*
 Modal Confirm
 */
    $(document).on('click', '.modal-confirm', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var idToCancel = $("#deletedId").val()
        var iscanceled = Cancel(idToCancel);
        if (iscanceled == "true") {
            new PNotify({
                title: 'Success!',
                text: 'Quotation has been canceled.',
                type: 'success'
            });
            //Delete row from table
            //var table = $('#datatable-default').DataTable();
            // table.row('#' + idToDelete).remove().draw(false);

            //$('#' + portId).remove();
        }
        else {
            new PNotify({
                title: 'Sorry!',
                text: iscanceled,
                type: 'error'
            });
        }
    });

}).apply(this, [jQuery]);

function Cancel(Id) {
    var isCanceled = "true";
    var orderFrom = $("#OrderFrom").val();

    $.ajax({
        url: ROOT + "Quotation/Cancel",
        type: "POST",
        data: { 'quotationId': Id, 'orderFrom': orderFrom },
        async: false,
        dataType: "json",
        success: function (data) {
            isCanceled = data;
            GetTbData();
        }
    });
    return isCanceled;
}

var quotationsJson;

function GetTbData(searchForm, carrierType) {
    var orderFrom = $("#OrderFrom").val();
    $.ajax({
        url: ROOT + "Quotation/GetTableJson",
        type: "POST",
        data: searchForm + "&CarrierType=" + carrierType + "&OrderFrom=" + orderFrom,
        async: false,
        success: function (data) {
            quotationsJson = data;

            if ((jQuery.parseJSON(quotationsJson).data) != "")
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
        table.fnAddData(jQuery.parseJSON(quotationsJson).data);
    }
    else {
        $('#datatable-default').dataTable({
            processing: true,
            // destroy: true,
            data: jQuery.parseJSON(quotationsJson).data,
            "columns": [
                { "data": "CarrierTypeImg", "orderable": false, "width": "2%" },
                { "data": "CreateDate", "sType": "date-uk" },
                { "data": "QuoteCode" },
                { "data": "ShipperName" },
                { "data": "ConsigneeName" },
                { "data": "CarrierName" },
                { "data": "FromPort" },
                { "data": "ToPort" },
                { "data": "DateOfDeparture", "sType": "date-uk" },
                { "data": "StatusName" },
                 { "data": null, "orderable": false, "width": "14%" }
            ],
            "columnDefs": [{
                "targets": -1,
                "data": null,
                "defaultContent": '<div class="btn-group"> ' +
                    '<button type="button" class="mb-xs mt-xs mr-xs btn btn-default dropdown-toggle" data-toggle="dropdown">Actions <span class="caret"></span></button> ' +
                    '<ul class="dropdown-menu" role="menu">' +
                    '<li><a class="details modal-details" href="javascript:void(0)"><i class="fa fa-info-circle"></i>  Details</a></li> ' +
                     '<li><a class="shippingdeclink" href="javascript:void(0)" title="Shipping Declaration"><i class="fa fa-info-circle"></i>  Shipping Decl.</a></li> ' +
                    '<li><a class="process" href="javascript:void(0)"><i class="fa fa-cog"></i> Process</a></li>' +
                    '<li class="divider"></li><li><a class="edit" href="javascript:void(0)"><i class="fa fa-pencil"></i>  Edit</a></li>'+
                    //'<li><a class="cancel modal-delete" href="javascript:void(0)"><i class="fa fa-trash-o"></i>  Cancel</a></li>'+
                    '</ul></div>'
            }],
            "order": [[1, "desc"]],
            buttons: ['copy', 'excel', 'pdf'],
            dom: 'Bfrtip',
            "fnCreatedRow": function (nRow, data, iDataIndex) {
                $(nRow).attr('id', data["QuoteId"]);
                $(nRow).attr('orderfromtext', data["OrderFromText"]);
                $(nRow).attr('orderfrom', data["OrderFrom"]);
                $(nRow).attr('statusId', data["StatusId"]);
            }
        });
    }
};


function ClearForm() {
    $("#searchform input:text").each(function () {
        $(this).val("");
    });

    $("#searchform select").val("").trigger("change");

}

function GetSearchForm() {
    $.ajax({
        url: ROOT + "Quotation/AdvSearch",
        type: "POST",
        async: false,
        dataType: "html",
        success: function (data) {
            $("#searchdiv").html(data);
            $("select").select2({
                allowClear: true
            });
            $(".date").datepicker();

        }
    });
}