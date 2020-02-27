import Vuex from 'vuex';
import * as Type from '../../types';
import { Site, SiteLevel, SiteLevelValue } from '../../../model/site';
import { Function, FunctionPolicy, FunctionPolicyValue } from '../../../model/function';
import { SiteState } from './state';
import DateTimeUtil from '../../../util/datetime';
import FunctionUtil from '../../../util/function';
import MessageBox from '../../../util/message-box';
import * as _ from 'lodash';

export const mutations: Vuex.MutationTree<SiteState> = {
    [Type.Mutation.SetRootRoute](state, rootRoute: string) {
        state.RootRoute = rootRoute;
    },

    [Type.Mutation.SetAllUserSites](state, sites: Array<Site>) {
        sites.forEach(s => s.ShowFunctionList = false);
        state.All = sites;
    },

    [Type.Mutation.SetCurrentSiteId](state, currentSiteId: number) {
        state.CurrentSiteId = currentSiteId;
    },

    [Type.Mutation.SetLoginUserHasAssignSiteHostPermissionSites](state, sites: Array<Site>) {
        state.LoginUserHasAssignSiteHostPermissionSites = sites;
    },

    [Type.Mutation.SetViewedUserId](state, viewUserId: string) {
        state.ViewedUserId = viewUserId;
    },

    [Type.Mutation.SetLoginUserHasAssignSitePermission](state, permission: boolean) {
        state.LoginUserHasAssignSitePermission = permission;
    },

    [Type.Mutation.SetLoginUserHasAssignSiteReadPermission](state, permission: boolean) {
        state.LoginUserHasAssignSiteReadPermission = permission;
    },

    [Type.Mutation.SetLoginUserId](state, loginUserId: string) {
        state.LoginUserId = loginUserId;
    },

    [Type.Mutation.SetLoginUserIsPortalAdmin](state, loginUserIsPortalAdmin) {
        state.LoginUserIsPortalAdmin = loginUserIsPortalAdmin;
    },

    [Type.Mutation.SetFunctionsToSite](state, { siteId, functions, loginUserAssignFunctionPermission }) {
        const siteIndex = state.All.findIndex(s => s.Id === siteId);
        const site: Site = state.All[siteIndex];
        const isSelfEdit = state.LoginUserId === state.ViewedUserId;

        site.Functions = functions;
        site.LoginUserAssignFunctionPermission = loginUserAssignFunctionPermission;
        FunctionUtil.InitFunctionPolicies(site);
        FunctionUtil.DisableFunctionPolicies(site, state.LoginUserIsPortalAdmin, state.LoginUserAssignFunctionPermissionInAdminSite, isSelfEdit);

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetSelectAllUserFunctionsForAllSites](state) {
        state.SelectAllReadFunctionsForAllSites = state.All.every(site => site.HasAllFunctionUserReadPolicy || site.DisableCheckBoxAllFunctionReadPolicy);
        state.SelectAllHostFunctionsForAllSites = state.All.every(site => site.HasAllFunctionUserHostPolicy || site.DisableCheckBoxAllFunctionHostPolicy);
    },

    [Type.Mutation.UpdateSelectAllUserFunctionForAllSites](state, { policy, isRead }) {
        if (isRead) {
            state.SelectAllReadFunctionsForAllSites = policy;

            if (policy === false) {
                state.SelectAllHostFunctionsForAllSites = policy;
            }
        } else {
            state.SelectAllHostFunctionsForAllSites = policy;

            if (policy === true) {
                state.SelectAllReadFunctionsForAllSites = policy;
            }
        }
    },

    [Type.Mutation.SetViewedUserFunctionPolicies](state, { siteId, functionPolicies, groupFunctionPolicies }) {
        const siteIndex = state.All.findIndex(s => s.Id === siteId);
        const site: Site = state.All[siteIndex];

        site.ViewedUserFunctionPolicies = functionPolicies;
        site.ViewedUserGroupFunctionPolicies = groupFunctionPolicies;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetLoginUserFunctionPolicies](state, { siteId, functionPolicies }) {
        const siteIndex = state.All.findIndex(s => s.Id === siteId);
        const site: Site = state.All[siteIndex];

        site.LoginUserFunctionPolicies = functionPolicies;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetSiteLevelToSite](state, payLoad: { siteId: number, siteLevel: SiteLevel }) {
        const siteIndex = state.All.findIndex(s => s.Id === payLoad.siteId);
        const site = state.All[siteIndex];

        if (payLoad.siteLevel !== undefined) {
            if (payLoad.siteLevel.IsNeverExpired) {
                payLoad.siteLevel.ExpiredDate = null;
            } else {
                payLoad.siteLevel.ExpiredDate = DateTimeUtil.ParseAndFormat(payLoad.siteLevel.ExpiredDate, 'DD MMM YYYY');
            }
        }

        site.SiteLevel = payLoad.siteLevel;
        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetLoginUserSiteLevels](state, siteLevels: Array<SiteLevel>) {
        state.LoginUserSiteLevels = _.cloneDeep(siteLevels);
    },

    [Type.Mutation.SetViewedUserSiteLevels](state, siteLevels: Array<SiteLevel>) {
        state.ViewedUserSiteLevels = _.cloneDeep(siteLevels);
    },

    [Type.Mutation.SetAllFunctionsUserPolicyForSite](state, payLoad: { siteId: number, rootPolicy: boolean, isRead: boolean }) {
        const siteIndex = state.All.findIndex(s => s.Id === payLoad.siteId);
        const site = state.All[siteIndex];

        if (payLoad.isRead && !site.DisableCheckBoxAllFunctionReadPolicy) {
            FunctionUtil.SetAllFunctionsReadPolicy(site, payLoad.rootPolicy);

            if (payLoad.rootPolicy === false) {
                FunctionUtil.SetAllFunctionsHostPolicy(site, payLoad.rootPolicy);
            }
        } else if (!site.DisableCheckBoxAllFunctionHostPolicy) {
            FunctionUtil.SetAllFunctionsHostPolicy(site, payLoad.rootPolicy);

            if (!site.HasAllFunctionUserReadPolicy) {
                FunctionUtil.SetAllFunctionsReadPolicy(site, payLoad.rootPolicy);
            }
        }

        site.HasFunctionsChange = true;
        state.All.splice(siteIndex, 1, site);
        state.SelectAllReadFunctionsForAllSites = state.All.filter(s => !s.DisableCheckBoxAllFunctionReadPolicy).every(s => s.HasAllFunctionUserReadPolicy);
        state.SelectAllHostFunctionsForAllSites = state.All.filter(s => !s.DisableCheckBoxAllFunctionHostPolicy).every(s => s.HasAllFunctionUserHostPolicy);
    },

    [Type.Mutation.SetFunctionUserPolicy](state, payLoad: { siteId: number, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean }) {
        const siteIndex = state.All.findIndex(s => s.Id === payLoad.siteId);
        const site = state.All[siteIndex];

        if (payLoad.rootFuncUserPolicy && (
            FunctionUtil.CheckExistFunctionPermissionInHouseWinLossByProducts(site, payLoad.functionId)
            || FunctionUtil.CheckExistFunctionPermissionInCN88HouseWinLossByProducts(site, payLoad.functionId))) {
            const func = site.Functions.find(f => f.Id === payLoad.functionId);
            func.HasUserReadPolicy = !func.HasUserReadPolicy;
            state.All.splice(siteIndex, 1, site);
            return;
        }

        FunctionUtil.SetCurrentFunctionPolicy(site, payLoad.functionId, payLoad.rootFuncUserPolicy, payLoad.isRead);
        FunctionUtil.SetChildrenFunctionPolicy(site, payLoad.functionId, payLoad.rootFuncUserPolicy, payLoad.isRead);
        FunctionUtil.SetParentFunctionPolicy(site, payLoad.functionId, payLoad.rootFuncUserPolicy, payLoad.isRead);
        FunctionUtil.SetSiteFunctionPolicy(site);

        site.HasFunctionsChange = true;
        state.All.splice(siteIndex, 1, site);

        state.SelectAllReadFunctionsForAllSites = state.All.filter(s => !s.DisableCheckBoxAllFunctionReadPolicy).every(s => s.HasAllFunctionUserReadPolicy);
        state.SelectAllHostFunctionsForAllSites = state.All.filter(s => !s.DisableCheckBoxAllFunctionHostPolicy).every(s => s.HasAllFunctionUserHostPolicy);
    },

    [Type.Mutation.ChangeFunctionPolicy](state, payLoad: { siteId: number, func: Function, isRead: boolean }) {
        const siteIndex = state.All.findIndex(s => s.Id === payLoad.siteId);
        const site = state.All[siteIndex];

        const currentFunc = site.Functions.find(f => f.Id === payLoad.func.Id);

        if (payLoad.isRead) {
            currentFunc.HasUserReadPolicy = !currentFunc.HasUserReadPolicy
        } else {
            currentFunc.HasUserHostPolicy = !currentFunc.HasUserHostPolicy
        }

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetLoginUserAssignFunctionPermissionInAdminSite](state, permission: number) {
        state.LoginUserAssignFunctionPermissionInAdminSite = permission;
    },

    [Type.Mutation.SetLoginUserAssignSitePermissionForSite](state, payload: { siteId: number, permission: number }) {
        const site = state.All.find(s => s.Id === payload.siteId);
        site.LoginUserAssignSitePermission = payload.permission;
    },

    [Type.Mutation.ShowHideSiteFunctions](state, { siteId, isShow }) {
        const siteIndex = state.All.findIndex(s => s.Id === siteId);
        const site = state.All[siteIndex];
        site.ShowFunctionList = isShow;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.EnableEditSite](state, siteId: number) {
        const siteIndex = state.All.findIndex(s => s.Id === siteId);
        const site = state.All[siteIndex];
        site.EnableEdit = !site.EnableEdit;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.EnableAssignSite](state, siteId: number) {
        const siteIndex = state.All.findIndex(s => s.Id === siteId);
        const site = state.All[siteIndex];
        site.EnableAssign = true;
        site.EnableEdit = true;
        site.SiteLevel = {
            Level: SiteLevelValue.User,
            IsActive: true,
            IsNeverExpired: true,
            Site: site
        };

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.DoneAssignSite](state, siteId: number) {
        const siteIndex = state.All.findIndex(s => s.Id === siteId);
        const site = state.All[siteIndex];
        site.EnableAssign = false;
        site.EnableEdit = false;

        const siteLevel = _.cloneDeep(site.SiteLevel);
        state.ViewedUserSiteLevels.push(siteLevel);

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.CancelAssignSite](state, siteId: number) {
        const siteIndex = state.All.findIndex(s => s.Id === siteId);
        const site = state.All[siteIndex];

        site.EnableAssign = false;
        site.EnableEdit = false;
        site.SiteLevel = undefined;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.ChangeSiteStatus](state, payload: { siteId: number, rootStatus: boolean }) {
        const siteIndex = state.All.findIndex(s => s.Id === payload.siteId);
        const site = state.All[siteIndex];
        site.SiteLevel.IsActive = payload.rootStatus;
        state.ViewedUserSiteLevels.find(sl => sl.Site.Id === payload.siteId).IsActive = payload.rootStatus;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.UpdateSiteLevel](state, payload: { siteId: number, siteLevel: SiteLevel }) {
        const currentSiteLevel = state.ViewedUserSiteLevels.find(sl => sl.Site.Id === payload.siteId);
        _.assign(currentSiteLevel, payload.siteLevel);
    },
};
