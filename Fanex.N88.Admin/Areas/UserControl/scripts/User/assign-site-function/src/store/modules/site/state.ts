import { Site, SiteLevel } from '../../../model/site';

export interface SiteState {
    All: Array<Site>;
    LoginUserHasAssignSiteHostPermissionSites: Array<Site>;
    LoginUserSiteLevels: Array<SiteLevel>;
    LoginUserAssignFunctionPermissionInAdminSite: number;
    LoginUserHasAssignSitePermission: boolean;
    LoginUserHasAssignSiteReadPermission: boolean;
    LoginUserId: string;
    LoginUserIsPortalAdmin: boolean;
    ViewedUserSiteLevels: Array<SiteLevel>;
    ViewedUserId: string;
    RootRoute: string;
    CurrentSiteId: number;
    SelectAllReadFunctionsForAllSites: boolean;
    SelectAllHostFunctionsForAllSites: boolean;
}

export const state: SiteState = {
    All: new Array<Site>(),
    LoginUserHasAssignSiteHostPermissionSites: new Array<Site>(),
    LoginUserSiteLevels: new Array<SiteLevel>(),
    LoginUserIsPortalAdmin: false,
    LoginUserId: '',
    LoginUserHasAssignSitePermission: false,
    LoginUserHasAssignSiteReadPermission: false,
    LoginUserAssignFunctionPermissionInAdminSite: 0,
    ViewedUserSiteLevels: new Array<SiteLevel>(),
    ViewedUserId: '',
    RootRoute: '',
    CurrentSiteId: 0,
    SelectAllReadFunctionsForAllSites: false,
    SelectAllHostFunctionsForAllSites: false
};