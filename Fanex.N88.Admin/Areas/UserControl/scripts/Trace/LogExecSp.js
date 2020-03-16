/* global NAP: false */

"use strict";

var LogExecSp = (function ($) {

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
				buttonImage: LogExecSp.buttonImage,
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
				buttonImage: LogExecSp.buttonImage,
				buttonText: "To",
				buttonImageOnly: true,
				onSelect: function () {
				}
			});
		}
	};

} (jQuery));

$(document).ready(function () {
	LogExecSp.grid = $('.grid');
	LogExecSp.buttonImage = $('#imagePath').val();
	LogExecSp.form = $('form:first');
	LogExecSp.InitDateTimePicker();

	$('.loginsert').click(function () {
		var logId = $(this).attr('logid');

		$.ajax({
			url: NAP.resolveClientUrl("Trace/GetLogInsertDetail/" + logId),
			type: 'POST',
			contentType: 'application/json',
			dataType: 'html',
			cache: false,
			beforeSend: function () {
				NAP.loadingBox.show();
			},
			complete: function () {
				NAP.loadingBox.close();
			},
			success: function (result) {
				if (result.length > 0) {
					NAP.popup("Log detail", result);
				}
			},
			error: function (xhr, status, err) {
				NAP.handleAjaxError(xhr, status, err);
			}
		});
	});

	// shows loading on change page
	LogExecSp.form.submit(function () {
		NAP.loadingBox.show();
	});

	// changes items/page
	LogExecSp.grid.on('change', '.items-per-page', function () {
		var $this = $(this),
			pageSize = $this.val(),
			username = $('#Username').val(),
			from = $('#From').val(),
			to = $('#To').val(),
			name = $('#Name').val(),
			filter = $('#FilterType').val();

		var url = NAP.resolveClientUrl('Trace/LogExecSp'+
			'?q=' + name +
			'&username=' + username +
			'&from=' + from +
			'&to=' + to +
			'&filterBy=' + filter +
			'&size=' + pageSize);

		NAP.loadingBox.show();
		window.location.replace(url);
	});
});