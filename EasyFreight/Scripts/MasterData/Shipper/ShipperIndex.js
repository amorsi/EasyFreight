﻿(function ($) {

    'use strict';
    /*
   Contact and Vessel Modal 
   */
    $('.modal-with-move-anim').magnificPopup({
        type: 'inline',

        fixedContentPos: false,
        fixedBgPos: true,

        overflowY: 'auto',

        closeBtnInside: true,
        preloader: false,

        midClick: true,
        removalDelay: 300,
        mainClass: 'my-mfp-slide-bottom',
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
                text: 'Can not delete this row.',
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

        $('body').on('click', '.contactList', function () {
            var shipperId = $(this).attr("shipperId");
            $.ajax({
                url: ROOT + "Shipper/GetShipperContacts",
                type: "POST",
                data: { 'shipperId': shipperId },
                async: false,
                dataType: "html",
                success: function (data) {
                    $("#ModalContent").html(data);
                    $('#table table-striped mb-none').dataTable();

                }
            });
        });

        $('body').on('click', '.remove-row', function () {
            $("#DelId").val($(this).attr("shipperId"));
        });
    });

}).apply(this, [jQuery]);


function Delete(shipperId) {
    var isDeleted = "true";
    $.ajax({
        url: ROOT + "Shipper/Delete",
        type: "POST",
        data: { 'id': shipperId },
        async: false,
        dataType: "json",
        success: function (data) {
            isDeleted = data;
        }
    });
    return isDeleted;
}
