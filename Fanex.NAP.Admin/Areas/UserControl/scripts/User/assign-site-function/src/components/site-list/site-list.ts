import Vue from 'vue';
import Component from 'vue-class-component';
import { Site, SiteLevel } from '../../model/site';
import { Function, FunctionPolicyValue } from '../../model/function';
import { FunctionNameComponent } from '../function-name';
import { SiteLevelComponent } from '../site-level';
import { SiteStatusComponent } from '../site-status';
import { ExpiredDateComponent } from '../expired-date';
import { EditFieldComponent } from '../edit-field';
import { SaveAllRowComponent } from '../save-all-row';
import { HeaderRowComponent } from '../header-row';
import * as Type from '../../store/types';
import SiteUtil from '../../util/site';
import FunctionUtil from '../../util/function';
import MessageBox from '../../util/message-box';
import './site-list.css';

@Component({
    template: require('./site-list.html'),
    components: {
        'function-name': FunctionNameComponent,
        'site-level': SiteLevelComponent,
        'site-status': SiteStatusComponent,
        'expired-date': ExpiredDateComponent,
        'edit-field': EditFieldComponent,
        'save-all-row': SaveAllRowComponent,
        'header-row': HeaderRowComponent,
        'vue-date-picker': require('vuejs-datepicker')
    },
})
export class SiteListComponent extends Vue {
    private viewedUserName = '';

    mounted() {
        this.viewedUserName = (<HTMLInputElement>document.getElementById('viewed-user-name')).value;
        const viewedUserId = (<HTMLInputElement>document.getElementById('viewed-user-id')).value;
        const loginUserId = (<HTMLInputElement>document.getElementById('login-user-id')).value;
        const loginUserIsPortalAdmin
            = JSON.parse((<HTMLInputElement>document.getElementById('login-user-is-portal-admin')).value.toLowerCase());
        const rootRoute = (<HTMLInputElement>document.getElementById('NapAdminRoot')).value;
        const currentSiteId = (<HTMLInputElement>document.getElementById('current-site-id')).value;
        const loginUserHasAssignSitePermission
            = JSON.parse((<HTMLInputElement>document.getElementById('login-user-has-assign-site-permission')).value.toLowerCase());
        const loginUserHasAssignSiteReadPermission
            = JSON.parse((<HTMLInputElement>document.getElementById('login-user-has-assign-site-read-permission')).value.toLowerCase());
        const loginUserAssignFunctionPermissionInAdminSite
            = Number((<HTMLInputElement>document.getElementById('login-user-assign-function-permission')).value);

        this.$store.commit(Type.Mutation.SetCurrentSiteId, currentSiteId);
        this.$store.commit(Type.Mutation.SetLoginUserAssignFunctionPermissionInAdminSite, loginUserAssignFunctionPermissionInAdminSite);
        this.$store.commit(Type.Mutation.SetRootRoute, rootRoute);
        this.$store.commit(Type.Mutation.SetLoginUserIsPortalAdmin, loginUserIsPortalAdmin);
        this.$store.commit(Type.Mutation.SetLoginUserId, loginUserId);
        this.$store.commit(Type.Mutation.SetViewedUserId, viewedUserId);
        this.$store.commit(Type.Mutation.SetLoginUserHasAssignSitePermission, loginUserHasAssignSitePermission);
        this.$store.commit(Type.Mutation.SetLoginUserHasAssignSiteReadPermission, loginUserHasAssignSiteReadPermission);

        this.$store.dispatch(Type.Action.GetAllUserSites);
        this.$store.dispatch(Type.Action.GetLoginUserHasAssignSiteHostPermissionSites);
        this.$store.dispatch(Type.Action.GetLoginUserSiteLevels);
    }

    public get siteList(): Array<Site> {
        return this.$store.getters[Type.Getter.AllUserSites];
    }

    public ToggleShowFunctionList(site: Site) {
        this.$store.commit(Type.Mutation.ShowHideSiteFunctions, { siteId: site.Id, isShow: !site.ShowFunctionList });
    }

    public ToggleSelectAllUserPolicy(siteId: number, currentPolicy: boolean, isRead: boolean) {
        this.$store.commit(Type.Mutation.SetAllFunctionsUserPolicyForSite,
            { siteId: siteId, rootPolicy: currentPolicy, isRead: isRead });
    }

    public ToggleAssignSite(site: Site) {
        this.$store.commit(Type.Mutation.EnableAssignSite, site.Id);
    }

    public ShowExpiredDateColumn(): boolean {
        return SiteUtil.ShowExpiredDateColumn(this.viewedUserName);
    }

    public SaveFunctionsForSite(site: Site) {
        this.$store.dispatch(Type.Action.UpdateFunctionsForSite, site);
    }

    public ColspanForUnassignedSite(site: Site) {
        if (!this.ShowSiteLevel(site) && !site.EnableAssign) {
            return 4;
        }

        return 1;
    }

    public OnCheckFunctionPolicy(site: Site, func: Function, currentFuncUserPolicy: boolean, isRead: boolean) {
        this.$store.commit(Type.Mutation.SetFunctionUserPolicy,
            { siteId: site.Id, functionId: func.Id, rootFuncUserPolicy: currentFuncUserPolicy, isRead });
    }

    public ShowHostPolicyCheckBox(func: Function) {
        return func !== undefined && func.Crud > 0;
    }

    public ShowAssignSiteButton(site: Site) {
        const loginUserSiteLevels: Array<SiteLevel> = this.$store.getters[Type.Getter.LoginUserSiteLevels];
        const loginUserCurrentSiteLevel = loginUserSiteLevels.find(sl => sl.Site.Id === site.Id);

        return site.LoginUserAssignSitePermission === FunctionPolicyValue.Host && SiteUtil.UserSiteIsActive(loginUserCurrentSiteLevel);
    }

    public ShowSiteLevel(site: Site) {
        const isLoginUserSelfEdit: boolean = this.$store.getters[Type.Getter.IsSelfEdit];

        return site.SiteLevel !== undefined && (site.LoginUserAssignSitePermission >= FunctionPolicyValue.Read || isLoginUserSelfEdit);
    }
}