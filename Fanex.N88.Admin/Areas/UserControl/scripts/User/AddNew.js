/* global NAP: false */

"use strict";

var AddNewUser = (function ($) {
	var domElement = {
		UserType: "#ViewModel_IsLicenseeUser",
		Site: "#ViewModel_SiteId",
		Username: "#ViewModel_Username",
		Group: "#ViewModel_GroupId",
		Prefix: ".username-prefix",
		PrefixInputClass: "input-prefix"
	};

	function initEventChangeGroupType() {
		$(domElement.UserType).on('change', function () {
			var value = $(this).val();
			if (value === "true") {
				getSiteList(value);
				$(domElement.Site).removeAttr("disabled").removeAttr("readonly");
				$(domElement.Prefix).hide();
				$(domElement.Username).removeClass(domElement.PrefixInputClass);
			} else {
				getGroupBySiteId(false);
				$(domElement.Site).html('');
				$(domElement.Site).attr("disabled", "disabled").attr("readonly", "readonly");
				$(domElement.Prefix).show();
				$(domElement.Username).addClass(domElement.PrefixInputClass);
			}
		});
	}

	function initEventChangeSite() {
		$(domElement.Site).on('change', function () {
			getGroupBySiteId(true);
		});
	}

	function getSiteList(isLicensee) {
		$.ajax({
			url: NAP.resolveClientUrl("Site/GetSitesByType" + "?IsLicensee=" + isLicensee),
			type: 'GET',
			contentType: 'application/json',
			dataType: 'json',
			cache: false,
			beforeSend: function () {
				NAP.loadingBox.show();
			},
			complete: function () {
				NAP.loadingBox.close();
			},
			success: function (sites) {
				var options = generateOptions(sites);
				$(domElement.Site).html(options);
				getGroupBySiteId(isLicensee);
			}
		});
	}

	function getGroupBySiteId(isLicensee) {
		let siteId = $(domElement.Site).val();
		console.log(siteId);
		$.ajax({
			url: NAP.resolveClientUrl("Group/GetGroupByTypeAndSite?IsLicensee=" + isLicensee + "&SiteId=" + siteId),
			type: 'GET',
			contentType: 'application/json',
			dataType: 'json',
			cache: false,
			beforeSend: function () {
				NAP.loadingBox.show();
			},
			complete: function () {
				NAP.loadingBox.close();
			},
			success: function (data) {
				var options = generateOptions(data);
				$(domElement.Group).html(options);
			}
		});
	}
	function generateOptions(sites) {
		let options = '';
		for (var i = 0; i < sites.length; i++) {
			options += `<option value="` + sites[i].Id + `">` + sites[i].Name + `</option>`;
		}
		return options;
	}

	function init() {
		initEventChangeSite();
		initEventChangeGroupType();
	}
	return {
		init: init
	};
})(jQuery);

$(document).ready(function () {
	AddNewUser.init();
});