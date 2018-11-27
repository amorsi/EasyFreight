$(function () {
    $("#EmpId").change(function () {
        var empId = $(this).val();
        GetSecView(empId);
    });

    $("#SaveForm").on('click', function () {
        SaveSecForm();
    });

    $(document).on('change', '#issuperusercheck', function () {
        if ($(this).is(':checked')) {
            $("#IsSuperUser").val("true");
        }
        else {
            $("#IsSuperUser").val("false");
        }
    })

});

function GetSecView(empId) {
    
    $.ajax({
        url: ROOT + "Security/GetSecRights",
        type: "POST",
        data: { 'empId': empId },
        async: false,
        dataType: "html",
        success: function (data) {
            $("#secview").html(data);
            $("select").select2();
            $('.panel')
			.on('click', '.panel-actions a.fa-caret-up', function (e) {
			    e.preventDefault();

			    var $this,
					$panel;

			    $this = $(this);
			    $panel = $this.closest('.panel');

			    $this
					.removeClass('fa-caret-up')
					.addClass('fa-caret-down');

			    $panel.find('.panel-body, .panel-footer').slideDown(200);
			})
			.on('click', '.panel-actions a.fa-caret-down', function (e) {
			    e.preventDefault();
			    var $this,
					$panel;

			    $this = $(this);
			    $panel = $this.closest('.panel');

			    $this
					.removeClass('fa-caret-down')
					.addClass('fa-caret-up');

			    $panel.find('.panel-body, .panel-footer').slideUp(200);
			});

            $('.panel').find('.panel-actions a.fa-caret-down').trigger('click');
        }
    });
}

function SaveSecForm() {

    $.ajax({
        url: ROOT + "Security/AddEditSecRights",
        type: "POST",
        data: $("#form1").serialize(),
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
