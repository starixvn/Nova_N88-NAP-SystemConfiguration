/* global NAP: false */

"use strict";

var AddExisting = (function () {
    return {
        gridWrp: null,
        grid: null,
        formWrp: null,
        form: null,
        txtUsername: null,
        formAdd: null
    };
}(jQuery));

$(document).ready(function () {
    AddExisting.gridWrp = $('#grid-wrp');
    AddExisting.grid = $('.grid');
    AddExisting.formWrp = $('#form-wrp');
    AddExisting.form = $('form:first');
    AddExisting.txtUsername = $('#Username');
    AddExisting.formAdd = $('#dialog-popup');

    AddExisting.gridWrp
	.on('click', '.btn-add-user', function () {
	    var $this = $(this),
			userId = $this.attr('uid'),
			username = $this.attr('uname');

	    NAP.popup("Add '" + username + "'", AddExisting.formWrp.html());
	    NAP.dialogPopup.find('input[type=submit]').attr('uid', userId).attr('uname', username);
	})
	.on('change', '.items-per-page', function () {
	    var $this = $(this),
			pageSize = $this.val(),
			username = $('#hdnUsename').val();

	    var url = NAP.resolveClientUrl('User/AddExisting?q=' + username + '&size=' + pageSize);

	    NAP.loadingBox.show();
	    window.location.replace(url);
	});

    AddExisting.formAdd.on('click', 'input[type=submit]', function () {
        var $this = $(this),
			form = $this.parents('#form'),
			ddlGroup = form.find('select'),
			groupId = ddlGroup.val(),
			userId = $this.attr('uid'),
			username = $this.attr('uname');

        var url = 'User/AddUserToGroup?userid=' + userId + '&groupid=' + groupId;

        var returnUserName = $('#ReturnUserName').val(),
            returnSiteId = $('#ReturnSiteId').val(),
            returnGroupId = $('#ReturnGroupId').val();

        $.ajax({
            url: NAP.resolveClientUrl(url),
            type: 'POST',
            dataType: 'html',
            cache: false,
            beforeSend: function () {
                NAP.loadingBox.show();
                NAP.closePopup();
            },
            success: function (result) {
                NAP.loadingBox.close();
                result = result.toLowerCase();

                if (result === 'error') {
                    NAP.headlineMsgBox.showErr("Unknowned error detected!");
                }
                else {
                    NAP.headlineMsgBox.showInfo("Added '" + username + "'. You will be redirected in 3s.", { closeBtn: false, modal: true });
                    var redirectUrl = 'User/Manage?userName=' + returnUserName
                        + '&groupId=' + returnGroupId
                        + '&siteId=' + returnSiteId;
                    NAP.redirect(redirectUrl, 3000);
                }
            },
            error: function (xhr, status, err) {
                NAP.handleAjaxError(xhr, status, err);
            }
        });
    });

    AddExisting.form.submit(function () {
        var username = AddExisting.txtUsername.val();

        if ($.trim(username).length === 0) {
            NAP.headlineMsgBox.showErr('Username cannot be blank!');

            return false;
        }

        NAP.loadingBox.show();
    });
});