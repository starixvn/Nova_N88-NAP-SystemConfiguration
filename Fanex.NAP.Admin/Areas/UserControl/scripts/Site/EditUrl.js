/* global NAP: false */

"use strict";

var EditSiteUrl = (function ($) {

    return {
        grid: null,
        btnAddUrl: null,
        txtAddUrl: null,
        chkAddUrl: null,
        btnApplyHandler: function () {
            var $this = $(this),
				btnCancel = $this.siblings('.btn-cancel'),
				btnEdit = $this.siblings('.btn-edit'),
				row = $this.parents('tr'),
				txt = row.find('input[type=text]:first'),
				chk = row.find('input[type=checkbox]:first'),
				siteUrlId = $this.attr('suid'),
				url = $.trim(txt.val()),
				isActive = chk.is(':checked'),
				siteUrlEnvironment = row.find('[name=site-url-environment]').val() || '';


            // checks url duplication
            var otherRows = row.siblings('tr'),
				otherUrls = otherRows.find('input[type=text]'),
				isUrlDuplicated = false;

            otherUrls.each(function () {
                if ($.trim($(this).val()) === url) {
                    NAP.headlineMsgBox.showErr("Urls must be unique!");
                    txt.addClass('input-validation-error');
                    $(this).addClass('input-validation-error');

                    return isUrlDuplicated === true;
                }
            });

            if (isUrlDuplicated) { return false; }

            $.ajax({
                url: NAP.resolveClientUrl('Site/UpdateSiteUrl'),
                type: 'POST',
                data: { id: siteUrlId, url: NAP.encodeURI(url), isActive: isActive, siteUrlEnvironment: siteUrlEnvironment },
                cache: false,
                beforeSend: function () {
                    NAP.loadingBox.show();
                },
                success: function (result) {
                    if (result.toLowerCase() === 'duplicate') {
                        NAP.headlineMsgBox.showErr("Urls must be unique!");
                    }
                    if (result.toLowerCase() === 'invalid') {
                        NAP.headlineMsgBox.showErr("Urls must be valid!");
                    }
                    if (result.toLowerCase() === 'error') {
                        NAP.headlineMsgBox.showErr("Unknowned error detected!");
                    }
                    if (result.toLowerCase() === 'ok') {
                        NAP.headlineMsgBox.showInfo("Your changes were saved!", { closeBtn: false });
                        location.reload();
                    }
                },
                complete: function () {
                    NAP.loadingBox.close();
                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });

            return false;
        }
    };

}(jQuery));

$(document).ready(function () {
    EditSiteUrl.grid = $('.grid');
    EditSiteUrl.btnAddUrl = $('#btn-add-url');
    EditSiteUrl.txtAddUrl = $('#txt-add-url');
    EditSiteUrl.chkAddUrl = $('#chk-add-url');

    EditSiteUrl.grid.find('tr:not(:last) input[type=text], tr:not(:last) input[type=checkbox]').attr('disabled', true);

    EditSiteUrl.grid.on('click', '.btn-edit', function () {
        var $this = $(this),
			thisRow = $this.parents('tr'),
			thisTxt = thisRow.find('input[type=text]'),
			thisChk = thisRow.find('input[type=checkbox]'),
			thisBtnApply = $this.next('.btn-apply'),
			thisBtnCancel = thisBtnApply.next('.btn-cancel');

        thisTxt.attr({ 'disabled': false, 'ori-value': thisTxt.val() }).focus().select(); // enables textbox and set focus on it
        thisChk.attr({ 'disabled': false, 'ori-value': thisChk.is(':checked') });
        thisRow.find('.site-url-environment-text').hide();
        thisRow.find('[name=site-url-environment]').show();

        $this.hide(); // hides edit button

        // shows cancel button and bind the click event, which will cancel the process
        thisBtnCancel.show().click(function () {
            thisTxt.val(thisTxt.attr('ori-value')).attr('disabled', true);
            if (thisChk.attr('ori-value') === "true") {
                thisChk.attr({ 'disabled': true, 'checked': Boolean(thisChk.attr('ori-value')) });
            }
            else {
                thisChk.removeAttr('checked');
                thisChk.attr({ 'disabled': true });
            }
            $this.show();
            thisBtnApply.hide();
            $(this).hide();
        });

        // shows apply button and bind the click event, which will updates the site url
        thisBtnApply.show().click(EditSiteUrl.btnApplyHandler);
        // apply changes when users press enter
        thisTxt.keypress(function (event) {
            if (event.which === 13) {
                EditSiteUrl.btnApplyHandler.apply(thisBtnApply);
            }
        });

    });

    EditSiteUrl.grid.on('mousedown', 'input[type=radio]', function () {
        var $this = $(this);

        if ($this.is(':checked')) {
            return false;
        }

        NAP.headlineMsgBox.confirm('Are you sure?', function () {
            $this.attr('checked', true);
            var siteUrlEnvironment = $this.parents('tr').find('[name=site-url-environment]').val() || '';
            $.ajax({
                url: NAP.resolveClientUrl('Site/SetDefaultSiteUrl'),
                type: 'POST',
                data: { id: $this.attr('suid'), siteUrlEnvironment: siteUrlEnvironment },
                cache: false,
                beforeSend: function () {
                    NAP.loadingBox.show();
                },
                success: function (result) {
                    if (result.toLowerCase() === 'error') {
                        NAP.headlineMsgBox.showErr("Unknowned error detected!");
                    }
                    if (result.toLowerCase() === 'ok') {
                        NAP.headlineMsgBox.showInfo("Your changes were saved!", { closeBtn: false });
                        $this.attr("name", "Url-" + siteUrlEnvironment);
                    }
                },
                complete: function () {
                    NAP.loadingBox.close();
                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        }, { modal: true, closeBtn: false });
    });

    EditSiteUrl.btnAddUrl.click(function () {
        var $this = $(this),
			row = $this.parents('tr'),
			chk = EditSiteUrl.chkAddUrl,
			txt = EditSiteUrl.txtAddUrl,
			siteUrlEnvironment = row.find('#site-url-environment').val() || '';



        txt.removeClass('input-validation-error');
        txt.val($.trim(txt.val()));

        // checks if url is blank
        if (txt.val().length === 0) {
            txt.addClass('input-validation-error');
            NAP.headlineMsgBox.showErr('URL cannot be blank!');

            return false;
        }

        var siteId = $this.attr('sid'),
			url = NAP.encodeURI(txt.val()),
			isActive = chk.is(':checked');

        $.ajax({
            url: NAP.resolveClientUrl('Site/AddSiteUrl'),
            type: 'POST',
            data: { siteId: siteId, url: url, isActive: isActive, siteUrlEnvironment: siteUrlEnvironment },
            cache: false,
            beforeSend: function () {
                NAP.loadingBox.show();
            },
            success: function (result) {
                if (result.toLowerCase() === 'duplicate') {
                    NAP.headlineMsgBox.showErr("Urls must be unique!");
                    txt.addClass('input-validation-error').focus().select();

                    return false;
                }
                if (result.toLowerCase() === 'invalid') {
                    NAP.headlineMsgBox.showErr("Urls must be valid!");
                    txt.addClass('input-validation-error').focus().select();

                    return false;
                }
                if (result.toLowerCase() === 'error') {
                    NAP.headlineMsgBox.showErr("Unknowned error detected!");
                }

                NAP.headlineMsgBox.showInfo("Url added!", { closeBtn: false });

                location.reload();
            },
            complete: function () {
                NAP.loadingBox.close();
            },
            error: function (xhr, status, err) {
                NAP.handleAjaxError(xhr, status, err);
            }
        });
    });
});