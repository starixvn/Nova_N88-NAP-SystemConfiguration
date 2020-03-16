import Vuex from 'vuex';
import * as Type from '../../types';
import { SiteState } from './state';

export const getters: Vuex.GetterTree<SiteState, SiteState> = {
    [Type.Getter.AllUserSites]: state => state.All,
    [Type.Getter.LoginUserHasAssignSiteHostPermissionSites]: state => state.LoginUserHasAssignSiteHostPermissionSites,
    [Type.Getter.LoginUserSiteLevels]: state => state.LoginUserSiteLevels,
    [Type.Getter.CurrentSiteId]: state => state.CurrentSiteId,
    [Type.Getter.ViewedUserId]: state => state.ViewedUserId,
    [Type.Getter.LoginUserId]: state => state.LoginUserId,
    [Type.Getter.LoginUserHasAssignSitePermission]: state => state.LoginUserHasAssignSitePermission,
    [Type.Getter.LoginUserHasAssignSiteReadPermission]: state => state.LoginUserHasAssignSiteReadPermission,
    [Type.Getter.LoginUserIsPortalAdmin]: state => state.LoginUserIsPortalAdmin,
    [Type.Getter.IsSelfEdit]: state => state.ViewedUserId === state.LoginUserId,
    [Type.Getter.SelectAllReadFunctionsForAllSites]: state => state.SelectAllReadFunctionsForAllSites,
    [Type.Getter.SelectAllHostFunctionsForAllSites]: state => state.SelectAllHostFunctionsForAllSites,
};