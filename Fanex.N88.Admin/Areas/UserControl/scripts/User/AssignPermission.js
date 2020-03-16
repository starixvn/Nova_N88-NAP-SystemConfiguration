"use strict";

var AssignUserPermission = (function ($) {
    return {
        updateForm: null,
        grid: null,

        updateParent: function (type) {
            var $this = $(this),
				pid = $this.attr('pid');

            if (pid && pid.length > 0) {
                var thisIsChecked = $this.is(':checked'),
					parent = $('#func' + type + pid);

                if (thisIsChecked && !parent.is(':checked')) {
                    parent.attr('checked', true);
                }

                if (!thisIsChecked) {
                    var siblings = $('[name=criteria\\.func' + type + '][pid=' + parent.val() + ']:checked');

                    if (siblings.length === 0) {
                        parent.attr('checked', false);

                        if (type === 'Read') {
                            parent.parents('tr').find('[name=criteria\\.funcHost]').attr('checked', false);
                        }
                    }
                }

                AssignUserPermission.updateParent.call(parent.get(0), type);
            }
        },

        // disabled parent if has child is disabled
        disabledParent: function () {
            AssignUserPermission.grid.find('tr').each(function () {
                var $this = $(this),
				chkHost = $this.find('[name=funcHost][type=hidden]'),
				chkRead = $this.find('[name=funcRead][type=hidden]');

                var parent;

                if (chkRead !== undefined) {
                    parent = $('#funcRead' + chkRead.attr('pid'));
                    if (parent.is(':checked') && parent.attr('pdisabled') === undefined) {
                        parent.attr('pdisabled', '1');
                        parent.after('<input type="checkbox" checked="checked" disabled="disabled"/>');
                        parent.hide();
                    }
                }

                if (chkHost !== undefined) {
                    parent = $('#funcHost' + chkHost.attr('pid'));
                    if (parent.is(':checked') && parent.attr('pdisabled') === undefined) {
                        parent.attr('pdisabled', '1');
                        parent.after('<input type="checkbox" checked="checked" disabled="disabled"/>');
                        parent.hide();
                    }
                }
            });
        },

        checkBoxAll: {
            checkBoxAllFuncRead: null,
            checkBoxAllFuncHost: null,

            init: function () {
                this.checkBoxAllFuncRead.attr('checked', this.areAllCheckBoxFuncsChecked('funcRead'));
                this.checkBoxAllFuncHost.attr('checked', this.areAllCheckBoxFuncsChecked('funcHost'));

                if (this.areAllCheckBoxFuncsDisable('funcRead')) {
                    this.checkBoxAllFuncRead.attr("disabled", true);
                }

                if (this.areAllCheckBoxFuncsDisable('funcHost')) {
                    this.checkBoxAllFuncHost.attr("disabled", true);
                }
            },

            areAllCheckBoxFuncsDisable: function (type) {
                var chkFuncs = AssignUserPermission.grid.find('[type=checkbox][name=criteria\\.' + type + ']:not(:disabled)');
                if (chkFuncs === typeof (undefined) || chkFuncs.length === 0) {
                    return true;
                }

                return false;
            },

            areAllCheckBoxFuncsChecked: function (type) {
                var chkFuncs = AssignUserPermission.grid.find('[type=checkbox][name=criteria\\.' + type + ']:not(:disabled)');
                if (chkFuncs === typeof (undefined) || chkFuncs.length === 0) {
                    return false;
                }

                var result = true;
                chkFuncs.each(function () {
                    var $this = $(this);
                    if (!$this.is(':checked')) {
                        result = false;
                    }
                });

                return result;
            },

            checkAllForCheckBoxFuncs: function (type, isChecked) {
                AssignUserPermission.grid.find('[type=checkbox][name=criteria\\.' + type + ']:not(:disabled)').attr('checked', isChecked).change();
            },

            checkForCheckBoxAllFuncRead: function () {
                if (this.areAllCheckBoxFuncsChecked('funcRead')) {
                    this.checkBoxAllFuncRead.attr('checked', true);
                }
                else {
                    this.checkBoxAllFuncRead.attr('checked', false);
                }
            },

            checkForCheckBoxAllFuncHost: function () {
                if (this.areAllCheckBoxFuncsChecked('funcHost')) {
                    this.checkBoxAllFuncHost.attr('checked', true);
                }
                else {
                    this.checkBoxAllFuncHost.attr('checked', false);
                }
            },
        },
    };
}(jQuery));

$(document).ready(function () {
    AssignUserPermission.grid = $('.grid:first');
    AssignUserPermission.updateForm = $('form:last');
    AssignUserPermission.disabledParent();
    AssignUserPermission.updateForm.submit(function () {
        AssignUserPermission.grid.find('tr').each(function () {
            var $this = $(this),
                chkHost = $this.find('[name=criteria\\.funcHost]'),
                chkRead = $this.find('[name=criteria\\.funcRead]');

            if (chkHost.is(':checked') || chkHost.attr('checked') === "checked") {
                chkRead.removeAttr('name');
            } else {
                chkHost.removeAttr('name');
            }
        });
    });

    var checkBoxAll = AssignUserPermission.checkBoxAll;
    checkBoxAll.checkBoxAllFuncRead = $('#chkAllFuncRead');
    checkBoxAll.checkBoxAllFuncHost = $('#chkAllFuncHost');
    checkBoxAll.init();

    AssignUserPermission.grid.find('[type=checkbox]').change(function () {
        var $this = $(this),
            thisIsChecked = $this.is(':checked'),
            row = $this.parents('tr'),
            table = row.parents('table');

        if (NAP.hasHouseWLByProductsPermission(table, $this)) {
            return;
        }

        if (NAP.hasCN88WLByProductsPermission(table, $this)) {
            return;
        }

        if ($this.attr('name') === 'criteria.funcHost') {
            // host checked -> read checked
            if (thisIsChecked) {
                AssignUserPermission.updateParent.call(row.find('[type=checkbox][name=criteria\\.funcRead]').attr('checked', true).get(0), 'Read');
                table.find('[type=checkbox][name=criteria\\.funcRead][pid=' + $this.val() + ']:not(:disabled)').attr('checked', true).change();
            }

            // parent changed -> children changed
            table.find('[type=checkbox][name=criteria\\.funcHost][pid=' + $this.val() + ']:not(:disabled)').attr('checked', thisIsChecked).change();

            // child checked -> parent checked
            // last child unchecked -> parent unchecked
            AssignUserPermission.updateParent.call($this.get(0), 'Host');

            checkBoxAll.checkForCheckBoxAllFuncHost();
            checkBoxAll.checkForCheckBoxAllFuncRead();
        } else if ($this.attr('name') === 'criteria.funcRead') { // funcRead
            // read unchecked -> host unchecked
            if (!thisIsChecked) {
                AssignUserPermission.updateParent.call(
                    row.find('[type=checkbox][name=criteria\\.funcHost]:not(:disabled)').attr('checked', false).get(0), 'Host'
                );
            }

            // parent changed -> children changed
            table.find('[type=checkbox][name=criteria\\.funcRead][pid=' + $this.val() + ']:not(:disabled)').attr('checked', thisIsChecked).change();

            // child checked -> parent checked
            // last child unchecked -> parent unchecked
            AssignUserPermission.updateParent.call($this.get(0), 'Read');

            checkBoxAll.checkForCheckBoxAllFuncHost();
            checkBoxAll.checkForCheckBoxAllFuncRead();
        }
    });

    var hasAssignPermission = $('#HasAssignPermission').val().toLowerCase();
    if (hasAssignPermission === "false") {
        $('input[type=checkbox]').attr('disabled', true);
        $('#btnAssignPermission').hide();
    }

    checkBoxAll.checkBoxAllFuncRead.change(function () {
        var $this = $(this),
            thisIsChecked = $this.is(':checked');

        if (thisIsChecked) {
            checkBoxAll.checkAllForCheckBoxFuncs('funcRead', true);
        }
        else {
            checkBoxAll.checkAllForCheckBoxFuncs('funcRead', false);

            checkBoxAll.checkBoxAllFuncHost.attr('checked', false);
            checkBoxAll.checkAllForCheckBoxFuncs('funcHost', false);
        }
    });

    checkBoxAll.checkBoxAllFuncHost.change(function () {
        var $this = $(this),
            thisIsChecked = $this.is(':checked');

        if (thisIsChecked) {
            checkBoxAll.checkAllForCheckBoxFuncs('funcHost', true);

            checkBoxAll.checkBoxAllFuncRead.attr('checked', true);
            checkBoxAll.checkAllForCheckBoxFuncs('funcRead', true);
        }
        else {
            checkBoxAll.checkAllForCheckBoxFuncs('funcHost', false);
        }
    });
});