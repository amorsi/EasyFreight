﻿/*
Name: 			Tables / Editable - Examples
Written by: 	Okler Themes - (http://www.okler.net)
Theme Version: 	1.3.0
*/

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

				    var countryid = $(this).attr('countryid');
				    var CountryNameAr = $(this).closest('tr').find(".input-block:eq('1')").val();
				    var countryNameEn = $(this).closest('tr').find(".input-block:eq('0')").val();
				    if (countryid == undefined)
				        countryid = '0';

				    var isSaved = SaveCountry(countryid, countryNameEn, CountryNameAr);

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
				    var countryid = $(this).closest('tr').find('td.actions').find('a.save-row').attr('countryid')
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
				                    // Here ajax remove country
				                    var isDeleted = DeleteCountry(countryid);
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

            data = this.datatable.row.add(['', '', actions]);
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

    // Ajax AddEdit Counry
    function SaveCountry(countryId, countryNameEn, CountryNameAr) {
        if (countryNameEn == '')
        {
            new PNotify({
                title: 'Sorry!',
                text: 'Please Add Country Name',
                type: 'error'
            });
            return;
        }
        var isSaved = 'false';
        $.ajax({
            url: ROOT + "CountryLibrary/AddEditCountry",
            type: "POST",
            data: { 'countryId': countryId, 'countyNameEn': countryNameEn, 'countryNameAr': CountryNameAr },
            async: false,
            dataType: "json",
            success: function (data) {
                isSaved = data;
                if (data == "true") {
                    new PNotify({
                        title: 'Success!',
                        text: countryId == 0 ? 'Country Added.' : 'Country Updated',
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
    //-------------------------------------------
    function DeleteCountry(countryId) {
        var isDeleted = "true";
        $.ajax({
            url: ROOT + "CountryLibrary/DeleteCountry",
            type: "POST",
            data: { 'countryId': countryId },
            async: false,
            dataType: "json",
            success: function (data) {
                isDeleted = data;
                if (data == "true") {
                    new PNotify({
                        title: 'Success!',
                        text: 'Country Deleted.',
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