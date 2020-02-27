import Vue from 'vue';
import Component from 'vue-class-component';
import SiteUtil from '../../util/site';
import { Site } from '../../model/site';
import * as Type from '../../store/types';
import './header-row.css';

@Component({
    template: require('./header-row.html'),
    props: {
        viewedUserName: String
    }
})

export class HeaderRowComponent extends Vue {
    private showAllSiteFunctions = false;
    private get ViewedUserName(): string {
        return this.$props.viewedUserName;
    }

    public get SelectAllReadFunctionsForAllSites() {
        return this.$store.getters[Type.Getter.SelectAllReadFunctionsForAllSites];
    }

    public get SelectAllHostFunctionsForAllSites() {
        return this.$store.getters[Type.Getter.SelectAllHostFunctionsForAllSites];
    }

    public get DisableSelectAllReadFunctionsForAllSites() {
        const allSites: Array<Site> = this.$store.getters[Type.Getter.AllUserSites];
        return allSites.every(s => s.DisableCheckBoxAllFunctionReadPolicy);
    }

    public get DisableSelectAllHostFunctionsForAllSites() {
        const allSites: Array<Site> = this.$store.getters[Type.Getter.AllUserSites];

        return allSites.every(s => s.DisableCheckBoxAllFunctionHostPolicy);
    }

    public OnCheckAllFunctionsForAllSites(currentPolicy: boolean, isRead: boolean) {
        this.$store.commit(Type.Mutation.UpdateSelectAllUserFunctionForAllSites, { policy: !currentPolicy, isRead: isRead });
        this.$store.dispatch(Type.Action.SetAllFunctionsUserPolicyForAllSites, { rootPolicy: !currentPolicy, isRead: isRead });
    }

    public ShowExpiredDateColumn(): boolean {
        return SiteUtil.ShowExpiredDateColumn(this.ViewedUserName);
    }

    public OnClickShowAllSiteFunctions() {
        const allSites: Array<Site> = this.$store.getters[Type.Getter.AllUserSites];
        this.showAllSiteFunctions = !this.showAllSiteFunctions;

        for (const site of allSites) {
            this.$store.commit(Type.Mutation.ShowHideSiteFunctions, { siteId: site.Id, isShow: this.showAllSiteFunctions });
        }
    }
}