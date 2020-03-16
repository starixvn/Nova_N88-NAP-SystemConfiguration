/* global NAP: false */

"use strict";

var AddNewMenuItem = (function ($) {

	return {
		form: null,
		siteId: null,
		parentId: null,
		parentName: null,

		getFunctionBySite: function (siteId, environment) {
			var url = 'ListFunctionBySite?siteId=' + siteId + '&environment=' + environment;

			$.ajax({
				url: url,
				type: 'POST',
				dataType: 'json',
				cache: false,
				async: false,
				success: function (result) {
					AddNewMenuItem.functionId.html('');

					if (result.length > 0) {
						AddNewMenuItem.functionId.attr('disabled', false);
						AddNewMenuItem.functionId.append($('#DropdownListTpl').render(result));
					}
					else {
						AddNewMenuItem.functionId.attr('disabled', true);
					}
				},
				error: function (xhr, status, err) {
					NAP.handleAjaxError(xhr, status, err);
				}
			});
		},

		getMenuBySite: function (siteId, environment) {

			var url = 'ListMenuItemBySite?siteId=' + siteId + '&environment=' + environment;

			$.ajax({
				url: url,
				type: 'POST',
				dataType: 'json',
				cache: false,
				async: false,
				success: function (result) {
					AddNewMenuItem.parentId.html('');

					if (result.length > 0) {
						AddNewMenuItem.parentId.attr('disabled', false);
						AddNewMenuItem.parentId.append($('#DropdownListTpl').render(result));
					}
					else {
						AddNewMenuItem.parentId.attr('disabled', true);
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
	AddNewMenuItem.form = $('form:first');
	AddNewMenuItem.siteId = $('#SiteId');
	AddNewMenuItem.parentId = $('#ParentId');
	AddNewMenuItem.parentName = $('#ParentName');
	AddNewMenuItem.functionId = $('#FunctionId');

	AddNewMenuItem.siteId.change(function () {

		var isLive = $('#Environment-LIVE').is(':checked');
		var isUAT = $('#Environment-UAT').is(':checked');
		var environment = "";
		if (isLive || isUAT) {
			environment = isLive ? "LIVE" : "UAT";
		}

		AddNewMenuItem.getFunctionBySite($(this).val(), environment);
		AddNewMenuItem.getMenuBySite($(this).val(), environment);
	});

	AddNewMenuItem.form.submit(function () {
		var $this = $(this),
		parentId = AddNewMenuItem.parentId.val();

		$this.find('input[type=submit]').focus();

		if (parentId.length > 0) {
			AddNewMenuItem.parentName.val(AddNewMenuItem.parentId.find("option[value=" + parentId + "]").text());
		}
	});

	$('.environment').click(function () {
		var environment = $(this).val();
		var siteId = $('#SiteId').val();
		AddNewMenuItem.getFunctionBySite(siteId, environment);
		AddNewMenuItem.getMenuBySite(siteId, environment);
	});
});