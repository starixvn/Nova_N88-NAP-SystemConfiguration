"use strict";

var AddNewSite = (function () {

	return {
		form: null,
		btnAddUrl: null,
		initTooltips: function () {
			AddNewSite.form.find('.btn-remove-url').tipsy({ gravity: 's' });
		}
	};

} (jQuery));

$(document).ready(function () {
	AddNewSite.form = $('form:first');

	var btnRemoveHandler = function () {
		var $this = $(this),
			parentRow = $this.parents('tr'),
			prevRows = parentRow.prevAll('tr');

		parentRow.fadeOut('fast', function () { $(this).remove(); });
		$('.tipsy').remove();

		// shifts index
		prevRows.each(function () {
			var $this = $(this),
				thisTxt = $this.find('input[type=text]:first'),
				thisRowChk = $this.find('input[type=checkbox]:first'),
				thisRowChkHdn = $this.next('input[type=hidden]'),
				thisRowRdo = $this.find('input[type=radio]:first'),
				thisIdx = parseInt(thisTxt.attr('idx'));

			thisIdx--;

			thisTxt.attr({ 'name': 'Urls[' + thisIdx + '].Url', 'idx': thisIdx });
			thisRowChk.attr('name', 'Urls[' + thisIdx + '].IsActive');
			thisRowChkHdn.attr('name', 'Urls[' + thisIdx + '].IsActive');
			thisRowRdo.attr('value', 'Urls[' + thisIdx + '].IsDefault');
		});
	};

	AddNewSite.form.find('.btn-remove-url').live('click', btnRemoveHandler);

	AddNewSite.form.find('.btn-add-url').tipsy({ gravity: 'se' }).click(function () {
		var $this = $(this),
			table = $this.parents('table'),
			newRow = table.find('tbody tr:first').clone(),
			newRowTxt = newRow.find('input[type=text]:first').removeClass('input-validation-error'),
			newRowChk = newRow.find('input[type=checkbox]:first'),
			newRowChkHdn = newRowChk.next('input[type=hidden]'),
			newRowRdo = newRow.find('input[type=radio]:first'),
			newIdx = parseInt(newRowTxt.attr('idx')) + 1,
			addBtn = newRow.find('.btn-add-url:first'),
			removeBtn = table.parents('.urls').children('.btn-remove-url:last').clone(true);

		newRowTxt.attr({ 'name': 'Urls[' + newIdx + '].Url', 'idx': newIdx });
		newRowChk.attr('name', 'Urls[' + newIdx + '].IsActive');
		newRowChkHdn.attr('name', 'Urls[' + newIdx + '].IsActive');
		newRowRdo.attr('value', 'Urls[' + newIdx + '].IsDefault');
		addBtn.after(removeBtn.show());
		addBtn.remove();

		newRow.css('display', 'none').prependTo(table).slideDown('fast', 'easeOutBack');
		AddNewSite.initTooltips(true);

		// workaround for the bug: pressing "add button" continuously
		AddNewSite.btnAddUrl = $this.hide();
		setTimeout(AddNewSite.btnAddUrl.show, 300);
	});

	AddNewSite.form.submit(function () {
		var isValid = true;

		AddNewSite.form.find('.urls input[type=text]').each(function () {
			var $this = $(this),
				thisValue = $.trim($this.val()),
				row = $this.parents('tr'),
				nextRows = row.nextAll('tr'),
				otherTextboxes = nextRows.find('input[type=text]');

			$this.val(thisValue);

			otherTextboxes.each(function () {
				if ($.trim($(this).val()) === thisValue) {
					$(this).addClass('input-validation-error');
					$this.addClass('input-validation-error');

					isValid = false;
				}
			});
		});

		if (isValid) {
			AddNewSite.form.find('.urls input[type=radio]').each(function () {
				var $this = $(this),
					thisValue = $this.val();

				$this.attr('name', thisValue).val($this.is(':checked'));
			});
		}

		return isValid;
	});
});