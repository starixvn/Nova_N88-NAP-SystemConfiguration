/* global NAP: false */

"use strict";

var EditFunction = (function ($) {

	return {
		form: null,
		parentId: null,
		parentName: null,
		parent: null,

		getFunctionParentByEnvironment: function (environment, functionId) {
			var url = NAP.resolveClientUrl('Function/GetFunctionParentByEnvironment?functionid=' + functionId + '&environment=' + environment);

			$.ajax({
				url: url,
				type: 'POST',
				dataType: 'JSON',
				cache: false,
				async: false,
				success: function (result) {
					EditFunction.parent.html('');

					if (result.length > 0) {

						EditFunction.parent.attr('disabled', false);
						EditFunction.parent.append($('#DropdownListTpl').render(result));

						EditFunction.parent.val(EditFunction.parentId);
					}
					else {
						EditFunction.parent.attr('disabled', true);
					}
				},
				error: function (xhr, status, err) {
					NAP.handleAjaxError(xhr, status, err);
				}
			});
		},

		getFunctionKey: function () {
			var functionId = $('#FunctionId').val();
			var functionName = $('#FunctionName').val();

			$.ajax({
				url: NAP.resolveClientUrl("Function/GetFunctionKey" + "?functionId=" + functionId + "&functionName=" + functionName),
				type: 'POST',
				contentType: 'application/json',
				dataType: 'json',
				cache: false,
				beforeSend: function () {
					//NAP.loadingBox.show();
				},
				complete: function () {
					//NAP.loadingBox.close();
				},
				success: function (result) {
					var template;

					switch (result) {
						case 0:
							$('#error').append("");

							template = '<div data-valmsg-summary="true" class="validation-summary-errors"><ul><li>' + 
								functionName + ' already exists. Please choose another!</li></ul></div>';

							$('#error').append(template);
							$('#FunctionName').addClass('input-validation-error');
							break;

						case -1:
							break;

						case -2:
							$('#error').append("");

							template = '<div data-valmsg-summary="true" class="validation-summary-errors"><ul><li>' + 
								result + ' already exists. Please choose another!</li></ul></div>';

							$('#error').append(template);
							$('#FunctionName').addClass('input-validation-error');
							break;

						default:
							$('#txtFunctionKey').removeAttr('disabled');
							$('#FunctionKey').val(result);
							$('#txtFunctionKey').val(result);
							$('#txtFunctionKey').attr('disabled', 'disabled');
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
	EditFunction.form = $('form:first');
	EditFunction.parent = $('#ParentId');
	EditFunction.parentName = $('#ParentName');
	EditFunction.parentId = EditFunction.parent.val();

	EditFunction.form.submit(function () {
		var $this = $(this),
			parentId = EditFunction.parent.val();

		$this.find('input[type=submit]').focus();

		if (parentId.length > 0) {
			EditFunction.parentName.val(EditFunction.parent.find("option[value=" + parentId + "]").text());
		}
	});

	$('input[type=radio]').click(function () {
		var environment = $(this).val();
		var functionId = $('#FunctionId').val();
		EditFunction.getFunctionParentByEnvironment(environment, functionId);
	});

	$('#lbtn-edit-funckey').click(function () {
		var msg = "Do you want to regenerate function key ?";
		NAP.headlineMsgBox.confirm(msg, function () {
			EditFunction.getFunctionKey();
		}, { modal: true, closeBtn: false });
	});

});