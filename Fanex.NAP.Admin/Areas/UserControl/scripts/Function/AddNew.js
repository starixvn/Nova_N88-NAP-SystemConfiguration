/* global NAP: false */

"use strict";

var AddNewFunction = (function ($) {

	return {
		form: null,
		siteId: null,
		parentId: null,
		parentName: null,

		getFunctionBySite: function (siteId) {
			var url = 'GetFunctionBySite?siteId=' + siteId;

			$.ajax({
				url: url,
				type: 'POST',
				dataType: 'json',
				cache: false,
				async: false,
				success: function (result) {
					AddNewFunction.parentId.html('');

					if (result.length > 0) {
						AddNewFunction.parentId.attr('disabled', false);
						AddNewFunction.parentId.append($('#DropdownListTpl').render(result));
					}
					else {
						AddNewFunction.parentId.attr('disabled', true);
					}
				},
				error: function (xhr, status, err) {
					NAP.handleAjaxError(xhr, status, err);
				}
			});
		},

		getFunctionParentByEnvironment: function (environment, siteId) {
			var url = NAP.resolveClientUrl('Function/GetFunctionParentByEnvironment?siteId=' + siteId + '&environment=' + environment);

			$.ajax({
				url: url,
				type: 'POST',
				dataType: 'JSON',
				cache: false,
				async: false,
				success: function (result) {
					AddNewFunction.parentId.html('');

					if (result.length > 0) {
						AddNewFunction.parentId.attr('disabled', false);
						AddNewFunction.parentId.append($('#DropdownListTpl').render(result));
					}
					else {
						AddNewFunction.parentId.attr('disabled', true);
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
	AddNewFunction.form = $('form:first');
	AddNewFunction.siteId = $('#SiteId');
	AddNewFunction.parentId = $('#ParentId');
	AddNewFunction.parentName = $('#ParentName');

	AddNewFunction.siteId.change(function () {
		AddNewFunction.getFunctionBySite($(this).val());
	});

	AddNewFunction.form.submit(function () {
		var parentId = AddNewFunction.parentId.val();

		if (parentId.length > 0) {
			AddNewFunction.parentName.val(AddNewFunction.parentId.find("option[value=" + parentId + "]").text());
		}
	});

	$('input[type=radio]').click(function () {
		var environment = $(this).val();
		var siteId = $('#SiteId').val();
		AddNewFunction.getFunctionParentByEnvironment(environment, siteId);
	});
});