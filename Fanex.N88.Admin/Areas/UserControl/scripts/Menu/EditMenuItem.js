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
		}
	};

} (jQuery));

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
});