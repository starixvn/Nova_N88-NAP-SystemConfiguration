import Vue from 'vue';
import Component from 'vue-class-component';
import { Site, SiteLevelValue, SiteLevel } from '../../model/site';
import * as Type from '../../store/types';

@Component({
    template: require('./site-level.html'),
    props: {
        enableEditSite: Boolean,
        site: Object
    },
})

export class SiteLevelComponent extends Vue {
    private get LoginUserIsPortalAdmin() {
        return this.$store.getters[Type.Getter.LoginUserIsPortalAdmin];
    }

    public GetSiteLevel(): string {
        const site: Site = this.$props.site;

        if (site.SiteLevel !== undefined) {
            switch (Number(site.SiteLevel.Level)) {
                case SiteLevelValue.User:
                    return 'User';

                case SiteLevelValue.SiteAdmin:
                    return 'Site Admin';

                case SiteLevelValue.PortalAdmin:
                    return 'Portal Admin';
            }
        }

        return '';
    }
}