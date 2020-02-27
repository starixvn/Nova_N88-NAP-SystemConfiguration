import { Site, SiteLevel, SiteLevelValue } from '../model/site';
import { Function, FunctionPolicy, FunctionPolicyValue } from '../model/function';
import MessageBox from './message-box';

export default class FunctionUtil {
    public static readonly HouseWinLossByProductsFunctionName = 'House Win Loss By Products - Product Permission';
    public static readonly CN88HouseWinLossByProductsFunctionName = 'CN88 Win Loss By Products - Product Permission';
    public static readonly HouseWinLossByProductsWarningMessage = 'You must select ONLY ONE function ';

    public static SetCurrentFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        const currentFunc = site.Functions.find(f => f.Id === functionId);

        if (isRead) {
            if (currentFunc.HasUserHostPolicy && !currentFunc.DisableHost) {
                currentFunc.HasUserHostPolicy = rootFuncUserPolicy;
            }
        } else {
            if (!currentFunc.HasUserReadPolicy && !currentFunc.DisableRead) {
                currentFunc.HasUserReadPolicy = rootFuncUserPolicy;
            }
        }
    }

    public static SetChildrenFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        const childrenFunc = site.Functions.filter(f => f.Parent !== null && f.Parent.Id === functionId);

        for (const func of childrenFunc) {
            if (rootFuncUserPolicy && this.CheckFunctionIsHouseWinLossByProducts(func)) {
                continue;
            }

            if (isRead && !func.DisableRead) {
                func.HasUserReadPolicy = rootFuncUserPolicy;
            } else if (!func.DisableHost) {
                func.HasUserHostPolicy = rootFuncUserPolicy;
            }

            this.SetCurrentFunctionPolicy(site, func.Id, rootFuncUserPolicy, isRead);
            this.SetChildrenFunctionPolicy(site, func.Id, rootFuncUserPolicy, isRead);
        }
    }

    public static SetParentFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        let parentFunc = site.Functions.find(f => f.Id === functionId).Parent;

        if (parentFunc !== null) {
            let existChildrenFuncHasUserRead = false;
            const childrenFunc = site.Functions.filter(f => f.Parent !== null && f.Parent.Id === parentFunc.Id);

            for (const func of childrenFunc) {
                if ((isRead && func.HasUserReadPolicy) || (!isRead && func.HasUserHostPolicy)) {
                    existChildrenFuncHasUserRead = true;
                }
            }

            parentFunc = site.Functions.find(f => f.Id === parentFunc.Id);

            if (isRead) {
                parentFunc.HasUserReadPolicy = existChildrenFuncHasUserRead ? true : rootFuncUserPolicy;
            } else {
                parentFunc.HasUserHostPolicy = (existChildrenFuncHasUserRead) ? true : rootFuncUserPolicy;
            }

            this.SetCurrentFunctionPolicy(site, parentFunc.Id, rootFuncUserPolicy, isRead);
            this.SetParentFunctionPolicy(site, parentFunc.Id, rootFuncUserPolicy, isRead);
        }
    }

    public static SetAllFunctionsReadPolicy(site: Site, rootPolicy: boolean) {
        site.Functions.forEach(f =>
            (rootPolicy && this.CheckFunctionIsHouseWinLossByProducts(f)) || f.DisableRead ?
                f.HasUserReadPolicy = f.HasUserReadPolicy : f.HasUserReadPolicy = rootPolicy);

        site.HasAllFunctionUserReadPolicy = rootPolicy;
    }

    public static SetAllFunctionsHostPolicy(site: Site, rootPolicy: boolean) {
        site.Functions.forEach(f => f.DisableHost ? f.HasUserHostPolicy = f.HasUserHostPolicy : f.HasUserHostPolicy = rootPolicy);
        site.HasAllFunctionUserHostPolicy = rootPolicy;
    }

    public static SetSiteFunctionPolicy(site: Site) {
        site.HasAllFunctionUserReadPolicy = site.Functions.every(f => f.HasUserReadPolicy);
        site.HasAllFunctionUserHostPolicy = site.Functions.filter(f => f.Crud > 0).every(f => f.HasUserHostPolicy);
    }

    public static InitFunctionPolicies(site: Site) {
        let allFuncHasHostPolicy = true;
        let allFuncHasReadPolicy = true;
        let allFuncHasGroupHostPolicy = true;
        let allFuncHasGroupReadPolicy = true;

        for (const func of site.Functions) {
            const policy = site.ViewedUserFunctionPolicies.find(fp => fp.Function.Id === func.Id);
            const groupPolicy = site.ViewedUserGroupFunctionPolicies.find(fp => fp.Function.Id === func.Id);

            if (policy !== undefined) {
                func.HasUserReadPolicy = policy.Policy >= FunctionPolicyValue.Read;
                func.HasUserHostPolicy = policy.Policy >= FunctionPolicyValue.Host;
            }

            if (groupPolicy !== undefined) {
                func.HasGroupReadPolicy = groupPolicy.Policy >= FunctionPolicyValue.Read;
                func.HasGroupHostPolicy = groupPolicy.Policy >= FunctionPolicyValue.Host;
            }

            if (func.HasUserHostPolicy === undefined || (!func.HasUserHostPolicy && func.Crud > 0)) {
                allFuncHasHostPolicy = false;
            }

            if (func.HasUserReadPolicy === undefined || !func.HasUserReadPolicy) {
                allFuncHasReadPolicy = false;
            }

            if (func.HasGroupHostPolicy === undefined || (!func.HasGroupHostPolicy && func.Crud > 0)) {
                allFuncHasGroupHostPolicy = false;
            }

            if (allFuncHasGroupReadPolicy === undefined || !func.HasGroupReadPolicy) {
                allFuncHasGroupReadPolicy = false;
            }
        }

        site.HasAllFunctionUserHostPolicy = allFuncHasHostPolicy;
        site.HasAllFunctionUserReadPolicy = allFuncHasReadPolicy || allFuncHasHostPolicy;
        site.HasAllFunctionGroupHostPolicy = allFuncHasGroupHostPolicy;
        site.HasAllFunctionGroupReadPolicy = allFuncHasGroupReadPolicy || allFuncHasGroupHostPolicy;
    }

    public static DisableFunctionPolicies(
        site: Site,
        loginUserIsPortalAdmin: boolean,
        loginUserAssignFunctionPermissionInAdminSite: number,
        isSelfEdit: boolean) {
        for (const func of site.Functions) {
            const isDisabledRead = FunctionUtil.CheckDisableFunctionPolicy(site, func, true, loginUserIsPortalAdmin, loginUserAssignFunctionPermissionInAdminSite, isSelfEdit);
            const isDisabledHost = FunctionUtil.CheckDisableFunctionPolicy(site, func, false, loginUserIsPortalAdmin, loginUserAssignFunctionPermissionInAdminSite, isSelfEdit);
            func.DisableRead = isDisabledRead;
            func.DisableHost = isDisabledHost;
            func.IsHidden = !loginUserIsPortalAdmin &&
                (site.LoginUserAssignFunctionPermission < FunctionPolicyValue.Read ||
                    loginUserAssignFunctionPermissionInAdminSite < FunctionPolicyValue.Read);
        }

        site.DisableCheckBoxAllFunctionReadPolicy = site.Functions.every(f => f.DisableRead);
        site.DisableCheckBoxAllFunctionHostPolicy = site.Functions.every(f => f.DisableHost);
        site.HideCheckBoxAllFunctionPolicy = site.Functions.every(f => f.IsHidden);
    }

    public static CheckDisableFunctionPolicy(
        site: Site,
        func: Function,
        isRead: boolean,
        loginUserIsPortalAdmin: boolean,
        loginUserAssignFunctionPermissionInAdminSite: number,
        isSelfEdit: boolean) {
        const viewedUserPolicy = site.ViewedUserFunctionPolicies.find(fp => fp.Function.Id === func.Id);
        const loginUserPolicy = site.LoginUserFunctionPolicies.find(fp => fp.Function.Id === func.Id);

        const isDisabled = this.ViewedUserSiteLevelIsAdmin(site) ||
            this.LoginUserHasAssignFunctionReadPermissionInCurrentSiteAndAdminSite(site, loginUserAssignFunctionPermissionInAdminSite) ||
            isSelfEdit ||
            (!loginUserIsPortalAdmin &&
                (this.LoginUserHasNoPolicy(loginUserPolicy, isRead) ||
                    this.LoginUserPolicyIsSmallerThanViewedUserPolicy(viewedUserPolicy, loginUserPolicy)));

        return isDisabled;
    }

    public static CheckExistFunctionPermissionInHouseWinLossByProducts(site: Site, currentFunctionId: number): boolean {
        const func = site.Functions.find(f => f.Id === currentFunctionId);
        let existPermission = false;

        if (this.HasNameOrParentNameIsHouseWinLossByProducts(func)) {
            existPermission = site.Functions.some(f => (
                  (f.Id !== currentFunctionId
                    && this.HasNameOrParentNameIsHouseWinLossByProducts(f))
                    || this.HasNameIsHouseWinLossByProducts(f))
                  && this.HasReadPolicyIncludeGroup(f));


            if (existPermission) {
              const message = this.HouseWinLossByProductsWarningMessage + this.HouseWinLossByProductsFunctionName;
              MessageBox.ShowError(message);
            }
        }

        return existPermission;
    }

    public static CheckExistFunctionPermissionInCN88HouseWinLossByProducts(site: Site, currentFunctionId: number): boolean {
      const func = site.Functions.find(f => f.Id === currentFunctionId);
      let existCN88Permission = false;

      if (this.HasNameOrParentNameIsCN88HouseWinLossByProducts(func)) {
          existCN88Permission = site.Functions.some(f => (
                  (f.Id !== currentFunctionId
                    && this.HasNameOrParentNameIsCN88HouseWinLossByProducts(f))
                    || this.HasNameIsCN88HouseWinLossByProducts(f))
                  && this.HasReadPolicyIncludeGroup(f));

          if (existCN88Permission) {
            const message = this.HouseWinLossByProductsWarningMessage + this.CN88HouseWinLossByProductsFunctionName;
            MessageBox.ShowError(message);
          }
      }

      return existCN88Permission;
    }

    public static CheckFunctionIsHouseWinLossByProducts(func: Function): boolean {
        const existFunction = this.HasNameOrParentNameIsHouseWinLossByProducts(func)
          || this.HasNameOrParentNameIsCN88HouseWinLossByProducts(func);

        return existFunction;
    }

    private static HasNameOrParentNameIsHouseWinLossByProducts(func: Function) {
        return this.HasNameIsHouseWinLossByProducts(func)
          || this.HasParentNameIsHouseWinLossByProducts(func);
    }

    private static HasNameIsHouseWinLossByProducts(func: Function) {
      return func.Name === this.HouseWinLossByProductsFunctionName;
    }

    private static HasParentNameIsHouseWinLossByProducts(func: Function) {
        return (func.Parent !== null && func.Parent !== undefined)
            && func.Parent.Name === this.HouseWinLossByProductsFunctionName;
    }

    private static HasNameOrParentNameIsCN88HouseWinLossByProducts(func: Function) {
        return this.HasNameIsCN88HouseWinLossByProducts(func)
          || this.HasParentNameIsCN88HouseWinLossByProducts(func);
    }

    private static HasNameIsCN88HouseWinLossByProducts(func: Function) {
        return func.Name === this.CN88HouseWinLossByProductsFunctionName;
    }

    private static HasParentNameIsCN88HouseWinLossByProducts(func: Function) {
          return (func.Parent !== null && func.Parent !== undefined)
            && func.Parent.Name === this.CN88HouseWinLossByProductsFunctionName;
    }

    private static HasReadPolicyIncludeGroup(func: Function) {
        return func.HasUserReadPolicy || func.HasGroupReadPolicy;
    }

    private static ViewedUserSiteLevelIsAdmin(site: Site) {
        return site.SiteLevel !== undefined && site.SiteLevel.Level >= SiteLevelValue.SiteAdmin;
    }

    private static LoginUserPolicyIsSmallerThanViewedUserPolicy(viewdUserPolicy: FunctionPolicy, loginUserPolicy: FunctionPolicy) {
        return viewdUserPolicy !== undefined && loginUserPolicy.Policy < viewdUserPolicy.Policy;
    }

    private static LoginUserHasNoPolicy(loginUserPolicy: FunctionPolicy, isRead: boolean) {
        return loginUserPolicy === undefined || loginUserPolicy.Policy < (isRead ? FunctionPolicyValue.Read : FunctionPolicyValue.Host);
    }

    private static LoginUserHasAssignFunctionReadPermissionInCurrentSiteAndAdminSite(
        site: Site,
        loginUserAssignFunctionPermissionInAdminSite: number): boolean {
        return (site.LoginUserAssignFunctionPermission <= FunctionPolicyValue.Read ||
            loginUserAssignFunctionPermissionInAdminSite <= FunctionPolicyValue.Read);
    }
}
