const NAP = require('../../../../NAPIndex.js');

export default class MessageBoxUtil {
    public static ShowUpdateSuccessfully(title: string = 'Your changes were saved!') {
        NAP.headlineMsgBox.showInfo(title, { closeBtn: false });
    }

    public static ShowError(title: string = 'Error has occured!') {
        NAP.headlineMsgBox.showErr(title, { closeBtn: false });
    }

    public static ShowWarning(title: string, callback: (ok: boolean) => any) {
        NAP.headlineMsgBox.confirm(title, function () {
            callback(true);
        }, { modal: true, closeBtn: false });
    }
}