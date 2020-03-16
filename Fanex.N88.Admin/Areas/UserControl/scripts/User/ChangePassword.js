/* File Created: July 17, 2014 */
/* global CryptoJS */
'use strict';

var ChangePassword = function () {
    return {
        mainForm: null,
        oldUserPwd: null,
        userPwd: null,
        confirmUserPwd: null,
        hiddenForm: null,
        token: null,
        oldPassword: null,
        newPassword: null,
        confirmPassword: null
    };
};

$(document).ready(function () {
    ChangePassword.mainForm = $('#mainForm');
    ChangePassword.oldUserPwd = $('#OldUserPwd');
    ChangePassword.userPwd = $('#UserPwd');
    ChangePassword.confirmUserPwd = $('#ConfirmUserPwd');
    ChangePassword.hiddenForm = $('#hiddenForm');
    ChangePassword.token = $('#token');
    ChangePassword.oldPassword = $('#oldPassword');
    ChangePassword.newPassword = $('#newPassword');
    ChangePassword.confirmPassword = $('#confirmPassword');

    ChangePassword.mainForm.on('submit', function () {
        if ($(this).valid()) {
            var encryptPass = '';
            var token = ChangePassword.token.val();

            encryptPass = CryptoJS.CfsEnCode(ChangePassword.oldUserPwd.val());
            ChangePassword.oldPassword.val(CryptoJS.SHA256(encryptPass + token));
            encryptPass = CryptoJS.CfsEnCode(ChangePassword.userPwd.val());
            ChangePassword.newPassword.val(encryptPass);
            encryptPass = CryptoJS.CfsEnCode(ChangePassword.confirmUserPwd.val());
            ChangePassword.confirmPassword.val(encryptPass);

            ChangePassword.hiddenForm.submit();

            return false;
        }
    });
});