import Vue from 'vue';
import Component from 'vue-class-component';
import { Site, SiteLevelValue, SiteLevel } from '../../model/site';
import * as Type from '../../store/types';
import './save-all-row.css';

@Component({
    template: require('./save-all-row.html'),
})

export class SaveAllRowComponent extends Vue {
    private get ViewedUserId(): string {
        return this.$store.getters[Type.Getter.ViewedUserId];
    }

    public OnClickSaveFunctionsForAllSites() {
        const sites: Array<Site> = this.$store.getters[Type.Getter.AllUserSites];

        for (const site of sites) {
            this.$store.dispatch(Type.Action.UpdateFunctionsForSite, site);
        }
    }

    public OnClickSaveAllSites() {
        const sites: Array<Site> = this.$store.getters[Type.Getter.AllUserSites];

        for (const site of sites) {
            if (site.EnableEdit && !site.EnableAssign) {
                this.$store.dispatch(Type.Action.UpdateSite, { site: site, isEdit: true });
            } else if (site.EnableAssign) {
                this.$store.dispatch(Type.Action.AssignSite, site);
            }
        }
    }
}