/* jshint freeze: false */

"use strict";

var NAP = (function ($) {
    return {
        NAPAdminRoot: null,
        NAPRoot: null,
        dialogMsg: null,
        dialogPopup: null,
        headlineMsgBox: null,
        loadingBox: null,
        alertCallback: null,
        confirmCallback: null,
        msieVersion: null,
        agentIsOldIe: null,
        agentIsApple: null,
        agentIsMobile: null,
        hasHouseWLByProductsPermission: null,
        hasCN88WLByProductsPermission: null,

        initCoreValues: function () {
            NAP.NAPAdminRoot = $('#NapAdminRoot').val();
            NAP.NAPRoot = $('#NapRoot').val();
            NAP.dialogPopup = $('<div id="dialog-popup"></div>').appendTo('body');

            NAP.msieVersion = NAP.getIeVersion();
            NAP.agentIsOldIe = NAP.msieVersion !== -1 && NAP.msieVersion < 9.0;
            NAP.agentIsApple = NAP.agentIs('apple') != null;
            NAP.agentIsMobile = NAP.agentIs('mobile') != null;
        },
        resolveClientUrl: function (relativeUrl) {
            return NAP.NAPAdminRoot + relativeUrl;
        },
        resolveRootUrl: function (relativeUrl) {
            return NAP.NAPRoot + relativeUrl;
        },
        // Handles common error when making ajax request
        handleAjaxError: function (xhr) {
            if (xhr.status === 403) {
                NAP.headlineMsgBox.showErr('Unauthorized request rejected!', { sticky: true });
            } else if (xhr.status === 404) {
                NAP.headlineMsgBox.showErr('Resources not found!', { sticky: true });
            } else {
                NAP.headlineMsgBox.showErr('An unexpected error has occurred!', { sticky: true });
            }
        },
        /* jshint ignore:start */
        getIeVersion: function () {
            var version = -1; // Return value assumes failure.

            if (window.navigator.userAgent.indexOf("MSIE") > 0) {
                var usrAgent = navigator.userAgent;
                var reg = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");

                if (reg.exec(usrAgent) != null) {
                    version = parseFloat(RegExp.$1);
                }
            }

            return version;
        },
        /* jshint ignore:end */
        agentIs: function (name) {
            switch (name.toLowerCase()) {
                case 'ipad':
                    return navigator.userAgent.match(/iPad/i);
                case 'iphone':
                    return navigator.userAgent.match(/iPhone/i);
                case 'ipod':
                    return navigator.userAgent.match(/iPod/i);
                case 'apple':
                    return navigator.userAgent.match(/iPad/i) ||
						navigator.userAgent.match(/iPhone/i) ||
						navigator.userAgent.match(/iPod/i);
                case 'mobile':
                    return navigator.userAgent.match(/iPad/i) ||		// Apple
						navigator.userAgent.match(/iPhone/i) ||
						navigator.userAgent.match(/iPod/i) ||
						navigator.userAgent.match(/BlackBerry/i) ||		// RIM
						navigator.userAgent.match(/PlayBook/i) ||
						navigator.userAgent.match(/Windows Phone/i) ||	// Windows Phone
						navigator.userAgent.match(/Android/i) ||		// Android
						navigator.userAgent.match(/Touch/i);			// General
                default:
                    return null;
            }
        },
        popup: function (title, html) {
            NAP.dialogPopup.html(html).dialog({
                width: 'auto',
                modal: true,
                position: ['center', 160]
            });

            NAP.dialogPopup.dialog('option', { title: title });
        },
        closePopup: function () {
            NAP.dialogPopup.dialog('close');
        },
        redirect: function (url, msec) {
            setTimeout('NAP.navigateTo("' + url + '")', msec);
        },
        navigateTo: function (url) {
            window.location.replace(NAP.resolveClientUrl(url));
        },
        navigateToRoot: function (url) {
            window.location.replace(NAP.resolveRootUrl(url));
        },
        htmlEncode: function (value) {
            return $('<div/>').text(value).html();
        },
        htmlDecode: function (value) {
            return $('<div/>').html(value).text();
        },
        showSuccessMsg: function (msg, url, msec) {
            var msgWrp = $('.validation-summary-valid');

            msgWrp.addClass('success').find('li:first').html(NAP.htmlDecode(msg)).show();

            NAP.showSuccessMsgCountDown(url, msec / 1000);
        },
        showSuccessMsgCountDown: function (url, msec) {
            if (msec-- > 0) {
                setTimeout('NAP.showSuccessMsgCountDown("' + url + '", ' + msec + ')', 1000);
                $('.validation-summary-valid .msec').html(msec);
            } else {
                window.location.replace(NAP.resolveClientUrl(url));
            }
        },

        encodeURI: function (str) {
            // ref: https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/encodeURI
            return encodeURI(str).replace(/%5B/g, '[').replace(/%5D/g, ']');
        }
    };
}(jQuery));

$(document).ready(function () {
    NAP.initCoreValues();

    NAP.headlineMsgBox = $.msgbox({
        message: '',
        //containerId: 'msg-bar',
        type: 'err',
        modal: false,
        position: ['top:130', 'center'],
        theme: '',
        zIndex: 9999,
        useAnimation: false
    });

    NAP.loadingBox = $.msgbox({
        message: 'Loading...',
        type: 'warn',
        modal: true,
        position: ['top:0', 'center'],
        closeBtn: false,
        theme: '',
        zIndex: 9999,
        sticky: true,
        useAnimation: false
    });

    NAP.hasHouseWLByProductsPermission = function (table, currentElement) {
        var houseWLByProductsFuncId = table
         .find("tr:contains('House Win Loss By Products - Product Permission') input").val();

        if (currentElement.attr('pid') === houseWLByProductsFuncId || currentElement.val() === houseWLByProductsFuncId) {
            var hasPermissionIsHouseWLByProducts = table
                .find('[type=checkbox][pid=' + houseWLByProductsFuncId + '][value!=' + currentElement.val() + ']')
                .filter(':checked').length > 0;

            if (currentElement.is(':checked') && (hasPermissionIsHouseWLByProducts || currentElement.val() === houseWLByProductsFuncId)) {
                NAP.headlineMsgBox.showErr('You must select ONLY ONE function House Win Loss by Products - Product Permission');
                currentElement.attr('checked', false);
                return true;
            }
        }
        return false;
    }

    NAP.hasCN88WLByProductsPermission = function (table, currentElement) {
        var cn88WLByProductsFuncId = table
            .find("tr:contains('CN88 Win Loss By Products - Product Permission') input").val();

        if (currentElement.attr('pid') === cn88WLByProductsFuncId || currentElement.val() === cn88WLByProductsFuncId) {
            var hasPermissionIsCN88WLByProducts = table
                .find('[type=checkbox][pid=' + cn88WLByProductsFuncId + '][value!=' + currentElement.val() + ']')
                .filter(':checked').length > 0;

            if (currentElement.is(':checked') && (hasPermissionIsCN88WLByProducts || currentElement.val() === cn88WLByProductsFuncId)) {
                NAP.headlineMsgBox.showErr('You must select ONLY ONE function CN88 Win Loss by Products - Product Permission');
                currentElement.attr('checked', false);
                return true;
            }
        }
        return false;
    }
});

// -- Utilities -- //

// String Format
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] !== 'undefined' ? args[number] : match;
    });
};

// Array Remove - By John Resig (MIT Licensed)
Array.prototype.remove = function (from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};