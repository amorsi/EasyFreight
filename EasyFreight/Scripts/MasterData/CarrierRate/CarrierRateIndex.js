(function ($) {
    $('.modal-with-form').magnificPopup({
        type: 'inline',
        preloader: false,
        focus: '#name',
        modal: true,

        // When elemened is focused, some mobile browsers in some cases zoom in
        // It looks not nice, so we disable it:
        callbacks: {
            beforeOpen: function () {
                if ($(window).width() < 700) {
                    this.st.focus = false;
                } else {
                    this.st.focus = '#name';
                }

            },
            open: function () {
                $('select').select2({
                    allowClear: true
                });
            }
        }
    });


    /*
	Modal Dismiss
	*/
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    /*
    Remove row Modal 
    */
    $('.remove-row').magnificPopup({
        type: 'inline',
        preloader: false,
        modal: true
    });

    /*
	Modal Confirm
	*/
    $(document).on('click', '.modal-confirm', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var idToDelete = $("#DelId").val()
        var isdeleted = Delete(idToDelete);
        if (isdeleted == "true") {
            new PNotify({
                title: 'Success!',
                text: 'Row Deleted.',
                type: 'success'
            });
            //Delete row from table
            var table = $('#datatable-default').DataTable();
            table.row('#' + idToDelete).remove().draw(false);
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

    /*
    Modal Search
    */
    $(document).on('click', '.modal-search', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        Search();
    });

    $(function () {
        GetTbData();
        $('body').on('click', '.remove-row', function () {
            var carrierRateId = $(this).closest('tr').attr("id");
            $("#DelId").val(carrierRateId);
            $.magnificPopup.open({
                items: {
                    src: '#dialog',
                    type: 'inline'
                },
                modal: true
            });
        });

        $('body').on('click', '.edit-row', function () {
            var carrierRateId = $(this).closest('tr').attr("id");
            window.location.href = ROOT + '/MasterData/CarrierRate/Add/' + carrierRateId;
        });

        $('#datatable-default tbody').on('click', '.contactList', function () {
            var carrierId = $(this).closest('tr').attr("carrierid");
            $.ajax({
                url: ROOT + "Carrier/GetCarrierContacts",
                type: "POST",
                data: { 'carrierId': carrierId },
                async: false,
                dataType: "html",
                success: function (data) {
                    $("#ModalContent").html(data);
                    $('#table table-striped mb-none').dataTable();

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

        //clear filter
        $(".filter0").click(function () {
            GetTbData();
            ClearForm();
        });

    });

}).apply(this, [jQuery]);


function Delete(carrierRateId) {
    var isDeleted = "true";
    $.ajax({
        url: ROOT + "CarrierRate/Delete",
        type: "POST",
        data: { 'id': carrierRateId },
        async: false,
        dataType: "json",
        success: function (data) {
            isDeleted = data;
        }
    });
    return isDeleted;
}

function Search() {
    GetTbData($("#searchform").serialize());
}

var carrierRatinJson;

function GetTbData(searchForm) {
    $.ajax({
        url: ROOT + "CarrierRateInquiry/GetTableJson",
        type: "POST",
        data: searchForm,
        async: false,
        success: function (data) {
            carrierRatinJson = data;
            datatableInit();
        }

    });
}


function datatableInit() {

    var actions = '';

    if ($("#screentype").val() == "ratesetup") {
        actions = ' <a href="javascript:void(0)"  title="View Contacts" ' +
                         'class="on-default contactList modal-with-move-anim"><i class="fa fa-phone"></i></a> ' +
                           ' <a href="javascript:void(0)")" title="Edit" style="padding-left:5px"' +
                         ' class="on-default edit-row"><i class="fa fa-pencil"></i></a> ' +
                         '   <a href="javascript:void(0)" title="Delete" style="padding-left:5px" ' +
                         ' class="on-default remove-row"><i class="fa fa-trash-o"></i></a>';
    }
    else {
        actions = '<a href="javascript:void(0)" title="View Contacts" ' +
                        ' class="on-default contactList modal-with-move-anim"><i class="fa fa-phone"></i></a>';
         
    }

    if ($.fn.dataTable.isDataTable('#datatable-default')) {
        table = $('#datatable-default').dataTable();
        table.fnClearTable();
        table.fnAddData(jQuery.parseJSON(carrierRatinJson).data);
        return;
    }

    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(carrierRatinJson).data,
        columns: [
            { data: "CarrierName" },
            { data: "ContainerTypeName" },
            { data: "FromPort", "width": "15%" },
            { data: "ToPort", "width": "15%" },
            { data: "FreightCost" },
            { data: "ThcCost" },
            { data: "ValidToDate", "sType": "date-uk" },
            { data: null, "orderable": false }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": actions
        }],
        order: [[0, "asc"]],
        buttons: ['copy', 'excel', 'pdf'],
        dom: 'Bfrtip',
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["CarrierRateId"]);
            $(nRow).attr('carrierId', data["CarrierId"]);
        }
    });

};

function ClearForm() {
    $("#searchform input:text").each(function () {
        $(this).val("");
    });

    $("#searchform select").val("").trigger("change");

}