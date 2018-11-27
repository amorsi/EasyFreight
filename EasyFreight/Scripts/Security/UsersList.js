(function ($) {

    // Modal Dismiss
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });


    $('#datatable-default tbody').on('click', '.deleteInv', function () {
        var itemId = $(this).closest('tr').attr("id");
        $("#ivnDele").val(itemId);

        $.magnificPopup.open({
            items: {
                src: '#modelReset',
                type: 'inline'
            },
            modal: true
        });
    });


}).apply(this, [jQuery]);