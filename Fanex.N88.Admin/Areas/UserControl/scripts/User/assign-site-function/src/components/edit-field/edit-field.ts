import Vue from 'vue';
import Component from 'vue-class-component';
import { Site, SiteLevel, SiteLevelValue } from '../../model/site';
import SiteUtil from '../../util/site';
import * as Type from '../../store/types';

@Component({
    template: require('./edit-field.html'),
    props: {
        enableEditSite: Boolean,
        enableAssignSite: Boolean,
        site: Object
    },
})
export class EditFieldComponent extends Vue {
    private get Site(): Site {
        return this.$props.site;
    }

    private get ViewedUserId(): string {
        return this.$store.getters[Type.Getter.ViewedUserId];
    }

    public IsAllowEdit() {
        const loginUserHasAssignSitePermission: boolean = this.$store.getters[Type.Getter.LoginUserHasAssignSitePermission];
        const loginUserIsPortalAdmin: boolean = this.$store.getters[Type.Getter.LoginUserIsPortalAdmin];
        const loginUserSiteLevels: Array<SiteLevel> = this.$store.getters[Type.Getter.LoginUserSiteLevels];
        const loginUserHasAssignSiteHostPermissionSites: Array<Site>
            = this.$store.getters[Type.Getter.LoginUserHasAssignSiteHostPermissionSites];
        const currentSiteId: number = this.$store.getters[Type.Getter.CurrentSiteId];
        const isSelfEdit: boolean = this.$store.getters[Type.Getter.IsSelfEdit];
        const loginUserCurrentSiteLevel = loginUserSiteLevels.find(sl => sl.Site.Id === this.Site.Id);

        const enableEditSiteStatus = SiteUtil.CheckForEnablingStatus(
            this.Site,
            loginUserSiteLevels,
            loginUserHasAssignSitePermission,
            loginUserIsPortalAdmin,
            loginUserHasAssignSiteHostPermissionSites,
            isSelfEdit,
            currentSiteId);

        return enableEditSiteStatus ? enableEditSiteStatus :
            !this.SiteIsActive() &&
            loginUserHasAssignSitePermission &&
            SiteUtil.UserSiteIsActive(loginUserCurrentSiteLevel) &&
            SiteUtil.LoginUserHasAssignSiteHostPermissionInSite(this.Site, loginUserHasAssignSiteHostPermissionSites);
    }

    public OnClickEditButton() {
        this.$store.commit(Type.Mutation.EnableEditSite, this.Site.Id);
    }

    public OnClickSaveSite() {
        this.$store.dispatch(Type.Action.UpdateSite, { site: this.Site, isEdit: true });
    }

    public OnClickAssignSite() {
        this.$store.dispatch(Type.Action.AssignSite, this.Site);
    }

    public OnClickCancelEditSite() {
        if (this.Site.EnableEdit && !this.Site.EnableAssign) {
            this.$store.dispatch(Type.Action.ResetSiteLevel, this.Site.Id);
        } else {
            this.$store.commit(Type.Mutation.CancelAssignSite, this.Site.Id);
        }
    }

    private SiteIsActive() {
        return this.Site.SiteLevel !== undefined && this.Site.SiteLevel.IsActive;
    }
}