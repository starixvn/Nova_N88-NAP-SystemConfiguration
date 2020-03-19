/* global NAP: false */

"use strict";

var AddNewGroup = (function ($) {
	var domElement = {
		GroupType: "#IsLicenseeGroup",
		Site: "#SiteId",
		GroupName: "#GroupName",
		Prefix: ".username-prefix",
		PrefixInputClass: "input-prefix"
	};

	function initEventChangeGroupType() {
		$(domElement.GroupType).on('change', function () {
			var value = $(this).val();
			if (value === "true") {
				reloadSiteList(value);
				$(domElement.Site).removeAttr("disabled").removeAttr("readonly");
				$(domElement.Prefix).hide();
				$(domElement.GroupName).removeClass(domElement.PrefixInputClass);
			} else {
				$(domElement.Site).attr("disabled", "disabled").attr("readonly", "readonly");
				$(domElement.Prefix).show();
				$(domElement.GroupName).addClass(domElement.PrefixInputClass);
			}
		});
	}

	function reloadSiteList(isLicensee) {
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
				var options = generateSiteOptions(sites);
				console.log(options);
				$(domElement.Site).html(options);
			}
		});
	}

	function generateSiteOptions(sites) {
		let options = '';
		for (var i = 0; i < sites.length; i++) {
			options += `<option value="` + sites[i].Id + `">` + sites[i].Name + `</option>`;
		}
		return options;
	}

	return {
		init: initEventChangeGroupType
	};
})(jQuery);

$(document).ready(function () {
	AddNewGroup.init();
});