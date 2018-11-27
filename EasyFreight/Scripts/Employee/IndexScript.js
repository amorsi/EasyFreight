$(function () {
    
    $(document).on('click', '.modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
    });

    GetTbData();

    $('#datatable-default tbody').on('click', '.edit', function () {
        var itemId = $(this).closest('tr').attr("id");
        window.location.href = ROOT + "HR/Employee/Add/" + itemId;
    });

    $('#datatable-default tbody').on('click', '.remove-row', function () {
        $("#DelId").val($(this).closest('tr').attr("id"));
        $.magnificPopup.open({
            items: {
                src: '#dialog',
                type: 'inline'
            },
            modal: true
        });
    });

    $(document).on('click', '#dialogConfirm', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        var idToDelete = $("#DelId").val()
        var isdeleted = DeleteEmp(idToDelete);
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

});

var EmployeeJson;

function GetTbData(searchForm) {
    $.ajax({
        url: ROOT + "Employee/GetTableJson",
        type: "POST",
        data: searchForm,
        async: false,
        success: function (data) {
            EmployeeJson = data;
            datatableInit();
        }

    });
}


function datatableInit() {

    if ($.fn.dataTable.isDataTable('#datatable-default')) {
        table = $('#datatable-default').dataTable();
        table.fnClearTable();
        table.fnAddData(jQuery.parseJSON(EmployeeJson).data);
        return;
    }

    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(EmployeeJson).data,
        columns: [
            { data: "EmpCode" },
            { data: "EmpNameEn", "width": "15%" },
            { data: "EmpNameAr", "width": "15%" },
            { data: "DepNameEn" },
            { data: null, "orderable": false }
        ],
        columnDefs: [{
            "targets": -1,
            "data": null,
            "defaultContent": '<a class="edit" title="Edit" style="padding-left:5px" href="javascript:void(0)"><i class="fa fa-pencil"></i></a>' +
                '<a class="remove-row" style="padding-left:10px" title="Delete" href="#"><i class="fa fa-trash-o"></i></a>'
        }],
        order: [[0, "asc"]]
        ,
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["EmpId"]);
        }
    });

};

function DeleteEmp(empId)
{
    var isDeleted = "true";
    $.ajax({
        url: ROOT + "Employee/DeleteEmployee",
        type: "POST",
        data: { 'id': empId },
        async: false,
        dataType: "json",
        success: function (data) {
            isDeleted = data;
        }
    });
    return isDeleted;
}