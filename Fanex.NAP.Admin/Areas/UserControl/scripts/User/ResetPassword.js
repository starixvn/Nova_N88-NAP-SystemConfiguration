/* File Created: July 21, 2014 */
/* global CryptoJS */
'use strict';

var ResetPassword = function () {
    return {
        mainForm: null,
        userPwd: null,
        confirmUserPwd: null,
        hiddenForm: null,
        newPassword: null,
        confirmPassword: null
    };
};

$(document).ready(function () {
    ResetPassword.mainForm = $('#mainForm');
    ResetPassword.userPwd = $('#UserPwd');
    ResetPassword.confirmUserPwd = $('#ConfirmUserPwd');
    ResetPassword.hiddenForm = $('#hiddenForm');
    ResetPassword.newPassword = $('#newPassword');
    ResetPassword.confirmPassword = $('#confirmPassword');

    ResetPassword.mainForm.on('submit', function () {
        if ($(this).valid()) {
            var encryptPass = '';

            encryptPass = CryptoJS.CfsEnCode(ResetPassword.userPwd.val());
            ResetPassword.newPassword.val(encryptPass);
            encryptPass = CryptoJS.CfsEnCode(ResetPassword.confirmUserPwd.val());
            ResetPassword.confirmPassword.val(encryptPass);
     
            ResetPassword.hiddenForm.submit();

            return false;
        }
    });
});