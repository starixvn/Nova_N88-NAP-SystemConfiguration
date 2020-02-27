"use strict";

var DuplicateFunction = (function () {

	return {
		form: null,
		siteId: null,
		parentId: null,
		parentName: null
	};

} (jQuery));

$(document).ready(function () {
	DuplicateFunction.form = $('form:first');
	DuplicateFunction.siteId = $('#SiteId');
	DuplicateFunction.parentId = $('#ParentId');
	DuplicateFunction.parentName = $('#ParentName');

	DuplicateFunction.form.submit(function () {
		var parentId = DuplicateFunction.parentId.val();

		if (parentId.length > 0) {
			DuplicateFunction.parentName.val(DuplicateFunction.parentId.find("option[value=" + parentId + "]").text());
		}
	});
});