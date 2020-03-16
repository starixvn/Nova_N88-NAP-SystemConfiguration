/* File Created: July 16, 2014 */
/* Keep login being a pure javascript page only */
/* global CryptoJS */
'use strict';

var msg, innerMsg, username, password, btnSubmit;

function shake(target, times, duration) {
    for (var i = times, j = 0; i >= 0; i--) {
        setTimeout(target + '.style.left = "-' + i + '0px"', j++ * duration);
        setTimeout(target + '.style.left = "' + i + '0px"', j++ * duration);
    }
}

// Returns the version of Internet Explorer or -1
// (indicating the use of another browser).
function getIeVersion() {
    var version = -1; // Return value assumes failure.

    if (window.navigator.userAgent.indexOf('MSIE') > 0) {
        var usrAgent = navigator.userAgent;
        var reg = new RegExp('MSIE ([0-9]{1,}[.0-9]{0,})');

        if (reg.exec(usrAgent) != null) {
            version = parseFloat(RegExp.$1);
        }
    }

    return version;
}

function checkBrowser() {
    var msieVersion = getIeVersion();

    if (msieVersion !== -1 && msieVersion < 8.0) {
        var warnPnl = document.getElementById('divWarningPanel');

        if (warnPnl.style.display === '' || warnPnl.style.display === 'none') {
            warnPnl.style.display = 'block';
        }
    }
}

window.onload = function () {
    /* jshint ignore: start */
    setTimeout('window.history.forward()', 0);
    /* jshint ignore: end */

    msg = document.getElementById('divMessage');
    innerMsg = document.getElementById('divInnerMsg');
    username = document.getElementById('txtUsername');
    password = document.getElementById('txtPassword');
    btnSubmit = document.getElementById('btnSubmit');

    checkBrowser(); // check if the client's browser is supported

    if (innerMsg.innerHTML.length > 0) {
        msg.style.display = 'block';
        shake('divMessage', 5, 100);
    }
    else {
        msg.style.display = 'none';
    }
};

document.forms[0].onsubmit = function () {
    msg.style.display = 'none';

    var isValid = true;

    username.value = username.value.trim();

    if (username.value.length === 0) {
        username.className += ' err';
        isValid = false;
        shake('txtUsername', 5, 100);
    } else {
        username.className = username.className.replace('err', '');
    }

    if (password.value.length === 0) {
        password.className += ' err';

        isValid = false;
        shake('txtPassword', 5, 100);
    } else {
        var loginToken = document.getElementById('hdnLoginToken').value;
        var encryptedPwd = CryptoJS.CfsEnCode(password.value);
        var hashPwd = '';

        password.className = password.className.replace('err', '');
        hashPwd = CryptoJS.SHA256(encryptedPwd + loginToken);
        document.getElementById('hdnPassword').value = hashPwd;
        console.log(hashPwd);
    }

    return isValid;
};

/* jshint freeze: false */
String.prototype.trim = function () {
    return this.replace(/^\s*/, '').replace(/\s*$/, '');
};