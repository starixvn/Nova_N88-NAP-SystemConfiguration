/* global NAP: false */

"use strict";

var ManageUser = (function () {

	return {
		grid: null,
		form: null,
		siteId: null
	};

} (jQuery));

$(document).ready(function () {
	ManageUser.grid = $('.grid');
	ManageUser.form = $('form:first');
	ManageUser.siteId = $('#SiteId');

	// shows tooltips when hovering on Usernames
	ManageUser.grid.find('.lnk-user-info').tipsy({ gravity: 'w' });

	// shows "User Details" popup when clicking on Usernames
	ManageUser.grid.on('click', '.lnk-user-info', function () {
		var $this = $(this),
			userId = $this.attr('uid'),
			username = $this.text();

		$.ajax({
			url: NAP.resolveClientUrl("User/UserInfo/" + userId),
			type: 'POST',
			dataType: 'html',
			cache: false,
			success: function (result) {
				if (result.length > 0) {
					NAP.popup("Details of '" + username + "'", result);
				}
			},
			error: function (xhr, status, err) {
				NAP.handleAjaxError(xhr, status, err);
			}
		});
	});

	// shows action-panel when clicking on the action buttons
	ManageUser.grid.on('click', '.btn-action', function (e) {
		var $this = $(this),
			parent = $this.parent(),
			dropdown = parent.find('.actions');

		if (parent.hasClass('active')) {
			parent.removeClass('active');
			dropdown.hide();
		} else {
			ManageUser.grid.find('.active').removeClass('active').find('.actions').hide();
			dropdown.css({ top: -(parent.height() + 10), right: parent.width() * 0.8 }).show();
			parent.addClass('active');
		}

		e.stopPropagation();
	});

	// hides action-panel when clicking outside the action buttons
	$('body, header, #headline, #body').on('click', function () {
		ManageUser.grid.find('.action-wrp.active').removeClass('active').find('.actions').hide();
	});

	// shows loading on change page
	ManageUser.form.submit(function () {
		var username = $('#Username').val();
		var siteId = $('#SiteId').val();
		var groupId = $('#GroupId').val();

		if (siteId === -9999 && username === '' && groupId === '') {
			$('#Username').addClass('input-validation-error');
			return false;
		}

		NAP.loadingBox.show();
	});

	// changes items/page
	ManageUser.grid.on('change', '.items-per-page', function () {
		var $this = $(this),
			pageSize = $this.val(),
			username = $('#hdnUsename').val(),
			groupId = $('#GroupId').val(),
			ddlSite = $('#SiteId');

		var url = NAP.resolveClientUrl('User/Manage' +
			'?q=' + username +
			'&groupid=' + groupId +
			'&size=' + pageSize);

		if (ddlSite.length > 0) {
			url = url + '&siteid=' + ddlSite.val();
		}

		NAP.loadingBox.show();
		window.location.replace(url);
	});
});