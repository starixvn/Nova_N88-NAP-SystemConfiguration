export const Action = {
    GetAllUserSites: 'GetAllUserSites',
    GetLoginUserHasAssignSiteHostPermissionSites: 'GetLoginUserHasAssignSiteHostPermissionSites',
    GetLoginUserSiteLevels: 'GetLoginUserSiteLevels',
    UpdateSiteStatus: 'UpdateSiteStatus',
    UpdateSite: 'UpdateSite',
    UpdateFunctionsForSite: 'UpdateFunctionsForSite',
    AssignSite: 'AssignSite',
    ResetSiteLevel: 'ResetSiteLevel',
    SetAllFunctionsUserPolicyForAllSites: 'SetAllFunctionsUserPolicyForAllSites'
};

export const Getter = {
    AllUserSites: 'AllUserSites',
    LoginUserHasAssignSiteHostPermissionSites: 'LoginUserHasAssignSiteHostPermissionSites',
    LoginUserSiteLevels: 'LoginUserSiteLevels',
    CurrentSiteId: 'CurrentSiteId',
    ViewedUserId: 'ViewedUserId',
    LoginUserId: 'LoginUserId',
    LoginUserHasAssignSiteReadPermission: 'LoginUserHasAssignSiteReadPermission',
    LoginUserHasAssignSitePermission: 'LoginUserHasAssignSitePermission',
    LoginUserIsPortalAdmin: 'LoginUserIsPortalAdmin',
    IsSelfEdit: 'IsSelfEdit',
    SelectAllReadFunctionsForAllSites: 'SelectAllReadFunctionsForAllSites',
    SelectAllHostFunctionsForAllSites: 'SelectAllHostFunctionsForAllSites'
};

export const Mutation = {
    SetAllUserSites: 'SetAllUserSites',
    SetRootRoute: 'SetRootRoute',
    SetLoginUserHasAssignSiteHostPermissionSites: 'SetLoginUserHasAssignSiteHostPermissionSites',
    SetLoginUserIsPortalAdmin: 'SetLoginUserIsPortalAdmin',
    SetFunctionsToSite: 'SetFunctionsToSite',
    SetViewedUserFunctionPolicies: 'SetViewedUserFunctionPolicies',
    SetLoginUserFunctionPolicies: 'SetLoginUserFunctionPolicies',
    SetLoginUserAssignSitePermissionForSite: 'SetLoginUserAssignSitePermissionForSite',
    SetSiteLevelToSite: 'SetSiteLevelToSite',
    SetLoginUserSiteLevels: 'SetLoginUserSiteLevels',
    SetViewedUserSiteLevels: 'SetViewedUserSiteLevels',
    SetAllFunctionsUserPolicyForSite: 'SetAllFunctionsUserPolicyForSite',
    SetFunctionUserPolicy: 'SetFunctionUserPolicy',
    ChangeFunctionPolicy: 'ChangeFunctionPolicy',
    SetLoginUserAssignFunctionPermissionInAdminSite: 'SetLoginUserAssignFunctionPermissionInAdminSite',
    SetLoginUserHasAssignSitePermission: 'SetLoginUserHasAssignSitePermission',
    SetLoginUserHasAssignSiteReadPermission: 'SetLoginUserHasAssignSiteReadPermission',
    SetCurrentSiteId: 'SetCurrentSiteId',
    SetLoginUserId: 'SetLoginUserId',
    SetViewedUserId: 'SetViewedUserId',
    SetSelectAllUserFunctionsForAllSites: 'SetSelectAllUserFunctionsForAllSites',
    ShowHideSiteFunctions: 'ShowHideSiteFunctions',
    EnableEditSite: 'EnableEdit',
    EnableAssignSite: 'EnableAssign',
    DoneAssignSite: 'DoneAssign',
    CancelAssignSite: 'CancelAssign',
    ChangeSiteStatus: 'ChangeStatus',
    UpdateSiteLevel: 'UpdateSiteLevel',
    UpdateSelectAllUserFunctionForAllSites: 'UpdateSelectAllUserFunctionForAllSites'
};