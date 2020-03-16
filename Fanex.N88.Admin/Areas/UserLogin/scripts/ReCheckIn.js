"use strict";

var Recheckin = (function ($) {

	return {
		time: null,
		root: null,
		headlineMsgBox: null,
		kickedOutMsg: null,

		init: function () {
			Recheckin.time = 5000;
			Recheckin.root = location.protocol + '//' + location.host + $('#NapRoot').val();
			Recheckin.kickedOutMsg = 'You have logged in from another session or your login session has expired!';
		},

		reCheckin: function () {

			$.ajax({
				url: Recheckin.root + "UserLogin/Login/ReCheckLogin",
				type: "POST",
				dataType: "json",
				success: function (result) {
					if (result === "1") {
						Recheckin.headlineMsgBox.showInfo(Recheckin.kickedOutMsg, { closeBtn: false });
					}
					if (result === "-1") {
						window.location.href = Recheckin.root + "UserLogin/Common/Offline";
					}
				}
			});
		},

		closeBrowser: function () {
			var index = window;
			while (index !== index.parent) // Jump out
			{
				index = index.parent;
			}
			index.close();
		}
	};

} (jQuery));


$(document).ready(function () {
	Recheckin.init();

	Recheckin.headlineMsgBox = $.msgbox({
		message: '',
		//containerId: 'msg-bar',
		type: 'err',
		modal: false,
		position: ['top:130', 'center'],
		theme: '',
		zIndex: 9999,
		duration: 2000,
		useAnimation: false,
		beforeClose: function () {
			window.location.href = Recheckin.root + "UserLogin/Login/LogOut";
		}
	});

	setInterval(Recheckin.reCheckin, Recheckin.time);
});