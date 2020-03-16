import moment from 'moment/moment';

export default class DateTimeUtil {
    public static ParseAndFormat(date: string, format: string): string {
        if (date === 'N/A') {
            return date;
        }
        const datetimeResponsePattern = /^\/Date\((d|-|.*)\)[\/|\\]$/;
        const regularExpression = datetimeResponsePattern.exec(date);

        if (regularExpression) {
            const dateArray = regularExpression[1].split(/[-+,.]/);
            const parsedDate = new Date(dateArray[0] ? +dateArray[0] : 0 - +dateArray[1]);

            if (parsedDate.getFullYear() !== 1) {
                return moment(parsedDate).format(format);
            }

            return 'N/A';
        }

        return moment(new Date(date)).format(format);
    }

    public static Format(date: string, format: string) {
        if (this.IsValid(date)) {
            const parsedDate = new Date(date);

            return moment(new Date(date)).format(format);
        }

        return date;
    }

    public static IsValid(date: string): boolean {
        return date !== 'N/A' && date !== undefined && date !== null && new Date(date).getFullYear() !== 1;
    }
}