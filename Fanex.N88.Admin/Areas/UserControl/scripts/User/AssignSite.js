/* global NAP: false */

"use strict";

// NOTE: in this document
//  + assigner indicates the current-logged-in user
//  + assignee indicates the the user assigned privileges to

var AssignSite = (function ($) {
    return {
        userId: null,
        returnUserName: null,
        returnSiteId: null,
        returnGroupId: null,
        returnPageIndex: null,
        form: null,
        grid: null,
        hasAssignSiteRight: null,

        UpdateSiteToUser: function (userId, siteId, level, isActive, expiredDate, isNeverExpired) {
            var url = NAP.resolveClientUrl('User/UpdateSiteToUser'),
				data = {
				    userId: userId,
				    siteId: siteId,
				    level: level,
				    isActive: isActive,
				    expiredDate: expiredDate,
				    isNeverExpired: isNeverExpired
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
                    if (result === "OK") {
                        NAP.headlineMsgBox.showInfo('Your changes were saved!', { closeBtn: false });
                    }
                    else if (result === "ERROR") {
                        NAP.redirect('User/AssignSite/' + userId + '?returnUserName=' + AssignSite.returnUserName + '&returnSiteId=' + AssignSite.returnSiteId +
                                    '&returnPageIndex=' + AssignSite.returnPageIndex + '&returnGroupId=' + AssignSite.returnGroupId, 800);
                    }
                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        },

        InitDateTimePicker: function (thisDateTimePicker) {
            thisDateTimePicker.datepicker({
                defaultDate: "1",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                dateFormat: "dd M yy",
                yearRange: "2016:2030",
                showOn: "button",
                buttonImage: $('#imagePath').val(),
                buttonText: "Expired Date",
                buttonImageOnly: true,
                onSelect: function () {
                }
            });
        },
    };
}(jQuery));

$(document).ready(function () {
    AssignSite.grid = $('.grid');
    AssignSite.userId = $('#UserId').val();
    AssignSite.returnUserName = $('#ReturnUserName').val();
    AssignSite.returnSiteId = $('#ReturnSiteId').val();
    AssignSite.returnGroupId = $('#ReturnGroupId').val();
    AssignSite.returnPageIndex = $('#ReturnPageIndex').val();
    AssignSite.hasAssignSiteRight = $('#HasAssignSiteRight').val().toLowerCase();

    if (AssignSite.hasAssignSiteRight === 'true') {
        AssignSite.grid.on('click', '.btn-edit', function () {
            var $this = $(this),
				thisRow = $this.parents('tr'),
				thisSelect = thisRow.find('select'),
				thisLevelLabel = thisSelect.prev(),
				thisChecked = thisRow.find('.chbSite'),
				originalChecked = thisChecked.is(':checked'),
				originalDisabledChecked = thisChecked.is(':disabled'),
				thisBtnApply = $this.next('.btn-apply'),
				thisBtnCancel = thisBtnApply.next('.btn-cancel'),
			    thisExpiredDatePickerArea = thisRow.find('.expiredDatePickerArea'),
                thisExpiredDateLabel = thisExpiredDatePickerArea.prev(),
                thisTxtExpiredDatePicker = thisExpiredDatePickerArea.find('.txtExpiredDatePicker'),
                thisCbNeverExpired = thisExpiredDatePickerArea.find('.cbNeverExpired');

            thisLevelLabel.hide();
            thisSelect.show();   // enables select level
            $this.hide();        // hides edit button
            thisChecked.attr('disabled', false);

            // shows cancel button and bind the click event
            thisBtnCancel.fadeIn().click(function () {
                if (thisChecked.is(':checked') !== originalChecked) {
                    thisChecked.attr('checked', originalChecked);
                }

                showHide($(this), thisBtnApply);
                thisChecked.attr('disabled', originalDisabledChecked);
                var level = thisChecked.val();
                thisSelect.find('option[value=' + level + ']').attr('selected', true);
            });

            // shows apply button and bind the click event
            thisBtnApply.fadeIn().click(function () {
                var siteId = thisChecked.attr('id').substr(8),
					level = thisSelect.val(),
					isActive = thisChecked.is(':checked'),
					levelName = thisSelect.find(':selected').text(),
					expiredDate = thisTxtExpiredDatePicker.val(),
                    isNeverExpired = thisCbNeverExpired.is(':checked');

                thisLevelLabel.text(levelName);
                thisChecked.val(level);

                if (isNeverExpired || (expiredDate === '' || typeof (expiredDate) === "undefined")) {
                    isNeverExpired = true;
                    expiredDate = new Date().toDateString('d MM y');
                    thisExpiredDateLabel.text('Never Expired');
                }
                else {
                    thisExpiredDateLabel.text(expiredDate);
                }

                AssignSite.UpdateSiteToUser(AssignSite.userId, siteId, level, isActive, expiredDate, isNeverExpired);
                showHide($(this), thisBtnCancel);
            });

            thisExpiredDatePickerArea.show(1, function () {
                AssignSite.InitDateTimePicker(thisTxtExpiredDatePicker);
                var expiredDataLblText = thisExpiredDateLabel.text().trim();

                if (!isNaN(Date.parse(expiredDataLblText))) {
                    thisTxtExpiredDatePicker.val(expiredDataLblText);
                }
                else {
                    thisCbNeverExpired.attr('checked', true);
                    thisTxtExpiredDatePicker.val("");
                    thisTxtExpiredDatePicker.datepicker("disable");
                }

                thisExpiredDateLabel.hide();
            });

            thisCbNeverExpired.click(function () {
                if (thisCbNeverExpired.attr('checked')) {
                    thisTxtExpiredDatePicker.val('');
                    thisTxtExpiredDatePicker.datepicker('disable');
                }
                else {
                    thisTxtExpiredDatePicker.datepicker('enable');
                }
            });

            var showHide = function (currentBtn, otherBtn) {
                thisSelect.hide();
                thisLevelLabel.show();
                thisExpiredDatePickerArea.hide();
                thisExpiredDateLabel.show();
                otherBtn.fadeOut();
                currentBtn.fadeOut(function () { $this.show(); });
            };
        });

        AssignSite.grid.on('mousedown', '.chbSite', function () {
            var $this = $(this);
            var thisIsChecked = $this.is(':checked')
            var isEnableEdit = $this.parents('tr').find('.btn-edit').is(':hidden');

            if (isEnableEdit) {
                $this.attr('checked', thisIsChecked);
                return;
            }

            if (!$this.is(':disabled')) {
                var siteName = $this.attr('name'),
					siteId = $this.attr('id').substr(8),
					level = $this.val(),
					isActive = !$this.is(':checked'),
					actionName = isActive ? 'activate' : 'deactivate',
                    expiredDate = new Date().toDateString(),
                    isNeverExpired = false,
					message = "Are you sure you want to " + actionName + " '" + siteName + "' ?";

                //Get expired date
                var thisRow = $this.parents('tr'),
                    thisExpiredDatePickerArea = thisRow.find('.expiredDatePickerArea'),
                    thisExpiredDateLabel = thisExpiredDatePickerArea.prev(),
                    expiredDataLblText = thisExpiredDateLabel.text().trim();

                if (!isNaN(Date.parse(expiredDataLblText))) {
                    expiredDate = expiredDataLblText;
                }
                else {
                    isNeverExpired = true;
                }

                NAP.headlineMsgBox.confirm(message, function () {
                    $this.attr('checked', isActive);
                    AssignSite.UpdateSiteToUser(AssignSite.userId, siteId, level, isActive, expiredDate, isNeverExpired);
                }, { modal: true, closeBtn: false });
            }
        });
    }
});