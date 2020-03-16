/*
* msgBox - jQuery Plugin
* msgBox is a jQuery plugin that makes managing notifications easier than ever
*
* Examples and documentation at: http://update.later
*
* Version: 0.0.1 (01 Feb 2012)
* Requires: jQuery v1.7+
*/

(function (window, document, $) {

    var str_msgbox = 'msgbox',
        str_msgboxOverlay = 'MsgBoxOverlay',
        agentLtIe9 = $.browser.msie && parseInt($.browser.version, 10) < 9,
        W = $(window),
        D = $(document),
        M = function (target, options) { // MsgBox Class.
            // Public attributes.
            this.target = target;
            this.index = M.numOfMsgBox;
            this.container = null;
            this.overlay = null;
            this.wrap = null;
            this.options = $.extend(true, {
                containerId: null, // id of the container
                message: '', // default message
                type: 'warn', // type of message (info | warn | err)

                width: 'auto',
                height: 'auto',
                minWidth: 'auto',
                minHeight: 'auto',
                maxWidth: 'auto',
                maxHeight: 'auto',
                overflow: 'auto',
                position: ['center', 'center'],
                autoReposition: false, // automatically reposition when window/container resizes.
                useAnimation: true,
                animation: {
                    duration: 330,
                    easing: 'easeOutBack'
                },
                zIndex: 1001,

                duration: 5000, // the lifespan of a non-sticky message on the screen.
                sticky: false, // when set to true, the msgbox will stick to the screen until it is intentionally closed by the user.
                closeBtn: true,
                modal: false,
                escape: true,
                //draggable: false,

                // HTML templates
                tpl: {
                    closeBtn: '<a href="javascript:;" class="msgbox-close"></a>',
                    okBtn: '<a href="javascript:;" class="msgbox-btn ok">OK</a>',
                    cancelBtn: '<a href="javascript:;" class="msgbox-btn cancel">Cancel</a>',
                    textbox: '<input type="text" class="msgbox-txt" />'
                },

                theme: 'chrome', // '' | msgbox-chrome

                // custom logger
                log: function (msg) { },

                // Callbacks
                beforeShow: function (msg, opts) { }, // Before showing
                afterShow: function (msg, opts) { }, // After showing
                beforeClose: function (opts) { }, // Before closing
                afterClose: function (opts) { } // After closing
            }, options),
            this.fixed = this.options.containerId == undefined,
            this.timeout = null;
            //this.dragObject = null;

            // Constructor code.
            this.init();
        };

    M.numOfMsgBox = 0; // number of msgboxes created.

    // PUBLIC METHODS.
    M.prototype = {
        init: function () {
            var _this = this,
                $this = _this.target,
                $body = $('body'),
                defaultOverlay = $('#' + str_msgboxOverlay);

            if (!defaultOverlay.length) defaultOverlay = $('<div id="' + str_msgboxOverlay + '" class="msgbox-overlay default" />').appendTo($body);

            if (_this.fixed) { // msgbox container is the body
                _this.container = $body;
                _this.overlay = defaultOverlay;
                $this.css('position', 'fixed');
            } else { // msgbox container is an existed DOM
                var container = $('#' + _this.options.containerId);

                if (container.length > 0) {
                    _this.container = container;
                    if (_this.container.css('position') != 'absolute') _this.container.css('position', 'relative'); // msgbox container position must be absolute or relative

                    _this.overlay = _this.container.find('.msgbox-overlay:first');
                    if (!_this.overlay.length) _this.overlay = $('<div class="msgbox-overlay" style="z-index: ' + (parseInt(_this.options.zIndex) - 1) + '" />').appendTo(_this.container);
                } else this.log('Container of ' + $this.attr('id') + ' does not exist.', true);

                $this.css('position', 'absolute');
            }

            _this.container.addClass(_this.options.theme);

            $this.addClass(str_msgbox + ' ' + _this.options.type) // init the msgbox itself
                .css({
                    width: _this.options.width,
                    height: _this.options.height,
                    'min-width': _this.options.minWidth,
                    'min-height': _this.options.minHeight,
                    'max-width': _this.options.maxWidth,
                    'max-height': _this.options.maxHeight,
                    overflow: _this.options.overflow,
                    'z-index': _this.options.zIndex
                }).appendTo(_this.container);

            if (_this.options.autoReposition) {
                W.resize(function () {
                    if ($this.is(':visible')) _this.setPosition(_this.options.position[0], _this.options.position[1]);
                });
            }

//            if (_this.options.draggable) {
//                _this.dragObject = new dragObject($this.attr('id'), $this.attr('id'));
//                $this.css({ cursor: 'move' });
//            }
        },

        show: function (msg, options) {
            this.checkDomExistence();
            var _this = this,
                $this = _this.target,
                opts = $.extend({}, _this.options, options);

            msg = msg ? msg : opts.message;

            if (opts.beforeShow.apply($this.get(0), [msg, opts]) != false) { // if beforeShow return true or return nothing
                clearTimeout(_this.timeout);

                // show overlay and message
                _this.overlay.visible(opts.modal);
                $this.attr('class', str_msgbox + ' ' + opts.type).html('<div class="msgbox-content">' + msg + '</div>').show();
                if (!opts.closeBtn) $this.children('.msgbox-content').css({ 'padding-right': (opts.theme == 'chrome' ? 20 : 0) });

                // show close button
                if (opts.closeBtn) $(opts.tpl.closeBtn).appendTo($this).click(function () { _this.close(); });

                // set msgbox position
                _this.setPosition(opts.position[0], opts.position[1], opts.useAnimation);

                // autohide msgbox
                if (!opts.sticky) _this.timeout = setTimeout('$.msgbox.manager.' + $this.attr('id') + '.close()', parseInt(opts.duration));

                // close on escape
                if (opts.escape) $('html').keydown(function (e) { if (e.keyCode == 27) { clearTimeout(_this.timeout); _this.close(); } });
            }

            opts.afterShow.apply($this.get(0), [msg, opts]) != false
        },

        showInfo: function (msg, options) {
            this.show(msg, $.extend(options, { type: 'info' }));
        },

        showWarn: function (msg, options) {
            this.show(msg, $.extend(options, { type: 'warn' }));
        },

        showErr: function (msg, options) {
            this.show(msg, $.extend(options, { type: 'err' }));
        },

        close: function (clearMsg) {
            this.checkDomExistence();
            var $this = this.target;

            if (this.options.beforeClose.apply($this.get(0), [this.options]) != false) { // if beforeClose return true or return nothing
                clearTimeout(this.timeout);
                $this.hide();
                if (clearMsg) $this.html('');
                this.overlay.visible(false);
            }

            this.options.afterClose.apply($this.get(0), [this.options]);
        },

        isOpen: function () {
            return this.target.is(':visible');
        },

        setPosition: function (verticalPos, horizontalPos, useAnimation) { // top:10, center
            this.checkDomExistence();
            verticalPos = verticalPos.split(':'); // ['top', '10']
            horizontalPos = horizontalPos.split(':'); // ['center']

            var yPos = verticalPos[0], // top
                yOffset = verticalPos[1], // 10

                xPos = horizontalPos[0], // center
                xOffset = horizontalPos[1],  // undefined

                $this = this.target,
                frame = this.fixed ? W : this.container,

                top, left;

            switch (xPos) {
                case 'center': left = (frame.width() / 2 - $this.width() / 2) + 'px'; break;
                case 'left': left = xOffset + 'px'; break;
                case 'right': left = (frame.width() - xOffset - $this.width()) + 'px'; break;
            }

            switch (yPos) {
                case 'center': top = (frame.height() / 2 - $this.height() / 2) + 'px'; break;
                case 'top': top = yOffset + 'px'; break;
                case 'bottom': top = (frame.height() - yOffset - $this.height()) + 'px'; break;
            }

            if (useAnimation && !agentLtIe9) {
                $this.css({ top: -($this.height()), left: left }).stop(true, true)
                    .animate({ top: top, left: left, right: 'auto', bottom: 'auto' }, {
                        duration: this.options.animation.duration,
                        easing: this.options.animation.easing
                    });
            } else {
                $this.css({ top: top, left: left, right: 'auto', bottom: 'auto' });
            }
        },

        alert: function (msg, callback, options) {
            this.checkDomExistence();
            var _this = this,
                $this = _this.target,
                cmdBtns = '<div class="msgbox-btn-wrap">' + _this.options.tpl.okBtn + '</div>';

            _this.show(msg, $.extend(options, { type: 'alert' }));
            _this.target.append(cmdBtns);
            clearTimeout(_this.timeout);

            $this.find('.ok').click(function () {
                _this.close();
                if ($.isFunction(callback)) callback.apply($this.get(0), [msg, _this.options]);
            }).focus();
        },

        confirm: function (msg, callback, options) {
            this.checkDomExistence();
            var _this = this,
                $this = _this.target,
                cmdBtns = '<div class="msgbox-btn-wrap">' + _this.options.tpl.okBtn + ' ' + _this.options.tpl.cancelBtn + '</div>';

            _this.show(msg, $.extend(options, { type: 'confirm' }));
            _this.target.append(cmdBtns);
            clearTimeout(_this.timeout);

            $this.find('.ok').click(function () {
                _this.close();
                if ($.isFunction(callback)) callback.apply($this.get(0), []);
            });

            $this.find('.cancel').click(function () { _this.close(); }).focus();
        },

        prompt: function (msg, defaultValue, callback, options) {
            this.checkDomExistence();
            var _this = this,
                $this = _this.target,
                textbox = '<div class="msgbox-input-wrap">' + _this.options.tpl.textbox + '</div>',
                cmdBtns = '<div class="msgbox-btn-wrap">' + _this.options.tpl.okBtn + ' ' + _this.options.tpl.cancelBtn + '</div>';

            _this.show(msg, $.extend(options, { type: 'prompt' }));
            _this.target.append(textbox + cmdBtns);
            clearTimeout(_this.timeout);

            var okBtn = $this.find('.ok');

            textbox = $this.find('.msgbox-txt').val(defaultValue || '').focus().select()
                .keypress(function (e) { if (e.keyCode == 13) okBtn.trigger('click'); }); // enter key is pressed

            okBtn.click(function () {
                _this.close();
                if ($.isFunction(callback)) callback.apply($this.get(0), [textbox.val()]);
            });

            $this.find('.cancel').click(function () { _this.close(); });
        },

        numOfMsgBox: function () { return M.numOfMsgBox; },

        checkDomExistence: function () {
            var thisDomId = this.target.attr('id'),
                $this = $('#' + thisDomId);

            if (!$this.length) {
                delete $.msgbox.manager[thisDomId];
                this.log('DOM of "' + thisDomId + '" does not exist.', true);
            }
        },

        log: function (msg, throwException) {
            this.options.log.apply(this.target.get(0), [msg]);
            if (throwException) throw msg;
        }
    };

    $.msgbox = function (options) {
        var msgBox = new M($('<div id="' + str_msgbox + (++M.numOfMsgBox) + '" />'), options);

        $.msgbox.manager[msgBox.target.attr('id')] = msgBox;

        return msgBox;
    };

    $.msgbox.manager = {};

    // HELPERS.
    $.fn.visible = function (visisble) {
        if (visisble) this.css('visibility', 'visible');
        else this.css('visibility', 'hidden');
    };

} (window, document, jQuery));