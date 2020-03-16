/* global NAP: false */

"use strict";

var ManageFunction = (function () {

	return {
		grid: null,
		form: null
	};

} (jQuery));

$(document).ready(function () {
	ManageFunction.grid = $('.grid');
	ManageFunction.form = $('form:first');

	// #region tooltips
	$('.tip').tipsy({ gravity: 'se' });
	$('#headline h1 .lnk').tipsy({ gravity: 'w' });
	$('.grid input[type=checkbox][fid]').tipsy({ gravity: 's' });
	// #endregion

	// #region tree view/ grid view
	$('.tree-grid-toggle').click(function () {
		var findLevel = function ($target) {
			return $target.children('td:first').find('[lvl]').attr('lvl');
		};

		var $this = $(this),
			parent = $this.parents('tr'),
			nextDom = parent.next(),
			thisLevel = findLevel(parent),
			level = nextDom.is('tr') ? findLevel(nextDom) : "",
			thisWillBeCollapsed = !$this.hasClass('collapse'),
			stopCondition;

		if (thisLevel > 0) {
			stopCondition = function (level) { return level > thisLevel; };
		} else {
			thisLevel = 0;
			stopCondition = function (level) { return level; };
		}

		while (nextDom.is('tr') && stopCondition(level)) {
			if (thisWillBeCollapsed) {
				nextDom.hide();
			} else {
				nextDom.show();
				var childToggleNodes = nextDom.find('td:first .tree-grid-toggle');
				if (childToggleNodes.length > 0) {
					childToggleNodes.removeClass('collapse');
				}
			}

			nextDom = nextDom.next();
			if (nextDom.is('tr')) {
				level = findLevel(nextDom);
			}
		}

		$this.toggleClass('collapse');
	});

	$('#btnGridView, #btnTreeView').bind('click', function () {
		var $this = $(this);

		if (!$this.hasClass('active')) {
			if (NAP.agentIsOldIe) {
				$('.grid-mode-only').toggle();
				$('.tree-mode-only').toggle();
				$('.tree-grid-toggle').toggle();
			} else {
				$('.grid-mode-only').toggle('slow');
				$('.tree-mode-only').toggle('slow');
				$('.tree-grid-toggle').toggle('slow');
			}
		}

		$this.addClass('active').siblings().removeClass('active');
	});

	$('#btnGridView').bind('click', function () {
		ManageFunction.grid.find('tr').show('slow');
		$('.tree-grid-toggle').removeClass('collapse');
	});
	// #endregion

	// #region activate/deactivate functions
	ManageFunction.grid.on('mousedown', 'input[type=checkbox][fid]', function () {
		var $this = $(this),
			functionName = $this.attr('fname'),
			active = !$this.is(':checked'),
			action = active ? 'activate' : 'deactivate',
			msg = "Are you sure you want to " + action + " '" + functionName + "' ?";

		NAP.headlineMsgBox.confirm(msg, function () {
			var functionId = $this.attr('fid');

			$this.attr('checked', active).change();

			$.ajax({
				url: NAP.resolveClientUrl("Function/Activate" + "?id=" + functionId + "&isActive=" + active),
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

	// #region redirect to privileged users & groups page
	//    ManageFunction.grid.on('click', '.btn-user-info', function () {
	//        var $this = $(this),
	//			functionid = $this.attr('fid'),
	//			functionName = $this.attr('fname'),
	//			siteId = $('#SiteId').val();

	//        url = NAP.resolveClientUrl("Function/AddPrivilegedGroupsAndUsers?siteid=" + siteId + "&functionId=" + functionid);

	//        window.location.replace(url);

	//        return false;
	//    });
	// #endregion

	// #region shows action-panel when clicking on the action buttons
	ManageFunction.grid.on('click', '.btn-action', function (e) {
		var $this = $(this),
			parent = $this.parent(),
			dropdown = parent.find('.actions');

		if (parent.hasClass('active')) {
			parent.removeClass('active');
			dropdown.hide();
		} else {
			ManageFunction.grid.find('.active').removeClass('active').find('.actions').hide();
			dropdown.css({ top: -(parent.height() + 10), right: parent.width() * 0.8 }).show();
			parent.addClass('active');
		}

		e.stopPropagation();
	});

	// hides action-panel when clicking outside the action buttons
	$('body, header, #headline, #body').on('click', function () {
		ManageFunction.grid.find('.action-wrp.active').removeClass('active').find('.actions').hide();
	});
	// #endregion

	ManageFunction.form.submit(function () {
		NAP.loadingBox.show();
	});
});