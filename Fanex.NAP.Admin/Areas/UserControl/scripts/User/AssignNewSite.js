"use strict";
/*global NAP: false*/

var AssignNewSiteToUser = (function ($) {
    return {
        userId: null,
        siteSelect: null,
        levelSelect: null,
        btnSubmit: null,
        AssignNewSite: function (userId, siteId, level) {
            var url = NAP.resolveClientUrl('User/AssignSiteToUser'),
				data = {
				    userId: userId,
				    siteId: siteId,
				    level: level,
				    isActive: true,
				};

            $.ajax({
                url: url,
                type: 'POST',
                data: data,
                dataType: 'html',
                cache: false,
                beforeSend: function () {
                    NAP.loadingBox.show(null, { modal: true });
                },
                complete: function () {
                    NAP.loadingBox.close();
                },
                success: function (result) {
                    if (result) {
                        NAP.headlineMsgBox.showInfo('Your changes were saved!', { closeBtn: false });

                        var returnUserName = $('#ReturnUserName').val();
                        var returnSiteId = $('#ReturnSiteId').val();
                        var returnGroupId = $('#ReturnGroupId').val();
                        var returnPageIndex = $('#ReturnPageIndex').val();
                        var redirectUrl = 'User/AssignSite/' + userId
                            + '?returnUserName=' + returnUserName
                            + '&returnSiteId=' + returnSiteId
                            + '&returnPageIndex=' + returnPageIndex
                            + '&returnGroupId=' + returnGroupId;
                        NAP.redirect(redirectUrl, 0);
                    }
                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        }
    };
}(jQuery));

$(document).ready(function () {
    AssignNewSiteToUser.form = $('form:first');
    AssignNewSiteToUser.siteSelect = $('#siteSelect');
    AssignNewSiteToUser.levelSelect = $('#levelSelect');
    AssignNewSiteToUser.btnSubmit = $('#btn-submit');

    AssignNewSiteToUser.form.submit(function () {
        var userId = $('#UserId').val();
        var siteId = AssignNewSiteToUser.siteSelect.val();
        var level = AssignNewSiteToUser.levelSelect.val();
        AssignNewSiteToUser.AssignNewSite(userId, siteId, level);
        return false;
    });
});