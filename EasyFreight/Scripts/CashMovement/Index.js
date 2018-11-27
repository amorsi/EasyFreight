(function ($) {
 

    var datatableInit = function () {

        $('#datatable-default').dataTable({
            bPaginate: false,
            bInfo: false,
            searching: false, 
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
                      }
                      ,
                      title: function () {
                          var d = new Date();
                          var month = d.getMonth() + 1;
                          var day = d.getDate();
                          var time = d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
                          var output =
                             (('' + day).length < 2 ? '0' : '') + day
                              + '/' +
                              (('' + month).length < 2 ? '0' : '') + month
                              + '/' +
                               d.getFullYear();
                          return "Cash & Bank Balance in " + output + " - " + time;
                      }
                  },
                   {
                       extend: 'print',
                       exportOptions: {
                           columns: [0, ':visible'],
                       }
                       ,
                       title: function () {
                           var d = new Date();
                           var month = d.getMonth() + 1;
                           var day = d.getDate();
                           var time = d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
                           var output =
                              (('' + day).length < 2 ? '0' : '') + day
                               + '/' +
                               (('' + month).length < 2 ? '0' : '') + month
                               + '/' +
                                d.getFullYear();
                           return "Cash & Bank Balance in " + output +" - " +time;
                       }
                   },
                  'colvis'

            ] 
        });

    };

    $(function () {
        datatableInit(); 
    });

}).apply(this, [jQuery]);


 