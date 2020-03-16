/* global NAP: false */

"use strict";

var ManageGroup = (function () {

	return {
		grid: null
	};

} (jQuery));

$(document).ready(function () {
	ManageGroup.grid = $('.grid');

	// #region tooltips
	$('.tip').tipsy({ gravity: 'se' });
	$('#headline h1 .lnk').tipsy({ gravity: 'w' });
	$('.grid input[type=checkbox][gid]').tipsy({ gravity: 's' });
	$('.grid .lnk-group-info').tipsy({ gravity: 'w' });
	$('.grid .num-of-user').tipsy({ gravity: 'w' });
	// #endregion

	// #region view details
	ManageGroup.grid.on('click', '.lnk-group-info', function () {
		var $this = $(this),
			groupId = $this.attr('gid'),
			groupName = $this.text();

		$.ajax({
			url: NAP.resolveClientUrl("Group/GroupInfo/" + groupId),
			type: 'POST',
			dataType: 'html',
			cache: false,
			success: function (result) {
				if (result.length > 0) {
					NAP.popup("Details of '" + groupName + "'", result);
				}
			},
			error: function (xhr, status, err) {
				NAP.handleAjaxError(xhr, status, err);
			}
		});
	});
	// #endregion

	// #region activate/deactivate functions
	ManageGroup.grid.on('mousedown', 'input[type=checkbox][gid]', function () {
		var $this = $(this),
			groupName = $this.attr('gname'),
			active = !$this.is(':checked'),
			action = active ? 'activate' : 'deactivate',
			msg = "Are you sure you want to " + action + " '" + groupName + "' ?";

		NAP.headlineMsgBox.confirm(msg, function () {
			var groupId = $this.attr('gid');

			$this.attr('checked', active).change();

			$.ajax({
				url: NAP.resolveClientUrl("Group/Activate" + "?id=" + groupId + "&isActive=" + active),
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

	ManageGroup.grid.on('click', '.btn-action', function (e) {
		var $this = $(this),
			parent = $this.parent(),
			dropdown = parent.find('.actions');

		if (parent.hasClass('active')) {
			parent.removeClass('active');
			dropdown.hide();
		} else {
			ManageGroup.grid.find('.active').removeClass('active').find('.actions').hide();
			dropdown.css({ top: -(parent.height() + 10), right: parent.width() * 0.7 }).show();
			parent.addClass('active');
		}

		e.stopPropagation();
	});

	$('body, header, #headline, #body').on('click', function () {
		ManageGroup.grid.find('.active').removeClass('active').find('.actions').hide();
	});
	// #endregion
});