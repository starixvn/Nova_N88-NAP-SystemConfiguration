/* global NAP: false */

"use strict";

var EditSite = (function ($) {

    return {
        form: null,
        parentId: null,
        parentName: null,
        parent: null,

        getSiteKey: function () {
            var siteId = $('#SiteId').val();
            var siteName = $('#SiteName').val();

            $.ajax({
                url: NAP.resolveClientUrl("Site/GetSiteKey" + "?siteId=" + siteId + "&siteName=" + siteName),
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                cache: false,
                beforeSend: function () {
                    //NAP.loadingBox.show();
                },
                complete: function () {
                    //NAP.loadingBox.close();
                },
                success: function (result) {
                    var template;

                    switch (result) {
                        case 0:
                            $('#error').append("");
                            var functionName = functionName || 'Not defined';
                            template = '<div data-valmsg-summary="true" class="validation-summary-errors"><ul><li>' +
								functionName + ' already exists. Please choose another!</li></ul></div>';

                            $('#error').append(template);
                            $('#SiteName').addClass('input-validation-error');
                            break;

                        case -1:
                            break;

                        case -2:
                            $('#error').append("");

                            template = '<div data-valmsg-summary="true" class="validation-summary-errors"><ul><li>' +
								result + ' already exists. Please choose another!</li></ul></div>';

                            $('#error').append(template);
                            $('#SiteName').addClass('input-validation-error');
                            break;

                        default:
                            $('#txtSiteKey').removeAttr('disabled');
                            $('#SiteKey').val(result);
                            $('#txtSiteKey').val(result);
                            $('#txtSiteKey').attr('disabled', 'disabled');
                    }

                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        },

        getSecretAccessKey: function () {
            $.ajax({
                url: NAP.resolveClientUrl("Site/GetSecretAccessKey"),
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                cache: false,
                beforeSend: function () {
                    //NAP.loadingBox.show();
                },
                complete: function () {
                    //NAP.loadingBox.close();
                },
                success: function (result) {
                    $('#txtAccessSecretKey').removeAttr('disabled');
                    $('#SecretAccessKey').val(result);
                    $('#txtAccessSecretKey').val(result);
                    $('#txtAccessSecretKey').attr('disabled', 'disabled');

                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        }
    };

}(jQuery));

$(document).ready(function () {
    EditSite.form = $('form:first');

    $('#lbtn-edit-siteKey').click(function () {
        var msg = "Do you want to regenerate site key ?";
        NAP.headlineMsgBox.confirm(msg, function () {
            EditSite.getSiteKey();
        }, { modal: true, closeBtn: false });
    });

    $('#lbtn-edit-accessKey').click(function () {
        var msg = "Do you want to regenerate secret access key ?";
        NAP.headlineMsgBox.confirm(msg, function () {
            EditSite.getSecretAccessKey();
        }, { modal: true, closeBtn: false });
    });
});