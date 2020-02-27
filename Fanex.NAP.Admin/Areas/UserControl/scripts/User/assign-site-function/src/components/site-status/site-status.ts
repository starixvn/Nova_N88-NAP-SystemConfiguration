import Vue from 'vue';
import Component from 'vue-class-component';
import SiteService from '../../service/site-service';
import { Site, SiteLevelValue, SiteLevel } from '../../model/site';
import SiteUtil from '../../util/site';
import MessageBox from '../../util/message-box';
import * as Type from '../../store/types';

@Component({
    template: require('./site-status.html'),
    props: {
        site: Object,
        enableEdit: Boolean,
    },
    watch: {
        enableEdit: 'onChangeSiteEnableEdit'
    }
})

export class SiteStatusComponent extends Vue {
    private get Site(): Site {
        return this.$props.site;
    }

    private get LoginUserHasAssignSitePermission() {
        return this.$store.getters[Type.Getter.LoginUserHasAssignSitePermission];
    }

    private get LoginUserHasAssignSiteReadPermission() {
        return this.$store.getters[Type.Getter.LoginUserHasAssignSiteReadPermission];
    }

    public get IsEnable(): boolean {
        const loginUserHasAssignSitePermission: boolean = this.$store.getters[Type.Getter.LoginUserHasAssignSitePermission];
        const loginUserIsPortalAdmin: boolean = this.$store.getters[Type.Getter.LoginUserIsPortalAdmin];
        const loginUserSiteLevels: Array<SiteLevel> = this.$store.getters[Type.Getter.LoginUserSiteLevels];
        const loginUserHasAssignSiteHostPermissionSites: Array<Site> = this.$store.getters[Type.Getter.LoginUserHasAssignSiteHostPermissionSites];
        const currentSiteId: number = this.$store.getters[Type.Getter.CurrentSiteId];
        const isSelfEdit: boolean = this.$store.getters[Type.Getter.IsSelfEdit];

        return SiteUtil.CheckForEnablingStatus(
            this.Site,
            loginUserSiteLevels,
            loginUserHasAssignSitePermission,
            loginUserIsPortalAdmin,
            loginUserHasAssignSiteHostPermissionSites,
            isSelfEdit,
            currentSiteId
        );
    }

    public OnChangeSiteStatus(newStatus: boolean) {
        if (!this.Site.EnableAssign && !this.Site.EnableEdit) {
            const activeMessage = newStatus ? ' activate ' : ' deactivate ';
            this.KeepOldSiteStatus(newStatus);

            MessageBox.ShowWarning('Are you sure you want to' + activeMessage + '\'' + this.Site.Name + '\'',
                (ok) => {
                    this.$store.dispatch(Type.Action.UpdateSiteStatus,
                        {
                            siteId: this.Site.Id,
                            siteStatus: newStatus
                        });

                    this.UpdateNewSiteStatusWhenAccept(newStatus);
                });
        }
    }

    public onChangeSiteEnableEdit() {
        if (!this.$props.enableEdit) {
            this.$forceUpdate();
        }
    }

    private KeepOldSiteStatus(newStatus: boolean) {
        this.Site.SiteLevel.IsActive = !newStatus;
        this.$forceUpdate();
    }

    private UpdateNewSiteStatusWhenAccept(newStatus: boolean) {
        this.Site.SiteLevel.IsActive = newStatus;
        this.$forceUpdate();
    }
}