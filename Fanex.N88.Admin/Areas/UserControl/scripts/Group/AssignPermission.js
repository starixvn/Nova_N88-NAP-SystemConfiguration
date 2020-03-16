/* global NAP: false */

"use strict";

var AssignGroupPermission = (function ($) {
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
                    var siblings = $('[name=func' + type + '][pid=' + parent.val() + ']:checked');

                    if (siblings.length === 0) {
                        parent.attr('checked', false);

                        if (type === 'Read') {
                            parent.parents('tr').find('[name=funcHost]').attr('checked', false);
                        }
                    }
                }

                AssignGroupPermission.updateParent.call(parent.get(0), type);
            }
        },

        // disabled parent if has child is disabled
        disabledParent: function () {
            AssignGroupPermission.grid.find('tr').each(function () {
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
        }
    };
}(jQuery));

$(document).ready(function () {
    AssignGroupPermission.grid = $('.grid:first');
    AssignGroupPermission.updateForm = $('form:last');
    AssignGroupPermission.disabledParent();

    AssignGroupPermission.updateForm.submit(function () {
        AssignGroupPermission.grid.find('tr').each(function () {
            var $this = $(this),
				chkHost = $this.find('[name=funcHost]'),
				chkRead = $this.find('[name=funcRead]');

            if (chkHost.is(':checked')) {
                chkRead.removeAttr('name');
            } else {
                chkHost.removeAttr('name');
            }
        });

        NAP.loadingBox.show();
    });

    AssignGroupPermission.grid.find('[type=checkbox]').change(function () {
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

        if ($this.attr('name') === 'funcHost') {
            // host checked -> read checked
            if (thisIsChecked) {
                AssignGroupPermission.updateParent.call(row.find('[name=funcRead]').attr('checked', true).get(0), 'Read');
                table.find('[name=funcRead][pid=' + $this.val() + ']').attr('checked', true).change();
            }

            // parent changed -> children changed
            table.find('[name=funcHost][pid=' + $this.val() + ']').attr('checked', thisIsChecked).change();

            // child checked -> parent checked
            // last child unchecked -> parent unchecked
            AssignGroupPermission.updateParent.call($this.get(0), 'Host');
        } else { // funcRead
            // read unchecked -> host unchecked
            if (!thisIsChecked) {
                AssignGroupPermission.updateParent.call(row.find('[name=funcHost]').attr('checked', false).get(0), 'Host');
            }

            // parent changed -> children changed
            table.find('[name=funcRead][pid=' + $this.val() + ']').attr('checked', thisIsChecked).change();

            // child checked -> parent checked
            // last child unchecked -> parent unchecked
            AssignGroupPermission.updateParent.call($this.get(0), 'Read');
        }
    });

    var hasAssignFunction = $('#HasAssignFunction').val();
    if (!hasAssignFunction) {
        $('[type=checkbox]').attr('disabled', 'disabled');
        $('#btnSaveAssign').hide();
    }
});