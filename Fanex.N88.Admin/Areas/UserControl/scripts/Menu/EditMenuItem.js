/* global NAP: false */

"use strict";

var EditMenuItem = (function ($) {
    return {
        form: null,
        parentId: null,
        parentName: null,
        functionId: null,
        parent: null,
        functions: null,
        getFunctionBySite: function (siteId, environment) {
            var url = 'ListFunctionBySite?siteId=' + siteId + '&environment=' + environment;

            $.ajax({
                url: url,
                type: 'POST',
                dataType: 'json',
                cache: false,
                async: false,
                success: function (result) {
                    EditMenuItem.functions.html('');

                    if (result.length > 0) {
                        EditMenuItem.functions.attr('disabled', false);
                        EditMenuItem.functions.append($('#DropdownListTpl').render(result));

                        EditMenuItem.functions.val(EditMenuItem.functionId);
                    }
                    else {
                        EditMenuItem.functions.attr('disabled', true);
                    }
                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        },
        getMenuBySite: function (menuItemId, environment) {
            var url = 'ListMenuItemBySite?menuItemId=' + menuItemId + '&environment=' + environment;

            $.ajax({
                url: url,
                type: 'POST',
                dataType: 'json',
                cache: false,
                async: false,
                success: function (result) {
                    EditMenuItem.parent.html('');

                    if (result.length > 0) {
                        EditMenuItem.parent.attr('disabled', false);
                        EditMenuItem.parent.append($('#DropdownListTpl').render(result));

                        EditMenuItem.parent.val(EditMenuItem.parentId);
                    }
                    else {
                        EditMenuItem.parent.attr('disabled', true);
                    }
                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        },
        getMenuItemKey: function () {
            var menuItemId = $('#MenuItemId').val(),
                menuItemName = $('#MenuItemName').val();

            $.ajax({
                url: NAP.resolveClientUrl('Menu/GetMenuItemKey' + '?menuItemId=' + menuItemId + '&menuItemName=' + menuItemName),
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                cache: false,
                success: function (result) {
                    if (result.IsSuccessful) {
                        $('#MenuItemKey').val(result.MenuItemKey);
                        $('#txtMenuItemKey').val(result.MenuItemKey);
                    } else {
                        $('#error').append('');
                        var template = '<div data-valmsg-summary="true" class="validation-summary-errors"><ul><li>' +
                            result.Message + '</li></ul></div>';

                        $('#error').append(template);
                        $('#MenuItemName').addClass('input-validation-error');
                    }
                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        }
    };
}(jQuery));

$(document).ready(function () {
    EditMenuItem.form = $('form:first');
    EditMenuItem.parent = $('#ParentId');
    EditMenuItem.parentName = $('#ParentName');
    EditMenuItem.functions = $('#FunctionId');

    EditMenuItem.functionId = EditMenuItem.functions.val();
    EditMenuItem.parentId = EditMenuItem.parent.val();

    EditMenuItem.form.submit(function () {
        var $this = $(this),
            parentId = EditMenuItem.parent.val();

        $this.find('input[type=submit]').focus();

        if (parentId.length > 0) {
            EditMenuItem.parentName.val(EditMenuItem.parent.find("option[value=" + parentId + "]").text());
        }
    });

    $('.environment').click(function () {
        var environment = $(this).val();
        var siteId = $('#SiteId').val();
        var menuItemId = $('#MenuItemId').val();
        EditMenuItem.getFunctionBySite(siteId, environment);
        EditMenuItem.getMenuBySite(menuItemId, environment);
    });

    $('#lbtn-edit-menuitemkey').click(function () {
        var msg = "Do you want to regenerate menu item key ?";
        NAP.headlineMsgBox.confirm(msg, function () {
            EditMenuItem.getMenuItemKey();
        }, { modal: true, closeBtn: false });
    });
});