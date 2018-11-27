$(function () {



    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });


    $(document).on('click', '#SaveHBCostForm', function () {
        var isValid = CheckFormIsValid('opercostform');
        if (isValid) {
            SaveHouseBillCost();
            //GetHbList();
        }
    });

});





    function SaveHouseBillCost() {
        $.ajax({
            url: ROOT + "Operation/export/AddEditAccountHbCost",
            type: "POST",
            data: $("#opercostform").serialize(),
            async: false,
            dataType: "json",
            success: function (data) {
                if (data == "true") {
                    new PNotify({
                        title: 'Success!',
                        text: 'Row Saved.',
                        type: 'success'
                    });
                }
                else {
                    new PNotify({
                        title: 'Sorry!',
                        text: data,
                        type: 'error'
                    });
                }
            }
        });

    }
