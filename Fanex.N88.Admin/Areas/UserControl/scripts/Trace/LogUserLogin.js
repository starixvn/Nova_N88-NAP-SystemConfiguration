/* global NAP: false */

"use strict";

var LogUserLogin = (function ($) {

	return {
		grid: null,
		buttonImage: null,
		form: null,

		InitDateTimePicker: function () {
			$("#From").datepicker({
				defaultDate: "+1w",
				changeMonth: true,
				numberOfMonths: 1,
				dateFormat: "M dd,yy",
				showOn: "button",
				buttonImage: LogUserLogin.buttonImage,
				buttonText: "From",
				buttonImageOnly: true,
				onSelect: function (selectedDate) {
					$("#To").datepicker("option", "minDate", selectedDate);
					if ($("#To").val() === '') {
						$("#To").val($(this).val());
					}
				}
			});

			$("#To").datepicker({
				defaultDate: "+1w",
				changeMonth: true,
				numberOfMonths: 1,
				dateFormat: "M dd,yy",
				showOn: "button",
				buttonImage: LogUserLogin.buttonImage,
				buttonText: "To",
				buttonImageOnly: true,
				onSelect: function () {
				}
			});
		}
	};

} (jQuery));

$(document).ready(function () {
	LogUserLogin.grid = $('.grid');
	LogUserLogin.buttonImage = $('#imagePath').val();
	LogUserLogin.form = $('form:first');
	LogUserLogin.InitDateTimePicker();

	// shows loading on change page
	LogUserLogin.form.submit(function () {
		NAP.loadingBox.show();
	});

	// changes items/page
	LogUserLogin.grid.on('change', '.items-per-page', function () {
		var $this = $(this),
			pageSize = $this.val(),
			from = $('#From').val(),
			to = $('#To').val(),
			name = $('#Name').val(),
			siteId = $('#SiteId').val();

		var url = NAP.resolveClientUrl('Trace/LogUserLogin' +
			'?q=' + name +
			'&from=' + from +
			'&to=' + to +
			'&siteId=' + siteId +
			'&size=' + pageSize);

		NAP.loadingBox.show();
		window.location.replace(url);
	});
});