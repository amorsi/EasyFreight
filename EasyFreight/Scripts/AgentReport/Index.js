(function ($) {

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
 
    var datatableInit = function () {

        $('#datatable-default').dataTable();

    };

    $(function () {
       // datatableInit();
        GetTbData(0);

        $('body').on('click', '.contactList', function () {
            var agentId = $(this).attr("agentId");
            $.ajax({
                url: ROOT + "Agent/GetAgentContacts",
                type: "POST",
                data: { 'agentId': agentId },
                async: false,
                dataType: "html",
                success: function (data) {
                    $("#ModalContent").html(data);
                    $('#table table-striped mb-none').dataTable();

                }
            });
        });


        $('body').on('click', '.contactList', function () {
            var agentId = $(this).attr("agentId");
            $.ajax({
                url: ROOT + "Agent/GetAgentContacts",
                type: "POST",
                data: { 'agentId': agentId },
                async: false,
                dataType: "html",
                success: function (data) {
                    $("#ModalContent").html(data);
                    $('#table table-striped mb-none').dataTable();

                }
            });
        });

          
        $('body').on('click', '#ClearSearch', function () {
              
            GetTbData(0);
        });

        $("#ddlCountry").change(function () {
            var countryId = $(this).val();
            GetTbData(countryId);
        });

    });

}).apply(this, [jQuery]);

var selectedAgents;


function GetTbData(countryid) {
     
    $.ajax({
        url: ROOT + "AgentReport/GetTableJson",
        type: "POST",
        data:  {'countryId':countryid},
        async: false,
        success: function (data) {
            selectedAgents = data;
            datatableInit();
            $("#print").attr("href", ROOT + "AgentReport/PrintAgents?countryId=" + countryid );
        }

    });
}


function datatableInit() {

    if ($.fn.dataTable.isDataTable('#datatable-default')) {
        table = $('#datatable-default').dataTable();
        table.fnClearTable();
        if(selectedAgents!=null)
        table.fnAddData(jQuery.parseJSON(selectedAgents).data);
        return;
    }

    $('#datatable-default').dataTable({
        //destroy: true,
        processing: true,
        data: jQuery.parseJSON(selectedAgents).data,
        columns: [  
            { data: "AgentNameEn", "defaultContent": 'No data found' },
            { data: "AgentAddressEn", "defaultContent": 'No data found' },
            { data: "PhoneNumber", "defaultContent": 'No data found' },
            { data: "FaxNumber", "defaultContent": 'No data found' },
            { data: "Email", "defaultContent": 'No data found' },
            { data: "Country", "defaultContent": 'No data found' },
            
        ], 
        order: [[1, "desc"]]
        ,
        fnCreatedRow: function (nRow, data, iDataIndex) {
            $(nRow).attr('id', data["AgentId"]); 
        }
    });

};

 