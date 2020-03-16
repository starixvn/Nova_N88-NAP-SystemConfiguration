/* global NAP: false */

"use strict";

var ManageMenu = (function () {

	return {
		grid: null,
		form: null
	};

} (jQuery));

$(document).ready(function () {
	ManageMenu.grid = $('.grid');
	ManageMenu.form = $('form:first');

	// #region tooltips
	$('.tip').tipsy({ gravity: 'se' });
	$('#headline h1 .lnk').tipsy({ gravity: 'w' });
	$('.grid input[type=checkbox][miid]').tipsy({ gravity: 'se' });
	// #endregion

	// #region tree view/ grid view
	$('#btnGridView, #btnTreeView').bind('click', function () {
		var $this = $(this);

		if (!$this.hasClass('active')) {
			if (NAP.agentIsOldIe) {
				$('.grid-mode-only').toggle();
				$('.tree-mode-only').toggle();
			} else {
				$('.grid-mode-only').toggle('slow');
				$('.tree-mode-only').toggle('slow');
			}
		}

		$this.addClass('active').siblings().removeClass('active');
	});
	// #endregion

	// #region activate/deactivate functions
	ManageMenu.grid.on('mousedown', 'input[type=checkbox][miid]', function () {
		var $this = $(this),
			menuItemName = $this.attr('miname'),
			active = !$this.is(':checked'),
			action = active ? 'activate' : 'deactivate',
			msg = "Are you sure you want to " + action + " '" + menuItemName + "' ?";

		NAP.headlineMsgBox.confirm(msg, function () {
			var menuItemId = $this.attr('miid');

			$this.attr('checked', active).change();

			$.ajax({
				url: NAP.resolveClientUrl("Menu/Activate" + "?id=" + menuItemId + "&isActive=" + active),
				type: 'POST',
				contentType: 'application/json',
				dataType: 'json',
				cache: false,
				beforeSend: function () {
					NAP.loadingBox.show();
				},
				complete: function () {
					NAP.loadingBox.close();
				},
				success: function (result) {
					if (result === 'OK') {
						var msg = active ? "Activation succeeded!" : "Deactivation succeeded!";

						NAP.headlineMsgBox.showInfo(msg, { closeBtn: false });
						if (active) {
							$this.attr('original-title', $this.attr('original-title').replace('Activate', 'Deactivate'));
						} else {

							$this.attr('original-title', $this.attr('original-title').replace('Deactivate', 'Activate'));
						}
					}
				},
				error: function (xhr, status, err) {
					NAP.handleAjaxError(xhr, status, err);
				}
			});
		}, { modal: true, closeBtn: false });
	});
	// #endregion

	ManageMenu.form.submit(function () {
		NAP.loadingBox.show();
	});
});