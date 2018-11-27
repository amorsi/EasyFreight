
(function ($) {

    'use strict';

    var EditableTable = {

        options: {
            addButton: '#addToTable',
            table: '#datatable-editable',
            dialog: {
                wrapper: '#dialog',
                cancelButton: '#dialogCancel',
                confirmButton: '#dialogConfirm',
            }
        },

        initialize: function () {
            this
				.setVars()
				.build()
				.events();
        },

        setVars: function () {
            this.$table = $(this.options.table);
            this.$addButton = $(this.options.addButton);

            // dialog
            this.dialog = {};
            this.dialog.$wrapper = $(this.options.dialog.wrapper);
            this.dialog.$cancel = $(this.options.dialog.cancelButton);
            this.dialog.$confirm = $(this.options.dialog.confirmButton);

            return this;
        },

        build: function () {
            this.datatable = this.$table.DataTable({
                aoColumns: [
					null,
					null,
					{ "bSortable": false }
                ]
            });

            window.dt = this.datatable;

            return this;
        },

        events: function () {
            var _self = this;

            this.$table
				.on('click', 'a.save-row', function (e) {
				    e.preventDefault();
				    
				    var incotermId = $(this).attr('incotermId');
				    var incotermCode = $(this).closest('tr').find('td').eq(0).find(".input-block").val();
				    var incotermName = $(this).closest('tr').find('td').eq(1).find(".input-block").val();
				    

				    if (incotermId == undefined)
				        incotermId = '0';
				    var isSaved = SaveIncoterm(incotermId, incotermCode, incotermName);
				    if (isSaved == 'true')
				        _self.rowSave($(this).closest('tr'));
				})
				.on('click', 'a.cancel-row', function (e) {
				    e.preventDefault();

				    _self.rowCancel($(this).closest('tr'));
				})
				.on('click', 'a.edit-row', function (e) {
				    e.preventDefault();

				    _self.rowEdit($(this).closest('tr'));
				})
				.on('click', 'a.remove-row', function (e) {
				    e.preventDefault();
				    var incotermId = $(this).closest('tr').find('td.actions').find('a.save-row').attr('incotermId')
				    var $row = $(this).closest('tr');

				    $.magnificPopup.open({
				        items: {
				            src: '#dialog',
				            type: 'inline'
				        },
				        preloader: false,
				        modal: true,
				        callbacks: {
				            change: function () {
				                _self.dialog.$confirm.on('click', function (e) {
				                    e.preventDefault();

				                    $.magnificPopup.close();
				                    // Here ajax remove container
				                    var isDeleted = DeleteIncoterm(incotermId);
				                    if (isDeleted == "true")
				                        _self.rowRemove($row);
				                });
				            },
				            close: function () {
				                _self.dialog.$confirm.off('click');
				            }
				        }
				    });
				});

            this.$addButton.on('click', function (e) {
                e.preventDefault();

                _self.rowAdd();
            });

            this.dialog.$cancel.on('click', function (e) {
                e.preventDefault();
                $.magnificPopup.close();
            });

            return this;
        },

        // ==========================================================================================
        // ROW FUNCTIONS
        // ==========================================================================================
        rowAdd: function () {
            this.$addButton.attr({ 'disabled': 'disabled' });

            var actions,
				data,
				$row;

            actions = [
				'<a href="#" class="hidden on-editing save-row"><i class="fa fa-save"></i></a>',
				'<a href="#" class="hidden on-editing cancel-row"><i class="fa fa-times"></i></a>',
				'<a href="#" class="on-default edit-row"><i class="fa fa-pencil"></i></a>',
				'<a href="#" class="on-default remove-row"><i class="fa fa-trash-o"></i></a>'
            ].join(' ');

            data = this.datatable.row.add(['', '',  actions]);
            $row = this.datatable.row(data[0]).nodes().to$();

            $row
				.addClass('adding')
				.find('td:last')
				.addClass('actions');

            this.rowEdit($row);

            this.datatable.order([0, 'asc']).draw(); // always show fields
        },

        rowCancel: function ($row) {
            var _self = this,
				$actions,
				i,
				data;

            if ($row.hasClass('adding')) {
                this.rowRemove($row);
            } else {

                data = this.datatable.row($row.get(0)).data();
                this.datatable.row($row.get(0)).data(data);

                $actions = $row.find('td.actions');
                if ($actions.get(0)) {
                    this.rowSetActionsDefault($row);
                }

                this.datatable.draw();
            }
        },

        rowEdit: function ($row) {
            var _self = this,
				data;

            data = this.datatable.row($row.get(0)).data();

            $row.children('td').each(function (i) {
                var $this = $(this);

                if ($this.hasClass('actions')) {
                    _self.rowSetActionsEditing($row);
                } else {
                    $this.html('<input type="text" class="form-control input-block" value="' + data[i] + '"/>');
                }
            });
        },

        rowSave: function ($row) {
            var _self = this,
				$actions,
				values = [];

            if ($row.hasClass('adding')) {
                this.$addButton.removeAttr('disabled');
                $row.removeClass('adding');
            }

            values = $row.find('td').map(function () {
                var $this = $(this);

                if ($this.hasClass('actions')) {
                    _self.rowSetActionsDefault($row);
                    return _self.datatable.cell(this).data();
                } else {
                    return $.trim($this.find('input').val());
                }
            });

            this.datatable.row($row.get(0)).data(values);

            $actions = $row.find('td.actions');
            if ($actions.get(0)) {
                this.rowSetActionsDefault($row);
            }

            this.datatable.draw();
        },

        rowRemove: function ($row) {
            if ($row.hasClass('adding')) {
                this.$addButton.removeAttr('disabled');
            }

            this.datatable.row($row.get(0)).remove().draw();
        },

        rowSetActionsEditing: function ($row) {
            $row.find('.on-editing').removeClass('hidden');
            $row.find('.on-default').addClass('hidden');
        },

        rowSetActionsDefault: function ($row) {
            $row.find('.on-editing').addClass('hidden');
            $row.find('.on-default').removeClass('hidden');
        }

    };

    $(function () {
        EditableTable.initialize();
    });


    // Ajax AddEdit Container
    function SaveIncoterm(incotermId, incotermCode, incotermName) {
        if (incotermCode == '' || incotermName == ''  ) {
            new PNotify({
                title: 'Sorry!',
                text: 'Please Complete All Data',
                type: 'error'
            });
            return;
        }
        var isSaved = 'false';
        $.ajax({
            url: ROOT + "SystemLibrary/AddEditIncoterm",
            type: "POST",
            data: { 'incotermId': incotermId, 'incotermCode': incotermCode, 'incotermName': incotermName },
            async: false,
            dataType: "json",
            success: function (data) {
                isSaved = data;
                if (data == "true") {
                    new PNotify({
                        title: 'Success!',
                        text: incotermId == 0 ? 'Incoterm  Added.' : 'Incoterm Updated',
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
        return isSaved;
    }
    // Ajax Delete Container
    function DeleteIncoterm(incotermId) {
        var isDeleted = "true";
        $.ajax({
            url: ROOT + "SystemLibrary/DeleteIncoterm",
            type: "POST",
            data: { 'incotermId': incotermId },
            async: false,
            dataType: "json",
            success: function (data) {
                isDeleted = data;
                if (data == "true") {
                    new PNotify({
                        title: 'Success!',
                        text: 'Incoterm Deleted.',
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
        return isDeleted;
    }
    //--------------------------------------



}).apply(this, [jQuery]);