import { Site, SiteLevelValue, SiteLevel } from '../model/site';

export default class SiteUtil {
    public static CheckForEnablingStatus(
        site: Site,
        loginUserSiteLevels: Array<SiteLevel>,
        loginUserHasAssignSitePermission: boolean,
        loginUserIsPortalAdmin: boolean,
        loginUserHasAssignSiteHostPermissionSites: Array<Site>,
        isSelfEdit: boolean,
        currentSiteId: number): boolean {
        if (isSelfEdit) {
            return loginUserIsPortalAdmin && site.Id !== currentSiteId;
        }

        const loginUserCurrentSiteLevel = loginUserSiteLevels.find(sl => sl.Site.Id === site.Id);

        return this.IsPortalAdmin(site, loginUserIsPortalAdmin) ||
            (loginUserHasAssignSitePermission &&
                this.LoginUserHasAssignSiteHostPermissionInSite(site, loginUserHasAssignSiteHostPermissionSites) &&
                this.UserSiteIsActive(loginUserCurrentSiteLevel) &&
                this.LoginUserSiteLevelIsGreaterThanViewedUserSiteLevel(site, loginUserCurrentSiteLevel));
    }

    public static IsPortalAdmin(site: Site, loginUserIsPortalAdmin: boolean): boolean {
        return loginUserIsPortalAdmin &&
            (site.SiteLevel === undefined || site.SiteLevel.Level < SiteLevelValue.PortalAdmin);
    }

    public static LoginUserHasAssignSiteHostPermissionInSite(site: Site, loginUserHasAssignSiteHostPermissionSites: Array<Site>): boolean {
        return loginUserHasAssignSiteHostPermissionSites.some(s => s.Id === site.Id);
    }

    public static UserSiteIsActive(siteLevel: SiteLevel): boolean {
        return siteLevel !== undefined && siteLevel.IsActive;
    }

    public static LoginUserSiteLevelIsGreaterThanViewedUserSiteLevel(
        site: Site,
        loginUserCurrentSiteLevel: SiteLevel): boolean {
        return site.SiteLevel === undefined || loginUserCurrentSiteLevel.Level > site.SiteLevel.Level ||
            (loginUserCurrentSiteLevel.Level === SiteLevelValue.User && site.SiteLevel.Level === SiteLevelValue.User);
    }

    public static ShowExpiredDateColumn(viewedUserName: string) {
        return viewedUserName.startsWith('starix');
    }
}