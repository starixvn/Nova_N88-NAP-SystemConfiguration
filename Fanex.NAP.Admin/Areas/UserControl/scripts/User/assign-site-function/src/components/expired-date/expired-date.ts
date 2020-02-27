import Vue from 'vue';
import Component from 'vue-class-component';
import { Site } from '../../model/site';
import DateTimeUtil from '../../util/datetime';

@Component({
    template: require('./expired-date.html'),
    components: {
        'vue-date-picker': require('vuejs-datepicker')
    },
    props: {
        site: Object,
        enableEditSite: Boolean
    }
})

export class ExpiredDateComponent extends Vue {
    private currentSite: Site = { SiteLevel: {} };

    mounted() {
        this.currentSite = this.$props.site;
    }

    public OnCheckNeverExpired() {
        this.$forceUpdate();
    }

    public FormatDate(date: string) {
        return DateTimeUtil.Format(date, 'DD MMM YYYY');
    }
}