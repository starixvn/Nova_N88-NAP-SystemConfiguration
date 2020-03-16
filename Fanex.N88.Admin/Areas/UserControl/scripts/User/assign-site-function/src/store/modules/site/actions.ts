import Vuex from 'vuex';
import { Function, FunctionPolicyValue } from '../../../model/function';
import { Site } from '../../../model/site';
import { SiteState } from './state';
import * as Type from '../../types';
import SiteService from '../../../service/site-service';
import FunctionService from '../../../service/function-service';
import MessageBox from '../../../util/message-box';
import DateTimeUtil from '../../../util/datetime';
import * as _ from 'lodash';

export const actions: Vuex.ActionTree<SiteState, SiteState> = {
    [Type.Action.GetAllUserSites]({ commit, state }) {
        SiteService.GetSites(state.RootRoute, state.ViewedUserId, sites => {
            commit(Type.Mutation.SetAllUserSites, sites);

            SiteService.GetUserSitesLevels(state.RootRoute, state.ViewedUserId, siteLevels => {
                for (const site of sites) {
                    const siteId = site.Id;
                    const siteLevel = siteLevels.find(sl => sl.Site.Id === siteId);

                    commit(Type.Mutation.SetSiteLevelToSite, { siteId, siteLevel });
                    commit(Type.Mutation.SetViewedUserSiteLevels, siteLevels);
                    GetFunctionsForSite(commit, state, siteId);
                }
            });
        });
    },

    [Type.Action.GetLoginUserHasAssignSiteHostPermissionSites]({ commit, state }) {
        SiteService.GetLoginUserHasAssignSiteHostPermissionSites(state.RootRoute, sites => {
            commit(Type.Mutation.SetLoginUserHasAssignSiteHostPermissionSites, sites);
        });
    },

    [Type.Action.GetLoginUserSiteLevels]({ commit, state }) {
        SiteService.GetUserSitesLevels(state.RootRoute, state.LoginUserId, siteLevels => {
            commit(Type.Mutation.SetLoginUserSiteLevels, siteLevels);
        });
    },

    [Type.Action.ResetSiteLevel]({ commit, state }, siteId: number) {
        ResetSiteLevel(commit, state, siteId);
    },

    [Type.Action.UpdateSiteStatus]({ commit, state }, { siteId, siteStatus }) {
        const site = state.All.find(s => s.Id === siteId);

        _.assign(site.SiteLevel, state.ViewedUserSiteLevels.find(sl => sl.Site.Id === siteId));
        site.SiteLevel.IsActive = siteStatus;

        if (!DateTimeUtil.IsValid(site.SiteLevel.ExpiredDate)) {
            site.SiteLevel.ExpiredDate = new Date().toDateString();
            site.SiteLevel.IsNeverExpired = true;
        }

        SiteService.UpdateSiteToUser(state.RootRoute, state.ViewedUserId, site, (result) => {
            if (result === 'OK') {
                commit(Type.Mutation.ChangeSiteStatus, { siteId: siteId, rootStatus: site.SiteLevel.IsActive });
                MessageBox.ShowUpdateSuccessfully();
            } else {
                MessageBox.ShowError();
            }
        });
    },

    [Type.Action.UpdateSite]({ commit, state }, { site, isEdit }) {
        UpdateSite(commit, state, site, isEdit);
    },

    [Type.Action.UpdateFunctionsForSite]({ commit, state }, site: Site) {
        if (site.HasFunctionsChange) {
            const funcHost: Array<number> = [];
            const funcRead: Array<number> = [];

            for (const func of site.Functions) {
                if (func.HasUserReadPolicy && (!func.HasUserHostPolicy || (func.HasUserHostPolicy && func.Crud <= 0))) {
                    funcRead.push(func.Id);
                }

                if (func.HasUserHostPolicy && func.Crud > 0) {
                    funcHost.push(func.Id);
                }
            }

            const data = [{
                FuncHost: funcHost,
                FuncRead: funcRead,
                SiteId: site.Id,
                UserId: state.ViewedUserId
            }];

            FunctionService.UpdateFunctions(state.RootRoute, data, (result) => {
                if (result === 'OK') {
                    MessageBox.ShowUpdateSuccessfully();
                    GetFunctionsForSite(commit, state, site.Id);
                } else {
                    MessageBox.ShowError();
                }
            });
        } else {
            MessageBox.ShowUpdateSuccessfully('There is no changes to be saved');
        }
    },

    [Type.Action.AssignSite]({ commit, state }, site) {
        SiteService.AssignSiteToUser(state.RootRoute, state.ViewedUserId, site, (assignResult) => {
            if (assignResult === 'OK') {
                UpdateSite(commit, state, site, false);
            } else {
                commit(Type.Mutation.CancelAssignSite, this.Site.Id);
                MessageBox.ShowError('Assign error');
            }
        });
    },

    [Type.Action.SetAllFunctionsUserPolicyForAllSites]({ commit, state }, payLoad: { rootPolicy: boolean, isRead: boolean }) {
        for (const site of state.All) {
            commit(Type.Mutation.SetAllFunctionsUserPolicyForSite,
                { siteId: site.Id, rootPolicy: payLoad.rootPolicy, isRead: payLoad.isRead });
        }
    }
};

const GetFunctionsForSite = (commit: Vuex.Commit, state: SiteState, siteId: number) => {
    FunctionService.GetFunctions(
        state.RootRoute,
        state.ViewedUserId,
        siteId,
        ({
            functions,
            functionPolicies,
            groupFunctionPolicies,
            loginUserFunctionPolicies,
            loginUserAssignFunctionPermission,
            loginUserAssignSitePermission
        }) => {
            commit(Type.Mutation.SetLoginUserFunctionPolicies, { siteId, functionPolicies: loginUserFunctionPolicies });
            commit(Type.Mutation.SetLoginUserAssignSitePermissionForSite, { siteId, permission: loginUserAssignSitePermission });
            commit(Type.Mutation.SetViewedUserFunctionPolicies, { siteId, functionPolicies, groupFunctionPolicies });
            commit(Type.Mutation.SetFunctionsToSite, { siteId, functions, loginUserAssignFunctionPermission });
            commit(Type.Mutation.SetSelectAllUserFunctionsForAllSites);
        });
};

const ResetSiteLevel = (commit: Vuex.Commit, state: SiteState, siteId: number) => {
    const siteLevel = _.cloneDeep(state.ViewedUserSiteLevels.find(sl => sl.Site.Id === siteId));
    commit(Type.Mutation.SetSiteLevelToSite, { siteId, siteLevel });
    commit(Type.Mutation.EnableEditSite, siteId);
};

const UpdateSite = (commit: Vuex.Commit, state: SiteState, site: Site, isEdit: boolean) => {
    if (!DateTimeUtil.IsValid(site.SiteLevel.ExpiredDate)) {
        site.SiteLevel.ExpiredDate = new Date().toDateString();
        site.SiteLevel.IsNeverExpired = true;
    }

    SiteService.UpdateSiteToUser(state.RootRoute, state.ViewedUserId, site, (result) => {
        if (result === 'OK') {
            MessageBox.ShowUpdateSuccessfully();

            GetFunctionsForSite(commit, state, site.Id);

            if (isEdit) {
                commit(Type.Mutation.EnableEditSite, site.Id);
                commit(Type.Mutation.UpdateSiteLevel, { siteId: site.Id, siteLevel: site.SiteLevel });
            } else {
                commit(Type.Mutation.DoneAssignSite, site.Id);
            }
        } else {
            MessageBox.ShowError();
            ResetSiteLevel(commit, state, site.Id);
        }
    });
};